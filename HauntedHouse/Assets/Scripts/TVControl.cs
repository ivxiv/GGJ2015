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
		Transform down_channel= this.transform.FindChild("down_channel");
		Transform up_channel= this.transform.FindChild("up_channel");
		
		if (null != down_channel)
		{
			Vector3 screen_point= Camera.main.WorldToScreenPoint(down_channel.position);
			
			if (GUI.Button(new Rect(screen_point.x, screen_point.y, 20, 20), "-"))
			{
				Debug.Log("DOWN channel");
			}
		}
		
		if (null != up_channel)
		{
			Vector3 screen_point= Camera.main.WorldToScreenPoint(up_channel.position);
			
			if (GUI.Button(new Rect(screen_point.x, screen_point.y, 20, 20), "+"))
			{
				Debug.Log("UP channel");
			}
		}
		
		return;
	}
}
