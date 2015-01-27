using UnityEngine;
using System.Collections;

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
	
	public bool UseNAT= false;
	
	private eNetworkRole m_networkRole;
	public eNetworkRole NetworkRole
	{
		get
		{
			return m_networkRole;
		}
	}
	
	private string m_hostIPString;
	private string m_hostPortString;
	
	#endregion
	
	#region methods
	
	void Awake()
	{
		m_networkRole= eNetworkRole.NONE;
		m_hostIPString= "127.0.0.1";
		m_hostPortString= "25001";
		
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
	
	//void OnGUI() GameManager needs to control explicitly when this is done
	public void RenderGUI()
	{
		int port= -1;
		bool portIsNumeric= false;
		
		if (m_hostPortString.Length > 0)
		{
			portIsNumeric= true;
			for (int charIndex= 0; charIndex < m_hostPortString.Length; ++charIndex)
			{
				if (!char.IsDigit(m_hostPortString, charIndex))
				{
					portIsNumeric= false;
					break;
				}
			}
		}
		
		if (portIsNumeric)
		{
			try
			{
				port= System.Convert.ToUInt16(m_hostPortString);
			}
			catch (System.Exception e)
			{
				Debug.LogError(string.Format("port needs to be an unsigned integer! {0}", e.ToString()));
				port= -1;
			}
		}
		
		// provide a toggle for networking
		GameManager.Instance.UseNetworking= GUI.Toggle(new Rect(Screen.width - 200, 10, 180, 30), GameManager.Instance.UseNetworking, "Use Networking", GameManager.Instance.UIStyle);
		
		if (GameManager.Instance.UseNetworking)
		{
			if (NetworkPeerType.Disconnected == Network.peerType)
			{
				GUI.Label(new Rect(10, 10, 300, 30), "Status: Disconnected", GameManager.Instance.UIStyle);
				
				m_hostIPString= GUI.TextField(new Rect(10, 40, 200, 30), m_hostIPString, GameManager.Instance.UIStyle);
				m_hostPortString= GUI.TextField (new Rect(220, 40, 100, 30), m_hostPortString, GameManager.Instance.UIStyle);
				
				if (GUI.Button(new Rect(10, 70, 200, 30), "Call Psychic Hotline", GameManager.Instance.UIStyle))
				{
					if (port > 0)
					{
						Network.Connect(m_hostIPString, port);
					}
				}
				
				if (GUI.Button(new Rect(10, 100, 200, 30), "Wait for Caller", GameManager.Instance.UIStyle))
				{
					if (port > 0)
					{
						const int kMaximumConnections= 2; // ourself + 1 remote client
						
						Network.InitializeServer(kMaximumConnections, port, this.UseNAT);
					}
				}
			}
			else if (NetworkPeerType.Connecting == Network.peerType)
			{
				GUI.Label(new Rect(10, 10, 300, 30), "Calling...");
			}
			else if (NetworkPeerType.Client == Network.peerType)
			{
				GUI.Label(new Rect(10, 10, 200, 30), "Connected to Psychic Hotline!", GameManager.Instance.UIStyle);
				
				if (GUI.Button(new Rect(10, 40, 200, 30), "Hang Up", GameManager.Instance.UIStyle))
				{
					int timeoutMilliseconds= 1000;
					
					Network.Disconnect(timeoutMilliseconds);
				}
			}
			else if (NetworkPeerType.Server == Network.peerType)
			{
				GUI.Label(new Rect(10, 10, 300, 30), "Psychic Hotline is OPEN", GameManager.Instance.UIStyle);
				
				if (GUI.Button(new Rect(10, 40, 200, 30), "Hang Up", GameManager.Instance.UIStyle))
				{
					int timeoutMilliseconds= 1000;
					
					Network.Disconnect(timeoutMilliseconds);
				}
			}
		}
		else // networking turned off
		{
			if (NetworkPeerType.Disconnected != Network.peerType)
			{
				// if networking been turned off, shut 'er down
				int timeoutMilliseconds= 1000;
				
				Network.Disconnect(timeoutMilliseconds);
			}
		}
		
		
		return;
	}
	
	/*
	networking events
	*/
	
	// called on the client when you have successfully connected to a server
	void OnConnectedToServer()
	{
		Debug.Log("Connected to Psychic hotline!");
		InitializeRole(eNetworkRole.HauntedClient);
		
		return;
	}
	
	// called on the client when the connection was lost or you disconnected from the server
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		Debug.Log("Disconnected from Psychic hotline!");
		DisposeRole();
		
		return;
	}
	
	// called on the client when a connection attempt fails for some reason
	void OnFailedToConnect(NetworkConnectionError error)
	{
		Debug.Log("Failed to connect to Psychic hotline!");
		DisposeRole();
		
		return;
	}
	
	// called on clients or servers when there is a problem connecting to the MasterServer
	void OnFailedToConnectToMasterServer(NetworkConnectionError error)
	{
		Debug.Log("Failed to connect to Psychic hotline!");
		DisposeRole();
		
		return;
	}
	
	// called on objects which have been network instantiated with Network.Instantiate
	void OnNetworkInstantiate(NetworkMessageInfo info)
	{
		return;
	}
	
	// used to customize synchronization of variables in a script watched by a network view
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		/* for example:
		// stream.IsWriting is true if we have authority on this object
		if (stream.isWriting)
		{
			Vector3 sendPosition= transform.position;
			
			stream.Serialize(ref sendPosition);
		}
		else
		{
			Vector3 receivedPosition= Vector3.zero;
			
			stream.Serialize(ref receivedPosition);
			transform.position= receivedPosition;
		}
		*/
		
		return;
	}
	
	// called on the server whenever a Network.InitializeServer was invoked and has completed
	void OnServerInitialized()
	{
		Debug.Log("The Psychic hotline is open for business!");
		InitializeRole(eNetworkRole.PsychicServer);
		
		return;
	}
	
	// called on the server whenever a new player has successfully connected
	void OnPlayerConnected(NetworkPlayer player)
	{
		Debug.Log(string.Format("The Psychic hotline has a new caller! {0}:{1} [{2}:{3}]", player.ipAddress, player.port, player.externalIP, player.externalPort));
		
		return;
	}
	
	// called on the server whenever a player disconnected from the server
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Debug.Log(string.Format("The Psychic hotline lost a caller! {0}:{1} [{2}:{3}]", player.ipAddress, player.port, player.externalIP, player.externalPort));
		
		return;
	}
	
	public void InitializeRole(eNetworkRole role)
	{
		m_networkRole= role;
		
		return;
	}
	
	public void DisposeRole()
	{
		m_networkRole= eNetworkRole.NONE;
		
		return;
	}
	
	#endregion
}
