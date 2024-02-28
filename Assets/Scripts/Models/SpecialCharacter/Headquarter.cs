using UnityEngine;

namespace Models.SpecialCharacter
{
    public sealed class Headquarter : Character
    {
        [SerializeField] private Transform keeperMesh;
        [SerializeField] private Transform explorerMesh;
        
        public void SetupHeadquarter(bool isExplorer, Vector3 rotation)
        {
            if (isExplorer)
            {
                keeperMesh.gameObject.SetActive(false);
            }
            else
            {
                explorerMesh.gameObject.SetActive(false);
            }

            transform.Rotate(rotation.x, rotation.y, rotation.z);
        }
    }
}