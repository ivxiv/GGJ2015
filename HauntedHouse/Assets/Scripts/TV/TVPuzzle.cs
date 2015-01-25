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
	
	public float secondsBetweenClues= 5.0f;
	private float m_lastClueTimeSeconds;
	
	public float secondsToWatchCorrectChannel= 3.0f;
	private float m_startSecondsOnCorrectChannel;
	
	#endregion
	
	#region methods
	
	void Start()
	{
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
	
	void Update()
	{
		if (channelStack.Count() > 0)
		{
			TVControl controller= GetTVControl();
			int currentChannel= (null != controller) ? controller.CurrentChannel : -1;
			bool onCorrectChannel= (currentChannel == channelStack.Peek());
			float now= Time.time;
			
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
					
					if (null != controller)
					{
						controller.InhibitInputForSeconds((null != this.victorySound) ? this.victorySound.length : 3.0f);
					}
					
					m_startSecondsOnCorrectChannel= -1.0f;
					Debug.Log("CORRECT!");
				}
			}
			
			if (0 == channelStack.Count())
			{
				Debug.Log("TV puzzle solved!!!");
				if (null != this.victorySound)
				{
					GameManager.Instance.PlaySound(this.victorySound);
				}
				Application.OpenURL("http://i0.kym-cdn.com/photos/images/newsfeed/000/562/322/4b8.gif");
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
									if (null != clue.audio)
									{
										GameManager.Instance.PlaySound(clue.audio);
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
		}
		
		return;
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
