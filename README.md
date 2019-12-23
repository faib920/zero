# fireasy zero sample

该 demo 演示了如何使用 fireasy 创建一个后台的管理系统。解决方案包含 asp.net mvc5 和 asp.net core 两个示例，使用 SQLite 数据库，基于 easyui 1.4.3 构建。

<b>注意</b>
* 最新的版本基于.net core 2.2，请确定你已安装sdk，否则将无法运行。
* 示例中使用 redis 作为数据缓存，你可以安装 redis 2.8 或在配置中将缓存关闭（删除配置节）。
* 用户名: admin   密码: admin

<b>特点</b>

* 数据库操作与 entity framework 的用法较相似，也是使用 linq ，降低了学习成本，然而与后者所不同的是，提供了 Insert、Update、Delete、Batch 等方法更快捷地进行操作，更贴近于 sql 原生语言。
* 基于 easyui 构建的后台管理系统，view 层采用 ajax 进行数据交互，因此在 action 接收参数时，直接通过 json 反序列化的方式进行 model bind，这样的好处是，对于复杂的提交的数据结构，比如多个 list，嵌套的对象等，都很容易在 action 里接收。
* action 接收的实体对象能够主动区分哪些属性被修改，这得益于 LightEntityJsonConverter 转换器，这样，新增或修改时，不必考虑原有数据被覆盖的可能。
* 没有创建 ViewModel，而直接使用 DataModel，这样节省了很多开发时间，然而这样带来的一个问题，可能也是使用 entity framework 一样面临的问题，那就是 <b>延迟加载</b> 属性会被一概序列化。因此 json 序列化也被优化过，采用 fireasy json serializer 能够主动识别延迟加载对象。
* 没有 ViewModel，但是可以扩展更多的属性给前端使用，在实体类_Ex 中，可以定义非 virtual 的属性，然后用 ExtendAs 扩展方法，这样避免了 new {   } 这样繁琐的赋值操作。
* services 是基于 AOP 的实例，提供了事务处理、缓存处理等拦截器，只需要标识特性就可以实现这些功能。这也必须使用 fireasy 的 IOC 容器。

<b>项目结构</b>

Fireasy.Zero.Models            实体模型

Fireasy.Zero.Infrastructure    基础设施

Fireasy.Zero.Helpers           相关辅助

Fireasy.Zero.Services          数据服务

Fireasy.Zero.AspNet            Mvc5示例

Fireasy.Zero.AspNetCore        Core示例

<b>Mvc5 配置</b>

有关 fireasy 的配置请参考 web.config 里的 fireasy 小节，其中: 

dataInstances 为数据库实例配置，当前使用 sqlite 数据库

cachings  缓存配置，当前采用 redis ，如果删除此配置，则使用内存作为缓存

containers    为IOC容器配置

mvc/bundles   配置bundle资源文件

注意 global 里对 fireasy 的配置，这些都是必要的

```C#
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

```

<b>core 配置</b>

以上面示例相似，core 的配置在 appsettings.json 里，也是对数据库实例，IOC容器进行配置。

Startup 类文件里的配置

```C#
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
			options.JsonSerializeOption.Converters.Add(new LightEntityJsonConverter()); //action接收的实体对象，是经过 fireasy 底层处理过的
		})
		.ConfigureEasyUI();

	services.AddSession()
		.AddSessionRevive<SessionReviveNotification>(); //session 复活

}

```


------------------------------------------------------------------------
QQ号： 55570729
QQ群： 225698098
------------------------------------------------------------------------

![](http://www.fireasy.cn/content/images/Donate_fireasy.png)

