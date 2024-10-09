// ******************************************************************
//@file         HfsmSample.cs
//@brief        层次状态机使用示例
//@author       yufulao, yufulao@qq.com
//@createTime   2024.07.19 01:53:11
// ******************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yu
{
    public class HfsmSample:MonoBehaviour
    {
        //Fsm{1,Hfsm2,3},   Hfsm2{2a,2b,Hfsm2c},   Hfsm2c{2c1,2c2}
        public FsmComponent<HfsmSample, string> Fsm;   
        public State2Hfsm Hfsm2;
        public State2cHfsm Hfsm2C;
        
        private void Update()
        {
            Fsm.OnUpdate();
        }

        private void Start()
        {
            //由上至下初始化，因为上层需要引用到下层Hfsm
            Hfsm2C = new State2cHfsm(this);
            Hfsm2C.SetFsm(new Dictionary<string, IFsmComponentState<HfsmSample>>()
            {
                {"2c1", new State2c1()},
                {"2c2", new State2c2()},
            });
            Hfsm2 = new State2Hfsm(this);
            Hfsm2.SetFsm(new Dictionary<string, IFsmComponentState<HfsmSample>>()
            {
                {"2a", new State2a()},
                {"2b", new State2b()},
                {"2c", Hfsm2C},
            });
            Fsm = new FsmComponent<HfsmSample, string>(this);
            Fsm.SetFsm(new Dictionary<string, IFsmComponentState<HfsmSample>>()
            {
                {"1", new State1()},
                {"2", Hfsm2},
                {"3", new State3()},
            });

            
            StartCoroutine(ChangeState());
        }
        
        
        private IEnumerator ChangeState()
        {
            Fsm.ChangeFsmState("1");
            
            yield return new WaitForSeconds(1f);
            
            //切换上层Hfsm和切换Hfsm的顺序不要求，切换到的状态为Hfsm时，该Hfsm默认为null状态（初始化和OnExit都设置为null当前状态）
            Fsm.ChangeFsmState("2");
            Hfsm2.ChangeFsmState("2a");
            
            yield return new WaitForSeconds(1f);
            
            Hfsm2.ChangeFsmState("2c");
            Hfsm2C.ChangeFsmState("2c1");

            Fsm.IsState("2a");
        }
    }
}