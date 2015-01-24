using UnityEngine;
using System.Collections;

public class TVPuzzle : Puzzle
{
	#region structures
	
	public class Channel : ScriptableObject
	{
		int id;
		Texture2D texture;
		MovieTexture movie;
		string closed_captions;
	};
	
	#endregion
	
	#region data
	
	public Channel[] channelList;
	
	#endregion
	
	#region methods
	void Start()
	{
		return;
	}
	
	void Update()
	{
		return;
	}
	
	#endregion
}
