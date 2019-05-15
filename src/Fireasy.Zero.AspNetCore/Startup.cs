using Fireasy.Common;
using Fireasy.Common.Ioc;
using Fireasy.Data.Entity;
using Fireasy.Data.Entity.Subscribes;
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
            services.AddFireasy(Configuration, options =>
                {
                    //注册实体持久化的订阅通知
                    options.AddSubscriber<EntityPersistentSubject>(string.Empty, subject => new EntitySubscriber().Accept(subject));
                })
                .AddIoc(ContainerUnity.GetContainer()) //添加 appsettings.json 里的 ioc 配置
                .AddEntityContext<DbContext>(options =>
                    {
                        options.AutoCreateTables = true; //此项为 true 时, 采用 codefirst 模式维护数据库表
                        options.NotifyEvents = true; //此项设为 true 时, 上面的实体持久化订阅通知才会触发
                    });

            services.AddMvc()
                .AddSessionStateTempDataProvider()
                .ConfigureFireasyMvc(options =>
                    {
                        options.JsonSerializeOption.Converters.Add(new LightEntityJsonConverter());
                    })
                .ConfigureEasyUI();

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

        /// <summary>
        /// Session 复活时自动设置 <see cref="SessionContext"/> 对象。
        /// </summary>
        class SessionReviveNotification : ISessionReviveNotification
        {
            public void Invoke(HttpContext context)
            {
                var adminService = context.RequestServices.GetRequiredService<IAdminService>();
                var userId = context.GetIdentity();
                if (userId != 0)
                {
                    var user = adminService.GetUser(userId);
                    if (user != null)
                    {
                        var session = new SessionContext { UserID = userId, UserName = user.Name, OrgID = user.OrgID };
                        context.SetSession(session);
                    }
                }
            }
        }
    }
}
