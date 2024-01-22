using Controllers;
using UnityEngine;

namespace GUI
{
    public class Droppable : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Material _material;
        [SerializeField] private bool activeChoosing;
        [SerializeField] private bool isDecrease;

        public int RoadIndex;
        public int Team1PositionIndex;
        public int Team2PositionIndex;

        private void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _material = _meshRenderer.material;
            activeChoosing = false;
        }

        private void Update()
        {
            if (activeChoosing)
            {
                var colorA = _material.color.a;

                if (isDecrease)
                {
                    colorA -= Time.deltaTime;
                }
                else
                {
                    colorA += Time.deltaTime;
                }

                var color = new Color(_material.color.r, _material.color.g, _material.color.b, colorA);
                _material.color = color;

                if (_material.color.a > .99f)
                {
                    isDecrease = true;
                }
                else if (_material.color.a < .7f)
                {
                    isDecrease = false;
                }
            }
        }

        private void OnMouseEnter()
        {
            //Debug.Log("Mouse enter");
            WorldManager.Instance.CardEnterDroppable(this);
        }

        private void OnMouseExit()
        {
            //Debug.Log("Mouse exit");
            WorldManager.Instance.CardExitDroppable(this);
        }

        public void ActiveChoosing()
        {
            activeChoosing = true;
            isDecrease = true;
        }

        public void DisableChoosing()
        {
            activeChoosing = false;
            
            var color = new Color(_material.color.r, _material.color.g, _material.color.b, 1f);
            _material.color = color;
        }
    }
}