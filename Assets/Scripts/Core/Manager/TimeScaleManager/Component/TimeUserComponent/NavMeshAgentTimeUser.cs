// ******************************************************************
//@file         NavMeshAgentTimeUser.cs
//@brief        timeUser封装的navMeshAgent
//@author       yufulao, yufulao@qq.com
//@createTime   2024.06.23 19:55:14
// ******************************************************************

using UnityEngine;

namespace Yu
{
    public class NavMeshAgentTimeUser: ComponentTimeUser<UnityEngine.AI.NavMeshAgent>
    {
        private float _speed;
        private float _angularSpeed;

        public NavMeshAgentTimeUser(TimeUser timeUser, UnityEngine.AI.NavMeshAgent component) : base(timeUser, component) { }

        public override void Update()
        {
            if (TimeUser.lastTimeScale > 0 && TimeUser.timeScale == 0)
            {
                Component.velocity = Vector3.zero;
            }
        }

        protected override void CopyProperties(UnityEngine.AI.NavMeshAgent source)
        {
            _speed = source.speed;
            _angularSpeed = source.angularSpeed;
        }

        protected override void AdjustProperties(float timeScale)
        {
            Component.speed = _speed * timeScale;
            Component.angularSpeed = _angularSpeed * timeScale;
        }
        
        /// <summary>
        /// 设置速度
        /// </summary>
        /// <returns></returns>
        public void SetSpeed(float value)
        {
            _speed = value;
            AdjustProperties();
        }
        
        /// <summary>
        /// 设置速度
        /// </summary>
        /// <returns></returns>
        public void SetAngularSpeed(float value)
        {
            _angularSpeed = value;
            AdjustProperties();
        }
    }
}