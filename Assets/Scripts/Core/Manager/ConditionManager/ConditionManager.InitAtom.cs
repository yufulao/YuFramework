// ******************************************************************
//@file         AtomCondition.cs
//@brief        注册初始原子条件
//@author       yufulao, yufulao@qq.com
//@createTime   2025.08.12 14:18:12 
// ******************************************************************

using System;
using System.Collections.Generic;

namespace Yu
{
    public partial class ConditionManager
    {
        /// <summary>
        /// 加载所有初始函数
        /// </summary>
        public void LoadInitAtomCondition()
        {
            RegisterAtomCondition(1, AtomCondition1);
            RegisterAtomCondition(2, AtomCondition2);
        }

        /// <summary>
        /// 返回同bool
        /// </summary>
        private bool AtomCondition1(IReadOnlyList<string> args)
        {
            // GameLog.Error(args[0]);
            return args.Count >= 1 && bool.Parse(args[0]);
        }

        /// <summary>
        /// p1+p2=p3
        /// </summary>
        private bool AtomCondition2(IReadOnlyList<string> args)
        {
            if (args.Count < 3)
            {
                GameLog.Error("条件参数不对，当前参数个数: ", args.Count, " 需要个数: ", 3);
                return false;
            }

            var p1 = float.Parse(args[0]);
            var p2 = float.Parse(args[1]);
            var p3 = float.Parse(args[2]);
            // GameLog.Error(p1, p2, p3);
            return Math.Abs(p1 + p2 - p3) < 0.001f;
        }
    }
}