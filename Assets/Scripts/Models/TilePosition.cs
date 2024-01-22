using System;
using UnityEngine;
using Object = System.Object;

namespace Models
{
    [System.Serializable]
    public class TilePosition
    {
        public float X;
        public float Z;

        public TileData TopLeftNeighbor;
        public TileData TopRightNeighbor;
        public TileData MiddleRightNeighbor;
        public TileData BottomRightNeighbor;
        public TileData BottomLeftNeighbor;
        public TileData MiddleLeftNeighbor;

        public override string ToString()
        {
            return $"({X};{Z})";
        }

        public static Vector3 operator +(TilePosition a, TilePosition b) =>
            new(a.X + b.X, 0f,  a.Z + b.Z);
        
        public static Vector3 operator -(TilePosition a, TilePosition b) =>
            new(a.X - b.X, 0f,  a.Z - b.Z);

        public static bool operator ==(TilePosition a, TilePosition b)
        {
            if (a is null || b is null) return false;

            return a.X.Equals(b.X) && a.Z.Equals(b.Z);
        }
        
        public static bool operator !=(TilePosition a, TilePosition b) => !(a == b);

        public override bool Equals(object obj) => ReferenceEquals(this, obj);

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Z);
        }
    }
}