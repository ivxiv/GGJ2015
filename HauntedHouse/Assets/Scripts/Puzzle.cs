using UnityEngine;
using System.Collections;

public abstract class Puzzle : MonoBehaviour 
{
	#region structures
	
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
	
	#endregion
	
	#region data
	
	public PuzzleClue[] clueList;
	
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
