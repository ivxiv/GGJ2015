using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Candelabra : Puzzle 
{
    [SerializeField]
    private Image[] m_flames = new Image[0];

    [SerializeField]
    private AudioClip[] m_audioTracks = new AudioClip[0];

    [SerializeField]
    private AudioSource[] m_audioSources = new AudioSource[0];

    [SerializeField]
    private Image m_solutionFlame = null;

    [SerializeField]
    private int m_numberOfCandlesForSolution = 2;

    private HashSet<int> m_answer = null;

    internal override void Start()
    {
    	base.Start();
    	
        if( m_answer == null )
        {
            m_answer = new HashSet<int>();

            m_numberOfCandlesForSolution = Random.Range(0, 2) == 0 ? 2 : 3;

            while( m_answer.Count < m_numberOfCandlesForSolution )
            {
                int newCandle = Random.Range(0, m_flames.Length);

                if(!m_answer.Contains(newCandle))
                {
                    m_answer.Add(newCandle);
                }
            }

            foreach( AudioClip clip in m_audioTracks )
            {
                AudioSource[] remainingSources = m_audioSources.Where(source => source.clip == null).ToArray();

                int index = Random.Range(0, remainingSources.Length);
                remainingSources[index].clip = clip;
                remainingSources[index].Play();
            }
        }
    }

    public void OnButton( int candleIndex )
    {
        SetCandleState( candleIndex, !m_flames[candleIndex].gameObject.activeSelf );
    }

    public void OnSolution()
    {
        bool active = !m_solutionFlame.gameObject.activeSelf;

        //force select correct answer
        //if (active && CheckSolution())
        //{
        //    StartCoroutine(DoWinSequence());
        //    return;
        //}

        m_solutionFlame.gameObject.SetActive(active);

        if( active )
        {
            for( int i = 0; i < m_flames.Length; ++i )
            {
                SetCandleState(i, false);
            }
        }

        for (int i = 0; i < m_audioSources.Length; ++i)
        {
            m_audioSources[i].volume = (m_answer.Contains(i) && active) ? 1.0f : 0.0f;
        }
    }

    protected override void OnTimeUp()
    {
    }

    protected override void OnComplete()
    {
    }

    private void SetCandleState( int candleIndex, bool active )
    {
        if( active && m_solutionFlame.gameObject.activeSelf )
        {
            OnSolution();
        }

        m_flames[candleIndex].gameObject.SetActive(active);
        m_audioSources[candleIndex].volume = active ? 1.0f : 0.0f;

        //auto select correct answer
        if (CheckSolution())
        {
            StartCoroutine(DoWinSequence());
            return;
        }
    }

    private bool CheckSolution()
    {
        HashSet<int> activeCandles = new HashSet<int>();
        
        for( int i = 0; i < m_flames.Length; ++i )
        {
            if( m_flames[i].gameObject.activeSelf )
            {
                activeCandles.Add(i);
            }
        }

        return activeCandles.IsSubsetOf(m_answer) && activeCandles.IsSupersetOf(m_answer);
    }

    private IEnumerator DoWinSequence()
    {
        foreach( Button button in GetComponentsInChildren<Button>() )
        {
            button.enabled = false;
        }

        GameManager.Instance.PlaySoundHauntedClient(GameManager.Instance.PuzzleSolvedSound);

        float timer = 0.0f;

        while(timer < 1.5f)
        {
            int index = Random.Range(0, m_flames.Length + 1);

            GameObject flame = m_solutionFlame.gameObject;

            if( index < m_flames.Length )
            {
                flame = m_flames[index].gameObject;
            }

            flame.SetActive(!flame.activeSelf);

            float interval = Random.Range(0.0f, 0.1f);

            yield return new WaitForSeconds(interval);

            timer += interval;
        }

        for (int i = 0; i < m_flames.Length; ++i )
        {
            m_flames[i].gameObject.SetActive(true);
            m_audioSources[i].volume = 0.0f;
        }

        m_solutionFlame.gameObject.SetActive(true);

		GameManager.Instance.OnPuzzleComplete ();

        yield return null;
    }
}
