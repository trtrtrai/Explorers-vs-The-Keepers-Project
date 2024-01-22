using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Tile Property", menuName = "Customs/New Tile Property")]
    public class TileProperty : ScriptableObject
    {
        public TileTag TileTag;
        public Material TileMaterial;
    }

    public enum TileTag
    {
        None,
        Forest,
        Rock,
        Water,
        Snowing,
        Magma,
    }
}