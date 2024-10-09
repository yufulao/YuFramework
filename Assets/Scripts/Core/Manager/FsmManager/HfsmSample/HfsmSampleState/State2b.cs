using UnityEngine;

namespace Yu
{
    public class State2b:IFsmComponentState<HfsmSample>
    {
        public void OnEnter(HfsmSample owner, params object[] objs)
        {
            Debug.Log("OnEnter2b");
        }

        public void OnUpdate(HfsmSample owner, params object[] objs)
        {
            Debug.Log("OnUpdate2b");
        }

        public void OnExit(HfsmSample owner)
        {
            Debug.Log("OnExit2b");
        }
    }
}