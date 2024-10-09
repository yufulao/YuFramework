// ******************************************************************
//@file         ParticleSystemTimeUser.cs
//@brief        timeUser封装的particleSystem
//@author       yufulao, yufulao@qq.com
//@createTime   2024.06.23 20:10:32
// ******************************************************************

using UnityEngine;

namespace Yu
{
    public class ParticleSystemTimeUser: ComponentTimeUser<ParticleSystem>
    {
        public ParticleSystemTimeUser(TimeUser timeUser, ParticleSystem component) : base(timeUser, component) { }

        public float Time
        {
            get => Component.time;
            set => Component.time = value;
        }

        public bool EnableEmission
        {
            get => Component.emission.enabled;
            set
            {
                var emission = Component.emission;
                emission.enabled = value;
            }
        }

        public bool IsPlaying => Component.isPlaying;
        public bool IsPaused => Component.isPaused;
        public bool IsStopped => Component.isStopped;

        public void Play(bool withChildren = true)
        {
            Component.Play(withChildren);
        }

        public void Pause(bool withChildren = true)
        {
            Component.Pause(withChildren);
        }

        public void Stop(bool withChildren = true)
        {
            Component.Stop(withChildren);
        }

        public bool IsAlive(bool withChildren = true)
        {
            return Component.IsAlive(withChildren);
        }
    }
}