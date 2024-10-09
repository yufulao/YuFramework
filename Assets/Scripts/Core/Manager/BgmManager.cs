// ******************************************************************
//@file         BGMManager.cs
//@brief        BGM管理器
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:28:32
// ******************************************************************

using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.Audio;

namespace Yu
{
    public class BGMManager : BaseSingleTon<BGMManager>, IMonoManager
    {
        private AudioMixer _audioMixer;
        private AudioMixerGroup _bgmMixerGroup;
        private CfgBGM _cfgBGM;
        private RowCfgBgm _rowCfgBGM;
        private AudioSource _audioSource;
        private float _cacheBaseVolume;

        public void OnInit()
        {
            _cfgBGM = ConfigManager.Tables.CfgBGM;
            _audioMixer = AssetManager.Instance.LoadAsset<AudioMixer>("Assets/AddressableAssets/AudioMixer/AudioMixer.mixer");
            _bgmMixerGroup = _audioMixer.FindMatchingGroups("BGM")[0];
            var root = new GameObject("BGMManager");
            root.transform.SetParent(GameManager.Instance.transform, false);
            _audioSource = root.AddComponent<AudioSource>();
            _audioSource.outputAudioMixerGroup = _bgmMixerGroup;
            _audioSource.playOnAwake = false;
            _cacheBaseVolume = 1f;
            //timeUser
            var timeUser = root.AddComponent<TimeUser>();
            timeUser.timeHolderKey = "Audio";
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
        }

        /// <summary>
        /// 重新设置音量，在OnInit设置，有bug，进到Start里莫名其妙变回0
        /// </summary>
        public void ReloadVolume()
        {
            //_audioMixer.SetFloat("BGMVolume", SaveGameManager.Instance.Get<float>("BGMVolume", 0f, SaveType.Cfg));
        }

        /// <summary>
        /// 播放bgm
        /// </summary>
        /// <param name="bgmName">bgm名称</param>
        /// <param name="baseVolume">bgm初始音量</param>
        public void PlayBgm(string bgmName, float baseVolume = 1f)
        {
            _audioSource.Stop();
            _rowCfgBGM = _cfgBGM[bgmName];
            var audioClip = AssetManager.Instance.LoadAsset<AudioClip>(_rowCfgBGM.AudioClipPath);
            PlayBgmByAudioClip(audioClip, baseVolume);
        }

        /// <summary>
        /// 停止播放当前bgm
        /// </summary>
        public void StopBgm()
        {
            _audioSource.Stop();
        }

        /// <summary>
        /// 延迟淡出bgm
        /// </summary>
        /// <param name="delayTime">延迟时间</param>
        /// <param name="fadeOutTime">淡出时长</param>
        /// <param name="callback">淡出至停止bgm时执行的回调</param>
        /// <returns></returns>
        public void StopBgmFadeDelay(float delayTime, float fadeOutTime, UnityAction callback = null)
        {
            GameManager.Instance.StartCoroutine(StopBgmFadeDelayCo(delayTime, fadeOutTime, callback));
        }

        /// <summary>
        /// StopBgmFadeDelay的协程
        /// </summary>
        public IEnumerator StopBgmFadeDelayCo(float delayTime, float fadeOutTime, UnityAction callback = null)
        {
            yield return new WaitForSeconds(delayTime);
            _audioSource.DOFade(0, fadeOutTime); //音量降为0
            yield return new WaitForSeconds(fadeOutTime);
            StopBgm();
            callback?.Invoke();
        }

        /// <summary>
        /// 淡出上一个bgm，等待间隔时间，淡入下一个bgm
        /// </summary>
        /// <param name="bgmName">下一个bgm的名称</param>
        /// <param name="fadeOutTime">淡出上一个bgm的时长</param>
        /// <param name="delayTime">延迟播放时间</param>
        /// <param name="fadeInTime">淡入下一个bgm的时长</param>
        /// <param name="baseVolume">初始音量</param>
        public void PlayBgmFadeDelay(string bgmName, float fadeOutTime, float delayTime, float fadeInTime, float baseVolume = 1f)
        {
            GameManager.Instance.StartCoroutine(PlayBgmFadeDelayCo(bgmName, fadeOutTime, delayTime, fadeInTime, baseVolume));
        }

        /// <summary>
        /// PlayBgmFadeDelay的协程
        /// </summary>
        public IEnumerator PlayBgmFadeDelayCo(string bgmName, float fadeOutTime, float delayTime, float fadeInTime, float baseVolume = 1f)
        {
            //Debug.Log(bgmName);
            _audioSource.loop = true;
            _audioSource.DOFade(0, fadeOutTime); //音量降为0
            yield return new WaitForSeconds(fadeOutTime);
            StopBgm();
            yield return new WaitForSeconds(delayTime);
            PlayBgm(bgmName, 0f);
            var tweener = _audioSource.DOFade(baseVolume, fadeInTime);
            _cacheBaseVolume = baseVolume;
            yield return tweener.WaitForCompletion();
            // Debug.Log(_audioSource.volume);
        }

        /// <summary>
        /// a播放一次然后b循环
        /// </summary>
        public void PlayLoopBgmWithIntro(string bgmNameA, string bgmNameB, float fadeOutTime, float delayTime, float fadeInTime, float baseVolume = 1f)
        {
            GameManager.Instance.StartCoroutine(PlayLoopBgmWithIntroCo(bgmNameA, bgmNameB, fadeOutTime, delayTime, fadeInTime, baseVolume));
        }

        /// <summary>
        /// bgm播放过程中设置音量
        /// </summary>
        /// <param name="volume"></param>
        public void SetVolumeRuntime(float volume)
        {
            _audioMixer.SetFloat("BGMVolume", volume);
        }

        /// <summary>
        /// bgm播放过程中调整音量
        /// </summary>
        /// <param name="volumeRate"></param>
        public void UpdateVolumeRuntime(float volumeRate)
        {
            var originalVolume = _audioSource.volume;
            _audioSource.DOFade(originalVolume + originalVolume * volumeRate, 0.5f);
        }

        /// <summary>
        /// 重置bgm初始音量
        /// </summary>
        public void ResetBgmVolume()
        {
            _audioSource.DOFade(_cacheBaseVolume, 0.5f);
        }

        /// <summary>
        /// 静音
        /// </summary>
        public void MuteVolume()
        {
            _audioMixer.SetFloat("BGMVolume", -100f);
        }

        /// <summary>
        /// 播放AudioClip
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="baseVolume"></param>
        private void PlayBgmByAudioClip(AudioClip clip, float baseVolume)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
            _audioSource.volume = baseVolume;
            _cacheBaseVolume = baseVolume;
        }
        
        /// <summary>
        /// PlayLoopBgmWithIntro的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlayLoopBgmWithIntroCo(string bgmNameA, string bgmNameB, float fadeOutTime, float delayTime, float fadeInTime, float baseVolume = 1f)
        {
            var audioClipA = AssetManager.Instance.LoadAsset<AudioClip>(_cfgBGM[bgmNameA].AudioClipPath);
            var audioClipB = AssetManager.Instance.LoadAsset<AudioClip>(_cfgBGM[bgmNameB].AudioClipPath);
            _audioSource.loop = true;
            _audioSource.DOFade(0, fadeOutTime); //音量降为0
            yield return new WaitForSeconds(fadeOutTime);
            StopBgm();
            yield return new WaitForSeconds(delayTime);
            PlayBgmByAudioClip(audioClipA, 0f);
            _audioSource.DOFade(baseVolume, fadeInTime);
            yield return new WaitForSeconds(audioClipA.length);
            PlayBgmByAudioClip(audioClipB, baseVolume);
        }
    }
}