using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Puzzle : MonoBehaviour 
{
	#region data
	
    protected class Timer
    {
        public delegate void CallBack();

        private CallBack    m_callBack;

        private float   m_timer;
        private float   m_duration;
        private bool    m_active;

        public Timer(float duration, CallBack callBack)
        {
            m_callBack = callBack;

            m_timer = 0.0f;
            m_duration = duration;
            m_active = false;
        }

        public void UpdateImpl(float deltaTime)
        {
            if (m_active)
            {
                m_timer += deltaTime;

                if (m_timer >= m_duration)
                {
                    m_callBack();
                    m_active = false;
                }
            }
        }

        public void StartTimer()
        {
            m_active = true;
        }

        public void PauseTimer()
        {
            m_active = false;
        }

        public void ResetTimer()
        {
            m_timer = 0.0f;
        }
    }

    [SerializeField] private float  m_puzzleDuration;
    public List<PuzzleClue> clueList;

    private Timer m_timer;

	#endregion
	
	#region methods
	
	void Start()
	{
        m_timer = new Timer(m_puzzleDuration, OnTimeUp);
        m_timer.StartTimer();

		return;
	}
	
	void Update()
	{
        m_timer.UpdateImpl(Time.smoothDeltaTime);

		return;
	}
	
	internal List<PuzzleClue> GetPuzzleClues()
	{
		return this.clueList;
	}
	
    protected abstract void OnComplete();
    protected abstract void OnTimeUp();

	#endregion    
}