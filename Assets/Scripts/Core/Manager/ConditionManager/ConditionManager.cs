// ******************************************************************
//@file         ConditionManager.cs
//@brief        条件管理器
//@author       yufulao, yufulao@qq.com
//@createTime   2025.08.12 13:52:34 
// ******************************************************************

using System;
using System.Collections.Generic;

namespace Yu
{
    public partial class ConditionManager : BaseSingleTon<ConditionManager>, IMonoManager
    {
        private static readonly Dictionary<int, Func<bool>> _conditionDict = new(); //条件
        private static readonly Dictionary<int, Func<string[], bool>> _atomConditionDict = new(); //原子条件

        public void OnInit()
        {
            LoadAllCondition();
            LoadInitAtomCondition();

            // GameLog.Info(ExecuteCondition(99995));
            // GameLog.Info(ExecuteCondition(99996));
            // GameLog.Info(ExecuteCondition(99997));
            // GameLog.Info(ExecuteCondition(99998));
            // GameLog.Info(ExecuteCondition(99999));
        }

        public void Update()
        {
        }

        public void FixedUpdate()
        {
        }

        public void LateUpdate()
        {
        }

        public void OnClear()
        {
        }
        
        /// <summary>
        /// 注册原子条件
        /// </summary>
        public static void RegisterAtomCondition(int conditionId, Func<string[], bool> func)
        {
            _atomConditionDict[conditionId] = func;
        }
        
        /// <summary>
        /// 取消注册原子条件
        /// </summary>
        public static void RemoveAtomCondition(int conditionId, Func<string[], bool> func)
        {
            if (_atomConditionDict.TryGetValue(conditionId, out var funcExist) && funcExist == func)
            {
                _atomConditionDict[conditionId] = null;
            }
        }
        
        /// <summary>
        /// 执行条件
        /// </summary>
        public static bool ExecuteCondition(int conditionId)
        {
            return _conditionDict.TryGetValue(conditionId, out var func) && func();
        }
        
        /// <summary>
        /// 获取原子条件
        /// </summary>
        public static bool GetAtomCondition(int conditionId, out Func<string[], bool> func)
        {
            return _atomConditionDict.TryGetValue(conditionId, out func);
        }

        /// <summary>
        /// 加载配表所有条件
        /// </summary>
        private static void LoadAllCondition()
        {
            var conditionList = ConfigManager.Tables.CfgCondition.DataList;
            foreach (var rowCfgCondition in conditionList)
            {
                var compiler = new ConditionCompiler(rowCfgCondition.Condition, rowCfgCondition.ParamList);
                _conditionDict[rowCfgCondition.Id] = compiler.Compile();
            }
        }
    }
}