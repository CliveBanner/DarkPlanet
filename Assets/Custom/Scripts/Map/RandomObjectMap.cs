using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RTS2D
{
    public class RandomObjectMap : MonoBehaviour
    {
        [Header("Tilemap")]
        [SerializeField] Tilemap m_ObjectMap;

        [Header("Tiles")]
        [SerializeField] List<RandomTile> m_RandomTiles;
        
        // Start is called before the first frame update
        void Start()
        {
            Tile l_Tile = null;
            foreach(Vector3Int l_Point in m_ObjectMap.cellBounds.allPositionsWithin) {
                l_Tile = null;
                foreach(RandomTile l_RandomTile in m_RandomTiles) {
                    if(Random.Range(0f, 1f) < l_RandomTile.probability) {
                        l_Tile = l_RandomTile.tile;
                    }
                }
                m_ObjectMap.SetTile(l_Point, l_Tile);
            }
        }
    }
}

