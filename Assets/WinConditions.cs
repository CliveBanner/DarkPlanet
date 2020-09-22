using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RTS2D
{
    public class WinConditions : MonoBehaviour
    {
        [SerializeField] Text m_GameOverText;
        RTSPlayer m_Player;
        float m_PlayTime;
        // Start is called before the first frame update
        void Start()
        {
            m_Player = GetComponent<RTSPlayer>();
            m_PlayTime = 0f;
        }

        // Update is called once per frame
        void Update()
        {
            m_PlayTime += Time.deltaTime;
            if(m_Player.GetAllBases().Length == 0) {
                GameOver();
            }
        }

        void GameOver() {
            float l_Score = m_PlayTime * 10f;
            if(l_Score > PlayerPrefs.GetFloat("HighScore")) {
                PlayerPrefs.SetFloat("HighScore", l_Score);
            }
            
            m_GameOverText.enabled = true;
            Invoke("LoadMenu", 5.0f);
        }

        void LoadMenu() {
            SceneManager.LoadScene("Menu");
        }
    }
}

