using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class OuijaBoard : Puzzle 
{
    [SerializeField]
    private string m_word = "MURDER";

    [SerializeField]
    private GameObject m_planchette = null;

    [SerializeField]
    private float m_planchetteSpeed = 5.0f;

    [SerializeField]
    private AudioSource m_audio = null;

    [SerializeField]
    private AudioClip m_successClip = null;

    [SerializeField]
    private float m_maxDistance = 20.0f;

    [SerializeField]
    private float m_minimumDistance = 1.0f;

    [SerializeField]
    private Text m_text = null;

    private int m_index = 0;
    private Vector3 m_originalPosition = Vector3.zero;
    private Dictionary<char, Transform> m_letters = new Dictionary<char,Transform>();

    public void Awake()
    {
        foreach( Transform letter in GetComponentsInChildren<Transform>().Where((tran) => m_word.ToUpper().Contains(tran.gameObject.name.ToUpper())) )
        {
            m_letters[letter.gameObject.name[0]] = letter;
        }

        m_originalPosition = m_planchette.transform.position;
        m_text.text = "";
    }

    public void Update()
    {
        Vector3 control = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        control.Normalize();

        m_planchette.transform.position = m_planchette.transform.position + control * m_planchetteSpeed;

        if( m_index < m_word.Length )
        {
            char letter = m_word[m_index];
            Transform letterTransform = m_letters[letter];

            float distance = (m_planchette.transform.position - letterTransform.position).magnitude;

            float t = Mathf.Clamp01(1.0f - (distance - 1) / m_maxDistance);
            t = t * t * t * t;
            m_audio.volume = t;

            if( distance < m_minimumDistance )
            {
                m_index++;
                GameManager.Instance.PlaySound(m_successClip, 1.0f);
                m_planchette.transform.position = m_originalPosition;

                m_text.text = m_word.Substring(0, m_index);

                if (m_index == m_word.Length)
                {
                    m_audio.volume = 0.0f;
                    m_planchette.gameObject.SetActive(false);
                }
            }

        }
    }
}
