using UnityEngine;
using UnityEditor;

public class PuzzleClueAsset
{
	#region methods
	
	[MenuItem("Assets/Create/HauntedHouse/PuzzleClue")]
	public static void CreateAsset()
	{
		ScriptableObjectUtility.CreateAsset<PuzzleClue>();
	}
	
	#endregion
}
