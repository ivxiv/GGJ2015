using UnityEngine;
using System.Collections;

public class TVChannel : ScriptableObject
{
	public int channelNumber;
	public string channelName;
	public Texture2D texture;
	public MovieTexture movie;
	public string closedCaptions;
}
