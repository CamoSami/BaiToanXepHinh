using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using Utils;
using static UnityEditor.FilePathAttribute;

public class AIManager : MonoBehaviour
{
	//		Instance
	public static AIManager Instance { get; private set; }

	//		Class
	public class AIStep
	{
		public string lastSteps;
		public int locationSwap;
		public string pictureIndexesBeforeSwap;

		public int evaluatedPriority;

		public int EvaluatePriority()
		{
			int temp = 0;

			for (int i = 0; i < 9; i++)
			{
				if (pictureIndexesBeforeSwap.IndexOf(char.Parse(i.ToString())) == i)
				{
					temp++;
				}
			}

			return temp;
		}

		public static bool CheckIfPictureIndexesSame(string pictureIndexes)
		{
			return AIManager.Instance.GetListOfVisitedPictureIndexes().Contains(pictureIndexes);
		}
	}

	//		Enum
	public enum AlgorithmType
	{
		AStar,
		None
	}

	public enum State
	{
		Idle,
		Auto
	}
	private State currentState;

	//		Hard Coded Lul
	private List<List<int>> listOfAdjacentTiles = new List<List<int>>
	{
		new List<int>{1, 3},
		new List<int>{0, 2, 4},
		new List<int>{1, 5},
		new List<int>{0, 4, 6},
		new List<int>{1, 3, 5, 7},
		new List<int>{2, 4, 8},
		new List<int>{3, 7},
		new List<int>{4, 6, 8},
		new List<int>{5, 7},
	};

	//		Unity Assigned Data
	[SerializeField] private float maxTime = 1f;

	//		Calculating Data
	private List<AIStep> listOfCorrectSteps;
	private List<string> listOfVisitedPictureIndexes;
	private int index = 0;
	private float currentTime = -3f;



	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		//this.listOfCorrectSteps = this.AlgorithmAStar(
		//	GameManager.Instance.GetLocationsList()
		//	);

		//this.index = 0;
		//this.currentTime = -1f;
		//this.currentState = State.Auto;

		//string temp = "012354678", evaluateString = "";
		//int tempDem = 0;

		//for (int i = 0; i < 9; i++)
		//{
		//	if (temp.IndexOf(char.Parse(i.ToString())) == i)
		//	{
		//		tempDem++;
		//	}

		//	evaluateString += char.Parse(i.ToString()) + "";
		//}

		//Debug.Log("temp: " + temp + ", evaluateString: " + evaluateString + ", tempDem: " + tempDem);
	}

	private void Update()
	{
		switch (this.currentState)
		{
			case State.Idle:


				break;
			case State.Auto:
				this.currentTime += Time.deltaTime;

				if (this.currentTime > maxTime)
				{
					this.currentTime = 0f;

					if (this.listOfCorrectSteps == null)
					{
						Debug.Log("IS NULL!");

						this.currentState = State.Idle;

						return;
					}

					//PositionManager.Instance.Initiate(this.listOfCorrectSteps[this.index]);
					GameManager.Instance.ClickLocation(
						GameManager.Instance.GetLocationsList()[this.listOfCorrectSteps[this.index].locationSwap]
						);

					//Debug.Log(this.listOfCorrectSteps[this.index].locationSwap);

					this.index++;

					if (this.index == this.listOfCorrectSteps.Count)
					{
						this.currentState = State.Idle;
					}
				}

				break;
		}
	}



	public void InitiateAlgorithm(AlgorithmType algorithmType)
	{
		switch (algorithmType)
		{
			case AlgorithmType.AStar:
				this.listOfCorrectSteps = this.AlgorithmAStar(GameManager.Instance.GetLocationsList());

				break;
			case AlgorithmType.None:

				this.index = 0;
				this.currentTime = 0f;
				this.currentState = State.Idle;

				break;
		}
	}

	public List<AIStep> AlgorithmAStar(List<Location> initialLocation)
	{
		//		TODO: Test with one set of List<Location> first!
		Debug.Log("Algorithm A Star Ran!");

		if (GameManager.Instance.CheckIfWin())
		{
			Debug.Log("Ur already win...");

			return null;
		}

		//		Initiate
		List<AIStep> priorListAlgorithmSteps = new List<AIStep>();
		string initialPictureIndexes = string.Empty;
		int currentEmptyTile = 0;
		this.listOfVisitedPictureIndexes = new List<string>();

		//		Initial Picture Indexes
		for (int i = 0; i < initialLocation.Count; i++)
		{
			initialPictureIndexes += initialLocation[i].GetPictureIndex() + "";

			if (initialPictureIndexes[i] == char.Parse(8.ToString()))
			{
				currentEmptyTile = i;
			}
		}

		//		Initial Possible Steps
		for (int i = 0; i < this.listOfAdjacentTiles[currentEmptyTile].Count; i++)
		{
			AIStep initialStep = new AIStep()
			{
				lastSteps = null,
				locationSwap = this.listOfAdjacentTiles[currentEmptyTile][i],
				pictureIndexesBeforeSwap = initialPictureIndexes
			};

			initialStep.evaluatedPriority = initialStep.EvaluatePriority();

			priorListAlgorithmSteps.Add(initialStep);
		}

		//		Pre Sort the List
		for (int i = 0; i < priorListAlgorithmSteps.Count; i++)
		{
			for (int j = i + 1; j < priorListAlgorithmSteps.Count; j++)
			{
				if (priorListAlgorithmSteps[j].evaluatedPriority > priorListAlgorithmSteps[i].evaluatedPriority)
				{
					(priorListAlgorithmSteps[i], priorListAlgorithmSteps[j]) = (priorListAlgorithmSteps[j], priorListAlgorithmSteps[i]);
				}
			}
		}

		//		Algorithm
		while (priorListAlgorithmSteps.Count > 0)
		{
			AIStep step = priorListAlgorithmSteps[0];
			priorListAlgorithmSteps.RemoveAt(0);

			//		Getting the PictureIndesesAfterSwap
			string pictureIndexesAfterSwap = string.Copy(step.pictureIndexesBeforeSwap);

			//		Current String
			int emptyLocationIndex = pictureIndexesAfterSwap.IndexOf('8'),
				swapLocationIndex = step.locationSwap,
				smallerIndex = emptyLocationIndex > swapLocationIndex ? swapLocationIndex : emptyLocationIndex,
				biggerIndex = emptyLocationIndex > swapLocationIndex ? emptyLocationIndex : swapLocationIndex;

			//Debug.Log("smallerIndex: " + smallerIndex + ", biggerIndex: " + biggerIndex);

			pictureIndexesAfterSwap =
				pictureIndexesAfterSwap.Substring(0, smallerIndex) +
				pictureIndexesAfterSwap.Substring(biggerIndex, 1) +
				pictureIndexesAfterSwap.Substring(smallerIndex + 1, biggerIndex - smallerIndex - 1) +
				pictureIndexesAfterSwap.Substring(smallerIndex, 1) +
				pictureIndexesAfterSwap.Substring(biggerIndex + 1, pictureIndexesAfterSwap.Length - biggerIndex - 1);

			currentEmptyTile = step.locationSwap;

			//Debug.Log(pictureIndexesAfterSwap.ToSafeString());

			//		Check if It is New
			if (AIStep.CheckIfPictureIndexesSame(pictureIndexesAfterSwap))
			{
				//Debug.Log(pictureIndexesAfterSwap);

				continue;
			}

			//		Add to Visisted
			this.listOfVisitedPictureIndexes.Add(pictureIndexesAfterSwap);

			//		WHY IS IT SO LONG
			if (this.listOfVisitedPictureIndexes.Count >= 30000)
			{
				Debug.Log(step.pictureIndexesBeforeSwap + ", evaluate: " + step.EvaluatePriority());

				break;
			}

			//		Check if Won
			if (GameManager.Instance.CheckIfWin(pictureIndexesAfterSwap))
			{
				//		Algorithm Finished
				//Debug.Log("Algorithm Finished! ");

				List<AIStep> listOfCorrectSteps = new List<AIStep>();
				
				foreach (char tempChar in step.lastSteps)
				{
					listOfCorrectSteps.Add(new AIStep()
					{
						locationSwap = int.Parse(tempChar.ToString()),
					});
				}

				listOfCorrectSteps.Add(step);

				this.index = 0;
				this.currentTime = 0f;
				this.currentState = State.Auto;

				return listOfCorrectSteps;
			}

			//		Add more!
			for (int i = 0; i < this.listOfAdjacentTiles[currentEmptyTile].Count; i++)
			{
				AIStep nextStep = new AIStep()
				{
					lastSteps = step.lastSteps + step.locationSwap,
					locationSwap = this.listOfAdjacentTiles[currentEmptyTile][i],
					pictureIndexesBeforeSwap = pictureIndexesAfterSwap
				};

				nextStep.evaluatedPriority = nextStep.EvaluatePriority();

				for (int j = 0; j < priorListAlgorithmSteps.Count; j++)
				{
					if (priorListAlgorithmSteps[j].evaluatedPriority < nextStep.evaluatedPriority)
					{
						priorListAlgorithmSteps.Add(nextStep);

						int tempMax = priorListAlgorithmSteps.Count - 1;

						while (tempMax != j)
						{
							priorListAlgorithmSteps[tempMax] = priorListAlgorithmSteps[tempMax-- - 1];
						}

						priorListAlgorithmSteps[j] = nextStep;

						break;
					}
				}

				if (!priorListAlgorithmSteps.Contains(nextStep))
				{
					priorListAlgorithmSteps.Add(nextStep);
				}
			}
		}

		return null;
	}

	public List<string> GetListOfVisitedPictureIndexes()
	{
		return this.listOfVisitedPictureIndexes;
	}
}
