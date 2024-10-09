// ******************************************************************
//@file         HfsmComponent.cs
//@brief        层次状态机
//@author       yufulao, yufulao@qq.com
//@createTime   2024.07.19 00:41:53
// ******************************************************************

namespace Yu
{
    public class HfsmComponent<Owner, Key> : FsmComponent<Owner, Key>, IFsmComponentState<Owner>
    {
        //先new HFSM，再new FSM，将FSM的State设为HFSM，否则引用null
        //ChangeState时请留意，可能成环


        protected HfsmComponent(Owner owner) : base(owner)
        {
        }

        public virtual void OnEnter(Owner owner, params object[] objs)
        {
            //不切换HFSM状态时，HFSM默认为null状态
            CurrentState?.OnEnter(owner,objs);
        }

        /// <summary>
        /// 不需要另外调用OnUpdate，上层FSM已调用
        /// </summary>
        public virtual void OnUpdate(Owner owner, params object[] objs)
        {
            CurrentState?.OnUpdate(owner,objs);
        }

        public virtual void OnExit(Owner owner)
        {
            CurrentState?.OnExit(owner);
            CurrentState = null;
        }
    }
}