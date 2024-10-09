// ******************************************************************
//@file         BaseHfsm.cs
//@brief        层次状态机
//@author       yufulao, yufulao@qq.com
//@createTime   2024.07.19 01:08:35
// ******************************************************************
namespace Yu
{
    public class BaseHfsm: BaseFsm, IFsmState
    {
        //先new HFSM，再new FSM，将FSM的State设为HFSM，否则引用null
        //ChangeState时请留意，可能成环
        
        
        public virtual void OnEnter(params object[] objs)
        {
            //不切换HFSM状态时，HFSM默认为null状态
            CurrentState?.OnEnter(objs);
        }

        /// <summary>
        /// 不需要另外调用OnUpdate，上层FSM已调用
        /// </summary>
        public virtual void OnUpdate()
        {
            CurrentState?.OnUpdate();
        }

        public virtual void OnFixedUpdate()
        {
            CurrentState?.OnFixedUpdate();
        }

        public virtual void OnExit()
        {
            CurrentState?.OnExit();
            CurrentState = null;
        }
    }
}