using System;
using UnityEngine;

namespace Models.Effects
{
    [Serializable]
    public class EffectsInfo
    {
        public string effectsName;
        public GameObject prefab;
        public Vector3 offset;
    }
}