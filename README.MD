# Tasks

[x] 集成 HttpApi
[x] 集成 App.Utils
[x] 集成 App.WebCore
[ ] 改造 App.WebCtrls
    GridPro
    FormPro
    FormPage
    GridPage
    MixPage
[ ] 写 Demo 页面







# EntityframeworkCore 版本的说明

如果使用sqlserver 2008r2，只能用 efcore 2.2。高版本的efcore只支持sqlserver 2012以后的版本。
融合 App.Utils、App.Entities、App.HttpApi 的项目详见 AppPlatCore


# 数据库测试

类型                   |   状态      | 说明
-----------------------|-------------|------------
sqlserve 2008r2        | ok          | 只能用efcore 2.2
linux sqlserver        | ok          | efcore 2.2 - 3.1 均可
linux mysql (mariadb)  | ok          |
sqlite                 | ok          | 
dameng                 | fail        | 只支持netcore 2.2，新版本的驱动还在开发







# Codes

```
    public class MyDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var sqliteConn = new SqliteConnection(@"DataSource = Chinook.db");
            optionsBuilder.UseSqlite(sqliteConn);
        }
    }

    using (var context = new MyDbContext())
    {
        context.Database.EnsureCreated();
    }
```
