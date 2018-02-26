using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScoreManager : MonoBehaviour
{

	public FacebookManager fbManager;
	public GameObject playerInfoPrefab;
	public Transform scoreObjectsParent;
	public GameObject loadingText;
	public int maxScore;
	public int minScore;


	private PersonalData playerData;
	private List<object> friendsList;
	private float timeToWait = 0.25f;


	void Awake () 
	{
		fbManager.onLogged += OnPlayerLogged;
		fbManager.onLoggedOut += OnPlayerLoggedOut;
		playerData = null;
		friendsList = null;
	}


	void OnPlayerLogged()
	{
		loadingText.SetActive(true);
		StartCoroutine(GetPlayersInfo());
	}


	void OnPlayerLoggedOut()
	{
		StopAllCoroutines();
		playerData = null;
		friendsList = null;
		loadingText.SetActive(false);
		for (int i = scoreObjectsParent.childCount - 1; i >= 0; i--)
		{
			Destroy(scoreObjectsParent.GetChild(i).gameObject);
		}
	}


	IEnumerator GetPlayersInfo()
	{
		//yield return new WaitForSeconds(5); //try to solve the bug with getting another profile's data
		playerData = new PersonalData();
		yield return StartCoroutine(TryGetUserName());
		yield return StartCoroutine(TryGetUserImage());
		yield return StartCoroutine(TryGetUserFriends());
		playerData.score = GetRandomScore();
		List<PersonalData> players = new List<PersonalData>();

		players.Add(playerData);
		yield return StartCoroutine(AddPlayersData(players));

		players.Sort((PersonalData x, PersonalData y) =>
			{
				return y.score.CompareTo(x.score);
			});

		for (int i = 0; i < players.Count; i++)
		{
			PlayerInfo info = Instantiate(playerInfoPrefab, scoreObjectsParent).GetComponent<PlayerInfo>();
			info.SetData(players[i]);
		}

		loadingText.SetActive(false);
	}


	IEnumerator TryGetUserName()
	{
		string playerName = null;
		fbManager.GetUserName();
		while (string.IsNullOrEmpty(playerName))
		{
			yield return new WaitForSeconds(timeToWait);
			playerName = fbManager.userName;
			Debug.Log("Try get name");
		}
		playerData.name = playerName;
	}


	IEnumerator TryGetUserImage()
	{
		Sprite playerImage = null;
		fbManager.GetUserImage();
		while (playerImage == null)
		{
			playerImage = fbManager.userSprite;
			Debug.Log("Try get image");
			yield return new WaitForSeconds(timeToWait);
		}
		playerData.image = playerImage;
	}


	IEnumerator TryGetUserFriends()
	{
		friendsList = null;
		fbManager.GetFriendsList();
		while (friendsList == null)
		{
			friendsList = fbManager.friends;
			Debug.Log("Try get friends");
			yield return new WaitForSeconds(timeToWait);
		}
	}


	int GetRandomScore()
	{
		return Random.Range(minScore, maxScore + 1);
	}


	IEnumerator AddPlayersData(List<PersonalData> players)
	{
		for (int i = 0; i < friendsList.Count; i++)
		{
			PersonalData data = new PersonalData();
			var friendInfo = ((Dictionary<string, object>)friendsList[i]);

			data.name = friendInfo["name"].ToString();
			string friendId = friendInfo["id"].ToString();
			fbManager.ClearFriendPicture();
			fbManager.GetFriendImage(friendId);
			Sprite friendImage = null;
			while (friendImage == null)
			{
				friendImage = fbManager.friendPicture;
				yield return new WaitForSeconds(timeToWait);
			}
			data.image = friendImage;
			data.score = GetRandomScore();
			players.Add(data);
		}
	}


	void OnDestroy () 
	{
		fbManager.onLogged -= OnPlayerLogged;
		fbManager.onLoggedOut -= OnPlayerLoggedOut;
	}
}
