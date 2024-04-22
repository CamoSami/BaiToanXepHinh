using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureObject : MonoBehaviour
{
	private Location currentLocation;

	private void Awake()
	{
		
	}

	public Location GetLocation() 
	{ 
		return currentLocation; 
	}

	public void SetLocation(Location location)
	{
		currentLocation = location;
	}
}
