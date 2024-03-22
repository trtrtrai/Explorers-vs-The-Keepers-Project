using UnityEngine;

namespace Models.Effects
{
    public class EffectsAction : MonoBehaviour
    {
        [SerializeField] protected Vector3 targeted;
        
        public virtual void Activate(Vector3 target)
        {
            
        }
    }
}