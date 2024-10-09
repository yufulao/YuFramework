using UnityEngine;

namespace Yu
{
    public class State2c2:IFsmComponentState<HfsmSample>
    {
        public void OnEnter(HfsmSample owner, params object[] objs)
        {
            Debug.Log("OnEnter2c2");
        }

        public void OnUpdate(HfsmSample owner, params object[] objs)
        {
            Debug.Log("OnUpdate2c2");
        }

        public void OnExit(HfsmSample owner)
        {
            Debug.Log("OnExit2c2");
        }
    }
}