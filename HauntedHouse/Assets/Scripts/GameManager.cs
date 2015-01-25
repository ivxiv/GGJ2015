using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
    private static GameManager m_instance = null;
    public static GameManager Instance
    {
        get { return m_instance; }
    }

    public AudioClip PuzzleSolvedSound = null;

    private AudioSource m_audioSource = null;

    private void Awake()
    {
        m_instance = this;
        m_audioSource = gameObject.AddComponent<AudioSource>();
    }

	public bool PlaySoundPsychicServer(AudioClip audioClip, float volume = 1.0f)
	{
		if (IsNetworkPsychicServer())
		{
			PlaySound(audioClip, volume);
			return true;
		}
		
		return false;
	}
	
	public bool PlaySoundHauntedClient(AudioClip audioClip, float volume = 1.0f)
	{
		if (IsNetworkHauntedClient())
		{
			PlaySound(audioClip, volume);
			return true;
		}
		
		return false;
	}
	
    private void PlaySound( AudioClip audioClip, float volume )
    {
		m_audioSource.volume = volume;
        m_audioSource.clip = audioClip;
        m_audioSource.Play();
    }
    
    internal NetworkManager GetNetworkManager()
    {
    	return GetComponent<NetworkManager>();
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
	
	/*
	RPC methods
	*/
}
