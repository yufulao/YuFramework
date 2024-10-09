// ******************************************************************
//@file         FsmComponent.cs
//@brief        自身组件式状态机
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:03:54
// ******************************************************************

using System.Collections.Generic;
using UnityEngine;

namespace Yu
{
    public class FsmComponent<Owner, Key>
    {
        private readonly Owner _owner;
        private Dictionary<Key, IFsmComponentState<Owner>> _fsmStateDic = new Dictionary<Key, IFsmComponentState<Owner>>();
        protected IFsmComponentState<Owner> CurrentState;
        public Key CurrentStateKey { get; private set; }


        /// <summary>
        /// 创建fsm组件
        /// </summary>
        public FsmComponent(Owner owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// 初始化这个状态机
        /// </summary>
        /// <param name="states">状态机将持有的状态</param>
        public void SetFsm(Dictionary<Key, IFsmComponentState<Owner>> states)
        {
            _fsmStateDic = states;
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public void ChangeFsmState(Key stateKey, params object[] objs)
        {
            if (!_fsmStateDic.ContainsKey(stateKey))
            {
                Debug.LogError("fsm里没有这个状态");
                return;
            }

            CurrentStateKey = stateKey;

            CurrentState?.OnExit(_owner);
            CurrentState = _fsmStateDic[stateKey];
            CurrentState.OnEnter(_owner, objs);
        }

        /// <summary>
        /// 查询是否是指定state，递归留意避免成环
        /// </summary>
        public bool IsState(Key stateKey)
        {
            if (CurrentState == null)
            {
                return false;
            }

            if (CurrentStateKey.Equals(stateKey))
            {
                return true;
            }

            if (CurrentState is FsmComponent<Owner, Key> hfsm)
            {
                return hfsm.IsState(stateKey);
            }

            return false;
        }

        /// <summary>
        /// Update每个状态机的OnUpdate，需要实现
        /// </summary>
        public void OnUpdate(params object[] objs)
        {
            CurrentState?.OnUpdate(_owner, objs);
        }
        
    }
}