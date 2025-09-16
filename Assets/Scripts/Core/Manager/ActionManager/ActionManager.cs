// ******************************************************************
//@file         ActionManager.cs
//@brief        简易action系统
//@author       yufulao, yufulao@qq.com
//@createTime   2025.07.27 22:43:08 
// ******************************************************************

using System;
using System.Collections.Generic;

namespace Yu
{
    public partial class ActionManager : BaseSingleTon<ActionManager> ,IMonoManager
    {
        private readonly Dictionary<int, Action<string[]>> _actionDict = new();
        
        public void OnInit()
        {
            LoadInitAtomAction();
            
            // ExecuteAction(99999);
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
        /// 执行action
        /// </summary>
        public void ExecuteAction(int id)
        {
            var rowCfgAction = ConfigManager.Tables.CfgAction.GetOrDefault(id);
            if (rowCfgAction == null)
            {
                GameLog.Error("未配置ActionId: ", id);
                return;
            }
            
            for (var i = 0; i < rowCfgAction.AtomActionList.Count; i++)
            {
                var atomActionId = rowCfgAction.AtomActionList[i];
                ExecuteAtomAction(atomActionId, rowCfgAction.ParamList[i]);
            }
        }
        
        /// <summary>
        /// 注册原子事件
        /// </summary>
        public void RegisterAtomAction(int atomActionId, Action<string[]> atomAction)
        {
            _actionDict[atomActionId] = atomAction;
        }
        
        /// <summary>
        /// 移除原子事件
        /// </summary>
        public void RemoveAtomAction(int atomActionId)
        {
            _actionDict.Remove(atomActionId);
        }
        
        /// <summary>
        /// 执行原子事件
        /// </summary>
        public void ExecuteAtomAction(int atomActionId, params string[] args)
        {
            if (!_actionDict.TryGetValue(atomActionId, out var atomAction) || atomAction == null)
            {
                GameLog.Error("未注册actionId: ", atomActionId);
                return;
            }
            
            atomAction.Invoke(args);
        }
    }
}