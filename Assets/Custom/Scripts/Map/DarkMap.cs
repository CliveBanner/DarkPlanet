using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RTS2D {
    public class DarkMap : MonoBehaviour
    {
        [Header("Tilemaps")]
        [SerializeField] Tilemap m_DarkMap;
        [SerializeField] Tilemap m_GroundMap;
        [Header("Tiles")]
        [SerializeField] Tile m_DarkTile;
        // Start is called before the first frame update
        void Start()
        {
            m_DarkMap.origin = m_GroundMap.origin;
            m_DarkMap.size = m_GroundMap.size;
            foreach(Vector3Int p in m_DarkMap.cellBounds.allPositionsWithin) {
                m_DarkMap.SetTile(p, m_DarkTile);
            }
        }
    }
}


