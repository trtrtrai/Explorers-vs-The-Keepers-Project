using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Models;
using UnityEngine;
using UnityEngine.Audio;

namespace Controllers
{
    public class AudioController : MonoBehaviour
    {
        public static AudioController Instance { get; private set; }

        [SerializeField] private AudioMixerGroup musicMixerGroup;
        [SerializeField] private AudioMixerGroup sfxMixerGroup;

        [SerializeField] private List<AudioInfo> audios;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        public void Play(string soundName)
        {
            var au = audios.FirstOrDefault(a => a.name.Equals(soundName));

            if (au is null) return;

            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = au.clip;
            audioSource.loop = au.isLoop;
            audioSource.volume = au.volume;

            switch (au.type)
            {
                case AudioMixerType.SoundEffect:
                    audioSource.outputAudioMixerGroup = sfxMixerGroup;
                    break;
                case AudioMixerType.Music:
                    audioSource.outputAudioMixerGroup = musicMixerGroup;
                    break;
            }
            
            audioSource.Play();
            if (!audioSource.loop) StartCoroutine(WaitToFlushSound(audioSource));
        }

        private IEnumerator WaitToFlushSound(AudioSource source)
        {
            while (source.isPlaying)
            {
                yield return null;
            }
            Destroy(source);
        }
        
        /// <summary>
        /// Test scene only
        /// </summary>
        /// <param name="index"></param>
        public void Play(int index)
        {
            if (index > -1 && index < audios.Count)
            {
                Play(audios[index].name);
            }
        }

        public void MixerGroupUpdate(AudioMixerType type, float value)
        {
            switch (type)
            {
                case AudioMixerType.SoundEffect:
                    sfxMixerGroup.audioMixer.SetFloat("Sound Mixer", Mathf.Log10(value) * 20f);
                    break;
                case AudioMixerType.Music:
                    musicMixerGroup.audioMixer.SetFloat("Music Mixer", Mathf.Log10(value) * 20f);
                    break;
            }
        }

        public void MuteMixerGroup(AudioMixerType type, float value) => MixerGroupUpdate(type, value);
        public void UnMuteMixerGroup(AudioMixerType type, float value) => MixerGroupUpdate(type, value);
    }
}