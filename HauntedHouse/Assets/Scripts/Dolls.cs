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

    [SerializeField]
    private GameObject[] m_dollEyes = new GameObject[0];

    [SerializeField]
    private GameObject m_solutionDollEyes = null;

    [SerializeField]
    private float m_shakeMagnitude = 0.1f;

    const int numDollHeads = 9;
    const int numSolutionTones = 2;

    private List<AudioClip> m_orderedGoodClips = null;
    private List<int> m_orderedGoodIndices = null;
    private HashSet<int> m_badIndices = null;

    private bool m_acceptInput = true;
    private int m_progress = 0;

    public void OnEnable()
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
        float totalTime = 0.0f;

        foreach( AudioClip clip in m_orderedGoodClips )
        {
            totalTime += clip.length;
        }

        for (int i = 0; i < m_dollEyes.Length; ++i)
        {
            m_dollEyes[i].SetActive(true);
            StartCoroutine(Shake(m_dollEyes[i], totalTime));
        }

        for (int i = 0; i < numSolutionTones; ++i)
        {
            yield return StartCoroutine(PlaySound(m_orderedGoodClips[i], -1));
        }

        for (int i = 0; i < m_dollEyes.Length; ++i)
        {
            m_dollEyes[i].SetActive(false);
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
            StartCoroutine(PlaySound(m_badClips[Random.Range(0, m_badClips.Length)], dollIndex));
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
                StartCoroutine(PlaySound(m_goodClips[orderedIndex], dollIndex));
            }
            else
            {
                StartCoroutine(PlayFinalSolution(m_goodClips[orderedIndex], dollIndex));
            }
        }
    }

    private IEnumerator PlaySound( AudioClip clip, int dollIndex )
    {
        m_acceptInput = false;

        if( dollIndex >= 0 )
        {
            m_dollEyes[dollIndex].SetActive(true);
            StartCoroutine(Shake(m_dollEyes[dollIndex], clip.length));
        }

        GameManager.Instance.PlaySoundPsychicServer(clip);
        yield return new WaitForSeconds(clip.length);

        if( dollIndex >= 0 )
        {
            m_dollEyes[dollIndex].SetActive(false);
        }

        m_acceptInput = true;
    }

    private IEnumerator PlayFinalSolution( AudioClip clip, int dollIndex )
    {
        yield return StartCoroutine(PlaySound(clip, dollIndex));

        m_acceptInput = false;

        GameManager.Instance.PlaySoundPsychicServer(GameManager.Instance.PuzzleSolvedSound);
        GameManager.Instance.PlaySoundHauntedClient(GameManager.Instance.PuzzleSolvedSound);

        for (int i = 0; i < m_dollEyes.Length; ++i )
        {
            yield return new WaitForSeconds(GameManager.Instance.PuzzleSolvedSound.length / m_dollEyes.Length);
            m_dollEyes[i].SetActive(true);
        }

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

            timer = Mathf.Clamp(timer, 0.0f, interval);

            float t = Mathf.Clamp01( 0.5f + (timer / interval) * 0.5f );

            Color color = Color.white;
            color.a = t;

            foreach(Image image in images)
            {
                image.color = color;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator Shake( GameObject obj, float time )
    {
        Vector3 originalPosition = obj.transform.position;

        while( time > 0.0f )
        {
            Vector2 random = Random.insideUnitCircle * m_shakeMagnitude;
            obj.transform.position = originalPosition + new Vector3(random.x, random.y, 0);

            time -= Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        obj.transform.position = originalPosition;

        yield return null;
    }
}
