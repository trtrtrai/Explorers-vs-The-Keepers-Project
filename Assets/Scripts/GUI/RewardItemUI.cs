using UnityEngine;

namespace GUI
{
    public abstract class RewardItemUI : MonoBehaviour
    {
        [SerializeField] protected int itemValue;

        public virtual void Setup(int value)
        {
            itemValue = value;
        }
    }
}