using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraVariables : MonoBehaviour
{

	public float CameraViewHorizontalBorder { get; private set; }
	public float CameraViewVerticalBorder { get; private set; }


	void Awake () 
	{
		Camera mainCamera = GetComponent<Camera>();

		float verticalDegrees = mainCamera.fieldOfView; 
		float horizontalDegrees = verticalDegrees * mainCamera.aspect;
		CameraViewHorizontalBorder = Mathf.Tan(horizontalDegrees / 2 * Mathf.Deg2Rad) * transform.position.y;
		CameraViewVerticalBorder = Mathf.Tan(verticalDegrees / 2 * Mathf.Deg2Rad) * transform.position.y;
	}
}
