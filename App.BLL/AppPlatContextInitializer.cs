﻿using App.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace App.DAL
{
    // https://docs.microsoft.com/zh-cn/aspnet/core/data/ef-rp/intro
    public static class AppPlatContextInitializer
    {
        public static void Initialize(AppPlatContext context)
        {
            context.Database.EnsureCreated();
            context.Database.Migrate(); // 若实体类变更，会自动修改数据库。先运行命令行 dotnet ef migrations add XXX。经测试sqllite怎么都会出错。

            // 已经初始化
            if (context.Users.Any())
                return;
            else
            {

                GetSiteConfigs().ForEach(c => context.SiteConfigs.Add(c));
                GetDepts().ForEach(d => context.Depts.Add(d));
                GetUsers().ForEach(u => context.Users.Add(u));
                GetRoles().ForEach(r => context.Roles.Add(r));
                GetPowers().ForEach(p => context.Powers.Add(p));
                GetTitles().ForEach(t => context.Titles.Add(t));
                context.SaveChanges();
                GetMenus(context).ForEach(m => context.Menus.Add(m));  // 添加菜单时需要指定ViewPower，所以上面需要先保存到数据库
                context.SaveChanges();
            }
        }

        private static List<SiteConfig> GetSiteConfigs()
        {
            return new List<SiteConfig>() {
                new SiteConfig
                {
                    Icon = "",
                    LoginBg = "/res/themes/image_blue_moon/moon.jpg",
                    BeiAnNo = "浙ICP备案XXXXX",
                    Title = "AppPlat",
                    PageSize = 20,
                    Theme = "Cupertino",
                    HelpList = "[{\"Text\":\"万年历\",\"Icon\":\"Calendar\",\"ID\":\"wannianli\",\"URL\":\"~/Admin/HelpWanNianLi\"},{\"Text\":\"科学计算器\",\"Icon\":\"Calculator\",\"ID\":\"jisuanqi\",\"URL\":\"~/Admin/HelpJiSuanQi\"},{\"Text\":\"系统帮助\",\"Icon\":\"Help\",\"ID\":\"help\",\"URL\":\"~/Admin/Help\"}]"
                }
            };

        }
        private static List<Menu> GetMenus(AppPlatContext context)
        {
            var menus = new List<Menu> {
                new Menu
                {
                    Name = "首饰识别",
                    SortIndex = 10,
                    Remark = "顶级菜单",
                    ImageUrl = "~/res/icon/folder.png",
                    Children = new List<Menu> {
                        new Menu
                        {
                            Name = "首饰识别",
                            NavigateUrl = "~/AI/Jewels",
                            ImageUrl = "~/res/icon/page.png",
                            SortIndex = 20,
                        },
                        new Menu
                        {
                            Name = "Grid",
                            NavigateUrl = "~/AI/Config",
                            ImageUrl = "~/res/icon/page.png",
                            SortIndex = 50,
                        }
                    }
                },
                new Menu
                {
                    Name = "系统管理",
                    SortIndex = 20,
                    Remark = "顶级菜单",
                    ImageUrl = "~/res/icon/folder.png",
                    Children = new List<Menu> {
                        new Menu
                        {
                            Name = "用户",
                            SortIndex = 10,
                            Remark = "二级菜单",
                            NavigateUrl = "~/Admin/UserList",
                            ImageUrl = "~/res/icon/page.png",
                            ViewPower = context.Powers.Where(p => p.Name == "CoreUserView").FirstOrDefault<Power>()
                        },
                        new Menu
                        {
                            Name = "部门",
                            SortIndex = 50,
                            Remark = "二级菜单",
                            NavigateUrl = "~/Admin/DeptUser",
                            ImageUrl = "~/res/icon/page.png",
                            ViewPower = context.Powers.Where(p => p.Name == "CoreDeptUserView").FirstOrDefault<Power>()
                        },
                        new Menu
                        {
                            Name = "职称",
                            SortIndex = 30,
                            Remark = "二级菜单",
                            NavigateUrl = "~/Admin/TitleUser",
                            ImageUrl = "~/res/icon/page.png",
                            ViewPower = context.Powers.Where(p => p.Name == "CoreTitleUserView").FirstOrDefault<Power>()
                        },
                        new Menu
                        {
                            Name = "角色用户",
                            SortIndex = 70,
                            Remark = "二级菜单",
                            NavigateUrl = "~/Admin/RoleUser",
                            ImageUrl = "~/res/icon/page.png",
                            ViewPower = context.Powers.Where(p => p.Name == "CoreRoleUserView").FirstOrDefault<Power>()
                        },
                        new Menu
                        {
                            Name = "角色权限",
                            SortIndex = 90,
                            Remark = "二级菜单",
                            NavigateUrl = "~/Admin/RolePower",
                            ImageUrl = "~/res/icon/page.png",
                            ViewPower = context.Powers.Where(p => p.Name == "CoreRolePowerView").FirstOrDefault<Power>()
                        },
                        new Menu
                        {
                            Name = "菜单",
                            SortIndex = 100,
                            Remark = "二级菜单",
                            NavigateUrl = "~/Admin/Menu",
                            ImageUrl = "~/res/icon/page.png",
                            ViewPower = context.Powers.Where(p => p.Name == "CoreMenuView").FirstOrDefault<Power>()
                        },
                        new Menu
                        {
                            Name = "在线",
                            SortIndex = 110,
                            Remark = "二级菜单",
                            NavigateUrl = "~/Admin/Online",
                            ImageUrl = "~/res/icon/page.png",
                            ViewPower = context.Powers.Where(p => p.Name == "CoreOnlineView").FirstOrDefault<Power>()
                        },
                        new Menu
                        {
                            Name = "配置",
                            SortIndex = 120,
                            Remark = "二级菜单",
                            NavigateUrl = "~/Admin/Config",
                            ImageUrl = "~/res/icon/cog.png",
                            ViewPower = context.Powers.Where(p => p.Name == "CoreConfigView").FirstOrDefault<Power>()
                        },
                    }
                },
                new Menu
                {
                    Name = "测试",
                    SortIndex = 30,
                    ImageUrl = "~/res/icon/folder.png",
                    Children = new List<Menu> {
                        new Menu
                        {
                            Name = "FineUI",
                            SortIndex = 20,
                            ImageUrl = "~/res/icon/folder.png",
                            Children = new List<Menu> {
                                new Menu
                                {
                                    Name = "Button",
                                    NavigateUrl = "~/Tests/Buttons",
                                    ImageUrl = "~/res/icon/page.png",
                                    SortIndex = 20,
                                },
                                new Menu
                                {
                                    Name = "Grid",
                                    NavigateUrl = "~/Tests/Grid",
                                    ImageUrl = "~/res/icon/page.png",
                                    SortIndex = 50,
                                },
                                new Menu
                                {
                                    Name = "Grid2",
                                    NavigateUrl = "~/Tests/DynamicGrid",
                                    ImageUrl = "~/res/icon/page.png",
                                    SortIndex = 50,
                                },
                                new Menu
                                {
                                    Name = "Responsive",
                                    NavigateUrl = "~/Tests/Responsive",
                                    ImageUrl = "~/res/icon/page.png",
                                    SortIndex = 40,
                                },
                            }
                        },
                        new Menu
                        {
                            Name = "Chat",
                            NavigateUrl = "~/Chats/Chat",
                            ImageUrl = "~/res/icon/page.png",
                            SortIndex = 30,
                        },
                        new Menu
                        {
                            Name = "Blazor",
                            NavigateUrl = "~/Blazors/Index",
                            ImageUrl = "~/res/icon/page.png",
                            SortIndex = 60,
                        },
                    }
                },
                new Menu
                {
                    Name = "修改密码",
                    SortIndex = 130,
                    Remark = "二级菜单",
                    NavigateUrl = "~/Admin/ChangePassword",
                    ImageUrl = "~/res/icon/tag_blue.png"
                },
                new Menu
                {
                    Name = "安全退出",
                    SortIndex = 140,
                    Remark = "二级菜单",
                    NavigateUrl = "~/Index?action=SignOut",
                    ImageUrl = "~/res/icon/tag_blue.png"
                },
            };

            return menus;
        }


        private static List<Title> GetTitles()
        {
            var titles = new List<Title>()
            {
                new Title()
                {
                    Name = "总经理"
                },
                new Title()
                {
                    Name = "部门经理"
                },
                new Title()
                {
                    Name = "高级工程师"
                },
                new Title()
                {
                    Name = "工程师"
                }
            };

            return titles;
        }

        private static List<Power> GetPowers()
        {
            var powers = new List<Power>
            {
                new Power
                {
                    Name = "CoreUserView",
                    Title = "浏览用户列表",
                    GroupName = "CoreUser"
                },
                new Power
                {
                    Name = "CoreUserNew",
                    Title = "新增用户",
                    GroupName = "CoreUser"
                },
                new Power
                {
                    Name = "CoreUserEdit",
                    Title = "编辑用户",
                    GroupName = "CoreUser"
                },
                new Power
                {
                    Name = "CoreUserDelete",
                    Title = "删除用户",
                    GroupName = "CoreUser"
                },
                new Power
                {
                    Name = "CoreUserChangePassword",
                    Title = "修改用户登陆密码",
                    GroupName = "CoreUser"
                },
                new Power
                {
                    Name = "CoreRoleView",
                    Title = "浏览角色列表",
                    GroupName = "CoreRole"
                },
                new Power
                {
                    Name = "CoreRoleNew",
                    Title = "新增角色",
                    GroupName = "CoreRole"
                },
                new Power
                {
                    Name = "CoreRoleEdit",
                    Title = "编辑角色",
                    GroupName = "CoreRole"
                },
                new Power
                {
                    Name = "CoreRoleDelete",
                    Title = "删除角色",
                    GroupName = "CoreRole"
                },
                new Power
                {
                    Name = "CoreRoleUserView",
                    Title = "浏览角色用户列表",
                    GroupName = "CoreRoleUser"
                },
                new Power
                {
                    Name = "CoreRoleUserNew",
                    Title = "向角色添加用户",
                    GroupName = "CoreRoleUser"
                },
                new Power
                {
                    Name = "CoreRoleUserDelete",
                    Title = "从角色中删除用户",
                    GroupName = "CoreRoleUser"
                },
                new Power
                {
                    Name = "CoreOnlineView",
                    Title = "浏览在线用户列表",
                    GroupName = "CoreOnline"
                },
                new Power
                {
                    Name = "CoreConfigView",
                    Title = "浏览全局配置参数",
                    GroupName = "CoreConfig"
                },
                new Power
                {
                    Name = "CoreConfigEdit",
                    Title = "修改全局配置参数",
                    GroupName = "CoreConfig"
                },
                new Power
                {
                    Name = "CoreMenuView",
                    Title = "浏览菜单列表",
                    GroupName = "CoreMenu"
                },
                new Power
                {
                    Name = "CoreMenuNew",
                    Title = "新增菜单",
                    GroupName = "CoreMenu"
                },
                new Power
                {
                    Name = "CoreMenuEdit",
                    Title = "编辑菜单",
                    GroupName = "CoreMenu"
                },
                new Power
                {
                    Name = "CoreMenuDelete",
                    Title = "删除菜单",
                    GroupName = "CoreMenu"
                },
                new Power
                {
                    Name = "CoreLogView",
                    Title = "浏览日志列表",
                    GroupName = "CoreLog"
                },
                new Power
                {
                    Name = "CoreLogDelete",
                    Title = "删除日志",
                    GroupName = "CoreLog"
                },
                new Power
                {
                    Name = "CoreTitleView",
                    Title = "浏览职务列表",
                    GroupName = "CoreTitle"
                },
                new Power
                {
                    Name = "CoreTitleNew",
                    Title = "新增职务",
                    GroupName = "CoreTitle"
                },
                new Power
                {
                    Name = "CoreTitleEdit",
                    Title = "编辑职务",
                    GroupName = "CoreTitle"
                },
                new Power
                {
                    Name = "CoreTitleDelete",
                    Title = "删除职务",
                    GroupName = "CoreTitle"
                },
                new Power
                {
                    Name = "CoreTitleUserView",
                    Title = "浏览职务用户列表",
                    GroupName = "CoreTitleUser"
                },
                new Power
                {
                    Name = "CoreTitleUserNew",
                    Title = "向职务添加用户",
                    GroupName = "CoreTitleUser"
                },
                new Power
                {
                    Name = "CoreTitleUserDelete",
                    Title = "从职务中删除用户",
                    GroupName = "CoreTitleUser"
                },
                new Power
                {
                    Name = "CoreDeptView",
                    Title = "浏览部门列表",
                    GroupName = "CoreDept"
                },
                new Power
                {
                    Name = "CoreDeptNew",
                    Title = "新增部门",
                    GroupName = "CoreDept"
                },
                new Power
                {
                    Name = "CoreDeptEdit",
                    Title = "编辑部门",
                    GroupName = "CoreDept"
                },
                new Power
                {
                    Name = "CoreDeptDelete",
                    Title = "删除部门",
                    GroupName = "CoreDept"
                },
                new Power
                {
                    Name = "CoreDeptUserView",
                    Title = "浏览部门用户列表",
                    GroupName = "CoreDeptUser"
                },
                new Power
                {
                    Name = "CoreDeptUserNew",
                    Title = "向部门添加用户",
                    GroupName = "CoreDeptUser"
                },
                new Power
                {
                    Name = "CoreDeptUserDelete",
                    Title = "从部门中删除用户",
                    GroupName = "CoreDeptUser"
                },
                new Power
                {
                    Name = "CorePowerView",
                    Title = "浏览权限列表",
                    GroupName = "CorePower"
                },
                new Power
                {
                    Name = "CorePowerNew",
                    Title = "新增权限",
                    GroupName = "CorePower"
                },
                new Power
                {
                    Name = "CorePowerEdit",
                    Title = "编辑权限",
                    GroupName = "CorePower"
                },
                new Power
                {
                    Name = "CorePowerDelete",
                    Title = "删除权限",
                    GroupName = "CorePower"
                },
                new Power
                {
                    Name = "CoreRolePowerView",
                    Title = "浏览角色权限列表",
                    GroupName = "CoreRolePower"
                },
                new Power
                {
                    Name = "CoreRolePowerEdit",
                    Title = "编辑角色权限",
                    GroupName = "CoreRolePower"
                },
                new Power
                {
                    Name = "TestPage1View",
                    Title = "浏览测试页面一",
                    GroupName = "Test"
                },
                new Power
                {
                    Name = "TestPage2View",
                    Title = "浏览测试页面二",
                    GroupName = "Test"
                }
            };

            return powers;
        }

        private static List<Role> GetRoles()
        {
            var roles = new List<Role>()
            {
                new Role()
                {
                    Name = "系统管理员",
                    Remark = ""
                },
                new Role()
                {
                    Name = "部门管理员",
                    Remark = ""
                },
                new Role()
                {
                    Name = "项目经理",
                    Remark = ""
                },
                new Role()
                {
                    Name = "开发经理",
                    Remark = ""
                },
                new Role()
                {
                    Name = "开发人员",
                    Remark = ""
                },
                new Role()
                {
                    Name = "后勤人员",
                    Remark = ""
                },
                new Role()
                {
                    Name = "外包人员",
                    Remark = ""
                }
            };

            return roles;
        }

        private static List<User> GetUsers()
        {
            string[] USER_NAMES = { "男", "童光喜", "男", "方原柏", "女", "祝春亚", "男", "涂辉", "男", "舒兆国", "男", "熊忠文", "男", "徐吉琳", "男", "方金海", "男", "包卫峰", "女", "靖小燕", "男", "杨习斌", "男", "徐长旺", "男", "聂建雄", "男", "周敦友", "男", "陈友庭", "女", "陆静芳", "男", "袁国柱", "女", "骆新桂", "男", "许治国", "男", "马先加", "男", "赵恢川", "男", "柯常胜", "男", "黄国鹏", "男", "柯尊北", "男", "刘海云", "男", "罗清波", "男", "张业权", "女", "丁溯鋆", "男", "吴俊", "男", "郑江", "男", "李亚华", "男", "石光富", "男", "谭志洪", "男", "胡中生", "男", "董龙剑", "男", "陈红", "男", "汪海平", "男", "彭道洲", "女", "尹莉君", "男", "占耀玲", "男", "付杰", "男", "王红艳", "男", "邝兴", "男", "饶玮", "男", "王方胜", "男", "陈劲松", "男", "邓庆华", "男", "王石林", "男", "胡俊明", "男", "索相龙", "男", "陈海军", "男", "吴文涛", "女", "熊望梅", "女", "段丽华", "女", "胡莎莎", "男", "徐友安", "男", "肖诗涛", "男", "王闯", "男", "余兴龙", "男", "芦荫杰", "男", "丁金富", "男", "谭军令", "女", "鄢旭燕", "男", "田坤", "男", "夏德胜", "男", "喻显发", "男", "马兴宝", "男", "孙学涛", "男", "陶云成", "男", "马远健", "男", "田华", "男", "聂子森", "男", "郑永军", "男", "余昌平", "男", "陶俊华", "男", "李小林", "男", "李荣宝", "男", "梅盈凯", "男", "张元群", "男", "郝新华", "男", "刘红涛", "男", "向志强", "男", "伍小峰", "男", "胡勇民", "男", "黄定祥", "女", "高红香", "男", "刘军", "男", "叶松", "男", "易俊林", "男", "张威", "男", "刘卫华", "男", "李浩", "男", "李寿庚", "男", "涂洋", "男", "曹晶", "男", "陈辉", "女", "彭博", "男", "严雪冰", "男", "刘青", "女", "印媛", "男", "吴道雄", "男", "邓旻", "男", "陈骏", "男", "崔波", "男", "韩静颐", "男", "严安勇", "男", "刘攀", "女", "刘艳", "女", "孙昕", "女", "郑新", "女", "徐睿", "女", "李月杰", "男", "吕焱鑫", "女", "刘沈", "男", "朱绍军", "女", "马茜", "女", "唐蕾", "女", "刘姣", "女", "于芳", "男", "吴健", "女", "张丹梅", "女", "王燕", "女", "贾兆梅", "男", "程柏漠", "男", "程辉", "女", "任明慧", "女", "焦莹", "女", "马淑娟", "男", "徐涛", "男", "孙庆国", "男", "刘胜", "女", "傅广凤", "男", "袁弘", "男", "高令旭", "男", "栾树权", "女", "申霞", "女", "韩文萍", "女", "隋艳", "男", "邢海洲", "女", "王宁", "女", "陈晶", "女", "吕翠", "女", "刘少敏", "女", "刘少君", "男", "孔鹏", "女", "张冰", "女", "王芳", "男", "万世忠", "女", "徐凡", "女", "张玉梅", "女", "何莉", "女", "时会云", "女", "王玉杰", "女", "谭素英", "女", "李艳红", "女", "刘素莉", "男", "王旭海", "女", "安丽梅", "女", "姚露", "女", "贾颖", "女", "曹微", "男", "黄经华", "女", "陈玉华", "女", "姜媛", "女", "魏立平", "女", "张萍", "男", "来辉", "女", "陈秀玫", "男", "石岩", "男", "王洪捍", "男", "张树军", "女", "李亚琴", "女", "王凤", "女", "王珊华", "女", "杨丹丹", "女", "教黎明", "女", "修晶", "女", "丁晓霞", "女", "张丽", "女", "郭素兰", "女", "徐艳丽", "女", "任子英", "女", "胡雁", "女", "彭洪亮", "女", "高玉珍", "女", "王玉姝", "男", "郑伟", "女", "姜春玲", "女", "张伟", "女", "王颖", "女", "金萍", "男", "孙望", "男", "闫宝东", "男", "周相永", "女", "杨美娜", "女", "欧立新", "女", "刘宝霞", "女", "刘艳杰", "女", "宋艳平", "男", "李克", "女", "梁翠", "女", "宗宏伟", "女", "刘国伟", "女", "敖志敏", "女", "尹玲" };
            string[] EMAIL_NAMES = { "qq.com", "gmail.com", "163.com", "126.com", "outlook.com", "foxmail.com" };

            var users = new List<User>();
            var rdm = new Random();

            for (int i = 0, count = USER_NAMES.Length; i < count; i += 2)
            {
                string gender = USER_NAMES[i];
                string chineseName = USER_NAMES[i + 1];
                string userName = "user" + i.ToString();

                users.Add(new User
                {
                    Name = userName,
                    Gender = gender,
                    Password = PasswordUtil.CreateDbPassword(userName),
                    ChineseName = chineseName,
                    Email = userName + "@" + EMAIL_NAMES[rdm.Next(0, EMAIL_NAMES.Length)],
                    Enabled = true,
                    CreateTime = DateTime.Now
                });
            }

            // 添加超级管理员
            users.Add(new User
            {
                Name = "admin",
                Gender = "男",
                Password = PasswordUtil.CreateDbPassword("admin"),
                ChineseName = "超级管理员",
                Email = "admin@fineui.com",
                Enabled = true,
                CreateTime = DateTime.Now
            });

            return users;
        }


        private static List<Dept> GetDepts()
        {
            var depts = new List<Dept> {
                new Dept
                {
                    Name = "研发部",
                    SortIndex = 1,
                    Remark = "顶级部门",
                    Children = new List<Dept> {
                        new Dept
                        {
                            Name = "开发部",
                            SortIndex = 1,
                            Remark = "二级部门"
                        },
                        new Dept
                        {
                            Name = "测试部",
                            SortIndex = 2,
                            Remark = "二级部门"
                        }
                    }
                },
                new Dept
                {
                    Name = "销售部",
                    SortIndex = 2,
                    Remark = "顶级部门",
                    Children = new List<Dept> {
                        new Dept
                        {
                            Name = "直销部",
                            SortIndex = 1,
                            Remark = "二级部门"
                        },
                        new Dept
                        {
                            Name = "渠道部",
                            SortIndex = 2,
                            Remark = "二级部门"
                        }
                    }
                },
                new Dept
                {
                    Name = "客服部",
                    SortIndex = 3,
                    Remark = "顶级部门",
                    Children = new List<Dept> {
                        new Dept
                        {
                            Name = "实施部",
                            SortIndex = 1,
                            Remark = "二级部门"
                        },
                        new Dept
                        {
                            Name = "售后服务部",
                            SortIndex = 2,
                            Remark = "二级部门"
                        },
                        new Dept
                        {
                            Name = "大客户服务部",
                            SortIndex = 3,
                            Remark = "二级部门"
                        }
                    }
                },
                new Dept
                {
                    Name = "财务部",
                    SortIndex = 4,
                    Remark = "顶级部门"
                },
                new Dept
                {
                    Name = "行政部",
                    SortIndex = 5,
                    Remark = "顶级部门",
                    Children = new List<Dept> {
                        new Dept
                        {
                            Name = "人事部",
                            SortIndex = 1,
                            Remark = "二级部门"
                        },
                        new Dept
                        {
                            Name = "后勤部",
                            SortIndex = 2,
                            Remark = "二级部门"
                        },
                        new Dept
                        {
                            Name = "运输部",
                            SortIndex = 3,
                            Remark = "二级部门",
                            Children = new List<Dept>{
                                new Dept{
                                    Name = "省内运输部",
                                    SortIndex = 1,
                                    Remark = "三级部门",
                                },
                                new Dept{
                                    Name = "国内运输部",
                                    SortIndex = 2,
                                    Remark = "三级部门",
                                },
                                new Dept{
                                    Name = "国际运输部",
                                    SortIndex = 3,
                                    Remark = "三级部门",
                                }
                            }
                        }
                    }
                }
            };

            return depts;
        }



    }
}