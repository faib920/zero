using Fireasy.Common.Ioc;
using Fireasy.Common.Serialization;
using Fireasy.Common.Subscribe;
using Fireasy.Data.Entity;
using Fireasy.Data.Entity.Subscribes;
using Fireasy.Web.EasyUI.Binders;
using Fireasy.Web.Mvc;
using Fireasy.Zero.Helpers;
using Fireasy.Zero.Infrastructure;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

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

        protected void Session_Start()
        {
            var id = HttpContext.Current.GetIdentity();

            if (id != 0)
            {
                var session = new SessionContext { UserID = id };
                HttpContext.Current.SetSession(session);
            }
        }
    }
}
