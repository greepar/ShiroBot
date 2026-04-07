<div align="center">

<a href="https://github.com/greepar/ShiroBot">
  <img src="./shirobana.webp" alt="ShiroBot" width="220" />
</a>

<p><strong><span style="font-size: 2.2em;">ShiroBot</span></strong></p>

<p><em>一个轻量的、基于 C# / .NET 10 实现的机器人框架。</em></p>

</div>

## 项目结构

- `ShiroBot`: 主程序
- `ShiroBot.SDK`: 插件与适配器开发 SDK
- `ShiroBot.Model`: 共享模型
- `ShiroBot.DemoPlugin`: 标准示例插件
- `templates/ShiroBot.PluginTemplate`: 可复制插件模板
- `ShiroBot.DemoAdapter`: 标准示例适配器

## 构建

```powershell
dotnet build .\ShiroBot.slnx
```

## 快速创建插件

基于模版插件(DemoPlugin)生成：

```bash
./scripts/new-plugin.sh HelloPlugin 你好插件
```

执行后会自动生成一个新的插件目录，并替换项目名、类名、配置类名、命名空间和元数据占位符。

如果不想跑脚本，也可以手动复制 `templates/ShiroBot.PluginTemplate` 并全局替换 `__PLUGIN_NAME__` 和 `__PLUGIN_DISPLAY_NAME__`。

## 许可证

本项目使用 GNU General Public License v2.0。
详见 [LICENSE](/C:/Users/greep/RiderProjects/QB/QBotSharp/LICENSE)。
