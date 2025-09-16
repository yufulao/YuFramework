// ******************************************************************
//@file         VirtualCameraController.cs
//@brief        虚拟相机控制器
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.29 14:25:32
// ******************************************************************

using System;
using System.Collections;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Yu
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class VirtualCameraController : MonoBehaviour
    {
        private CinemachineVirtualCamera _vCam; //虚拟摄像机
        private CinemachineConfiner _confiner; //虚拟摄像机的限制collider
        private CinemachineBasicMultiChannelPerlin _perlin; //虚拟摄像机的噪声组件
        [Header("移动")] private CinemachineFramingTransposer _transposer; //摄像机的trans组件
        private Coroutine _screenOffsetCoroutine; //当前的shake协程
        private Coroutine _moveCoroutine; //当前的move协程
        private float _originalLookaheadTime;//初始对准follower的缓动时长
        private float _originalDeadZoneWidth; //初始死区宽
        private float _originalDeadZoneHeight; //初始死区高
        private float _originalDamping; //初始平滑时间
        [Header("摇晃")] public float defaultShakeAmplitude = 0.5f; //默认振幅
        public float defaultShakeFrequency = 10f; //默认频率
        public float idleAmplitude; //相机闲置时噪声的幅度
        public float idleFrequency = 1f; //相机闲置时噪声的频率
        public float lerpSpeed = 5f; //插值抖动的速度
        private float _currentAmplitude; //晃动幅度
        private float _currentFrequency; //晃动频率
        private Coroutine _shakeCoroutine; //当前的shake协程
        [Header("缩放")] public AnimationCurve zoomCurve; //要应用于缩放过渡的动画曲线
        private bool _zooming; //当前是否正在缩放
        private CameraZoomMode _zoomMode; //当前的缩放模式
        private float _initialFieldOfView; //游戏一开始保存的视野值
        private float _startFieldOfView; //缩放开始的视野值，每次缩放都更新
        private float _targetFieldOfView; //缩放的目标视野值
        private float _zoomDuration; //当前缩放持续时间
        private float _zoomDelta; //当前缩放的每帧变量

        private void Awake()
        {
            _vCam = GetComponent<CinemachineVirtualCamera>();
            _confiner = GetComponent<CinemachineConfiner>();
            _perlin = _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _transposer = _vCam.GetCinemachineComponent<CinemachineFramingTransposer>();
            //_initialFieldOfView = _vCam.m_Lens.FieldOfView;//3D
            _initialFieldOfView = _vCam.m_Lens.OrthographicSize; //2D
        }

        private void Start()
        {
            if (_perlin != null)
            {
                idleAmplitude = _perlin.m_AmplitudeGain;
                idleFrequency = _perlin.m_FrequencyGain;
            }

            _currentAmplitude = idleAmplitude;
            _currentFrequency = idleFrequency;
        }

        private void Update()
        {
            UpdateShake();
            UpdateZoom();
        }

        /// <summary>
        /// 设置follower
        /// </summary>
        public void RegisterFollower(Transform follower)
        {
            _vCam.Follow = follower;
            var followerPosition = follower.position;
            var targetPosition = new Vector3(followerPosition.x, followerPosition.y, -30f); //todo 为什么z是-30f
            _vCam.ForceCameraPosition(targetPosition, follower.rotation);
        }

        /// <summary>
        /// 设置confiner
        /// </summary>
        public void RegisterConfiner(Collider confiner)
        {
            if (!_confiner)
            {
                GameLog.Info(transform.name + "虚拟摄像机没有confiner");
                return;
            }

            _confiner.m_BoundingVolume = confiner;
        }

        /// <summary>
        /// 设置摄像机偏移值
        /// </summary>
        public void SetScreenOffset(float xOffset, float yOffset, float duration = 0f)
        {
            if (_screenOffsetCoroutine != null)
            {
                StopCoroutine(_screenOffsetCoroutine);
            }

            if (!isActiveAndEnabled)
            {
                _transposer.m_ScreenX = xOffset;
                _transposer.m_ScreenY = yOffset;
                return;
            }
            
            _screenOffsetCoroutine = StartCoroutine(SetScreenOffsetCo(xOffset, yOffset, duration));
        }

        /// <summary>
        /// 瞬时对准follower
        /// </summary>
        public void AimFollower()
        {
            var follower = _vCam.Follow;
            if (!follower)
            {
                return;
            }
            
            _transposer.ForceCameraPosition(follower.position, transform.rotation);
        }

        /// <summary>
        /// 死区为0，在指定持续时间内强制锁定摄像机的follow行为，配合RegisterFollower完成摄像机移动
        /// </summary>
        public void LockFollower(float duration, bool lockDeadZone = true)
        {
            //记录初始值
            _originalLookaheadTime = _transposer.m_LookaheadTime;
            _originalDeadZoneWidth = _transposer.m_DeadZoneWidth;
            _originalDeadZoneHeight = _transposer.m_DeadZoneHeight;
            _originalDamping = _transposer.m_XDamping;
            //设置Dead Zone和缓动时间
            if (lockDeadZone)
            {
                _transposer.m_DeadZoneWidth = 0;
                _transposer.m_DeadZoneHeight = 0;
            }
            
            _transposer.m_LookaheadTime = duration;
            _transposer.m_XDamping = duration;
            _transposer.m_YDamping = duration;
            _transposer.m_ZDamping = duration;
        }

        /// <summary>
        /// 取消锁定目标
        /// </summary>
        public void CancelLockFollower()
        {
            _transposer.m_LookaheadTime = _originalLookaheadTime;
            _transposer.m_DeadZoneWidth = _originalDeadZoneWidth;
            _transposer.m_DeadZoneHeight = _originalDeadZoneHeight;
            _transposer.m_XDamping = _originalDamping;
            _transposer.m_YDamping = _originalDamping;
            _transposer.m_ZDamping = _originalDamping;
        }

        /// <summary>
        /// 停止摇晃摄像机，将相机的噪点值重置为空闲值
        /// </summary>
        public void StopShakeCamera()
        {
            _currentAmplitude = idleAmplitude;
            _currentFrequency = idleFrequency;
        }

        /// <summary>
        /// 以默认值shake
        /// </summary>
        public void ShakeCamera(float duration, bool infinite)
        {
            StartCoroutine(ShakeCameraCo(duration, defaultShakeAmplitude, defaultShakeFrequency, infinite));
        }

        /// <summary>
        /// 在指定的时间内（秒）、振幅和频率内摇晃相机
        /// </summary>
        /// <param name="duration">持续时间</param>
        /// <param name="amplitude">振幅.</param>
        /// <param name="frequency">频率.</param>
        /// <param name="infinite">是否一直持续</param>
        public void ShakeCamera(float duration, float amplitude, float frequency, bool infinite)
        {
            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
            }

            _shakeCoroutine = StartCoroutine(ShakeCameraCo(duration, amplitude, frequency, infinite));
        }

        /// <summary>
        /// 进行摄像机缩放
        /// </summary>
        /// <param name="mode">缩放模式</param>
        /// <param name="newFieldOfView">新的视野值</param>
        /// <param name="duration">缩放时间</param>
        /// <param name="relative">是否是相对于初始视野值的</param>
        public void Zoom(CameraZoomMode mode, float newFieldOfView, float duration, bool relative = false)
        {
            if (_zooming)
            {
                return;
            }

            _zooming = true;
            _zoomDelta = 0f;
            _zoomMode = mode;
            //_startFieldOfView = _vCam.m_Lens.FieldOfView;//3D
            _startFieldOfView = _vCam.m_Lens.OrthographicSize; //2D
            _zoomDuration = duration;

            switch (mode)
            {
                case CameraZoomMode.Set:
                    _targetFieldOfView = newFieldOfView;
                    break;
                case CameraZoomMode.Reset:
                    _targetFieldOfView = _initialFieldOfView;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, "没有处理这个Zoom模式的缩放方式");
            }

            if (relative)
            {
                _targetFieldOfView += _initialFieldOfView;
            }
        }
        
        /// <summary>
        /// 设置摄像机偏移值的协程
        /// </summary>
        private IEnumerator SetScreenOffsetCo(float xOffset, float yOffset, float duration)
        {
            var elapsedTime = 0f;
            //逐帧插值移动摄像机
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                var curTimePoint = elapsedTime / duration;
                _transposer.m_ScreenX = Mathf.Lerp(_transposer.m_ScreenX, xOffset, curTimePoint);
                _transposer.m_ScreenY = Mathf.Lerp(_transposer.m_ScreenY, yOffset, curTimePoint);
                yield return null;
            }
        }

        /// <summary>
        /// ShakeCamera的协程 
        /// </summary>
        private IEnumerator ShakeCameraCo(float duration, float amplitude, float frequency, bool infinite)
        {
            _currentAmplitude = amplitude;
            _currentFrequency = frequency;
            if (infinite)
            {
                yield break;
            }

            yield return new WaitForSeconds(duration);
            StopShakeCamera();
        }

        /// <summary>
        /// 每帧调用摇晃
        /// </summary>
        private void UpdateShake()
        {
            if (!_perlin)
            {
                return;
            }

            _perlin.m_AmplitudeGain = _currentAmplitude;
            _perlin.m_FrequencyGain = Mathf.Lerp(_perlin.m_FrequencyGain, _currentFrequency, lerpSpeed * Time.deltaTime);
        }

        /// <summary>
        /// 每帧调用缩放
        /// </summary>
        private void UpdateZoom()
        {
            if (!_zooming)
            {
                return;
            }

            //进行缩放3D
            // if (Math.Abs(_vCam.m_Lens.FieldOfView - _targetFieldOfView) > 0f)
            // {
            //     _zoomDelta += Time.deltaTime / _zoomDuration;
            //     _vCam.m_Lens.FieldOfView = Mathf.LerpUnclamped(_startFieldOfView, _targetFieldOfView, zoomCurve.Evaluate(_zoomDelta));
            //     return;
            // }
            //进行缩放2D
            if (Math.Abs(_vCam.m_Lens.OrthographicSize - _targetFieldOfView) > 0f)
            {
                _zoomDelta += Time.deltaTime / _zoomDuration;
                _vCam.m_Lens.OrthographicSize = Mathf.LerpUnclamped(_startFieldOfView, _targetFieldOfView, zoomCurve.Evaluate(_zoomDelta));
                return;
            }

            _zooming = false;
        }

        /// <summary>
        /// 测试摇晃
        /// </summary>
        [Button("测试摇晃")]
        private void TestShake()
        {
            ShakeCamera(0.1f, 1f, 40f, false);
        }

        /// <summary>
        /// 测试缩放
        /// </summary>
        [Button("测试缩放")]
        private void TestZoom()
        {
            Zoom(CameraZoomMode.Set, 8f, 0.1f, false);
        }
    }
}