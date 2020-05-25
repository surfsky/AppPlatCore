# AppPlat

Aspnetcore 应用快速开发平台，基于 AppBoxCore 6.0 改造。

功能
- [x] 集成框架
    - [x] 集成 HttpApi 数据接口辅助类库
    - [x] 集成 Utils 辅助类库
    - [x] 集成 WebCore 辅助类库
    - [x] 集成 Entities ORM 框架
    - [x] 集成 SignalR IM 框架
    - [x] 集成 Blazor 应用开发框架
    - [x] 集成 Blazor Radzen 控件库
    - [x] 集成 Serilog 日志框架
    - [ ] 集成 MQ 消息队列
    - [ ] 集成 OAuth 授权框架
- [x] 新建中间件
    - [x] 新建 Monitor 中间件：监视页面访问时间
    - [x] 新建 Image 中间件：图片缩略图及水印
- [x] 美化
    - [x] 登录页配色
    - [x] 首页配色
- [x] 逻辑功能
    - [x] 用户管理
    - [x] 职称管理
    - [x] 部门管理
    - [x] 角色管理
    - [x] 权限管理
    - [x] 菜单管理
- [ ] 改造 App.WebEngine
    - [ ] 模型层改造（用EntityBase改造）
    - [ ] GridPro, FormPro
    - [ ] FormPage, GridPage, MixPage
    - [ ] PopupBox, FileBox, GPSBox...
    - [ ] UIAuto

TIP
- 默认管理员账号登陆（用户名：admin 密码：admin）
- 本项目采用Entity Framework Core 的Code First开发模式，数据库会在网站第一次运行时自动创建。
- 如果尚未安装 .Net Core 3.1，请先安装 SDK：<https://dotnet.microsoft.com/download/dotnet-core/3.1>

# Author

- https://surfsky.github.com
- https://surfsky.cnblogs.com
- LastUpdate: 2020-05



# 关于命名空间的问题

- 基于 netframework 的为 App.xxx
- 基于 netcore/standard 的为 AppPlat.xxx


# 数据库测试

类型                   |   状态      | 说明
-----------------------|-------------|------------
sqlserve 2008r2        | ok          | 只能用efcore 2.2
linux sqlserver        | ok          | efcore 2.2 - 3.1 均可
linux mysql (mariadb)  | ok          | efcore 2.2 - 3.1 均可
sqlite                 | ok          | efcore 2.2 - 3.1 均可
dameng                 | fail        | 只支持netcore 2.2（测试失败），新版本的驱动还在开发

**如果使用sqlserver 2008r2，只能用 efcore 2.2**



# Remarks
- Sirilog 日志框架扩展 <https://github.com/serilog/serilog-aspnetcore>
