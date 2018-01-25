# fireasy zero sample

该 demo 演示了如何使用 fireasy 创建一个后台的管理系统。解决方案包含 asp.net mvc5 和 asp.net core 两个示例，使用 SQLite 数据库，基于 easyui 1.4.3 构建。

用户名: admin   密码: admin

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
	services.AddFireasy(Configuration)
		.AddIoc(ContainerUnity.GetContainer()); //添加IOC容器

	services.AddMvc()
		.ConfigureFireasyMvc(options =>
			{
				options.Converters.Add(new LightEntityJsonConverter()); //action接收的实体对象，是经过 fireasy 底层处理过的
			})
		.ConfigureEasyUI();

}

```