using UnityEngine;
using UnityEditor;

public class TVChannelAsset
{
	#region methods
	
	[MenuItem("Assets/Create/HauntedHouse/TVChannel")]
	public static void CreateAsset()
	{
		ScriptableObjectUtility.CreateAsset<TVChannel>();
	}
	
	#endregion
}
