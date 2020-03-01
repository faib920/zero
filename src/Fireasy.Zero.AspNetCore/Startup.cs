using Fireasy.Common.Ioc;
using Fireasy.Common.Serialization;
using Fireasy.Common.Subscribes;
using Fireasy.Data;
using Fireasy.Data.Entity;
using Fireasy.Data.Entity.Subscribes;
using Fireasy.Data.Extensions;
using Fireasy.MongoDB;
using Fireasy.Web.Mvc;
using Fireasy.Zero.Helpers;
using Fireasy.Zero.Infrastructure;
using Fireasy.Zero.Services;
using Fireasy.Zero.Services.Impls;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Fireasy.Zero.AspNetCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            SerializeOption.GlobalConverters.Add(new LightEntityJsonConverter());

            services.AddFireasy(Configuration)
                .AddIoc(ContainerUnity.GetContainer()); //添加 appsettings.json 里的 ioc 配置

            // ############################ 演示两个实体上下文的配置 ############################
            services.AddEntityContext<DbContext>(builder =>
                {
                    builder.Options.NotifyEvents = true; //此项设为 true 时, 上面的实体持久化订阅通知才会触发
                });

            // mongodb 是用来记录日志的
            services.AddEntityContext<MongodbContext>(builder =>
                {
                    //builder.Options.ConfigName = "mongodb"; //指定配置文件中的实例名称
                    builder.UseMongoDB("server=mongodb://192.168.1.106;database=test"); //指定连接串
                });

            // ############################ 演示消息队列的订阅与发布 ############################
            // 以下两个配置要注释掉其中一个
            // redis配置
            services.AddRedisSubscriber(options =>
                {
                    options.Hosts = "localhost";
                    options.Initializer = s => s.AddSubscriber<CommandLogSubject>(d =>
                        {
                            Console.ForegroundColor = d.Level == 0 ? ConsoleColor.Green : ConsoleColor.Red;
                            Console.WriteLine("========= 来自 redis 的消息通知 =========");
                            if (d.Level == 0)
                            {
                                Console.WriteLine($"耗时：{d.Period} 毫秒");
                            }
                            Console.WriteLine(d.CommandText);
                            Console.ResetColor();
                        });
                });

            // rabittmq配置
            //services.AddRabbitMQSubscriber(options =>
            //    {
            //        options.Server = "amqp://127.0.0.1:5672";
            //        options.UserName = "guest";
            //        options.Password = "123";
            //        options.Initializer = s => s.AddSubscriber<CommandLogSubject>(d =>
            //            {
            //                Console.ForegroundColor = d.Type == 0 ? ConsoleColor.Green : ConsoleColor.Red;
            //                Console.WriteLine("========= 来自 rabbitmq 的消息通知 =========");
            //                if (d.Level == 0)
            //                {
            //                    Console.WriteLine($"耗时：{d.Period} 毫秒");
            //                }
            //                Console.WriteLine(d.CommandText);
            //                Console.ResetColor();
            //            });
            //    });

            // 改变数据库操作sql日志跟踪，配合 mq 来进行日志跟踪
            // 即先由 MQCommandTracker 将消息发送到 mq，mq在上面的配置处分别使用 redis 或 rabbitmq 来接收消息显示
            services.AddTransient<ICommandTracker, MQCommandTracker>();

            // ############################ 演示使用第三方的日志组件 ############################
            // NLog日志
            //services.AddNLogger();

            // Log4net日志
            services.AddLog4netLogger();

            // 注册实体持久化的订阅通知
            EntityPersistentSubscribeManager.AddSubscriber(subject => new EntitySubscriber().Accept(subject));
            services.AddMvc()
                .AddSessionStateTempDataProvider()
                .ConfigureFireasyMvc(options =>
                    {
                        //options.JsonSerializeOption.Converters.Add(new LightEntityJsonConverter());
                    })
                .ConfigureEasyUI();

            // ############################ 演示Session自动复活 ############################
            services.AddSession()
                .AddSessionRevive<SessionReviveNotification>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                    {
                        options.LoginPath = new PathString("/login");
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //添加静态文件映射
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseSession();
            app.UseSessionRevive();

            app.UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "areas",
                        template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");
                });
        }
    }

    /// <summary>
    /// Session 复活时自动设置 <see cref="SessionContext"/> 对象。
    /// </summary>
    public class SessionReviveNotification : ISessionReviveNotification
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var adminService = context.RequestServices.GetRequiredService<IAdminService>();
            var userId = context.GetIdentity();
            if (userId != 0)
            {
                var user = await adminService.GetUserAsync(userId);
                if (user != null)
                {
                    var session = new SessionContext { UserID = userId, UserName = user.Name, OrgID = user.OrgID };
                    context.SetSession(session);
                }
            }
        }
    }

    /// <summary>
    /// 使用 mq 来发布日志。
    /// </summary>
    public class MQCommandTracker : ICommandTracker
    {
        private readonly ISubscribeManager subMgr;

        public MQCommandTracker(ISubscribeManager subMgr)
        {
            this.subMgr = subMgr;
        }

        public void Write(IDbCommand command, TimeSpan period)
        {
            subMgr.Publish(new CommandLogSubject { Level = 0, CommandText = command.Output(), Period = period.TotalMilliseconds });
        }

        public async Task WriteAsync(IDbCommand command, TimeSpan period, CancellationToken cancellationToken = default)
        {
            await subMgr.PublishAsync(new CommandLogSubject { Level = 0, CommandText = command.Output(), Period = period.TotalMilliseconds });
        }

        public void Fail(IDbCommand command, Exception exception)
        {
            subMgr.Publish(new CommandLogSubject { Level = 1, CommandText = command.Output() });
        }

        public async Task FailAsync(IDbCommand command, Exception exception, CancellationToken cancellationToken = default)
        {
            await subMgr.PublishAsync(new CommandLogSubject { Level = 1, CommandText = command.Output() });
        }
    }

    /// <summary>
    /// 执行 sql 脚本的订阅主题。
    /// </summary>
    public class CommandLogSubject
    {
        /// <summary>
        /// 获取或设置级别。
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 获取或设置 sql 脚本。
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// 获取或设置耗时。
        /// </summary>
        public double Period { get; set; }
    }
}
