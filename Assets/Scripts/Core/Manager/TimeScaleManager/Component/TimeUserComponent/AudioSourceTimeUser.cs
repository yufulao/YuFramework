// ******************************************************************
//@file         AudioSourceTimeUser.cs
//@brief        timeUser封装的audioSource
//@author       yufulao, yufulao@qq.com
//@createTime   2024.06.23 19:53:49
// ******************************************************************

using UnityEngine;

namespace Yu
{
    public class AudioSourceTimeUser: ComponentTimeUser<AudioSource>
    {
        private float _pitch;

        public AudioSourceTimeUser(TimeUser timeUser, AudioSource component) : base(timeUser, component) { }

        protected override void CopyProperties(AudioSource source)
        {
            _pitch = source.pitch;
        }

        protected override void AdjustProperties(float timeScale)
        {
            Component.pitch = _pitch * timeScale;
        }
        
        /// <summary>
        /// 设置音调
        /// </summary>
        /// <returns></returns>
        public void SetPitch(float value)
        {
            _pitch = value;
            AdjustProperties();
        }
    }
}