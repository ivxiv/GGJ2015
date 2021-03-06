﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent (typeof(NetworkView))]
public class GameManager : MonoBehaviour 
{
	#region constants
	
	public enum eGameState
	{
		NONE= -1,
		RoomHubIntro= 0,
		RoomHub,
		OuijaPuzzleIntro,
		OuijaPuzzleActive,
		OuijaPuzzleOutro,
		CandelabraPuzzleIntro,
		CandelabraPuzzleActive,
		CandelabraPuzzleOutro,
		PictureSwapPuzzleIntro,
		PictureSwapPuzzleActive,
		PictureSwapPuzzleOutro,
		DollsPuzzleIntro,
		DollsPuzzleActive,
		DollsPuzzleOutro,
		TVPuzzleIntro,
		TVPuzzleActive,
		TVPuzzleOutro,
		GameOver
	};
	
	#endregion
	
	#region singleton
	
    private static GameManager m_instance = null;
    public static GameManager Instance
    {
        get { return m_instance; }
    }
    
    #endregion
    
	#region data

	public Action puzzleCompleteCallback = null;

	public GUIStyle UIStyle;

    public AudioClip PuzzleSolvedSound = null;
    
    public Texture2D RoomHubIntroTexture= null;
    public Texture2D RoomHubTexture= null;
    public Texture2D OuijaPuzzleIntroTexture= null;
    public Texture2D CandelabraPuzzleIntroTexture= null;
    public Texture2D PictureSwapPuzzleIntroTexture= null;
    public Texture2D DollsPuzzleIntroTexture= null;
    public Texture2D TVPuzzleIntroTexture= null;
    public Texture2D RoomOutroTexture= null;
    public Texture2D GameOverTexture= null;
    
    public Texture2D PsychicHotlineTexture= null;

    private AudioSource m_audioSource = null;
    
    private eGameState m_desiredGameState= eGameState.NONE;
    private eGameState m_gameState= eGameState.NONE;
    
    // extra gravy to send us into flavor country
    public bool UseNetworking= false;
    
    public OuijaBoard OuijaPuzzleObject;
    public Candelabra CandelabraPuzzleObject;
    public PictureSwap PictureSwapPuzzleObject;
    public Dolls DollsPuzzleObject;
    public TVPuzzle TVPuzzleObject;
    public GameObject PuzzleHubObject;
    
	public float[] NetworkedVolume= new float[10] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
    
    public List<AudioClip> AudioClipCatalog;
    
    #endregion
    
	#region methods

    void Awake()
    {
        m_instance = this;
        m_audioSource = gameObject.AddComponent<AudioSource>();
        
		m_desiredGameState= eGameState.RoomHubIntro;
    }
    
    void Update()
    {
    	// monitor for game state change requests
    	if (m_desiredGameState != m_gameState)
    	{
    		switch (m_desiredGameState)
    		{
				case eGameState.RoomHubIntro:
				case eGameState.OuijaPuzzleIntro:
				case eGameState.OuijaPuzzleOutro:
				case eGameState.CandelabraPuzzleIntro:
				case eGameState.CandelabraPuzzleOutro:
				case eGameState.PictureSwapPuzzleIntro:
				case eGameState.PictureSwapPuzzleOutro:
				case eGameState.DollsPuzzleIntro:
				case eGameState.DollsPuzzleOutro:
				case eGameState.TVPuzzleIntro:
				case eGameState.TVPuzzleOutro:
				case eGameState.GameOver:
				{
					UnloadPuzzleRoom();
					m_gameState= m_desiredGameState;
					break;
				}
				case eGameState.RoomHub:
				{
					LoadPuzzleRoom(this.PuzzleHubObject);
					m_gameState= m_desiredGameState;
					break;
				}
				case eGameState.OuijaPuzzleActive:
				{
					LoadPuzzleRoom(this.OuijaPuzzleObject.gameObject);
					m_gameState= m_desiredGameState;
					break;
				}
				case eGameState.CandelabraPuzzleActive:
				{
					LoadPuzzleRoom(this.CandelabraPuzzleObject.gameObject);
					m_gameState= m_desiredGameState;
					break;
				}
				case eGameState.PictureSwapPuzzleActive:
				{
					LoadPuzzleRoom(this.PictureSwapPuzzleObject.gameObject);
					m_gameState= m_desiredGameState;
					break;
				}
				case eGameState.DollsPuzzleActive:
				{
					LoadPuzzleRoom(this.DollsPuzzleObject.gameObject);
					m_gameState= m_desiredGameState;
					break;
				}
				case eGameState.TVPuzzleActive:
				{
					LoadPuzzleRoom(this.TVPuzzleObject.gameObject);
					m_gameState= m_desiredGameState;
					break;
				}
				default:
				{
					break;
				}
    		}
    	}

    	return;
    }
    
    void OnGUI()
    {
    	// GameManager UI is drawn first, as appropriate
		switch (m_gameState)
		{
			case eGameState.RoomHubIntro: RenderGameIntroGUI(string.Empty); break;
			case eGameState.RoomHub: RenderRoomHubGUI(); break;
			case eGameState.OuijaPuzzleIntro: RenderLoadingGUI("Proceed to the Ouija Board..."); break;
			case eGameState.OuijaPuzzleActive: RenderPuzzleGameGUI(); break;
			case eGameState.OuijaPuzzleOutro: RenderOutroGUI("Jeepers! That ouija was creepy!"); break;
			case eGameState.CandelabraPuzzleIntro: RenderLoadingGUI("Gaze into the candle light..."); break;
			case eGameState.CandelabraPuzzleActive: RenderPuzzleGameGUI(); break;
			case eGameState.CandelabraPuzzleOutro: RenderOutroGUI("Well I'll never play with fire again."); break;
			case eGameState.PictureSwapPuzzleIntro: RenderLoadingGUI("Peruse the Family Photos..."); break;
			case eGameState.PictureSwapPuzzleActive: RenderPuzzleGameGUI(); break;
			case eGameState.PictureSwapPuzzleOutro: RenderOutroGUI("Am I really related to these people?"); break;
			case eGameState.DollsPuzzleIntro: RenderLoadingGUI("Grandma's Creepy Doll Collection..."); break;
			case eGameState.DollsPuzzleActive: RenderPuzzleGameGUI(); break;
			case eGameState.DollsPuzzleOutro: RenderOutroGUI("OK... I hate dolls now."); break;
			case eGameState.TVPuzzleIntro: RenderLoadingGUI("Who left the TV on?"); break;
			case eGameState.TVPuzzleActive: RenderPuzzleGameGUI(); break;
			case eGameState.TVPuzzleOutro: RenderOutroGUI("Cable seems to be on the fritz..."); break;
			case eGameState.GameOver: RenderGameOverGUI(); break;
			default: break;
		}
    	
    	return;
    }
    
    private bool RenderPsychicHotlineGUI()
    {
    	if (this.UseNetworking && this.IsNetworkPsychicServer())
    	{
			if (null != this.PsychicHotlineTexture)
			{
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.PsychicHotlineTexture);
			}
			
			// we need to ensure that we draw the networking GUI last, so we do that here
			NetworkManager networkManager= GetNetworkManager();
			
			if (null != networkManager)
			{
				networkManager.RenderGUI();
			}
			
			return true;
    	}
    	
    	return false;
    }
    
	private void RenderGameIntroGUI(string message)
    {
		if (!RenderPsychicHotlineGUI())
		{
			if (null != this.RoomHubIntroTexture)
			{
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.RoomHubIntroTexture);
			}
			
			// we need to ensure that we draw the networking GUI last, so we do that here
			NetworkManager networkManager= GetNetworkManager();
			
			if (null != networkManager)
			{
				networkManager.RenderGUI();
			}
			
			// we key off of network state to advance from this point on, either as the psychic or the haunted player
			if (this.IsNetworkActive())
			{
				// once we have a network connection, advance to the next state
				if (eGameState.RoomHub != this.m_gameState)
				{
					this.SetDesiredGameState(eGameState.RoomHub);
				}
			}
			else
			{
				// render a button to allow manual advance into the room hub
				if (GUI.Button(new Rect(200, Screen.height - 50, 200, 30), "Enter the Home", this.UIStyle))
				{
					this.SetDesiredGameState(eGameState.RoomHub);
				}
			}
		}
		
    	return;
    }
    
	private void RenderRoomHubGUI()
	{
		if (!RenderPsychicHotlineGUI())
		{
#if false // not using the new hub hotness?
			if (null != this.RoomHubTexture)
			{
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.RoomHubTexture);
			}
			
			// render a button for each puzzle
			if (GUI.Button(new Rect(10, 200, 200, 30), "Ouija Board", GameManager.Instance.UIStyle))
			{
				this.SetDesiredGameState(eGameState.OuijaPuzzleIntro);
			}
			else if (GUI.Button(new Rect(10, 240, 200, 30), "Doll Collection", GameManager.Instance.UIStyle))
			{
				this.SetDesiredGameState(eGameState.DollsPuzzleIntro);
			}
			else if (GUI.Button(new Rect(10, 280, 200, 30), "Family Photos", GameManager.Instance.UIStyle))
			{
				this.SetDesiredGameState(eGameState.PictureSwapPuzzleIntro);
			}
			else if (GUI.Button(new Rect(10, 320, 200, 30), "Watch TV", GameManager.Instance.UIStyle))
			{
				this.SetDesiredGameState(eGameState.TVPuzzleIntro);
			}
			else if (GUI.Button(new Rect(10, 360, 200, 30), "Candelabra", GameManager.Instance.UIStyle))
			{
				this.SetDesiredGameState(eGameState.CandelabraPuzzleIntro);
			}
			
			if (UseNetworking)
			{
				// we need to ensure that we draw the networking GUI last, so we do that here
				NetworkManager networkManager= GetNetworkManager();
				
				if (null != networkManager)
				{
					networkManager.RenderGUI();
				}
			}
#endif
		}
		
		return;
	}
	
	private void RenderLoadingGUI(string message)
	{
		if (!RenderPsychicHotlineGUI())
		{
			// render a button for beginning the next puzzle
			if (GUI.Button(new Rect(10, 200, 400, 30), message, GameManager.Instance.UIStyle))
			{
				this.SetDesiredGameState((eGameState)(((int)m_gameState)+1));
			}
		}
		
		return;
	}
	
    private void RenderOutroGUI(string message)
    {
		if (!RenderPsychicHotlineGUI())
		{
			// render a button to allow manual advance back to the hub
			if (GUI.Button(new Rect(10, 200, 400, 30), message, GameManager.Instance.UIStyle))
			{
				this.SetDesiredGameState(eGameState.RoomHub);
			}
		}
		
    	return;
    }
    
    
    
    private void RenderPuzzleGameGUI()
    {
		if (!RenderPsychicHotlineGUI())
		{
	    	// render progress bar, timer, etc.
	    }
    	
    	return;
    }
    
    public void RenderGameOverGUI()
    {
		if (!RenderPsychicHotlineGUI())
		{
	    	// render something glorious or something hellishly terrible
	    }
    	
    	return;
    }
    
    public void SetNetworkedVolume(int volumeIndex, float value)
    {
		if (this.UseNetworking && IsNetworkActive())
		{
	    	// only the clinet gets to set networked volumes
			if (IsNetworkHauntedClient())
			{
				// call everything on both host + client
				RPCMode callMode= RPCMode.All;
				
				networkView.RPC("SetNetworkedVolume_Internal", callMode, volumeIndex, value);
			}
		}
		else
		{
			// just set the volume
			SetNetworkedVolume_Internal(volumeIndex, value);
		}
		
		return;
    }
    
    [RPC]
	private void SetNetworkedVolume_Internal(int volumeIndex, float value)
	{
		this.NetworkedVolume[volumeIndex]= value;
		
		return;
	}
    
    public void SetDesiredGameState(eGameState newState)
    {
		if (IsNetworkActive())
		{
			// call everything on both host + client
			RPCMode callMode= RPCMode.All;
			
			networkView.RPC("SetDesiredGameState_Internal", callMode, (int)newState);
			Debug.Log("RPC=> SetDesiredGameState_Internal()");
		}
		else
		{
			// just switch to the room
			SetDesiredGameState_Internal((int)newState);
		}
		
		return;
    }
    
	[RPC]
	private void SetDesiredGameState_Internal(int stateValue)
	{
		m_desiredGameState= (eGameState)stateValue;
		
		return;
	}
    
    private GameObject GetPuzzleRoomObjectByName(string puzzleObjectName)
    {
    	string testName= puzzleObjectName.ToLowerInvariant();
    	
		if ((null != OuijaPuzzleObject) && testName.Equals(OuijaPuzzleObject.name.ToLowerInvariant())) return OuijaPuzzleObject.gameObject;
		if ((null != CandelabraPuzzleObject) && testName.Equals(CandelabraPuzzleObject.name.ToLowerInvariant())) return CandelabraPuzzleObject.gameObject;
		if ((null != PictureSwapPuzzleObject) && testName.Equals(PictureSwapPuzzleObject.name.ToLowerInvariant())) return PictureSwapPuzzleObject.gameObject;
		if ((null != DollsPuzzleObject) && testName.Equals(DollsPuzzleObject.name.ToLowerInvariant())) return DollsPuzzleObject.gameObject;
		if ((null != TVPuzzleObject) && testName.Equals(TVPuzzleObject.name.ToLowerInvariant())) return TVPuzzleObject.gameObject;
		if ((null != PuzzleHubObject) && testName.Equals(PuzzleHubObject.name.ToLowerInvariant())) return PuzzleHubObject;
		
		return null;
    }
    
	private AudioClip LocateAudioClipByName(string audioClipName)
	{
		string testName= audioClipName.ToLowerInvariant();
		
		foreach (AudioClip clip in AudioClipCatalog)
		{
			if ((null != clip) && testName.Equals(clip.name.ToLowerInvariant()))
			{
				return clip;
			}
		}
		
		return null;
	}
    
    void LoadPuzzleRoom(GameObject roomObject)
    {
    	if (IsNetworkActive())
    	{
			// call everything on both host + client
			RPCMode callMode= RPCMode.All;
			
			networkView.RPC("LoadPuzzleRoom_Internal", callMode, roomObject.name);
			Debug.Log("RPC=> LoadPuzzleRoom_Internal()");
    	}
    	else
    	{
    		// just switch to the room
    		LoadPuzzleRoom_Internal(roomObject.name);
    	}
    	
    	return;
    }
    
	[RPC]
    private void LoadPuzzleRoom_Internal(string roomObjectName)
    {
    	GameObject puzzleRoom= GetPuzzleRoomObjectByName(roomObjectName);
		
		if (null != puzzleRoom)
		{
			if (null != OuijaPuzzleObject) OuijaPuzzleObject.gameObject.SetActive(false);
			if (null != CandelabraPuzzleObject) CandelabraPuzzleObject.gameObject.SetActive(false);
			if (null != PictureSwapPuzzleObject) PictureSwapPuzzleObject.gameObject.SetActive(false);
			if (null != DollsPuzzleObject) DollsPuzzleObject.gameObject.SetActive(false);
			if (null != TVPuzzleObject) TVPuzzleObject.gameObject.SetActive(false);
			if (null != PuzzleHubObject) PuzzleHubObject.gameObject.SetActive(false);
			
			puzzleRoom.SetActive(true);
			Debug.Log(string.Format("Room '{0}' is now ACTIVE", roomObjectName));
		}
		else
		{
			Debug.LogWarning(string.Format("failed to locate puzzle room '{0}'", roomObjectName));
		}
		
    	return;
    }
    
	void UnloadPuzzleRoom()
	{
		if (IsNetworkActive())
		{
			// call everything on both host + client
			RPCMode callMode= RPCMode.All;
			
			networkView.RPC("UnoadPuzzleRoom_Internal", callMode);
			Debug.Log("RPC=> UnoadPuzzleRoom_Internal()");
		}
		else
		{
			// just unload the room
			UnoadPuzzleRoom_Internal();
		}
		
		return;
	}
	
	[RPC]
	private void UnoadPuzzleRoom_Internal()
	{
		if (null != OuijaPuzzleObject) OuijaPuzzleObject.gameObject.SetActive(false);
		if (null != CandelabraPuzzleObject) CandelabraPuzzleObject.gameObject.SetActive(false);
		if (null != PictureSwapPuzzleObject) PictureSwapPuzzleObject.gameObject.SetActive(false);
		if (null != DollsPuzzleObject) DollsPuzzleObject.gameObject.SetActive(false);
		if (null != TVPuzzleObject) TVPuzzleObject.gameObject.SetActive(false);
		if (null != PuzzleHubObject) PuzzleHubObject.gameObject.SetActive(false);
		
		Debug.Log("All rooms deactivated");
		
		return;
	}

	public void PlaySoundPsychicServer(AudioClip audioClip, float volume = 1.0f)
	{
		if (IsNetworkActive())
		{
			// call everything on both host + client
			RPCMode callMode= RPCMode.All;
			
			networkView.RPC("PlaySoundPsychicServer_Internal", callMode, audioClip.name, volume);
			Debug.Log("RPC=> PlaySoundPsychicServer_Internal()");
		}
		else
		{
			// just play the sound
			PlaySound(audioClip, volume);
		}
		
		return;
	}
	
	[RPC]
	private void PlaySoundPsychicServer_Internal(string audioClipName, float volume )
	{
		if (IsNetworkPsychicServer())
		{
			AudioClip clip= LocateAudioClipByName(audioClipName);
			
			if (null != clip)
			{
				Debug.Log(string.Format("playing server sound '{0}'", audioClipName));
				PlaySound(clip, volume);
			}
			else
			{
				Debug.LogWarning(string.Format("audio clip '{0}' is missing from the GameManager's catalog", audioClipName));
			}
		}
		else
		{
			Debug.Log("skipping sound b/c we're not the server");
		}
		
		return;
	}
	
	public void PlaySoundHauntedClient(AudioClip audioClip, float volume = 1.0f)
	{
		if (IsNetworkActive())
		{
			// call everything on both host + client
			RPCMode callMode= RPCMode.All;
			
			networkView.RPC("PlaySoundHauntedClient_Internal", callMode, audioClip.name, volume);
			Debug.Log("RPC=> PlaySoundHauntedClient_Internal()");
		}
		else
		{
			// just play the sound
			PlaySound(audioClip, volume);
		}
		
		return;
	}
	
	[RPC]
	private void PlaySoundHauntedClient_Internal(string audioClipName, float volume )
	{
		if (IsNetworkHauntedClient())
		{
			AudioClip clip= LocateAudioClipByName(audioClipName);
			
			if (null != clip)
			{
				Debug.Log(string.Format("playing client sound '{0}'", audioClipName));
				PlaySound(clip, volume);
			}
			else
			{
				Debug.LogWarning(string.Format("audio clip '{0}' is missing from the GameManager's catalog", audioClipName));
			}
		}
		else
		{
			Debug.Log("skipping sound b/c we're not the client");
		}
		
		return;
	}
	
    private void PlaySound( AudioClip audioClip, float volume )
    {
		m_audioSource.volume = volume;
        m_audioSource.clip = audioClip;
        m_audioSource.Play();
    }
    
    internal NetworkManager GetNetworkManager()
    {
    	return this.GetComponentInChildren<NetworkManager>();
    }
    
    public bool IsNetworkActive()
    {
		NetworkManager networkManager= GetNetworkManager();
		
		return (null != networkManager) ?
			((NetworkManager.eNetworkRole.HauntedClient == networkManager.NetworkRole) || (NetworkManager.eNetworkRole.PsychicServer == networkManager.NetworkRole)) :
			false;
	}
	
	public bool IsNetworkPsychicServer()
	{
		NetworkManager networkManager= GetNetworkManager();
		
		return (null != networkManager) ? (NetworkManager.eNetworkRole.PsychicServer == networkManager.NetworkRole) : false;
	}
	
	public bool IsNetworkHauntedClient()
	{
		NetworkManager networkManager= GetNetworkManager();
		
		return (null != networkManager) ? (NetworkManager.eNetworkRole.HauntedClient == networkManager.NetworkRole) : false;
	}
    
	public void PerformRPC(string methodName, params object[] args)
	{
		// call everything on both host + client
		RPCMode callMode= RPCMode.All;
		
		networkView.RPC(methodName, callMode, args);
		
		return;
	}

	public void OnPuzzleComplete()
	{
		Debug.Log ("PUZZLE COMPLETE!");

		if( puzzleCompleteCallback != null )
		{
			puzzleCompleteCallback();
		}
	}

	#endregion
}
