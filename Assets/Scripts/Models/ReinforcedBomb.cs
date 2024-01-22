using UnityEngine;

namespace Models
{
    public class ReinforcedBomb : MonoBehaviour
    {
        [SerializeField] private int damage;
        [SerializeField] private bool active;
        
        private void Start()
        {
            var parent = transform.parent;
            var layer = parent.gameObject.layer;
            foreach (Transform t in parent)
            {
                t.gameObject.layer = layer;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log(("Triggered"));
            if (active) return;
            if (other.transform.parent.gameObject.TryGetComponent(out Character character))
            {
                if (character.gameObject.layer != gameObject.layer)
                {
                    //Debug.Log("Bomb detect enemy!");
                    active = true;
                    character.TakeDamage(damage);
                    Destroy();
                }
            }
        }

        private void Destroy()
        {
            var parent = transform.parent.gameObject;
            
            parent.SetActive(false);
            Destroy(parent, 0.5f);
        }
    }
}