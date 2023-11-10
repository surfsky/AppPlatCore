using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.DAL
{
    /// <summary>
    /// 含主键 ID
    /// </summary>
    public interface IKeyID
    {
        long ID { get; set; }

    }

    /// <summary>
    /// 含主键 ID1, ID2
    /// </summary>
    public interface IKey2ID
    {
        long ID1 { get; set; }

        long ID2 { get; set; }

    }

    /// <summary>
    /// 树节点
    /// </summary>
    public interface ITreeNode
    {
        /// <summary>名称</summary>
        string Name { get; set; }

        /// <summary>菜单在树形结构中的层级（从0开始）</summary>
        int TreeLevel { get; set; }

        /// <summary>是否可用（默认true）,在模拟树的下拉列表中使用</summary>
        bool Enabled { get; set; }

        /// <summary>是否叶子节点（默认true）</summary>
        bool IsTreeLeaf { get; set; }
    }
}