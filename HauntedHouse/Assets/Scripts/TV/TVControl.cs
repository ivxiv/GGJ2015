using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TODO: reference list of movies

[RequireComponent (typeof(AudioSource))]

public class TVControl : MonoBehaviour
{
	#region data
	
	public int minimumChannelNumber= 2;
	public int maximumChannelNumber= 99;
	
	public MovieTexture staticChannelMovie;
	
	public AudioClip channelClick;
	
	private int m_currentChannel;
	public int CurrentChannel
	{
		get
		{
			return m_currentChannel;
		}
	}
	
	private float m_nextInputCheckTime= 0.0f;
	
	#endregion
	
	#region methods
	
	void Start()
	{
		// we start on an invalid channel
		m_currentChannel= minimumChannelNumber-1;
		ChangeToChannel(null);
		
		return;
	}
	
	void OnGUI()
	{
		Transform downChannel= this.transform.FindChild("down_channel");
		Transform upChannel= this.transform.FindChild("up_channel");
		Transform channelLabel= this.transform.FindChild("channel_label");
		bool canTakeInput= (this.m_nextInputCheckTime <= Time.time);
		
		if (null != downChannel)
		{
			Vector3 screenPoint= Camera.main.WorldToScreenPoint(downChannel.position);
			
			if (GUI.Button(new Rect(screenPoint.x, screenPoint.y, 20, 20), "-"))
			{
				if (canTakeInput)
				{
					Debug.Log("DOWN channel");
					ChannelChange(-1);
				}
			}
		}
		
		if (null != upChannel)
		{
			Vector3 screenPoint= Camera.main.WorldToScreenPoint(upChannel.position);
			
			if (GUI.Button(new Rect(screenPoint.x, screenPoint.y, 20, 20), "+"))
			{
				if (canTakeInput)
				{
					Debug.Log("UP channel");
					ChannelChange(+1);
				}
			}
		}
		
		if (null != channelLabel)
		{
			Vector3 screenPoint= Camera.main.WorldToScreenPoint(channelLabel.position);
			
			GUI.Label(new Rect(screenPoint.x, screenPoint.y, 40, 20), m_currentChannel.ToString());
		}
		
		return;
	}
	
	public void InhibitInputForSeconds(float seconds)
	{
		this.m_nextInputCheckTime= Time.time + seconds;
		
		return;
	}
	
	private void ChannelChange(int direction)
	{
		TVPuzzle puzzle= GetPuzzle();
		string error= string.Empty;
		
		m_currentChannel+= direction;
		
		if (m_currentChannel < minimumChannelNumber)
		{
			m_currentChannel= maximumChannelNumber;
		}
		else if (m_currentChannel > maximumChannelNumber)
		{
			m_currentChannel= minimumChannelNumber;
		}
		
		if (null != puzzle)
		{
			List<TVChannel> channelList= puzzle.GetChannelsSorted();
			
			if (null != channelList && channelList.Count > 0)
			{
				bool found= false;
				
				foreach (TVChannel channel in channelList)
				{
					if (null != channel && channel.channelNumber == m_currentChannel)
					{
						ChangeToChannel(channel);
						found= true;
						break;
					}
				}
				
				if (!found)
				{
					ChangeToChannel(null);
				}
			}
			else
			{
				error= "###TVPuzzle has no channels!";
			}
		}
		else
		{
			error= "### failed to locate TVPuzzle object in parent!";
		}
		
		if (null != this.channelClick)
		{
			GameManager.Instance.PlaySoundHauntedClient(this.channelClick);
		}
		
		Debug.Log(string.Format("{0} channel to: {1} {2}", (direction > 0 ? "UP" : "DOWN"), m_currentChannel, (string.IsNullOrEmpty(error) ? string.Empty : error)));
		
		return;
	}
	
	private void ChangeToChannel(TVChannel channel)
	{
		MovieTexture currentMovie= (renderer.material.mainTexture as MovieTexture);
		
		// stop anything that is currently playing
		if (null != this.audio)
		{
			this.audio.Stop();
		}
		if (null != currentMovie)
		{
			currentMovie.Stop();
		}
		
		// start up the next channel media
		if (null == channel)
		{
			renderer.material.mainTexture= staticChannelMovie;
			
			staticChannelMovie.loop= true;
			staticChannelMovie.Play();
			audio.clip= staticChannelMovie.audioClip;
			audio.loop= true;
			audio.Play();
		}
		else
		{
			if (null != channel.movie)
			{
				renderer.material.mainTexture= channel.movie;
				
				channel.movie.loop= true;
				channel.movie.Play();
				audio.clip= channel.movie.audioClip;
				audio.loop= true;
				audio.Play();
			}
			else if (null != channel.texture)
			{
				renderer.material.mainTexture= channel.texture;
			}
			else
			{
				Debug.Log("###changed to channel with no video sources!");
				ChangeToChannel(null);
			}
			
			if (null != channel.audio)
			{
				audio.clip= channel.audio;
				audio.loop= true;
				audio.Play();
			}
			
			// TODO implement closed captions?
		}
		
		return;
	}
	
	private TVPuzzle GetPuzzle()
	{
		TVPuzzle puzzle= this.GetComponentInParent<TVPuzzle>();
		
		return puzzle;
	}
	
	#endregion
}
