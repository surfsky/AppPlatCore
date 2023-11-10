﻿using App.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DAL
{
    /// <summary>
    /// 权限清单（权限是程序内定的，作为枚举进行存储很合适)
    /// </summary>
    /// <remarks>
    /// 用户-角色-权限 三级授权机制
    /// 权限
    ///     系统只根据权限来限制操作，如修改按钮是否可用，删除按钮是否可用等
    ///     编写页面时无需考虑角色
    ///     权限是可以预料且内置的，如产品查看、产品修改、产品新建、产品删除权限
    ///     既然是固定的，可以用枚举来描述，也便于强类型编码，避免编码变更导致权限错乱的情况
    ///     提示：初期也可以简化权限，如系统管理、产品管理；在后期再根据需求拆分为产品新建、产品修改等。
    ///     
    /// 角色
    ///     角色是权限的集合
    ///     角色和权限的关系可在后台进行配置
    ///     角色可以动态增减，可在后台进行配置
    ///     
    /// 用户
    ///     拥有多个角色
    ///     系统可根据用户拥有的角色对应的权限列表，来进行授权操作
    ///     注意：不推荐直接根据角色来授权，会导致逻辑不清晰
    /// </remarks>
    public enum Power : int
    {
        //---------------------------------------------
        // AppPlat 通用权限（000-099）
        //---------------------------------------------
        // 基础框架权限
        [UI("Core", "访问后台")]            Backend = 0,


        //---------------------------------------------
        // 管理相关(0-19)
        //---------------------------------------------
        //[UI("管理", "Admin")]               Admin = 1,          // Admin专用权限，根据需求再细分
        //[UI("管理", "配置员")]              AdminConfig = 2,
        //[UI("管理", "监管员")]              AdminMonitor = 3,
        //[UI("Admin", "开发")]             AdminDevelop = 4,           
        //[UI("Admin", "配置")]             AdminConfig = 5,            
        //[UI("Admin", "测试")]             AdminTest = 6,            



        //---------------------------------------------
        // 配置
        //---------------------------------------------
        [UI("配置", "站点配置")]            ConfigSite = 21,
        [UI("配置", "菜单管理")]            ConfigMenu = 22,
        [UI("配置", "序列号维护")]          ConfigSequence = 23,
        [UI("配置", "AI配置")]              ConfigAI = 24,


        //---------------------------------------------
        // 监管
        //---------------------------------------------
        [UI("监管", "监管")]                Monitor = 30,
        [UI("监管", "日志管理")]            MonitorLog = 31,
        [UI("监管", "在线用户管理")]        MonitorOnline = 32,
        [UI("监管", "消息管理")]            MonitorMessage = 33,
        [UI("监管", "资源管理")]            MonitorRes = 34,
        [UI("监管", "IP黑名单维护")]        MonitorIP = 35,


        //---------------------------------------------
        // 用户相关
        //---------------------------------------------
        [UI("用户", "用户新增")]            UserNew = 50,
        [UI("用户", "用户浏览")]            UserView = 51,
        [UI("用户", "用户修改")]            UserEdit = 52,
        [UI("用户", "用户删除")]            UserDelete = 53,
        [UI("用户", "用户密码管理")]        UserPassword = 54,

        //---------------------------------------------
        // 橘色相关
        //---------------------------------------------
        [UI("角色", "角色管理")]            RoleEdit = 56,
        [UI("角色", "角色权限管理")]        RolePowerEdit = 56,
        [UI("角色", "角色用户管理")]        RoleUserEdit = 57,


        //---------------------------------------------
        // 基础数据
        //---------------------------------------------
        [UI("基础数据", "部门新增")]        DeptNew = 60,
        [UI("基础数据", "部门查看")]        DeptView = 61,
        [UI("基础数据", "部门修改")]        DeptEdit = 62,
        [UI("基础数据", "部门删除")]        DeptDelete = 63,


        //---------------------------------------------
        // 文档（100-109）
        //---------------------------------------------                  
        [UI("文档", "新增")]               ArticleNew = 100,
        [UI("文档", "查看")]               ArticleView = 101,
        [UI("文档", "修改")]               ArticleEdit = 102,
        [UI("文档", "删除")]               ArticleDelete = 103,
        [UI("文档", "配置")]               ArticleConfig = 111,



        //---------------------------------------------
        // 报表相关权限（900-999）
        //---------------------------------------------
        // 报表
        [UI("报表", "报表")]            Report = 900,      // 报表管理大权限，根据需要再细分
        [UI("报表", "报表新增")]        ReportNew = 901,
        [UI("报表", "报表查看")]        ReportView = 902,
        [UI("报表", "报表管理")]        ReportEdit = 903,
        [UI("报表", "报表删除")]        ReportDelete = 904,
    }
}
