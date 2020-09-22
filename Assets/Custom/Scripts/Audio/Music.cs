using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    [SerializeField] List<AudioClip> m_MusicSFX;
    AudioSource m_Audio;

    /// overrides ////////////////////////////////////////////////////////

    void Start() {
        m_Audio = GetComponent<AudioSource>();
    }

    void Update() {
        UpdateMusic();
    }

    /// music //////////////////////////////////////////////////////////////////////////////////////

    void UpdateMusic() {
        if(!m_Audio.isPlaying) {
            m_Audio.clip = m_MusicSFX[Random.Range(0, m_MusicSFX.Count)];
            m_Audio.Play();
        }
    }
}
