using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Puzzle : MonoBehaviour 
{
	#region data
	
	public List<PuzzleClue> clueList;
	
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
	
	internal List<PuzzleClue> GetPuzzleClues()
	{
		return this.clueList;
	}
	
	#endregion
    
}
