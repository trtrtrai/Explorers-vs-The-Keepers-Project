using UnityEngine;

namespace GUI
{
    public class CharacterTarget : MonoBehaviour
    {
        [SerializeField] private Vector3 offset;
        [SerializeField] private MeshRenderer _renderer;
        
        private void Start()
        {
            DeselectTarget();
        }

        private void Update()
        {
            if (_renderer.enabled)
            {
                var rotate = transform.localRotation;
                transform.Rotate(Vector3.up, 50f * Time.deltaTime, Space.Self);
            }
        }

        public void SetupTarget(GameObject target)
        {
            transform.SetParent(target.transform);
            transform.localPosition = offset;

            _renderer.enabled = true;
        }

        public void DeselectTarget()
        {
            transform.SetParent(null);

            _renderer.enabled = false;
            transform.localEulerAngles = Vector3.zero;
        }
    }
}