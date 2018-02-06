using Fireasy.Common.Ioc;
using Fireasy.Common.Serialization;
using Fireasy.Common.Subscribe;
using Fireasy.Data.Entity;
using Fireasy.Data.Entity.Subscribes;
using Fireasy.Web.EasyUI.Binders;
using Fireasy.Web.Mvc;
using Fireasy.Zero.Helpers;
using Fireasy.Zero.Infrastructure;
using Fireasy.Zero.Services;
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
            var container = GetContainer();
            ControllerBuilder.Current.SetControllerFactory(new ControllerFactory(container));

            //easyui验证绑定
            SettingsBindManager.RegisterBinder("validatebox", new ValidateBoxSettingBinder());
            SettingsBindManager.RegisterBinder("numberbox", new NumberBoxSettingBinder());

            //使用 LightEntity 反序列化转换器
            GlobalSetting.Converters.Add(new LightEntityJsonConverter());

            //从 Container 里反转类型反序列化
            GlobalSetting.Converters.Add(new ContainerJsonConverter(container));

            //注册实体持久化的订阅通知
            SubscribeManager.Register<EntityPersistentSubject>(new EntitySubscriber());
        }

        private Container GetContainer()
        {
            return ContainerUnity.GetContainer();
        }

        protected void Session_Start()
        {
            var id = HttpContext.Current.GetIdentity();

            if (id != 0)
            {
                var service = GetContainer().Resolve<IAdminService>();
                var user = service.GetUser(id);
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
