using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{
    [SerializeField]
    string GameScene;
    public void PlayGame() {
        SceneManager.LoadScene(GameScene);
    }
}
