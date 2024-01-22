using System;
using System.Collections.Generic;
using Extensions;
using Models;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Controllers
{
    [ExecuteInEditMode]
    public class MapGenerateController : MonoBehaviour
    {
        public int WidthRadius;
        public int HeightRadius;

        public GameObject OriginPrefab;
        public Transform WarField;

        [SerializeField] private TileData[] _vertexes;
        [SerializeField] private List<TileData> _tileDatas;
        
        /// <summary>
        /// Function use for test only.
        /// </summary>
        [ContextMenu("Reset Empty")]
        private void RemoveAll()
        {
            _vertexes = new TileData[6];
            foreach (var tile in _tileDatas)
            {
                DestroyImmediate(tile.gameObject);
            }

            _tileDatas = new List<TileData>();
        }

        #region Generate Map
        public void Generate()
        {
            if (WidthRadius == 0 || HeightRadius == 0 || 2 * WidthRadius < HeightRadius) return;
            if (!OriginPrefab || !WarField.gameObject) return;
            
            if (_tileDatas.Count == 0)
            {
                var firstTile = CreateNewTileData();

                if (firstTile)
                {
                    _tileDatas.Add(firstTile);
                }
            }

            var pivotTile = _tileDatas[0];
            var x = 1;
            var z = 1;
            do
            {
                CreateHexagonSkeleton(pivotTile, x, z);
                
                if (x != 1) CreateAround(x, z); // odd difference with even number
                
                x++;
                z++;

                if (x > WidthRadius && z > HeightRadius) break;
                
                if (x <= WidthRadius && z > HeightRadius) z--;
                else if (x > WidthRadius && z <= HeightRadius) x--;
                // x <= Width && z <= Height passed
            } while (true);
        }

        private TileData CreateNewTileData()
        {
            var tileObj = PrefabUtility.InstantiatePrefab(OriginPrefab, WarField) as GameObject;

            if (tileObj!.TryGetComponent(out TileData tileData))
            {
                return tileData;
            }

            return null;
        }

        private void CreateHexagonSkeleton(TileData pivotTile, int X, int Z)
        {
            // x is Horizontal, z is Vertical

            if (X == Z)
            {
                for (int i = 0; i < 6; i++)
                {
                    var newTile = CreateNewTileData();
                
                    if (X == 1) newTile.SetTilePosition(pivotTile, i);
                    else
                    {
                        /*var neighborOfNewTile = FindNeighborFromDirection(pivotTile, i, X-1); //_vertexes?

                        if (neighborOfNewTile is null) return;*/
                        var neighborOfNewTile = _vertexes[i];

                        newTile.SetTilePosition(neighborOfNewTile, i);
                    }
                
                    newTile.UpdateWorldPosition();
                    newTile.FillAllNeighborFrom(_tileDatas, (i+3)%6);
                
                    _tileDatas.Add(newTile);
                    try
                    {
                        _vertexes[i] = newTile;
                    }
                    catch
                    {
                        _vertexes = new TileData[6];
                        _vertexes[i] = newTile;
                    }
                }
            }
            else
            {
                if (X > Z) // always X = Z+1
                {
                    var middleRightVertex = CreateNewTileData();
                    
                    middleRightVertex.SetTilePosition(_vertexes[2], 2);
                    middleRightVertex.UpdateWorldPosition();
                    middleRightVertex.FillAllNeighborFrom(_tileDatas, 5);
                    
                    _tileDatas.Add(middleRightVertex);
                    _vertexes[2] = middleRightVertex;
                    
                    var middleLeftVertex = CreateNewTileData();
                    
                    middleLeftVertex.SetTilePosition(_vertexes[5], 5);
                    middleLeftVertex.UpdateWorldPosition();
                    middleLeftVertex.FillAllNeighborFrom(_tileDatas, 2);
                    
                    _tileDatas.Add(middleLeftVertex);
                    _vertexes[5] = middleLeftVertex;
                }
                else // always Z > X, Z = X+1
                {
                    var topLeftVertex = CreateNewTileData();
                    
                    topLeftVertex.SetTilePosition(_vertexes[0], 0);
                    topLeftVertex.UpdateWorldPosition();
                    topLeftVertex.FillAllNeighborFrom(_tileDatas, 3);
                    
                    _tileDatas.Add(topLeftVertex);
                    _vertexes[0] = topLeftVertex;
                    
                    var topRightVertex = CreateNewTileData();
                    
                    topRightVertex.SetTilePosition(_vertexes[1], 1);
                    topRightVertex.UpdateWorldPosition();
                    topRightVertex.FillAllNeighborFrom(_tileDatas, 4);
                    
                    _tileDatas.Add(topRightVertex);
                    _vertexes[1] = topRightVertex;
                    
                    var bottomRightVertex = CreateNewTileData();
                    
                    bottomRightVertex.SetTilePosition(_vertexes[3], 3);
                    bottomRightVertex.UpdateWorldPosition();
                    bottomRightVertex.FillAllNeighborFrom(_tileDatas, 0);
                    
                    _tileDatas.Add(bottomRightVertex);
                    _vertexes[3] = bottomRightVertex;
                    
                    var bottomLeftVertex = CreateNewTileData();
                    
                    bottomLeftVertex.SetTilePosition(_vertexes[4], 4);
                    bottomLeftVertex.UpdateWorldPosition();
                    bottomLeftVertex.FillAllNeighborFrom(_tileDatas, 1);
                    
                    _tileDatas.Add(bottomLeftVertex);
                    _vertexes[4] = bottomLeftVertex;
                }
            }
        }

        private void CreateAround(int X, int Z)
        {
            //get vertex direction topleft-0, middleright-2 and bottomleft-4

            if (X == Z)
            {
                for (int i = 0; i < 6; i+=2)
                {
                    // expand with 2 near other vertex at direction (i+5)%6 and i+1
                    var curVertex = _vertexes[i];
                    var nearVertex1 = _vertexes[(i + 5) % 6];
                    var nearVertex2 = _vertexes[i + 1];

                    FillClusterSymmetric(curVertex, nearVertex1, X-1);
                    FillClusterSymmetric(curVertex, nearVertex2, X-1);
                }
            }
            else if (X > Z)
            {
                FillClusterAsymmetric(_vertexes[2], _vertexes[1], 0, X > Z);
                FillClusterAsymmetric(_vertexes[2], _vertexes[3], 4, X > Z);
                
                FillClusterAsymmetric(_vertexes[5], _vertexes[0], 1, X > Z);
                FillClusterAsymmetric(_vertexes[5], _vertexes[4], 3, X > Z);
            }
            else // Z > X
            {
                FillClusterSymmetric(_vertexes[0], _vertexes[1], Z - 1);
                FillClusterSymmetric(_vertexes[3], _vertexes[4],Z - 1);
                
                FillClusterAsymmetric(_vertexes[0], _vertexes[5], 4, X > Z);
                FillClusterAsymmetric(_vertexes[0], _vertexes[5], 5, X > Z);
                
                FillClusterAsymmetric(_vertexes[1], _vertexes[2], 3, X > Z);
                FillClusterAsymmetric(_vertexes[1], _vertexes[2], 2, X > Z);
                
                FillClusterAsymmetric(_vertexes[3], _vertexes[2], 1, X > Z);
                FillClusterAsymmetric(_vertexes[3], _vertexes[2], 2, X > Z);
                
                FillClusterAsymmetric(_vertexes[4], _vertexes[5], 0, X > Z);
                FillClusterAsymmetric(_vertexes[4], _vertexes[5], 5, X > Z);
            }
        }

        private TileData FindNeighborFromDirection(TileData originTile, int direct, int step)
        {
            TileData returnTile = originTile;
            for (int i = 0; i < step; i++)
            {
                if (returnTile is null) return null;
                
                returnTile = returnTile.GetNeighbor(direct);
            }

            return returnTile;
        }

        /// <summary>
        /// X == Z
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="radius"></param>
        private void FillClusterSymmetric(TileData from, TileData to, int radius)
        {
            var direct = from.TilePosition.GetDirectionFromPosition(to.TilePosition);

            if (direct == -1)
            {
                Debug.Log("Get direction failed");
                return;
            }

            var curTileSeed = from;
            
            for (int i = 0; i < radius; i++)
            {
                var newTile = CreateNewTileData();

                newTile.SetTilePosition(curTileSeed, direct);
                newTile.UpdateWorldPosition();
                newTile.FillAllNeighborFrom(_tileDatas, -1);
                
                _tileDatas.Add(newTile);

                curTileSeed = newTile;
            }
        }
        
        private void FillClusterAsymmetric(TileData from, TileData to, int direct, bool isHorizontal)
        {
            if (direct == -1)
            {
                Debug.Log("Get direction failed");
                return;
            }

            var curTileSeed = from;

            if (isHorizontal) // X > Z
            {
                var maximumZ = to.TilePosition.Z;

                do
                {
                    var newTile = CreateNewTileData();

                    newTile.SetTilePosition(curTileSeed, direct);
                    newTile.UpdateWorldPosition();
                    newTile.FillAllNeighborFrom(_tileDatas, -1);

                    _tileDatas.Add(newTile);

                    var signal = newTile.TilePosition.GetPositionFromDirection(direct, out float nextX, out float nextZ);

                    if (signal)
                    {
                        if (Mathf.Abs(nextZ) > Mathf.Abs(maximumZ)) break;
                        
                        curTileSeed = newTile;
                    }
                    else break;
                } while (true);
            }
            else // Z > X
            {
                var maximumX = to.TilePosition.X;
                var firstTest = from.TilePosition.GetPositionFromDirection(direct, out float nextX, out float nextZ);

                if (firstTest && Mathf.Abs(nextX) > Mathf.Abs(maximumX)) return;

                do
                {
                    var newTile = CreateNewTileData();

                    newTile.SetTilePosition(curTileSeed, direct);
                    newTile.UpdateWorldPosition();
                    newTile.FillAllNeighborFrom(_tileDatas, -1);

                    _tileDatas.Add(newTile);

                    var signal = newTile.TilePosition.GetPositionFromDirection(direct, out nextX, out nextZ);

                    if (signal)
                    {
                        if (Mathf.Abs(nextX) > Mathf.Abs(maximumX)) break;
                        
                        curTileSeed = newTile;
                    }
                    else break;
                } while (true);
            }
        }
        #endregion

        #region DrawingMap
        public void UpdateAllTileData()
        {
            var none = Resources.Load<TileProperty>("ScriptableObjects/" + TileTag.None).TileMaterial;
            var forest = Resources.Load<TileProperty>("ScriptableObjects/" + TileTag.Forest).TileMaterial;
            var water = Resources.Load<TileProperty>("ScriptableObjects/" + TileTag.Water).TileMaterial;
            var rock = Resources.Load<TileProperty>("ScriptableObjects/" + TileTag.Rock).TileMaterial;
            var snowing = Resources.Load<TileProperty>("ScriptableObjects/" + TileTag.Snowing).TileMaterial;
            var magma = Resources.Load<TileProperty>("ScriptableObjects/" + TileTag.Magma).TileMaterial;

            foreach (var tile in _tileDatas)
            {
                switch (tile.TileTag)
                {
                    case TileTag.None:
                        tile.CurMaterial = none;
                        break;
                    case TileTag.Forest:
                        tile.CurMaterial = forest;
                        break;
                    case TileTag.Rock:
                        tile.CurMaterial = rock;
                        break;
                    case TileTag.Water:
                        tile.CurMaterial = water;
                        break;
                    case TileTag.Snowing:
                        tile.CurMaterial = snowing;
                        break;
                    case TileTag.Magma:
                        tile.CurMaterial = magma;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                tile.RendererUpdateMesh();
            }
        }
        #endregion
        
        //TODO: generate all map, update map with exist map already, reset localPosition if tile moving while edit, if tile GameObject was manually delete?
        //rule: 2x >= z, fill circle with every loop to x == z == min(x, z), continue expand larger axis and fill tile position <= smaller position
    }
}