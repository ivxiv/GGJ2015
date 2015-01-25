using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(NetworkView))]
public class GameManager : MonoBehaviour 
{
	#region singleton
	
    private static GameManager m_instance = null;
    public static GameManager Instance
    {
        get { return m_instance; }
    }
    
    #endregion
    
	#region data

    public AudioClip PuzzleSolvedSound = null;

    private AudioSource m_audioSource = null;
    
    public List<AudioClip> AudioClipCatalog;
    
    #endregion
    
	#region methods

    private void Awake()
    {
        m_instance = this;
        m_audioSource = gameObject.AddComponent<AudioSource>();
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
	
	#endregion
}
