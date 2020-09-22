using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScore : MonoBehaviour
{
    [SerializeField] Text m_Text;

    // Update is called once per frame
    void Update()
    {
        m_Text.text = "HIGHSCORE: "+PlayerPrefs.GetFloat("HighScore");
    }
}
