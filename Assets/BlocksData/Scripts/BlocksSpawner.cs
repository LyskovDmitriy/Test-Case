using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocksSpawner : MonoBehaviour 
{

	public CameraVariables cameraData;
	public GameObject blockParentPrefab;
	public GameObject blockPrefab;
	public float additionalHorizontalOffset; //just to be sure that blocks don't spawn where camera can see them
	public float blockVerticalSize; 
	public float blockHorizontalSize; 


	private Queue<GameObject> activeBlockParents; //block parents that are currently active and moving
	private Stack<GameObject> inactiveBlockParents; //parents that are to be reused
	private Transform furthestBlockParent; //thr furthest from the place to spawn blocks
	private Transform closestBlockParent;
	private float horizontalRightActiveFieldBorder; //here blocks can be spawned without being noticed by the camera


	void Start () 
	{
		activeBlockParents = new Queue<GameObject>();
		inactiveBlockParents = new Stack<GameObject>();
		horizontalRightActiveFieldBorder = cameraData.CameraViewHorizontalBorder + additionalHorizontalOffset;
		SpawnStartingPlatform();
	}


	void SpawnStartingPlatform()
	{
		float posX = -cameraData.CameraViewHorizontalBorder;
		Vector3 positionForFirstLine = new Vector3(posX, 0.0f, 0.0f);

		GameObject newBlockParent = GetLine(positionForFirstLine);
		activeBlockParents.Enqueue(newBlockParent);
		closestBlockParent = newBlockParent.transform;
		furthestBlockParent = closestBlockParent;

		while (closestBlockParent.position.x <= horizontalRightActiveFieldBorder - blockHorizontalSize)
		{
			CheckPlatformEnd(); //this method works well in this situation and helps spawn the entire platform
		}
	}


	void Update()
	{
		CheckPlatformStart();
		CheckPlatformEnd();
	}

	//creates or reuses inactive line
	GameObject GetLine(Vector3 position)
	{
		GameObject blockParent = null;

		if (inactiveBlockParents.Count == 0)
		{
			blockParent = Instantiate(blockParentPrefab, position, Quaternion.identity);
			blockParent.transform.SetParent(transform);
			float posZ = -cameraData.CameraViewVerticalBorder;
			while (posZ < cameraData.CameraViewVerticalBorder)
			{
				GameObject block = Instantiate(blockPrefab, blockParent.transform);
				block.transform.localPosition = new Vector3(0.0f, 0.0f, posZ);
				posZ += blockVerticalSize;
			}
		}
		else
		{
			blockParent = inactiveBlockParents.Pop();
			blockParent.transform.position = position;
			blockParent.SetActive(true);
		}

		return blockParent;
	}

	//first finds potition and then calls GetLine with params to actually get a line
	GameObject GetLine()
	{
		Vector3 blockParentPosition;

		if (closestBlockParent == null)
		{
			blockParentPosition = new Vector3(horizontalRightActiveFieldBorder, 0.0f, 0.0f);
		}
		else
		{
			blockParentPosition = closestBlockParent.position + new Vector3(blockHorizontalSize, 0.0f, 0.0f);
		}

		GameObject line = GetLine(blockParentPosition);
		return line;
	}

	//checks if the furthest block parent left active field. In that case it sets it inactive and pushes in stack to be reused
	void CheckPlatformStart()
	{
		if (furthestBlockParent != null && furthestBlockParent.position.x < -horizontalRightActiveFieldBorder)
		{
			GameObject furthestLine = activeBlockParents.Dequeue();
			furthestLine.SetActive(false);
			inactiveBlockParents.Push(furthestLine);
			furthestBlockParent = activeBlockParents.Peek().transform;
		}
	}

	//checks if a new line should be spawned and does that if necessary
	void CheckPlatformEnd()
	{
		if (closestBlockParent == null || closestBlockParent.position.x <= horizontalRightActiveFieldBorder - blockHorizontalSize)
		{
			GameObject newBlockParent = GetLine();
			activeBlockParents.Enqueue(newBlockParent);
			closestBlockParent = newBlockParent.transform;
			if (furthestBlockParent == null)
			{
				furthestBlockParent = closestBlockParent;
			}
		}
	}
}
