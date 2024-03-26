using UnityEngine;

namespace GUI
{
    public class TurnAroundLight : MonoBehaviour
    {
        [SerializeField] private float speed;
        
        private void Update()
        {
            transform.RotateAround(transform.localPosition, Vector3.up, -speed * Time.deltaTime);
        }
    }
}