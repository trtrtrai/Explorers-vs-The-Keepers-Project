using UnityEngine;
using ScriptableObjects;

namespace Models
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class TileData : MonoBehaviour
    {
        public TileTag TileTag;
        public Material CurMaterial;
        public TilePosition TilePosition;
        
        private Renderer rendererComp;
        
        private void Awake()
        {
            rendererComp = GetComponent<Renderer>();
            
            RendererUpdateMesh();
        }

        public void RendererUpdateMesh()
        {
            if (!rendererComp)
            {
                rendererComp = GetComponent<Renderer>();
            }

            rendererComp.material = CurMaterial;
        }
    }
}