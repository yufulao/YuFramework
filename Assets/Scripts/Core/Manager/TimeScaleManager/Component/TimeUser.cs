// ******************************************************************
//@file         TimeUser.cs
//@brief        施加TimeHolder的速率到组件上
//@author       yufulao, yufulao@qq.com
//@createTime   2024.06.20 16:30:22
// ******************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Yu
{
    public class TimeUser : MonoBehaviour
    {
        public string timeHolderKey;
        //component
        private readonly List<IComponentTimeUser> _components = new();
        [HideInInspector] public AnimationTimeUser Animation;
        [HideInInspector] public AnimatorTimeUser Animator;
        [HideInInspector] public readonly List<AudioSourceTimeUser> AudioSources = new();
        [HideInInspector] public NavMeshAgentTimeUser NavMeshAgent;
        [HideInInspector] public ParticleSystemTimeUser ParticleSystem;
        [HideInInspector] public RigidbodyTimeUser3D Rigidbody;
        [HideInInspector] public RigidbodyTimeUser2D Rigidbody2D;
        [HideInInspector] public TransformTimeUser Transform;

        //time
        [HideInInspector] public float timeScale = 1;
        [HideInInspector] public float lastTimeScale = 1;
        [HideInInspector] public float deltaTime;
        [HideInInspector] public float fixedDeltaTime;
        private float _time; //自创建此时间线以来的时间（以秒为单位）
        private float _unscaledTime; //自创建此时间线以来的未缩放时间（以秒为单位）
        private TimeHolder _timeHolder;


        private void Awake()
        {
            SetComponent();
        }

        private void Start()
        {
            OnInit();
        }

        private void OnDisable()
        {
            foreach (var comp in _components)
            {
                comp.OnDisable();
            }
        }

        private void Update()
        {
            lastTimeScale = timeScale;
            timeScale = _timeHolder.TimeScale;

            if (Math.Abs(timeScale - lastTimeScale) > 0f)
            {
                foreach (var comp in _components)
                {
                    comp.AdjustProperties();
                }
            }

            var unscaledDeltaTime = TimeScaleManager.UnscaledDeltaTime;
            deltaTime = unscaledDeltaTime * timeScale;
            fixedDeltaTime = Time.fixedDeltaTime * timeScale;
            // _time += DeltaTime;
            // _unscaledTime += unscaledDeltaTime;

            foreach (var comp in _components)
            {
                comp.Update();
            }
        }

        private void FixedUpdate()
        {
            foreach (var comp in _components)
            {
                comp.FixedUpdate();
            }
        }
        
        /// <summary>
        /// 手动重置timeUser。使用游戏对象池时，应在销毁后或生成前调用此方法。
        /// </summary>
        public void Reset()
        {
            foreach (var comp in _components)
            {
                comp.Reset();
            }

            timeScale = lastTimeScale = 1;
        }

        /// <summary>
        /// 封装gameObject自带component
        /// </summary>
        private void SetComponent()
        {
            _components.Clear();
            SetAnimator();
            SetAnimation();
            SetAudioSources();
            SetNavMeshAgent();
            SetParticleSystem();
            SetRigidbody();
            SetTransform();
        }

        /// <summary>
        /// 初始化，Start时调用
        /// </summary>
        private void OnInit()
        {
            _timeHolder = FindTimeHolder();
            timeScale = lastTimeScale = _timeHolder.TimeScale;
            foreach (var comp in _components)
            {
                comp.AdjustProperties();
                comp.OnStartOrReEnable();
            }
        }

        /// <summary>
        /// 查找TimeHolder
        /// </summary>
        private TimeHolder FindTimeHolder()
        {
            var oldTimeHolder = _timeHolder;
            if (oldTimeHolder)
            {
                oldTimeHolder.Unregister(this);
            }

            var timeHolder = TimeScaleManager.Instance.GetTimeHolder(timeHolderKey);
            if (timeHolder)
            {
                timeHolder.Register(this);
                return timeHolder;
            }

            GameLog.Error("没有timeHolder:" + timeHolderKey);
            return null;
        }

        #region ComponentCache

        /// <summary>
        /// 缓存AnimatorTimeUser
        /// </summary>
        private void SetAnimator()
        {
            var animatorComponent = GetComponent<Animator>();
            if (Animator == null && animatorComponent)
            {
                Animator = new AnimatorTimeUser(this, animatorComponent);
                Animator.Initialize();
                _components.Add(Animator);
                return;
            }

            if (Animator != null && animatorComponent == null)
            {
                Animator = null;
            }
        }

        /// <summary>
        /// 缓存AnimationTimeUser
        /// </summary>
        private void SetAnimation()
        {
            var animationComponent = GetComponent<Animation>();
            if (Animation == null && animationComponent)
            {
                Animation = new AnimationTimeUser(this, animationComponent);
                Animation.Initialize();
                _components.Add(Animation);
                return;
            }

            if (Animation != null && !animationComponent)
            {
                Animation = null;
            }
        }

        /// <summary>
        /// 缓存所有AudioSourceTimeUser
        /// </summary>
        private void SetAudioSources()
        {
            var audioSourceComponentArray = GetComponents<AudioSource>();
            // 删除缺失组件的timeUser
            DeleteAbsentAudioSource(audioSourceComponentArray);
            // 添加AudioSourceTimeUser组件
            AddAudioSourceTimeUser(audioSourceComponentArray);
            //AudioSource = _audioSources.Count > 0 ? _audioSources[0] : null;
        }

        /// <summary>
        /// 删除缺失组件的timeUser
        /// </summary>
        private void DeleteAbsentAudioSource(AudioSource[] audioSourceComponentArray)
        {
            for (var i = 0; i < AudioSources.Count; i++)
            {
                var audioSourceTemp = AudioSources[i];
                var audioSourceComponentExists = false;
                foreach (var audioSourceComponent in audioSourceComponentArray)
                {
                    if (audioSourceTemp.Component != audioSourceComponent)
                    {
                        continue;
                    }

                    audioSourceComponentExists = true;
                    break;
                }

                if (audioSourceComponentExists)
                {
                    continue;
                }

                AudioSources.Remove(audioSourceTemp);
            }
        }

        /// <summary>
        /// 添加AudioSourceTimeUser组件
        /// </summary>
        private void AddAudioSourceTimeUser(IEnumerable<AudioSource> audioSourceComponentList)
        {
            foreach (var audioSourceComponent in audioSourceComponentList)
            {
                var audioSourceExists = false;
                foreach (var audioSourceTimeUser in AudioSources)
                {
                    if (audioSourceTimeUser.Component != audioSourceComponent)
                    {
                        continue;
                    }

                    audioSourceExists = true;
                    break;
                }

                if (audioSourceExists)
                {
                    continue;
                }

                var newAudioSource = new AudioSourceTimeUser(this, audioSourceComponent);
                newAudioSource.Initialize();
                AudioSources.Add(newAudioSource);
                _components.Add(newAudioSource);
            }
        }

        /// <summary>
        /// 缓存NavMeshAgent
        /// </summary>
        private void SetNavMeshAgent()
        {
            var navMeshAgentComponent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (NavMeshAgent == null && navMeshAgentComponent)
            {
                NavMeshAgent = new NavMeshAgentTimeUser(this, navMeshAgentComponent);
                NavMeshAgent.Initialize();
                _components.Add(NavMeshAgent);
                return;
            }

            if (Animation != null && !navMeshAgentComponent)
            {
                NavMeshAgent = null;
            }
        }

        /// <summary>
        /// 缓存ParticleSystem
        /// </summary>
        private void SetParticleSystem()
        {
            var particleSystemComponent = GetComponent<ParticleSystem>();
            if (ParticleSystem == null && particleSystemComponent)
            {
                ParticleSystem = new ParticleSystemTimeUser(this, particleSystemComponent);
                ParticleSystem.Initialize();
                _components.Add(ParticleSystem);
                return;
            }

            if (ParticleSystem != null && !particleSystemComponent)
            {
                ParticleSystem = null;
            }
        }

        /// <summary>
        /// 缓存Rigidbody
        /// </summary>
        private void SetRigidbody()
        {
            var rigidbodyComponent = GetComponent<Rigidbody>();
            var rigidbody2DComponent = GetComponent<Rigidbody2D>();
            if (Rigidbody == null && rigidbodyComponent)
            {
                Rigidbody = new RigidbodyTimeUser3D(this, rigidbodyComponent);
                Rigidbody.Initialize();
                _components.Add(Rigidbody);
                Rigidbody2D = null;
                Transform = null;
                return;
            }

            if (Rigidbody2D != null || !rigidbody2DComponent)
            {
                return;
            }

            Rigidbody2D = new RigidbodyTimeUser2D(this, rigidbody2DComponent);
            Rigidbody2D.Initialize();
            _components.Add(Rigidbody2D);
            Rigidbody = null;
            Transform = null;
        }

        /// <summary>
        /// 缓存Transform
        /// </summary>
        private void SetTransform()
        {
            var transformComponent = GetComponent<Transform>();
            if (Transform != null)
            {
                return;
            }

            Transform = new TransformTimeUser(this, transformComponent);
            Transform.Initialize();
            _components.Add(Transform);
            Rigidbody = null;
            Rigidbody2D = null;
        }

        #endregion
    }
}