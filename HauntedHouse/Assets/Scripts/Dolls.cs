using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Dolls : Puzzle 
{
    [SerializeField]
    private AudioClip[] m_goodClips = new AudioClip[0];

    [SerializeField]
    private AudioClip[] m_badClips = new AudioClip[0];

    //[SerializeField]
    //private GameObject[] m_dollEyes = new GameObject[0];

    [SerializeField]
    private GameObject m_solutionDollEyes = null;

    const int numDollHeads = 9;
    const int numSolutionTones = 2;

    private List<AudioClip> m_orderedGoodClips = null;
    private List<int> m_orderedGoodIndices = null;
    private HashSet<int> m_badIndices = null;

    private bool m_acceptInput = true;
    private int m_progress = 0;

    public void OnEnabled()
    {
        m_acceptInput = true;
        StartCoroutine(PulseEyes());
    }

    public void Start()
    {
        m_acceptInput = true;

        if (m_orderedGoodIndices == null || m_orderedGoodIndices == null || m_badIndices == null)
        {
            List<AudioClip> goodClips = new List<AudioClip>(m_goodClips);
            List<int> goodIndices = new List<int>();

            //fill up the good indices
            for (int i = 0; i < numDollHeads; ++i) { goodIndices.Add(i); }

            m_orderedGoodClips = new List<AudioClip>();
            m_orderedGoodIndices = new List<int>();

            while (m_orderedGoodClips.Count < numSolutionTones)
            {
                AudioClip randomClip = goodClips[Random.Range(0, goodClips.Count)];
                m_orderedGoodClips.Add(randomClip);
                goodClips.Remove(randomClip);

                int randomDollIndex = goodIndices[Random.Range(0, goodIndices.Count)];
                m_orderedGoodIndices.Add(randomDollIndex);
                goodIndices.Remove(randomDollIndex);
            }

            m_badIndices = new HashSet<int>(goodIndices);
        }
    }

    public void OnSolutionDollClick()
    {
        if(!m_acceptInput)
        {
            return;
        }

        m_progress = 0;

        StartCoroutine(PlaySolution());
    }

    private IEnumerator PlaySolution()
    {
        for( int i = 0; i < numSolutionTones; ++i )
        {
            yield return StartCoroutine(PlaySound(m_orderedGoodClips[i]));
        }

        yield return null;
    }

    public void OnDollClick(int dollIndex)
    {
        if (!m_acceptInput)
        {
            return;
        }
     
        if( m_badIndices.Contains(dollIndex))
        {
            m_progress = 0;
            StartCoroutine(PlaySound(m_badClips[Random.Range(0, m_badClips.Length)]));
        }
        else
        {
            int orderedIndex = m_orderedGoodIndices.IndexOf( dollIndex );

            if (m_progress == orderedIndex)
            {
                ++m_progress;
            }
            
            if( m_progress < numSolutionTones )
            {
                StartCoroutine(PlaySound(m_goodClips[orderedIndex]));
            }
            else
            {
                StartCoroutine(PlayFinalSolution(m_goodClips[orderedIndex]));
            }
        }
    }

    private IEnumerator PlaySound( AudioClip clip )
    {
        m_acceptInput = false;

        GameManager.Instance.PlaySound(clip);
        yield return new WaitForSeconds(clip.length);

        m_acceptInput = true;
    }

    private IEnumerator PlayFinalSolution( AudioClip clip )
    {
        yield return StartCoroutine(PlaySound(clip));

        m_acceptInput = false;

        GameManager.Instance.PlaySound(GameManager.Instance.PuzzleSolvedSound);

        yield return null;
    }

    private IEnumerator PulseEyes()
    {
        Image[] images = m_solutionDollEyes.GetComponentsInChildren<Image>();

        float interval = 1.0f;

        bool direction = false;
        float timer = 0.0f;

        while(true)
        {
            if( direction )
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer -= Time.deltaTime;
            }

            if( timer >= interval || timer <= 0.0f )
            {
                direction = !direction;
            }

            float t = Mathf.Clamp01(timer / interval);

            Color color = Color.white;
            color.a = t;

            foreach(Image image in images)
            {
                image.color = color;
            }
        }
    }
}
