using UnityEngine;
using System.Collections;

[RequireComponent (typeof(NetworkView))]
public class NetworkManager : MonoBehaviour
{
	#region constants
	
	public enum eNetworkRole
	{
		NONE= -1,
		PsychicServer= 0,
		HauntedClient,
	};
	
	#endregion
	
	#region data
	
	private eNetworkRole m_networkRole;
	public eNetworkRole NetworkRole
	{
		get
		{
			return m_networkRole;
		}
	}
	
	#endregion
	
	#region methods
	
	void Awake()
	{
		m_networkRole= eNetworkRole.NONE;
		
		return;
	}
	
	void Start()
	{
		return;
	}
	
	void Update()
	{
		return;
	}
	
	public void InitializeRole(eNetworkRole role)
	{
	
	}
	
	public void DisposeRole()
	{
		m_networkRole= eNetworkRole.NONE;
		
		return;
	}
	
	#endregion
}
