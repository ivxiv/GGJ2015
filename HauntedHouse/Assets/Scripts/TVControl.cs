using UnityEngine;
using System.Collections;

// TODO: reference list of movies

[RequireComponent (typeof(AudioSource))]

public class TVControl : MonoBehaviour
{
	void Start()
	{
		MovieTexture movie= renderer.material.mainTexture as MovieTexture;
		
		audio.clip= movie.audioClip;
		movie.loop= true;
		movie.Play();
		audio.Play();
		
		return;
	}
	
	void OnGUI()
	{
		Transform downChannel= this.transform.FindChild("down_channel");
		Transform upChannel= this.transform.FindChild("up_channel");
		
		if (null != downChannel)
		{
			Vector3 screenPoint= Camera.main.WorldToScreenPoint(downChannel.position);
			
			if (GUI.Button(new Rect(screenPoint.x, screenPoint.y, 20, 20), "-"))
			{
				Debug.Log("DOWN channel");
			}
		}
		
		if (null != upChannel)
		{
			Vector3 screenPoint= Camera.main.WorldToScreenPoint(upChannel.position);
			
			if (GUI.Button(new Rect(screenPoint.x, screenPoint.y, 20, 20), "+"))
			{
				Debug.Log("UP channel");
			}
		}
		
		return;
	}
}
