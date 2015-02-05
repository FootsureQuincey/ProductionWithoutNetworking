//Created A class that houses all of the networking Commands
//Wyatt

using UnityEngine;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class NetworkCommands
{
	private static bool Authenticate()
	{
		bool rc = false;
		Social.localUser.Authenticate (success => {	
			if (success)
			{
				Debug.Log ("Authentication successful");
				string userInfo = "Username: " + Social.localUser.userName + 
					"\nUser ID: " + Social.localUser.id + 
						"\nIsUnderage: " + Social.localUser.underage;
				Debug.Log (userInfo);
				rc = true;
			}
			else
				Debug.Log ("Authentication failed");
		});
		return rc;
	}
    public static bool JoinLobby(string s)
    {
		bool rc = false;
		if (Authenticate())
		{
        	PhotonNetwork.JoinRoom(s);
			rc = true;
		}
		return rc;
    }

	public static bool JoinRandom()
	{
		bool rc = false;
		if (Authenticate())
		{
			PhotonNetwork.JoinRandomRoom();
			rc = true;
		}
		return rc;
	}

    public static bool Create(string ToBeCreated, Vector3 PositionCreated)
    {
        if (NetworkInstantiate(ToBeCreated, PositionCreated, Quaternion.identity, 0))
        {
            return true;
        }
        return false;
    }

    private static bool NetworkInstantiate(string ToBeCreated, Vector3 PositionCreated, Quaternion Rotation, int group)
    {
        if (ToBeCreated == null)
        {
            return false;
        }
        if (Rotation.Equals(null))
        {
            Rotation = Quaternion.identity;
        }
        if (group < 0)
        {
            group = 0;
        }
        if (PhotonNetwork.Instantiate(ToBeCreated, PositionCreated, Rotation, group))
        {
            return true;
        }
        return false;
    }

    public static bool isMasterClient()
    {
        return PhotonNetwork.isMasterClient;
    }
}