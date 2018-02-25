using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BlockMovement : MonoBehaviour 
{

	public float horizontalSpeed;

	//Everything can be set in Awake, because with current task architecture doesn't need to be flexible and velocity can be set only once
	//In bigger projects it would've been better but here was targeted only performance
	void Awake()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		Vector3 velocity = new Vector3(horizontalSpeed, 0.0f, 0.0f);
		rb.velocity = velocity;
	}
}
