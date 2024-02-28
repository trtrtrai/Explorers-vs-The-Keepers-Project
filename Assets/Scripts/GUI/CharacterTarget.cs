using System.Collections;
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
                transform.Rotate(Vector3.up, 150f * Time.deltaTime, Space.Self);
            }
        }

        public void SetupTarget(GameObject target)
        {
            transform.SetParent(target.transform);
            transform.localPosition = offset;

            _renderer.enabled = true;
            _animator.enabled = true;
        }

        public void DeselectTarget()
        {
            transform.SetParent(null);

            _animator.enabled = false;
            _renderer.enabled = false;
            transform.localEulerAngles = Vector3.zero;
        }
    }
}