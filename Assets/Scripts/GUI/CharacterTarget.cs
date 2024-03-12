using UnityEngine;

namespace GUI
{
    public class CharacterTarget : MonoBehaviour
    {
        [SerializeField] private Vector3 offset;
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private Animator _animator;
        
        private void Start()
        {
            DeselectTarget();
        }

        private void Update()
        {
            if (_renderer.enabled)
            {
                transform.parent.Rotate(Vector3.up, 150f * Time.deltaTime, Space.Self);
            }
        }

        public void SetupTarget(GameObject target)
        {
            transform.parent.SetParent(target.transform);
            transform.parent.localPosition = Vector3.zero;
            transform.localPosition = offset;

            _renderer.enabled = true;
            _animator.enabled = true;
        }

        public void DeselectTarget()
        {
            transform.parent.SetParent(null);

            _animator.enabled = false;
            _renderer.enabled = false;
            transform.parent.localEulerAngles = Vector3.zero;
        }
    }
}