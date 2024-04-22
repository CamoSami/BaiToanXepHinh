using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
	//		Unity Assigned Data
	[SerializeField] private float moveSpeed = 10f;
	[SerializeField] private List<Location> adjacentLocations;

	//		Calculating Data
	private GameObject pictureObject;
	private int pictureIndex;

	private void Awake()
	{
		
	}

	private void Start()
	{
		
	}

	private void Update()
	{
		if (this.pictureObject == null)
		{
			return;
		}

		if (this.pictureObject.transform.position != this.gameObject.transform.position)
		{
			this.pictureObject.transform.position = Vector3.MoveTowards(
				this.pictureObject.transform.position,
				this.gameObject.transform.position,
				this.moveSpeed * Time.deltaTime
				);
		}
	}

	public bool IsAdjacent( Location location )
	{
		return adjacentLocations.Contains( location );
	}

	//		Getters & Setters
	public GameObject GetPictureObject()
	{
		return this.pictureObject;
	}

	public void SetPictureObject(GameObject pictureObject )
	{
		this.pictureObject = pictureObject;
	}

	public int GetPictureIndex()
	{
		return this.pictureIndex;
	}

	public void SetPictureIndex(int index)
	{
		this.pictureIndex = index;
	}
}
