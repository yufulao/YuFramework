// ******************************************************************
//@file         CameraManager.cs
//@brief        摄像机管理器
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:28:46
// ******************************************************************

using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using DG.Tweening;

namespace Yu
{
    public class CameraManager : BaseSingleTon<CameraManager>, IMonoManager
    {
        public CinemachineBrain CinemachineBrain;
        public Camera CurCamera => Camera.main;
        private readonly Dictionary<string, VirtualCameraController> _vCamDic = new();
        private Transform _cameraContainer;
        private Transform _virtualCameraContainer;
        private Sequence _objSequence;


        public void OnInit()
        {
            _cameraContainer = GameObject.Find("CameraContainer").transform;
            _virtualCameraContainer = _cameraContainer.Find("VCamContainer");
            EventManager.Instance.AddListener(EventName.ChangeScene, OnChangeScene);
            SetCinemachineBrain();
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
        /// 获取虚拟摄像机controller
        /// </summary>
        public VirtualCameraController GetVCamController(string vCamName)
        {
            if (_vCamDic.TryGetValue(vCamName, out var controller))
            {
                return controller;
            }

            GameLog.Warn("没有这个虚拟相机" + vCamName);
            return null;
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
                if (!vCam || _vCamDic.TryAdd(vCamName, vCam))
                {
                    continue;
                }

                _vCamDic[vCamName] = vCam;
            }
        }

        /// <summary>
        /// 设置cinemachineBrain
        /// </summary>
        private void SetCinemachineBrain()
        {
            if (Camera.main)
            {
                CinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
            }

            if (!CinemachineBrain)
            {
                GameLog.Error("找不到CinemachineBrain");
            }
        }
    }
}