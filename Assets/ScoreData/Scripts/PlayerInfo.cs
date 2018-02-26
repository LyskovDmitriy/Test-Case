using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{	

	public Text nameText;
	public Text scoreValueText;
	public Image playerImage;


	private PersonalData playerData;


	public void SetData(PersonalData data)
	{
		nameText.text = data.name;
		playerImage.sprite = data.image;
		scoreValueText.text = data.score.ToString();
	}
}
