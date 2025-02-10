// ******************************************************************
//@file         SFXManager.cs
//@brief        音效管理器
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:30:57
// ******************************************************************

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace Yu
{
    public class SFXManager : BaseSingleTon<SFXManager>, IMonoManager
    {
        private CfgSFX _cfgSfx;
        private AudioMixer _audioMix;
        private readonly Dictionary<DefSFXType, AudioMixerGroup> _sfxMixerGroupDic = new Dictionary<DefSFXType, AudioMixerGroup>();
        private readonly Dictionary<string, RowCfgSFX> _dataDictionary = new Dictionary<string, RowCfgSFX>();
        private Dictionary<RowCfgSFX, AudioSource> _sfxItems;


        /// <summary>
        /// 初始化Manager，设置SfxItem，为每个sfx生成SfxItem
        /// </summary>
        public void OnInit()
        {
            _cfgSfx = ConfigManager.Tables.CfgSFX;
            _audioMix = AssetManager.Instance.LoadAsset<AudioMixer>("Assets/AddressableAssets/AudioMixer/AudioMixer.mixer");
            _sfxMixerGroupDic.Add(DefSFXType.Se, _audioMix.FindMatchingGroups(DefSFXType.Se.ToString())[0]);
            _sfxMixerGroupDic.Add(DefSFXType.Voice, _audioMix.FindMatchingGroups(DefSFXType.Voice.ToString())[0]);

            var root = new GameObject("SFXManager");
            root.transform.SetParent(GameManager.Instance.transform, false);

            _sfxItems = new Dictionary<RowCfgSFX, AudioSource>();
            foreach (var rowCfgSfx in _cfgSfx.DataList)
            {
                if (string.IsNullOrEmpty(rowCfgSfx.Id))
                {
                    return;
                }

                var sfxObjTemp = new GameObject(rowCfgSfx.Id);
                sfxObjTemp.transform.SetParent(root.transform);
                var sfxObjAudioSource = sfxObjTemp.AddComponent<AudioSource>();
                sfxObjAudioSource.outputAudioMixerGroup = _sfxMixerGroupDic[rowCfgSfx.SFXType];
                sfxObjAudioSource.playOnAwake = false;
                _sfxItems.Add(rowCfgSfx, sfxObjAudioSource);
                _dataDictionary.Add(rowCfgSfx.Id, rowCfgSfx);
            }
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
            // _audioMix.SetFloat("SeVolume", SaveGameManager.Instance.Get<float>(DefSFXType.Se+"Volume", 0f,SaveType.Cfg));
            // _audioMix.SetFloat("VoiceVolume", SaveGameManager.Instance.Get<float>(DefSFXType.Voice+"Volume", 0f,SaveType.Cfg));
        }

        /// <summary>
        /// 播放Sfx
        /// </summary>
        /// <param name="sfxName">sfx名称</param>
        public void PlaySfx(string sfxName)
        {
            if (!_dataDictionary.TryGetValue(sfxName, out var rowCfgSfx))
            {
                Debug.LogError("没有这个sfx：" + sfxName);
                return;
            }

            var clip = AssetManager.Instance.LoadAsset<AudioClip>(rowCfgSfx.ClipPaths[Random.Range(0, rowCfgSfx.ClipPaths.Count)]);
            if (rowCfgSfx.OneShot)
            {
                _sfxItems[rowCfgSfx].PlayOneShot(clip, rowCfgSfx.Volume);
                return;
            }

            _sfxItems[rowCfgSfx].Stop();
            _sfxItems[rowCfgSfx].clip = clip;
            _sfxItems[rowCfgSfx].loop = rowCfgSfx.Loop;
            _sfxItems[rowCfgSfx].volume = rowCfgSfx.Volume;
            _sfxItems[rowCfgSfx].Play();
            // Debug.Log(rowCfgSfx.key+"   "+_sfxItems[rowCfgSfx].loop);
        }

        /// <summary>
        /// 延迟播放SFX
        /// </summary>
        /// <returns></returns>
        public IEnumerator PlaySfxDelay(string sfxName, float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            PlaySfx(sfxName);
        }

        /// <summary>
        /// 停止播放音效
        /// </summary>
        /// <param name="sfxName">音效名称</param>
        public void Stop(string sfxName)
        {
            if (!_dataDictionary.TryGetValue(sfxName, out var rowCfgSfx))
            {
                Debug.LogError("没有这个sfx名：" + sfxName);
                return;
            }

            _sfxItems[rowCfgSfx].Stop();
        }

        /// <summary>
        /// 停止播放所有音效
        /// </summary>
        public void StopAllSfx()
        {
            foreach (var kvp in _sfxItems)
            {
                var sfxItem = kvp.Value;
                sfxItem.Stop();
            }
        }

        /// <summary>
        /// 延迟淡出停止音效
        /// </summary>
        /// <param name="sfxName"></param>
        /// <param name="delayTime"></param>
        /// <param name="fadeOutTime"></param>
        /// <returns></returns>
        public IEnumerator StopDelayFadeIEnumerator(string sfxName, float delayTime, float fadeOutTime)
        {
            if (!_dataDictionary.TryGetValue(sfxName, out var rowCfgSfx))
            {
                Debug.LogError("没有这个sfx名：" + sfxName);
                yield break;
            }

            var audioSource = _sfxItems[rowCfgSfx];
            yield return new WaitForSeconds(delayTime);
            audioSource.DOFade(0, fadeOutTime); //音量降为0
            yield return new WaitForSeconds(fadeOutTime);
            audioSource.Stop();
        }

        /// <summary>
        /// 播放sfx过程中设置sfx音量
        /// </summary>
        /// <param name="sfxType">音效类型</param>
        /// <param name="volumeBase">要改变的音量</param>
        public void SetVolumeRuntime(DefSFXType sfxType, float volumeBase)
        {
            _audioMix.SetFloat(sfxType.ToString() + "Volume", volumeBase);
        }

        public void MuteVolume(DefSFXType sfxType)
        {
            _audioMix.SetFloat(sfxType.ToString() + "Volume", -100f);
        }
    }
}