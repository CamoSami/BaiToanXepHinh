using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	//		Instance
	public static GameManager Instance { get; private set; }

	//		Unity Assigned Data
	[SerializeField] private List<GameObject> pictureObjectsList;
	[SerializeField] private List<Location> locationsList;
	[SerializeField] private Transform pictureObjectSpawnsHere;

	//		Calculating Data
	private Location currentEmptyLocation;
	private string currentPictureObjectsIndexes;



	private void Awake()
	{
		Instance = this;

		this.ResetGame();

		//Debug.Log(this.currentPictureObjectsIndexes);
	}

	private void Start()
	{

	}

	private void Update()
	{

	}

	public void ClickLocation(Location location)
	{
		if (!this.currentEmptyLocation.IsAdjacent(location))
		{
			return;
		}

		//		Current String
		int emptyLocationIndex = this.currentPictureObjectsIndexes.IndexOf('8'),
			swapLocationIndex = this.currentPictureObjectsIndexes.IndexOf(char.Parse(location.GetPictureIndex().ToString())),
			smallerIndex = emptyLocationIndex > swapLocationIndex ? swapLocationIndex : emptyLocationIndex,
			biggerIndex = emptyLocationIndex > swapLocationIndex ? emptyLocationIndex : swapLocationIndex;

		//Debug.Log("smallerIndex: " + smallerIndex + ", biggerIndex: " + biggerIndex);

		this.currentPictureObjectsIndexes =
			this.currentPictureObjectsIndexes.Substring(0, smallerIndex) +
			this.currentPictureObjectsIndexes.Substring(biggerIndex, 1) +
			this.currentPictureObjectsIndexes.Substring(smallerIndex + 1, biggerIndex - smallerIndex - 1) +
			this.currentPictureObjectsIndexes.Substring(smallerIndex, 1) +
			this.currentPictureObjectsIndexes.Substring(biggerIndex + 1, this.currentPictureObjectsIndexes.Length - biggerIndex - 1);

		//		Picture Object
		location.GetPictureObject().GetComponent<PictureObject>().SetLocation(this.currentEmptyLocation);

		//		Location
		this.currentEmptyLocation.SetPictureObject(location.GetPictureObject());
		location.SetPictureObject(null);

		//		Index
		this.currentEmptyLocation.SetPictureIndex(location.GetPictureIndex());
		location.SetPictureIndex(-1);

		//		Swap!
		this.currentEmptyLocation = location;

		//		Check!
		if (this.CheckIfWin())
		{
			Debug.Log("You won congrats");
		}

		//Debug.Log(this.currentPictureObjectsIndexes);
	}

	public void ResetGame()
	{
		if (locationsList[0].GetPictureObject() || locationsList[1].GetPictureObject())
		{
			foreach (Location location in locationsList)
			{
				GameObject.Destroy(location.GetPictureObject());
			}
		}

		string pictureObjectsIndexes = this.SearchSolvablePuzzle();

		for (int i = 0; i < pictureObjectsIndexes.Length; i++)
		{
			int objectIndex = pictureObjectsIndexes[i] - 48;

			if (objectIndex >= 8)
			{
				this.locationsList[i].SetPictureIndex(objectIndex);

				this.currentPictureObjectsIndexes += objectIndex + "";
			}
			else
			{
				GameObject pictureObject = GameObject.Instantiate(
						this.pictureObjectsList[objectIndex],
						this.locationsList[i].transform.position,
						Quaternion.identity,
						this.pictureObjectSpawnsHere.transform
					);

				this.locationsList[i].SetPictureObject(pictureObject);
				this.locationsList[i].SetPictureIndex(objectIndex);

				this.currentPictureObjectsIndexes += objectIndex + "";

				pictureObject.GetComponent<PictureObject>().SetLocation(this.locationsList[i]);
			}
		}
	}

	private string SearchSolvablePuzzle()
	{
		string pictureObjectsIndexes = "";
		List<bool> usedPictureObject = new List<bool> { false, false, false, false, false, false, false, false, false };
		bool hasEmptyLocation = false;

		for (int i = 0; i < this.locationsList.Count; i++)
		{
			bool isEmpty = false;

			//		If it already have an Empty Object
			if (!hasEmptyLocation)
			{
				isEmpty = UnityEngine.Random.Range(i, this.locationsList.Count + 1) == this.locationsList.Count;
			}

			if (isEmpty)
			{
				//		An Empty Tile!
				hasEmptyLocation = true;

				this.currentEmptyLocation = this.locationsList[i];

				pictureObjectsIndexes += "8";
			}
			else
			{
				//		Not An Empty Tile!

				//		Get Object Index
				int objectIndex = UnityEngine.Random.Range(0, 8);

				while (usedPictureObject[objectIndex])
				{
					objectIndex = UnityEngine.Random.Range(0, 8);
				}

				usedPictureObject[objectIndex] = true;
				pictureObjectsIndexes += objectIndex.ToString() + "";
			}
		}

		if (!IsSolvable(pictureObjectsIndexes))
		{
			Debug.Log("RESET FAILED!");

			return "888888888";
		}

		return pictureObjectsIndexes;
	}

	private bool IsSolvable(string puzzle)
	{
		int inversions = 0;
		int size = puzzle.Length;

		for (int i = 0; i < size; i++)
		{
			for (int j = i + 1; j < size; j++)
			{
				if (puzzle[i] > puzzle[j] && puzzle[i] != '8' && puzzle[j] != '8')
				{
					inversions++;
				}
			}
		}

		// Check if the grid width is even or odd
		int width = (int)Math.Sqrt(size);
		int emptyTileRow = puzzle.IndexOf('8') / width + 1;

		if (width % 2 == 1) // Odd width
		{
			return inversions % 2 == 0;
		}
		else // Even width
		{
			return (inversions + emptyTileRow) % 2 == 0;
		}
	}

	public bool CheckIfWin(string preList = null)
	{
		bool isWon = true;
		//string longAssDebugString = "list: ";

		if (preList == null)
		{
			//		Standard
			for (int i = 0; i < this.pictureObjectsList.Count; i++)
			{
				//longAssDebugString += this.locationsList[i].GetPictureIndex() + ", ";

				if (this.locationsList[i].GetPictureIndex() != i)
				{
					isWon = false;

					break;
				}
			}
		}
		else
		{
			//		Custom
			//			Hard coded, I know
			if (preList.CompareTo("012345678") != 0)
			{
				isWon = false;
			}
		}

		//Debug.Log(longAssDebugString + " isWon: " + isWon);

		//if (isWon && preList != null)
		//{
		//	Debug.Log(preList + ", isWon = true");
		//}

		return isWon;
	}



	//		Getter & Setter
	public List<Location> GetLocationsList()
	{
		return this.locationsList;
	}
}
