// ******************************************************************
//@file         FsmManager.cs
//@brief        状态机系统
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:29:01
// ******************************************************************
using System.Collections.Generic;

namespace Yu
{
    public class FsmManager : BaseSingleTon<FsmManager>, IMonoManager
    {
        private readonly Dictionary<string, BaseFsm> _fsmDict = new();

        /// <summary>
        /// 获取状态机
        /// </summary>
        public T GetFsm<T>() where T : BaseFsm, new()
        {
            var fsmName = typeof(T).ToString();
            if (_fsmDict.TryGetValue(fsmName, out var fsm))
            {
                return fsm as T;
            }

            var newFsm = new T();
            _fsmDict.Add(fsmName, newFsm);
            return newFsm;
        }

        public void OnInit()
        {
        }

        public void Update()
        {
            foreach (var fsm in _fsmDict.Values)
            {
                fsm.OnFsmUpdate();
            }
        }

        public void FixedUpdate()
        {
            foreach (var fsm in _fsmDict.Values)
            {
                fsm.OnFsmFixedUpdate();
            }
        }

        public void LateUpdate()
        {
        }

        public void OnClear()
        {
        }
    }
}