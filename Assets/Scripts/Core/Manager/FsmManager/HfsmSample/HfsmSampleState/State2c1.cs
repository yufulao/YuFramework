using UnityEngine;

namespace Yu
{
    public class State2c1:IFsmComponentState<HfsmSample>
    {
        public void OnEnter(HfsmSample owner, params object[] objs)
        {
            Debug.Log("OnEnter2c1");
        }

        public void OnUpdate(HfsmSample owner, params object[] objs)
        {
            Debug.Log("OnUpdate2c1");
        }

        public void OnExit(HfsmSample owner)
        {
            Debug.Log("OnExit2c1");
        }
    }
}