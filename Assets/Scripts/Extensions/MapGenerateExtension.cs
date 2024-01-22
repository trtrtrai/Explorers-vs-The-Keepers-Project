using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Models;
using UnityEngine;

namespace Extensions
{
    public enum NeighborDirection
    {
        TopLeft,
        TopRight,
        MiddleRight,
        BottomRight,
        BottomLeft,
        MiddleLeft
    }
    
    public static class MapGenerateExtension
    {
        public static void SetTilePosition(this TileData tileData, TileData neighbor, int direct)
        {
            var neighborPos = neighbor.TilePosition;
            var directEnum = (NeighborDirection)direct;
            //Top z+1f, Bottom z-1f, Left x-0.5f, Right x+0.5f, Middle x+-1f,z+0f
            switch (directEnum)
            {
                case NeighborDirection.TopLeft:
                    tileData.TilePosition.X = neighborPos.X - .5f;
                    tileData.TilePosition.Z = neighborPos.Z + 1f;

                    tileData.TilePosition.BottomRightNeighbor = neighbor;
                    neighbor.TilePosition.TopLeftNeighbor = tileData;
                    break;
                case NeighborDirection.TopRight:
                    tileData.TilePosition.X = neighborPos.X + .5f;
                    tileData.TilePosition.Z = neighborPos.Z + 1f;

                    tileData.TilePosition.BottomLeftNeighbor = neighbor;
                    neighbor.TilePosition.TopRightNeighbor = tileData;
                    break;
                case NeighborDirection.MiddleRight:
                    tileData.TilePosition.X = neighborPos.X + 1f;
                    tileData.TilePosition.Z = neighborPos.Z + 0f;

                    tileData.TilePosition.MiddleLeftNeighbor = neighbor;
                    neighbor.TilePosition.MiddleRightNeighbor = tileData;
                    break;
                case NeighborDirection.BottomRight:
                    tileData.TilePosition.X = neighborPos.X + .5f;
                    tileData.TilePosition.Z = neighborPos.Z - 1f;

                    tileData.TilePosition.TopLeftNeighbor = neighbor;
                    neighbor.TilePosition.BottomRightNeighbor = tileData;
                    break;
                case NeighborDirection.BottomLeft:
                    tileData.TilePosition.X = neighborPos.X - .5f;
                    tileData.TilePosition.Z = neighborPos.Z - 1f;

                    tileData.TilePosition.TopRightNeighbor = neighbor;
                    neighbor.TilePosition.BottomLeftNeighbor = tileData;
                    break;
                case NeighborDirection.MiddleLeft:
                    tileData.TilePosition.X = neighborPos.X - 1f;
                    tileData.TilePosition.Z = neighborPos.Z + 0f;

                    tileData.TilePosition.MiddleRightNeighbor = neighbor;
                    neighbor.TilePosition.MiddleLeftNeighbor = tileData;
                    break;
            }
        }

        public static void UpdateWorldPosition(this TileData tileData)
        {
            var position = new Vector3(1.73f * tileData.TilePosition.X, 0f, 1.5f * tileData.TilePosition.Z);

            tileData.transform.localPosition = position;
        }

        public static void FillAllNeighborFrom(this TileData target, List<TileData> world, int except)
        {
            for (int i = 0; i < 6; i++)
            {
                if (i == except) continue;
                var tilePosition = new TilePosition();
                TileData tileNeighbor = null;
                
                switch ((NeighborDirection)i)
                {
                    case NeighborDirection.TopLeft:
                        tilePosition.X = target.TilePosition.X - .5f;
                        tilePosition.Z = target.TilePosition.Z + 1f;

                        tileNeighbor = world.FirstOrDefault(t =>
                            t.TilePosition.X.Equals(tilePosition.X) && t.TilePosition.Z.Equals(tilePosition.Z));
                        if (tileNeighbor is not null)
                        {
                            tileNeighbor.TilePosition.BottomRightNeighbor = target;
                            target.TilePosition.TopLeftNeighbor = tileNeighbor;
                        }
                        break;
                    case NeighborDirection.TopRight:
                        tilePosition.X = target.TilePosition.X + .5f;
                        tilePosition.Z = target.TilePosition.Z + 1f;

                        tileNeighbor = world.FirstOrDefault(t =>
                            t.TilePosition.X.Equals(tilePosition.X) && t.TilePosition.Z.Equals(tilePosition.Z));
                        if (tileNeighbor is not null)
                        {
                            tileNeighbor.TilePosition.BottomLeftNeighbor = target;
                            target.TilePosition.TopRightNeighbor = tileNeighbor;
                        }
                        break;
                    case NeighborDirection.MiddleRight:
                        tilePosition.X = target.TilePosition.X + 1f;
                        tilePosition.Z = target.TilePosition.Z + 0f;

                        tileNeighbor = world.FirstOrDefault(t =>
                            t.TilePosition.X.Equals(tilePosition.X) && t.TilePosition.Z.Equals(tilePosition.Z));
                        if (tileNeighbor is not null)
                        {
                            tileNeighbor.TilePosition.MiddleLeftNeighbor = target;
                            target.TilePosition.MiddleRightNeighbor = tileNeighbor;
                        }
                        break;
                    case NeighborDirection.BottomRight:
                        tilePosition.X = target.TilePosition.X + .5f;
                        tilePosition.Z = target.TilePosition.Z - 1f;

                        tileNeighbor = world.FirstOrDefault(t =>
                            t.TilePosition.X.Equals(tilePosition.X) && t.TilePosition.Z.Equals(tilePosition.Z));
                        if (tileNeighbor is not null)
                        {
                            tileNeighbor.TilePosition.TopLeftNeighbor = target;
                            target.TilePosition.BottomRightNeighbor = tileNeighbor;
                        }
                        break;
                    case NeighborDirection.BottomLeft:
                        tilePosition.X = target.TilePosition.X - .5f;
                        tilePosition.Z = target.TilePosition.Z - 1f;

                        tileNeighbor = world.FirstOrDefault(t =>
                            t.TilePosition.X.Equals(tilePosition.X) && t.TilePosition.Z.Equals(tilePosition.Z));
                        if (tileNeighbor is not null)
                        {
                            tileNeighbor.TilePosition.TopRightNeighbor = target;
                            target.TilePosition.BottomLeftNeighbor = tileNeighbor;
                        }
                        break;
                    case NeighborDirection.MiddleLeft:
                        tilePosition.X = target.TilePosition.X - 1f;
                        tilePosition.Z = target.TilePosition.Z + 0f;

                        tileNeighbor = world.FirstOrDefault(t =>
                            t.TilePosition.X.Equals(tilePosition.X) && t.TilePosition.Z.Equals(tilePosition.Z));
                        if (tileNeighbor is not null)
                        {
                            tileNeighbor.TilePosition.MiddleRightNeighbor = target;
                            target.TilePosition.MiddleLeftNeighbor = tileNeighbor;
                        }
                        break;
                }
            }
        }

        [CanBeNull]
        public static TileData GetNeighbor(this TileData tileData, int direct)
        {
            if (tileData is null) return null;
            
            var directionEnum = (NeighborDirection)direct;

            switch (directionEnum)
            {
                case NeighborDirection.TopLeft:
                    return tileData.TilePosition.TopLeftNeighbor ? tileData.TilePosition.TopLeftNeighbor : null;
                case NeighborDirection.TopRight:
                    return tileData.TilePosition.TopRightNeighbor ? tileData.TilePosition.TopRightNeighbor : null;
                case NeighborDirection.MiddleRight:
                    return tileData.TilePosition.MiddleRightNeighbor ? tileData.TilePosition.MiddleRightNeighbor : null;
                case NeighborDirection.BottomRight:
                    return tileData.TilePosition.BottomRightNeighbor ? tileData.TilePosition.BottomRightNeighbor : null;
                case NeighborDirection.BottomLeft:
                    return tileData.TilePosition.BottomLeftNeighbor ? tileData.TilePosition.BottomLeftNeighbor : null;
                case NeighborDirection.MiddleLeft:
                    return tileData.TilePosition.MiddleLeftNeighbor ? tileData.TilePosition.MiddleLeftNeighbor : null;
                default:
                    return null;
            }
        }

        public static int GetDirectionFromPosition(this TilePosition a, TilePosition b)
        {
            if (b.X < a.X && b.Z > a.Z)
            {
                return (int)NeighborDirection.TopLeft;
            }
            if (b.X > a.X && b.Z > a.Z)
            {
                return (int)NeighborDirection.TopRight;
            }
            if (b.X > a.X && b.Z.Equals(a.Z))
            {
                return (int)NeighborDirection.MiddleRight;
            }
            if (b.X > a.X && b.Z < a.Z)
            {
                return (int)NeighborDirection.BottomRight;
            }
            if (b.X < a.X && b.Z < a.Z)
            {
                return (int)NeighborDirection.BottomLeft;
            }
            if (b.X < a.X && b.Z.Equals(a.Z))
            {
                return (int)NeighborDirection.MiddleLeft;
            }

            return -1;
        }

        public static bool GetPositionFromDirection(this TilePosition tilePosition, int direct, out float x, out float z)
        {
            x = 0f;
            z = 0f;
            var signal = true;
            switch ((NeighborDirection)direct)
            {
                case NeighborDirection.TopLeft:
                    x = tilePosition.X - 0.5f;
                    z = tilePosition.Z + 1f;
                    break;
                case NeighborDirection.TopRight:
                    x = tilePosition.X + 0.5f;
                    z = tilePosition.Z + 1f;
                    break;
                case NeighborDirection.MiddleRight:
                    x = tilePosition.X + 1f;
                    z = tilePosition.Z + 0f;
                    break;
                case NeighborDirection.BottomRight:
                    x = tilePosition.X + 0.5f;
                    z = tilePosition.Z - 1f;
                    break;
                case NeighborDirection.BottomLeft:
                    x = tilePosition.X - 0.5f;
                    z = tilePosition.Z - 1f;
                    break;
                case NeighborDirection.MiddleLeft:
                    x = tilePosition.X - 1f;
                    z = tilePosition.Z + 0f;
                    break;
                default:
                    signal = false;
                    break;
            }

            return signal;
        }

        public static bool GetEqualsTilePositionFrom(this List<TileData> listTiles, TilePosition position, out TileData tileData)
        {
            tileData = listTiles.FirstOrDefault(t => t.TilePosition == position);

            if (tileData is null) return false;

            return true;
        }
    }
}