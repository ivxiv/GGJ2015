﻿using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class OuijaBoard : Puzzle 
{
    [SerializeField]
    private string[] m_words = new string[0];

    [SerializeField]
    private GameObject m_planchette = null;

    [SerializeField]
    private float m_planchetteSpeed = 5.0f;

    [SerializeField]
    private AudioSource m_audio = null;

    [SerializeField]
    private AudioSource m_nearTargetAudioSource = null;

    [SerializeField]
    private AudioSource m_planchetteMovingAudioSource = null;

    [SerializeField]
    private AudioClip m_successClip = null;

    [SerializeField]
    private float m_maxDistance = 20.0f;

    [SerializeField]
    private float m_minimumDistance = 1.0f;

    [SerializeField]
    private Text m_text = null;

    [SerializeField]
    private float m_nearTargetTime = 1.0f;

    private int m_index = 0;
    private Vector3 m_originalPosition = Vector3.zero;
    private Dictionary<char, Transform> m_letters = new Dictionary<char,Transform>();

    private float m_nearTargetTimer = 0.0f;

    private string m_word = string.Empty;
    
    private const int kPlanchetteMovingNetworkedVolumeIndex= 0;
    private const int kOuijaNetworkedVolumeIndex= 1;
    private const int kNearTargetNetworkedVolumeIndex= 2;

    void Awake()
    {
        m_word = m_words[Random.Range(0, m_words.Length)];

        foreach( Transform letter in GetComponentsInChildren<Transform>().Where((tran) => m_word.ToUpper().Contains(tran.gameObject.name.ToUpper())) )
        {
            m_letters[letter.gameObject.name[0]] = letter;
        }

        m_originalPosition = m_planchette.transform.position;
        m_text.text = "";

        m_audio.volume = 0.0f;
        m_nearTargetAudioSource.volume = 0.0f;
        m_planchetteMovingAudioSource.volume = 0.0f;
    }
    
    internal override void Start()
    {
    	base.Start();
    	
		GameManager.Instance.SetNetworkedVolume(kPlanchetteMovingNetworkedVolumeIndex, 0.0f);
		GameManager.Instance.SetNetworkedVolume(kOuijaNetworkedVolumeIndex, 0.0f);
		GameManager.Instance.SetNetworkedVolume(kNearTargetNetworkedVolumeIndex, 0.0f);
		
		return;
    }
    
    private float AdjustPlaybackVolumeForNetwork(float volume)
    {
    	if (GameManager.Instance.UseNetworking)
    	{
    		if (GameManager.Instance.IsNetworkHauntedClient())
    		{
    			return 0.0f;
    		}
    	}
    	
    	return volume;
    }

    internal override void Update()
    {
    	base.Update();
    	
		m_planchetteMovingAudioSource.volume= AdjustPlaybackVolumeForNetwork(GameManager.Instance.NetworkedVolume[kPlanchetteMovingNetworkedVolumeIndex]);
		m_audio.volume= AdjustPlaybackVolumeForNetwork(GameManager.Instance.NetworkedVolume[kOuijaNetworkedVolumeIndex]);
		m_nearTargetAudioSource.volume= AdjustPlaybackVolumeForNetwork(GameManager.Instance.NetworkedVolume[kNearTargetNetworkedVolumeIndex]);
    	
        if( Input.GetMouseButton(0) )
        {
            Camera mainCamera = GetComponentInParent<Camera>();
            Vector3 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            Vector3 controlDirection = worldPoint - m_planchette.transform.position;
            controlDirection.z = 0.0f;
            
            if( controlDirection.magnitude < m_planchetteSpeed )
            {
                controlDirection = Vector3.zero;
            }
            else
            {
                controlDirection.Normalize();
            }
            
			float planchetteVolume= controlDirection.magnitude > 0.0f ? 1.0f : 0.0f;
			
			if (GameManager.Instance.UseNetworking && !GameManager.Instance.IsNetworkPsychicServer())
			{
				planchetteVolume = 0.0f;
			}
			
			GameManager.Instance.SetNetworkedVolume(kPlanchetteMovingNetworkedVolumeIndex, planchetteVolume);

            m_planchette.transform.position = m_planchette.transform.position + controlDirection * m_planchetteSpeed;
        }
        else
        {
			GameManager.Instance.SetNetworkedVolume(kPlanchetteMovingNetworkedVolumeIndex, 0.0f);
        }

        if( m_index < m_word.Length )
        {
            char letter = m_word[m_index];
            Transform letterTransform = m_letters[letter];

            float distance = (m_planchette.transform.position - letterTransform.position).magnitude;

            float t = Mathf.Clamp01(1.0f - (distance - 1) / m_maxDistance);
            t = t * t * t * t;
            
			GameManager.Instance.SetNetworkedVolume(kOuijaNetworkedVolumeIndex, t);
            
            if( distance < m_minimumDistance )
            {
                m_nearTargetTimer += Time.deltaTime;

                if (m_nearTargetTimer >= m_nearTargetTime)
                {
                    m_index++;
                    m_planchette.transform.position = m_originalPosition;

                    m_text.text = m_word.Substring(0, m_index);

                    if (m_index == m_word.Length)
                    {
                        m_nearTargetTimer = 0.0f;
						GameManager.Instance.SetNetworkedVolume(kOuijaNetworkedVolumeIndex, t);
                        m_planchette.gameObject.SetActive(false);

						StartCoroutine( PlayCompleteSound() );
                    }
                    else
                    {
                        GameManager.Instance.PlaySoundPsychicServer(m_successClip, 1.0f);
                    }
                }
            }
            else
            {
                m_nearTargetTimer = 0.0f;
            }
        }

		GameManager.Instance.SetNetworkedVolume(kNearTargetNetworkedVolumeIndex, ((m_nearTargetTimer > 0.0f) ? 1.0f : 0.0f));
		
		return;
    }

	private IEnumerator PlayCompleteSound()
	{
		GameManager.Instance.PlaySoundPsychicServer(GameManager.Instance.PuzzleSolvedSound);
		GameManager.Instance.PlaySoundHauntedClient(GameManager.Instance.PuzzleSolvedSound);

		yield return new WaitForSeconds( GameManager.Instance.PuzzleSolvedSound.length );

		GameManager.Instance.OnPuzzleComplete ();
	}

    protected override void OnTimeUp()
    {
    }
    
    protected override void OnComplete()
    {
    }
}
