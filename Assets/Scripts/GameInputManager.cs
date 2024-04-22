using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameInputManager : MonoBehaviour
{
	//		Instance 
	public static GameInputManager Instance { get; private set; }

	//		Unity Assigned Data
	[SerializeField] private Button resetButton;
	[SerializeField] private Button aStarButton;

	//		Calculating Data


	private void Awake()
	{
		Instance = this; 
	}

	private void Start()
	{
		this.resetButton.onClick.AddListener(() =>
		{
			GameManager.Instance.ResetGame();
		});

		this.aStarButton.onClick.AddListener(() =>
		{
			AIManager.Instance.InitiateAlgorithm(AIManager.AlgorithmType.AStar);
		});
	}

	private void Update()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			if (this.CheckIfMouseClickObject(out Location location))
			{
				GameManager.Instance.ClickLocation(location);
			}

			//Debug.Log("test: " + test + ", location: " + location + ", object: " + location.GetPictureObject());
		}
	}

	public bool CheckIfMouseClickObject(out Location location)
	{
		if (this.CheckIfMouseClickColliderRaycast(out RaycastHit hitInfo))
		{
			//Debug.Log("_MapManager.Start()\n" +
			//	"hitInfo: " + hitInfo.collider.name);

			Transform transform = hitInfo.transform;

			location = transform.GetComponent<PictureObject>().GetLocation();
		
			return true;
		}
		else
		{
			location = null;

			return false;
		}
	}

	public bool CheckIfMouseClickColliderRaycast(out RaycastHit hitInfo)
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		Physics.Raycast(ray, out hitInfo, 100, 128);

		if (hitInfo.collider == null)
		{
			//		Can not scan a Collider, that's all

			return false;
		}
		else
		{
			//		Works normally

			return true;
		}
	}
}
