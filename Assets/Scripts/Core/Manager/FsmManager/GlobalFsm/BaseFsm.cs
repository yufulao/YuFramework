// ******************************************************************
//@file         BaseFsm.cs
//@brief        状态机基类
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:07:55
// ******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yu
{
    public abstract class BaseFsm
    {
        private Dictionary<Type, IFsmState> _fsmStateDic = new();
        protected IFsmState CurrentState;

        /// <summary>
        /// 初始化这个状态机
        /// </summary>
        /// <param name="states">状态机将持有的状态</param>
        public void SetFsm(Dictionary<Type, IFsmState> states)
        {
            _fsmStateDic = states;
        }

        /// <summary>
        /// 设置为空状态
        /// </summary>
        public void ChangeToNullState()
        {
            CurrentState?.OnExit();
            CurrentState = null;
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="stateName">状态机名Enum</param>
        /// <param name="enterParams">进入状态的参数</param>
        public void ChangeFsmState(Type stateName, params object[] enterParams)
        {
            if (!_fsmStateDic.ContainsKey(stateName))
            {
                GameLog.Info("fsm里没有这个状态");
                return;
            }

            CurrentState?.OnExit();
            CurrentState = _fsmStateDic[stateName];
            CurrentState.OnEnter(enterParams);
        }
        
        /// <summary>
        /// 查询是否是指定state，递归留意避免成环
        /// </summary>
        public bool IsState(Type state)
        {
            if (CurrentState==null)
            {
                return false;
            }
            
            if (_fsmStateDic.TryGetValue(state, out var fsmState))
            {
                return CurrentState == fsmState;
            }

            if (CurrentState is BaseFsm hfsm)
            {
                return hfsm.IsState(state);
            }

            return false;
        }

        /// <summary>
        /// Update,fsmManager中实现
        /// </summary>
        public void OnFsmUpdate()
        {
            CurrentState?.OnUpdate();
        }

        /// <summary>
        /// FixedUpdate,fsmManager中实现
        /// </summary>
        public void OnFsmFixedUpdate()
        {
            CurrentState?.OnFixedUpdate();
        }
    }
}