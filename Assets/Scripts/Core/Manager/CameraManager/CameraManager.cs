// ******************************************************************
//@file         CameraManager.cs
//@brief        摄像机管理器
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:28:46
// ******************************************************************
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using DG.Tweening;

namespace Yu
{
    public class CameraManager : BaseSingleTon<CameraManager>, IMonoManager
    {
        private readonly Dictionary<string,VirtualCameraController> _vCamDic = new Dictionary<string,VirtualCameraController>();
        private Transform _cameraContainer;
        private Transform _virtualCameraContainer;
        private Camera _objCamera;
        private Camera _uiCamera;
        private Sequence _objSequence;
        private Camera _cacheMainObjCamera;


        public void OnInit()
        {
            _cameraContainer = GameObject.Find("CameraContainer").transform;
            _virtualCameraContainer = _cameraContainer.Find("VCamContainer");
            _objCamera = _cameraContainer.Find("ObjCamera").GetComponent<Camera>();
            _cacheMainObjCamera = _objCamera;
            _uiCamera = _cameraContainer.Find("UICamera").GetComponent<Camera>();
            EventManager.Instance.AddListener(EventName.ChangeScene, OnChangeScene);
            LoadVCam();
        }

        public void Update()
        {
        }

        public void FixedUpdate()
        {
        }

        public void LateUpdate()
        {
        }

        public void OnClear()
        {
            _objSequence?.Kill();
        }

        /// <summary>
        /// 重置原来的objCamera
        /// </summary>
        public void ResetObjCamera()
        {
            if (_objCamera)
            {
                _objCamera.gameObject.SetActive(false);
            }

            _objCamera = _cacheMainObjCamera;
            _objCamera.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// 获取虚拟摄像机controller
        /// </summary>
        /// <param name="vCamName"></param>
        public VirtualCameraController GetVCamController(string vCamName)
        {
            if (_vCamDic.ContainsKey(vCamName))
            {
                return _vCamDic[vCamName];
            }
            Debug.LogWarning("没有这个虚拟相机"+vCamName);
            return null;

        }

        /// <summary>
        /// 移动摄像机
        /// </summary>
        /// <param name="cameraName"></param>
        /// <param name="during"></param>
        /// <returns></returns>
        public IEnumerator MoveObjCamera(string cameraName, float during = 0f)
        {
            if (!_objCamera)
            {
                yield break;
            }

            var objCameraTransform = _objCamera.gameObject.transform;
            var rowCfgCamera = ConfigManager.Tables.CfgCamera[cameraName];
            var cameraPosition = rowCfgCamera.Position;
            var cameraRotation = rowCfgCamera.Rotation;
            var cameraFieldOfView = rowCfgCamera.FieldOfView;
            _objSequence?.Kill();
            _objSequence = DOTween.Sequence();
            _objSequence.Join(_objCamera.transform.DOLocalMove(cameraPosition, during));
            _objSequence.Join(_objCamera.transform.DOLocalRotateQuaternion(Quaternion.Euler(cameraRotation), during));
            _objSequence.Join(_objCamera.DOFieldOfView(cameraFieldOfView, during));
            _objSequence.SetAutoKill(false);
            yield return _objSequence.WaitForCompletion();
        }

        /// <summary>
        /// 获取Obj摄像机
        /// </summary>
        /// <returns></returns>
        public Camera GetObjCamera()
        {
            return _objCamera;
        }
        
        /// <summary>
        /// 获取UI摄像机
        /// </summary>
        /// <returns></returns>
        public Camera GetUICamera()
        {
            return _uiCamera;
        }
        
        /// <summary>
        /// 当场景切换时
        /// </summary>
        private void OnChangeScene()
        {
            _objSequence?.Kill();
            // _cacheMainObjCamera.gameObject.SetActive(false);
            // _objCamera = Utils.FindTByName<Camera>("Main Camera") ?? _cacheMainObjCamera;
            // _objCamera.gameObject.SetActive(true);
        }

        /// <summary>
        /// 加载虚拟摄像机
        /// </summary>
        private void LoadVCam()
        {
            for (var i = 0; i < _virtualCameraContainer.childCount; i++)
            {
                var transform = _virtualCameraContainer.GetChild(i);
                var vCam = transform.GetComponent<VirtualCameraController>();
                var vCamName = transform.name;
                if (!vCam )
                {
                    continue;
                }
                
                if (_vCamDic.ContainsKey(vCamName))
                {
                    _vCamDic[vCamName] = vCam;
                    continue;
                }
                _vCamDic.Add(vCamName,vCam);
            }
        }
        
    }
}