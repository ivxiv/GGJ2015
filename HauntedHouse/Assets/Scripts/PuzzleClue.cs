using UnityEngine;

public class PuzzleClue : ScriptableObject
{
	#region data
	
	public string id;
	
	public Texture2D image;
	public MovieTexture video;
	public AudioClip audio;
	public string text;
	
	#endregion
}
