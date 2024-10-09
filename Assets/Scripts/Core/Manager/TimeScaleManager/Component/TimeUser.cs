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
        private readonly List<IComponentTimeUser> _components = new List<IComponentTimeUser>();
        private AnimationTimeUser _animation;
        private AnimatorTimeUser _animator;
        private readonly List<AudioSourceTimeUser> _audioSources = new List<AudioSourceTimeUser>();
        private NavMeshAgentTimeUser _navMeshAgent;
        private ParticleSystemTimeUser _particleSystem;
        private RigidbodyTimeUser3D _rigidbody;
        private RigidbodyTimeUser2D _rigidbody2D;
        private TransformTimeUser _transform;

        //time
        [HideInInspector] public float timeScale = 1;
        [HideInInspector] public float lastTimeScale = 1;
        [HideInInspector] public float deltaTime;
        [HideInInspector] public float fixedDeltaTime;
        public float SmoothDeltaTime => (deltaTime + _previousDeltaTimes.Sum()) / (_previousDeltaTimes.Count + 1);
        private float _time; //自创建此时间线以来的时间（以秒为单位）
        private float _unscaledTime; //自创建此时间线以来的未缩放时间（以秒为单位）
        private readonly Queue<float> _previousDeltaTimes = new Queue<float>();
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
            _previousDeltaTimes.Clear();
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
        /// <returns></returns>
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

            Debug.LogError("没有timeHolder:" + timeHolderKey);
            return null;
        }

        #region ComponentCache

        /// <summary>
        /// 缓存AnimatorTimeUser
        /// </summary>
        private void SetAnimator()
        {
            var animatorComponent = GetComponent<Animator>();
            if (_animator == null && animatorComponent)
            {
                _animator = new AnimatorTimeUser(this, animatorComponent);
                _animator.Initialize();
                _components.Add(_animator);
                return;
            }

            if (_animator != null && animatorComponent == null)
            {
                _animator = null;
            }
        }

        /// <summary>
        /// 缓存AnimationTimeUser
        /// </summary>
        private void SetAnimation()
        {
            var animationComponent = GetComponent<Animation>();
            if (_animation == null && animationComponent)
            {
                _animation = new AnimationTimeUser(this, animationComponent);
                _animation.Initialize();
                _components.Add(_animation);
                return;
            }

            if (_animation != null && !animationComponent)
            {
                _animation = null;
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
            for (var i = 0; i < _audioSources.Count; i++)
            {
                var audioSourceTemp = _audioSources[i];
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

                _audioSources.Remove(audioSourceTemp);
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
                foreach (var audioSourceTimeUser in _audioSources)
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
                _audioSources.Add(newAudioSource);
                _components.Add(newAudioSource);
            }
        }

        /// <summary>
        /// 缓存NavMeshAgent
        /// </summary>
        private void SetNavMeshAgent()
        {
            var navMeshAgentComponent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (_navMeshAgent == null && navMeshAgentComponent)
            {
                _navMeshAgent = new NavMeshAgentTimeUser(this, navMeshAgentComponent);
                _navMeshAgent.Initialize();
                _components.Add(_navMeshAgent);
                return;
            }

            if (_animation != null && !navMeshAgentComponent)
            {
                _navMeshAgent = null;
            }
        }

        /// <summary>
        /// 缓存ParticleSystem
        /// </summary>
        private void SetParticleSystem()
        {
            var particleSystemComponent = GetComponent<ParticleSystem>();
            if (_particleSystem == null && particleSystemComponent)
            {
                _particleSystem = new ParticleSystemTimeUser(this, particleSystemComponent);
                _particleSystem.Initialize();
                _components.Add(_particleSystem);
                return;
            }

            if (_particleSystem != null && !particleSystemComponent)
            {
                _particleSystem = null;
            }
        }

        /// <summary>
        /// 缓存Rigidbody
        /// </summary>
        private void SetRigidbody()
        {
            var rigidbodyComponent = GetComponent<Rigidbody>();
            var rigidbody2DComponent = GetComponent<Rigidbody2D>();
            if (_rigidbody == null && rigidbodyComponent)
            {
                _rigidbody = new RigidbodyTimeUser3D(this, rigidbodyComponent);
                _rigidbody.Initialize();
                _components.Add(_rigidbody);
                _rigidbody2D = null;
                _transform = null;
                return;
            }

            if (_rigidbody2D != null || !rigidbody2DComponent)
            {
                return;
            }

            _rigidbody2D = new RigidbodyTimeUser2D(this, rigidbody2DComponent);
            _rigidbody2D.Initialize();
            _components.Add(_rigidbody2D);
            _rigidbody = null;
            _transform = null;
        }

        /// <summary>
        /// 缓存Transform
        /// </summary>
        private void SetTransform()
        {
            var transformComponent = GetComponent<Transform>();
            if (_transform != null)
            {
                return;
            }

            _transform = new TransformTimeUser(this, transformComponent);
            _transform.Initialize();
            _components.Add(_transform);
            _rigidbody = null;
            _rigidbody2D = null;
        }

        #endregion
    }
}