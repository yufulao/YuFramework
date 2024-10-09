using UnityEngine;

namespace Yu
{
    public class State2a:IFsmComponentState<HfsmSample>
    {
        public void OnEnter(HfsmSample owner, params object[] objs)
        {
            Debug.Log("OnEnter2a");
        }

        public void OnUpdate(HfsmSample owner, params object[] objs)
        {
            Debug.Log("OnUpdate2a");
        }

        public void OnExit(HfsmSample owner)
        {
            Debug.Log("OnExit2a");
        }
    }
}