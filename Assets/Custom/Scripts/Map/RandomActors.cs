using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RTS2D
{
    [System.Serializable]
    public class Wave {
        public List<EnemyEntry> Entries;
        public float TimeOffset;
    }
    [System.Serializable]
    public class EnemyEntry {
        public GameObject Prefab;
        public int Count;
    }
    
    public class RandomActors : MonoBehaviour
    {
        [Header("Position")]
        [SerializeField] float m_MinRange;
        [SerializeField] float m_MaxRange;

        [Header("Minerals")]
        [SerializeField] GameObject m_MineralsPrefab;
        [SerializeField] GameObject m_NeutralPlayer;
        [SerializeField] float m_MineralCount;

        [Header("Enemies")]
        [SerializeField] List<GameObject> m_EnemyPrefabs;
        [SerializeField] GameObject m_EnemyPlayer;
        [SerializeField] float m_EnemyCount;

        [Header("Waves")]
        [SerializeField] Text m_WaveText;
        [SerializeField] List<Wave> m_Waves;
        Wave m_NextWave;
        float m_CurrentTime;
        int m_WaveNumber;

        /// overrides ///////////////////////////////////////////////////////////////////////////
        void Start()
        {
            PlaceMinerals();
            PlaceEnemies();
        }

        void Update() {
            SelectWave();
            if(m_NextWave != null) {
                m_CurrentTime += Time.deltaTime;
                if(m_CurrentTime > m_NextWave.TimeOffset) {
                    SpawnWave();
                }
            }
            UpdateWaveText();
        }

        /// minerals ///////////////////////////////////////////////////////////////////////////

        void PlaceMinerals() {
            for(int i = 0; i < m_MineralCount; i++) {
                GameObject l_Object = Instantiate(m_MineralsPrefab, m_NeutralPlayer.transform);
                l_Object.transform.position = RandomPosition();
                l_Object.transform.rotation = RandomRotation();
            }
        }

        /// enemies ///////////////////////////////////////////////////////////////////////////

        void PlaceEnemies() {
            for(int i = 0; i < m_EnemyCount; i++) {
                GameObject l_Object = Instantiate(m_EnemyPrefabs[Random.Range(0, m_EnemyPrefabs.Count)], m_EnemyPlayer.transform);
                Transform l_Body = l_Object.transform.GetChild(0);
                l_Body.position = RandomPosition();
                l_Body.rotation = RandomRotation();
            }
        }

        /// waves ///////////////////////////////////////////////////////////////////////////

        void SelectWave() {
            if(m_NextWave == null && m_Waves.Count > 0) {
                m_NextWave = m_Waves[0];
                m_Waves.RemoveAt(0);
                m_CurrentTime = 0f;
                m_WaveNumber++;
            }
        }

        void SpawnWave() {
            foreach(EnemyEntry l_Entry in m_NextWave.Entries) {
                for(int i = 0; i < l_Entry.Count; i++) {
                    SpawnEnemy(l_Entry.Prefab);
                }
            }
            m_NextWave = null;
        }

        void SpawnEnemy(GameObject a_Prefab) {
            GameObject l_Object = Instantiate(a_Prefab, m_EnemyPlayer.transform);
            Transform l_Body = l_Object.transform.GetChild(0);
            l_Body.position = RandomPosition();
            l_Body.rotation = RandomRotation();
            (l_Object.GetComponent<Actor2D>() as ICommandable).Attack(Vector3.zero);
        }

        void UpdateWaveText() {
            if(m_NextWave != null) {
                m_WaveText.text = "Wave: "+m_WaveNumber.ToString("0")+" in "+(m_NextWave.TimeOffset - m_CurrentTime).ToString("0.0");
            }else {
                m_WaveText.text = "";
            }
        }

        /// helpers ///////////////////////////////////////////////////////////////////////////

        Vector3 RandomPosition() {
            return RandomRotation() * Vector3.up * Random.Range(m_MinRange, m_MaxRange);
        }

        Quaternion RandomRotation() {
            return Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        }
    }
}

