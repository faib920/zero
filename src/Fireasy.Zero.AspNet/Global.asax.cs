using Fireasy.Common.Extensions;
using Fireasy.Common.Ioc;
using Fireasy.Common.Serialization;
using Fireasy.Common.Subscribes;
using Fireasy.Data.Entity;
using Fireasy.Data.Entity.Subscribes;
using Fireasy.Web.EasyUI.Binders;
using Fireasy.Web.Mvc;
using Fireasy.Zero.Helpers;
using Fireasy.Zero.Infrastructure;
using Fireasy.Zero.Services;
using Fireasy.Zero.Services.Impls;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace Fireasy.Zero.AspNet
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);

            BundleManager.Config();

            //MVC控制器工厂添加IOC容器
            var container = ContainerUnity.GetContainer();
            ControllerBuilder.Current.SetControllerFactory(new ControllerFactory(container));

            //注入所有控制器
            container.RegisterControllers(Assembly.GetExecutingAssembly());

            //注入DbContext
            container.Register<DbContext>(Lifetime.Scoped);

            //easyui验证绑定
            SettingsBindManager.RegisterBinder("validatebox", new ValidateBoxSettingBinder());
            SettingsBindManager.RegisterBinder("numberbox", new NumberBoxSettingBinder());

            //使用 LightEntity 反序列化转换器
            GlobalSetting.Converters.Add(new LightEntityJsonConverter());

            //从 Container 里反转类型反序列化
            GlobalSetting.Converters.Add(new ContainerJsonConverter(container));

            //注册实体持久化的订阅通知
            DefaultSubscribeManager.Instance.AddSubscriber<EntityPersistentSubject>(subject => new EntitySubscriber().Accept(subject));
        }

        protected void Session_Start()
        {
            var id = HttpContext.Current.GetIdentity();

            if (id != 0)
            {
                using (var scope = ContainerUnity.GetContainer().CreateScope())
                {
                    var service = scope.Resolve<IAdminService>();

                    var user = service.GetUserAsync(id).AsSync();
                    if (user != null)
                    {
                        var session = new SessionContext { UserID = id, UserName = user.Name, OrgID = user.OrgID };
                        HttpContext.Current.SetSession(session);
                    }
                    else
                    {
                        HttpContext.Current.Response.Redirect(FormsAuthentication.LoginUrl);
                    }
                }
            }
        }
    }
}
