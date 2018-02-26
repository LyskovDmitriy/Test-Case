using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;

public class FacebookManager : MonoBehaviour 
{
	
	public event System.Action onLogged;
	public event System.Action onLoggedOut;


	public string userName;
	public Sprite userSprite;
	public Sprite friendPicture;
	public List<object> friends;


	private string userId;


	public void LogIn()
	{
		if (!FB.IsLoggedIn)
		{
			List<string> permissions = new List<string>(){ "public_profile", "user_friends" };
			FB.LogInWithReadPermissions(permissions, AuthCallback);
		}
	}


	public void LogOut()
	{
		if (FB.IsLoggedIn)
		{
			FB.LogOut();
			userId = null;
			userName = null;
			userSprite = null;
			friends = null;
			friendPicture = null;
			if (onLoggedOut != null)
			{
				onLoggedOut();
			}
		}
	}


	public void GetUserName()
	{
		FB.API("/" + userId + "?fields=name", HttpMethod.GET, AddUserName);
	}


	public void GetUserImage()
	{
		FB.API("/" + userId + "/picture?type=square&height=128&width=128", HttpMethod.GET, AddUserPicture);
	}


	public void GetFriendImage(string id)
	{
		FB.API("/" + id + "/picture?type=square&height=128&width=128", HttpMethod.GET, AddFriendPicture);
	}


	public void GetFriendsList()
	{
		FB.API("/" + userId + "/friends", HttpMethod.GET, result =>
			{
				var dictionary = (Dictionary<string, object>) Facebook.MiniJSON.Json.Deserialize(result.RawResult);
				friends = (List<object>)dictionary["data"];
			});
	}


	public void ClearFriendPicture()
	{
		friendPicture = null;
	}


	void Awake()
	{
		FB.Init(OnInit, OnHideUnity);
	}


	void OnInit()
	{
		if (FB.IsInitialized)
		{
			Debug.Log("Initialized");
			FB.ActivateApp();
			if (FB.IsLoggedIn)
			{
				LogOut();
			}
		}
		else
		{
			Debug.Log("Initialization error");
		}
	}


	void OnHideUnity(bool isGameShown)
	{
		if (isGameShown)
		{
			Time.timeScale = 1.0f;
		}
		else
		{
			Time.timeScale = 0.0f;
		}
	}


	void AuthCallback(IResult result)
	{
		if (result.Error == null)
		{
			if (FB.IsLoggedIn)
			{
				Debug.Log("Successfully logged");
				userId = result.ResultDictionary["user_id"].ToString();	
				if (onLogged != null)
				{
					onLogged();
				}
			}
			else
			{
				Debug.Log("Can't log in");
			}
		}
		else
		{
			Debug.Log("Authorization mistake " + result.Error.ToString());
		}
	}


	void AddUserName(IResult result)
	{
		if (result.Error == null)
		{
			userName = result.ResultDictionary["name"].ToString();
		}
		else
		{
			Debug.Log(result.Error.ToString());
		}
	}


	void AddUserPicture(IGraphResult result)
	{
		if (result.Error == null && result.Texture != null)
		{
			userSprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), Vector2.zero);
		}
	}


	void AddFriendPicture(IGraphResult result)
	{
		if (result.Error == null && result.Texture != null)
		{
			friendPicture = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), Vector2.zero);
		}
	}
}
