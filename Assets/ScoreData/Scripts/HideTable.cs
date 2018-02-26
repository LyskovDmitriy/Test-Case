using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideTable : MonoBehaviour 
{

	public Button button;
	public Animator targetAnimator;
	public Text buttonText;


	private bool isHiding = false;


	public void Hide()
	{
		if (isHiding)
		{
			isHiding = false;
			buttonText.text = "Hide Table"; 
			targetAnimator.SetTrigger("Appear");
		}
		else
		{
			isHiding = true;
			buttonText.text = "Show Table"; 
			targetAnimator.SetTrigger("Hide");
		}
		StartCoroutine(SetNotInteractable(1.0f));
	}


	IEnumerator SetNotInteractable(float time)
	{
		button.interactable = false;
		yield return new WaitForSeconds(time);
		button.interactable = true;
	}
}
