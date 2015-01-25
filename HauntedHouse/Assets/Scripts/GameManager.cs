using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
    private static GameManager m_instance = null;
    public static GameManager Instance
    {
        get { return m_instance; }
    }

    public AudioClip PuzzleSolvedSound = null;

    private AudioSource m_audioSource = null;

    private void Awake()
    {
        m_instance = this;
        m_audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlaySound( AudioClip audioClip, float volume = 1.0f )
    {
		m_audioSource.volume = volume;
        m_audioSource.clip = audioClip;
        m_audioSource.Play();
    }
}
