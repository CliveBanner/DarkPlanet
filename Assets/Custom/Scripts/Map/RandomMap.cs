using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RTS2D
{
    public class RandomMap : MonoBehaviour
    {
        [Header("Tilemap")]
        [SerializeField] Tilemap m_GroundMap;

        [Header("Tiles")]
        [SerializeField] Tile m_DefaultTile;
        [SerializeField] List<RandomTile> m_RandomTiles;
        [System.Serializable]
        public class RandomTile {
            public Tile tile;
            public float probability;
        }
        // Start is called before the first frame update
        void Start()
        {
            foreach(Vector3Int l_Point in m_GroundMap.cellBounds.allPositionsWithin) {
                Tile l_Tile = null;
                foreach(RandomTile l_RandomTile in m_RandomTiles) {
                    if(Random.Range(0f, 1f) < l_RandomTile.probability) {
                        l_Tile = l_RandomTile.tile;
                    }
                }
                if(l_Tile == null) l_Tile = m_DefaultTile;
                m_GroundMap.SetTile(l_Point, l_Tile);
            }
        }
    }
}


