using App.Models;
using FineUICore;
using System.Collections.Generic;
using System;

namespace App.Components
{
    /// <summary>
    /// UI 相关的控制方法
    /// </summary>
    public class UI
    {
        protected readonly static List<string> VALID_FILE_TYPES = new List<string> { "jpg", "bmp", "gif", "jpeg", "png" };

        /// <summary>上传文件类型校验</summary>
        public static bool ValidateFileType(string fileName)
        {
            string fileType = String.Empty;
            int lastDotIndex = fileName.LastIndexOf(".");
            if (lastDotIndex >= 0)
                fileType = fileName.Substring(lastDotIndex + 1).ToLower();

            return VALID_FILE_TYPES.Contains(fileType);
        }


        //-----------------------------------------------------------------
        // 对话框
        //-----------------------------------------------------------------
        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        public static void ShowNotify(string message)
        {
            ShowNotify(message, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageIcon"></param>
        public static void ShowNotify(string message, MessageBoxIcon messageIcon)
        {
            ShowNotify(message, messageIcon, Target.Top);
        }

        /// <summary>
        /// 显示通知对话框
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageIcon"></param>
        /// <param name="target"></param>
        public static void ShowNotify(string message, MessageBoxIcon messageIcon, Target target)
        {
            Notify n = new Notify
            {
                Target = target,
                Message = message,
                MessageBoxIcon = messageIcon,
                PositionX = Position.Center,
                PositionY = Position.Top,
                DisplayMilliseconds = 3000,
                ShowHeader = false
            };

            n.Show();
        }

        //-----------------------------------------------------------------
        // 模拟树的下拉列表
        //-----------------------------------------------------------------
        public static List<T> ResolveDDL<T>(List<T> mys) where T : ICustomTree, ICloneable, IKeyID, new()
        {
            return ResolveDDL<T>(mys, -1, true);
        }

        public static List<T> ResolveDDL<T>(List<T> mys, int currentId) where T : ICustomTree, ICloneable, IKeyID, new()
        {
            return ResolveDDL<T>(mys, currentId, true);
        }


        // 将一个树型结构放在一个下列列表中可供选择
        public static List<T> ResolveDDL<T>(List<T> source, int currentID, bool addRootNode) where T : ICustomTree, ICloneable, IKeyID, new()
        {
            List<T> result = new List<T>();

            if (addRootNode)
            {
                // 添加根节点
                T root = new T
                {
                    Name = "--根节点--",
                    ID = -1,
                    TreeLevel = 0,
                    Enabled = true
                };
                result.Add(root);
            }

            foreach (T item in source)
            {
                T newT = (T)item.Clone();
                result.Add(newT);

                // 所有节点的TreeLevel加一
                if (addRootNode)
                {
                    newT.TreeLevel++;
                }
            }

            // currentId==-1表示当前节点不存在
            if (currentID != -1)
            {
                // 本节点不可点击（也就是说当前节点不可能是当前节点的父节点）
                // 并且本节点的所有子节点也不可点击，你想如果当前节点跑到子节点的子节点，那么这些子节点就从树上消失了
                bool startChileNode = false;
                int startTreeLevel = 0;
                foreach (T my in result)
                {
                    if (my.ID == currentID)
                    {
                        startTreeLevel = my.TreeLevel;
                        my.Enabled = false;
                        startChileNode = true;
                    }
                    else
                    {
                        if (startChileNode)
                        {
                            if (my.TreeLevel > startTreeLevel)
                            {
                                my.Enabled = false;
                            }
                            else
                            {
                                startChileNode = false;
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
