// ******************************************************************
//@file         AnimatorTimeUser.cs
//@brief        timeUser封装的animator
//@author       yufulao, yufulao@qq.com
//@createTime   2024.06.23 19:52:42
// ******************************************************************

using UnityEngine;

namespace Yu
{
    public class AnimatorTimeUser: ComponentTimeUser<Animator>
    {
        public AnimatorTimeUser(TimeUser timeUser, Animator component) : base(timeUser, component) { }
        private float _speed;

        protected override void CopyProperties(Animator source)
		{
			_speed = source.speed;
		}

        protected override void AdjustProperties(float timeScale)
		{
			if (timeScale > 0)
			{
				Component.speed = _speed * timeScale;
				return;
			}
			Component.speed = 0;
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
    }
}