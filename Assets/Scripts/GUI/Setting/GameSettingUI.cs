using System;
using Controllers;
using Data;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GUI.Setting
{
    public class GameSettingUI : MonoBehaviour
    {
        [SerializeField] private Toggle bmgNSfxToggle;
        [SerializeField] private Slider musicsSlider;
        [SerializeField] private Slider soundsSlider;
        [SerializeField] private TMP_Text musicsTxt;
        [SerializeField] private TMP_Text soundsTxt;

        private void Start()
        {
            /*DataManager.LoadGameData(); //test scene*/
            
            var gameSetting = DataManager.SettingData;

            musicsSlider.value = gameSetting.MusicVolume;
            soundsSlider.value = gameSetting.SoundVolume;
            
            if (!gameSetting.BgmNSfx)
            {
                bmgNSfxToggle.isOn = false;
            }
        }

        public void OnToggleBgmAndSfx(bool value)
        {
            musicsSlider.interactable = value;
            soundsSlider.interactable = value;
            DataManager.SettingData.BgmNSfx = value;
            if (!value)
            {
                AudioController.Instance.MuteMixerGroup(AudioMixerType.Music, musicsSlider.minValue);
                AudioController.Instance.MuteMixerGroup(AudioMixerType.SoundEffect, musicsSlider.minValue);
            }
            else
            {
                AudioController.Instance.UnMuteMixerGroup(AudioMixerType.Music, musicsSlider.value);
                AudioController.Instance.UnMuteMixerGroup(AudioMixerType.SoundEffect, soundsSlider.value);
            }
        }
        
        public void OnMusicsSliding(float value)
        {
            DataManager.SettingData.MusicVolume = value;
            musicsTxt.text = "" + (int)(DataManager.SettingData.MusicVolume * 100f);
            AudioController.Instance.MixerGroupUpdate(AudioMixerType.Music, value);
        }
        
        public void OnSoundsSliding(float value)
        {
            DataManager.SettingData.SoundVolume = value;
            soundsTxt.text = "" + (int)(DataManager.SettingData.SoundVolume * 100f);
            AudioController.Instance.MixerGroupUpdate(AudioMixerType.SoundEffect, value);
        }

        /*private void OnApplicationQuit() //test scene
        {
            DataManager.SaveGameData();
        }*/
    }
}