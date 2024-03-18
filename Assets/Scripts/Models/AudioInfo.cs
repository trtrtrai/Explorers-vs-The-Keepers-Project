using System;
using UnityEngine;

namespace Models
{
    public enum AudioMixerType
    {
        SoundEffect,
        Music
    }
    
    [Serializable]
    public class AudioInfo
    {
        public AudioMixerType type;
        public string name;
        public AudioClip clip;
        public bool isLoop;
        [Range(0, 1f)] public float volume = 0.5f;
    }
}