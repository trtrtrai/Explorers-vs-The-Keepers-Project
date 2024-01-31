using System;
using EventArgs;
using UnityEngine;

namespace Models.Generals
{
    public abstract class GeneralCheckCanSummon : MonoBehaviour
    {
        public event EventHandler<GeneralRequireTriggerEventArgs> OnRequireTrigger; 
        
        [SerializeField] protected bool CanSummon = false;
        public int Team; // check 2 card deck to initial team

        public bool GeneralCanSummon() => CanSummon;

        public abstract string GetDescription();

        protected void InvokeRequireTrigger(float oldProgress, float newProgress)
        {
            //Debug.Log(oldProgress + " " + newProgress);
            OnRequireTrigger?.Invoke(this, new GeneralRequireTriggerEventArgs(oldProgress, newProgress));
        }
    }
}