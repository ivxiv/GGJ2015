using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TVPuzzle : Puzzle
{
	#region data
	
	public List<TVChannel> channelList;
	
	public List<int> puzzleChannelSequence;
	private Stack<int> channelStack= new Stack<int>();
	
	public AudioClip victorySound;
	
	public float secondsBetweenClues= 3.0f;
	private float m_lastClueTimeSeconds;
	
	public float secondsToWatchCorrectChannel= 3.0f;
	private float m_startSecondsOnCorrectChannel;
	
	private bool m_victorySoundPlaying= false;
	
	#endregion
	
	#region methods
	
	internal override void Start()
	{
		base.Start();
		
		m_lastClueTimeSeconds= Time.time;
		m_startSecondsOnCorrectChannel= -1.0f;
		List<int> reversedChannels= puzzleChannelSequence;
		
		reversedChannels.Reverse();
		foreach (int channel in puzzleChannelSequence)
		{
			channelStack.Push(channel);
		}
		
		return;
	}
	
	internal override void Update()
	{
		base.Update();
		
		if (GameManager.Instance.UseNetworking && GameManager.Instance.IsNetworkPsychicServer())
		{
			// no TV for the psychic
			return;
		}
		
		if (channelStack.Count() > 0)
		{
			TVControl controller= GetTVControl();
			int currentChannel= (null != controller) ? controller.CurrentChannel : -1;
			bool onCorrectChannel= (currentChannel == channelStack.Peek());
			float now= Time.time;
			bool channelCorrect= false;
			bool puzzleComplete= false;
			
			// did we get a channel correct?
			if (onCorrectChannel)
			{
				if (m_startSecondsOnCorrectChannel < 0.0f)
				{
					m_startSecondsOnCorrectChannel= now;
				}
				
				if ((now - m_startSecondsOnCorrectChannel) >= secondsToWatchCorrectChannel)
				{
					// correct! clear this channel...
					channelStack.Pop();
					m_lastClueTimeSeconds= 0.0f;
					
					m_startSecondsOnCorrectChannel= -1.0f;
					channelCorrect= true;
					Debug.Log("CORRECT!");
				}
			}
			else
			{
				m_startSecondsOnCorrectChannel= -1.0f;
			}
			
			if (0 == channelStack.Count())
			{
				puzzleComplete= true;
				Debug.Log("TV puzzle solved!!!");
			}
			else if (!onCorrectChannel)
			{
				if ((now - m_lastClueTimeSeconds) >= secondsBetweenClues)
				{
					int nextClueChannel= channelStack.Peek();
					List<PuzzleClue> clueList= GetPuzzleClues();
					
					foreach (PuzzleClue clue in clueList)
					{
						if (null != clue)
						{
							try
							{
								int channelValue= System.Convert.ToInt32(clue.id);
								
								// TODO add multiple clues per channel, pick one from random
								if (channelValue == nextClueChannel)
								{
									if (null != clue.audio && !m_victorySoundPlaying)
									{
										GameManager.Instance.PlaySoundPsychicServer(clue.audio);
										break;
									}
								}
							}
							catch (System.Exception e)
							{
								Debug.LogError(string.Format("###channel id '{0}' needs to be a number! {1}", clue.id, e.ToString()));
							}
						}
					}
					m_lastClueTimeSeconds= now;
				}
			}
			
			if (channelCorrect || puzzleComplete)
			{
				StartCoroutine(PlayCompleteSound(puzzleComplete));
			}
		}
		
		return;
	}
	
	private IEnumerator PlayCompleteSound(bool puzzleFinished)
	{
		TVControl controller= GetTVControl();
		float durationSeconds= (null != this.victorySound) ? this.victorySound.length : 3.0f;
		
		if (null != controller)
		{
			controller.InhibitInputForSeconds(durationSeconds);
		}
		
		if (null != this.victorySound)
		{
			m_victorySoundPlaying= true;
			GameManager.Instance.PlaySoundPsychicServer(this.victorySound);
			GameManager.Instance.PlaySoundHauntedClient(this.victorySound);
		}
		
		yield return new WaitForSeconds(durationSeconds);
		m_victorySoundPlaying= false;
		
		if (puzzleFinished)
		{
			GameManager.Instance.OnPuzzleComplete();
		}
		
		yield break;
	}

    protected override void OnTimeUp()
    {
    }
    
    protected override void OnComplete()
    {
    }
	
	public List<TVChannel> GetChannelsSorted()
	{
		return channelList.OrderBy(x => (null != x) ? x.channelNumber : 0).ToList();
	}
	
	private TVControl GetTVControl()
	{
		TVControl controller= this.GetComponentInChildren<TVControl>();
		
		return controller;
	}
	
	#endregion
}
