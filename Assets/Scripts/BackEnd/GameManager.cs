//Created December 10, 2014
//Updated December 16, 2014
//Copywrite Wyatt 2014
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    //publics
	public int sPlayersInRoom;
	public int sPlayersTurn;
	public int sTargetsAlive;
	public int sInstaniated = 0;
	public Player[] sPlayers;
	public BaseTarget[] sTargets;

	private bool newPlayerAdded = false;
	//Call this to restart the lobby
	public void Init()
	{
		if(sInstaniated <= 1)
		{
			Debug.Log("Instantiated");
			sPlayers.Initialize();
			sTargets.Initialize();
			sPlayersInRoom = 0;
			sPlayersTurn = 0;
			sTargetsAlive = 0;
			sInstaniated = 1;
		}
	}

	//Adds Players to the game
	public bool AddPlayer(Player p)
	{
		bool rc = true;
		if(sPlayers.Length == 0)
		{
			sPlayers.SetValue(p, 0);
			sPlayersInRoom++;
		}
		else
		{
			foreach(Player j in sPlayers)
			{
				if(Equals(p,j))
				{
					Debug.Log("player already exists");
					rc = false;
				}
			}
			if(rc)
			{
				sPlayers.SetValue (p, sPlayersInRoom);
				sPlayersInRoom++;
			}
		}
		Debug.Log(sPlayersInRoom);
		newPlayerAdded = rc;
		return rc;
	}

	//Adds targets into the game
	public bool AddTarget(BaseTarget t)
	{
		sTargets.SetValue(t, sTargetsAlive);
		sTargetsAlive++;
		return true;
	}
	   
	public Player CurrentPlayer()
	{
		return (Player)sPlayers [sPlayersTurn];
	}

    // Call this to Have the game logic function
	public void GameLoop()
    {
		if(newPlayerAdded)
		{
			CheckPlayers();
		}
		if(sPlayersTurn < sPlayersInRoom)
		{
			PlayerTurn((Player)sPlayers[sPlayersTurn]);
			//Debug.Log(sPlayersTurn);
		}
		else if (sPlayersTurn >= sPlayersInRoom)
		{
			AITurn();
			sPlayersTurn++;
			sPlayersTurn = sPlayersTurn % (sPlayersTurn);
			//Debug.Log(sPlayersTurn);
		}
    }

	//this is what the player can do on their turn
	private void PlayerTurn(Player p)
    {
		if(p)
		{
			if(!p.mMoved)
			{
				if(PhotonNetwork.offlineMode)
				{
					p.UpdatePlayer();
				}
				else
				{
					if(p.networkView.isMine)
					{
						p.UpdatePlayer();
						//Debug.Log(sPlayersTurn);
					}
				}
			}
			else
			{
				p.mAttacked = false;
				p.mMoved = false;
				//p.mHand.PlayedCard = false;
				sPlayersTurn++;
				Debug.Log(sPlayersTurn);
			}
		}
    }

	//Do AI stuff in this function
	private void AITurn()
	{
		foreach(BaseTarget t in sTargets)
		{
			if(t)
			{
				if(t.UpdateTarget())
				{
					t.mTargetTurn = false;
				}
			}
		}
	}
	void CheckPlayers()
	{
		Player[] tempdeck = new Player[5];
		int cintd = 0;
		for (int i = 0; i < sPlayers.Length; i++) 
		{
			if (sPlayers[i] != null)
			{
				tempdeck[cintd] = sPlayers[i];
				cintd++;
			}
		}
		for (int i = 0; i < sPlayers.Length; i++) 
		{
			sPlayers[i] = tempdeck[i];
		}
		newPlayerAdded = false;
	}
	[RPC]
	public byte[] SerializeGameManager(object customObject)
	{
		GameManager gm = (GameManager)customObject;
		int index = 0;
		byte[] playerBytes = ExitGames.Client.Photon.Protocol.Serialize (gm.sPlayers);
		byte[] targetBytes = ExitGames.Client.Photon.Protocol.Serialize (gm.sTargets);
		byte[] bytes = new byte[playerBytes.Length + targetBytes.Length + 16];
		ExitGames.Client.Photon.Protocol.Serialize (playerBytes.Length, bytes, ref index);
		System.Array.Copy (playerBytes, 0, bytes, index, playerBytes.Length);
		index += playerBytes.Length;
		ExitGames.Client.Photon.Protocol.Serialize (targetBytes.Length, bytes, ref index);
		System.Array.Copy (targetBytes, 0, bytes, index, targetBytes.Length);
		index += targetBytes.Length;
		ExitGames.Client.Photon.Protocol.Serialize (gm.sPlayersInRoom, bytes, ref index);
		ExitGames.Client.Photon.Protocol.Serialize (gm.sPlayersTurn, bytes, ref index);
		ExitGames.Client.Photon.Protocol.Serialize (gm.sTargetsAlive, bytes, ref index);
		ExitGames.Client.Photon.Protocol.Serialize (gm.sInstaniated, bytes, ref index);
		return bytes;
	}

	[RPC]
	public object DeserializeMethod(byte[] bytes)
	{
		GameManager gm = new GameManager();
		int index = 0;
		int playerBytesLength;
		int targetBytesLength;
		ExitGames.Client.Photon.Protocol.Deserialize (out playerBytesLength, bytes, ref index);
		byte[] playerBytes = new byte[playerBytesLength];
		System.Array.Copy(bytes, index, playerBytes, 0, playerBytesLength);
		gm.sPlayers = ((Player[])ExitGames.Client.Photon.Protocol.Deserialize (playerBytes));
		index += playerBytesLength;
		ExitGames.Client.Photon.Protocol.Deserialize (out targetBytesLength, bytes, ref index);
		byte[] targetBytes = new byte[targetBytesLength];
		System.Array.Copy(bytes, index, targetBytes, 0, targetBytesLength);
		gm.sPlayers = ((Player[])ExitGames.Client.Photon.Protocol.Deserialize(targetBytes));
		index += targetBytesLength;
		ExitGames.Client.Photon.Protocol.Deserialize (out gm.sPlayersInRoom, bytes, ref index);
		ExitGames.Client.Photon.Protocol.Deserialize (out gm.sPlayersTurn, bytes, ref index);
		ExitGames.Client.Photon.Protocol.Deserialize (out gm.sTargetsAlive, bytes, ref index);
		ExitGames.Client.Photon.Protocol.Deserialize (out gm.sInstaniated, bytes, ref index);
		return gm;
	}

	[RPC]
	public void SetPlayersInRoom(int iPlayersInRoom)
	{
		if(PhotonNetwork.isMasterClient)
		{
			networkView.RPC("SetPlayersInRoom", RPCMode.Others, iPlayersInRoom);
		}
		sPlayersInRoom = iPlayersInRoom;
	}

	[RPC]
	public void SetPlayersTurn(int iPlayersTurn)
	{
		if(PhotonNetwork.isMasterClient)
		{
			networkView.RPC("SetPlayersTurn", RPCMode.Others, iPlayersTurn);
		}
		sPlayersTurn = iPlayersTurn;
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		int Instantiated = 0;
		int PlayersInRoom = 0;
		int PlayersTurn = 0;
		int TargetsAlive = 0;
		if (stream.isWriting)
		{
			//We own this player: send the others our data
			Instantiated = sInstaniated;
			PlayersInRoom = sPlayersInRoom;
			PlayersTurn = sPlayersTurn;
			TargetsAlive = sTargetsAlive;

			stream.Serialize(ref Instantiated);
			stream.Serialize(ref PlayersInRoom);
			stream.Serialize(ref PlayersTurn);
			stream.Serialize(ref TargetsAlive);
		}
		else
		{
			//Network player, receive data
			stream.Serialize(ref Instantiated);
			stream.Serialize(ref PlayersInRoom);
			stream.Serialize(ref PlayersTurn);
			stream.Serialize(ref TargetsAlive);

			sInstaniated = Instantiated;
			sPlayersInRoom = PlayersInRoom;
			sPlayersTurn = PlayersTurn;
			sTargetsAlive = TargetsAlive;
		}
	}

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			//We own this player: send the others our data			
			stream.SendNext( sInstaniated );
			stream.SendNext( sPlayersInRoom );
			stream.SendNext( sPlayersTurn );
			stream.SendNext( sTargetsAlive );
		}
		else
		{
			//Network player, receive data
			sInstaniated = (int)stream.ReceiveNext();
			sPlayersInRoom = (int)stream.ReceiveNext();
			sPlayersTurn = (int)stream.ReceiveNext();
			sTargetsAlive = (int)stream.ReceiveNext();
		}
	}
	void Awake()
	{
		if(GameObject.Find("GameClient"))
		{
			PhotonNetwork.offlineMode = false;
		}
		else
		{
			PhotonNetwork.offlineMode = true;
		}
	}

	void Update()
	{
		GameLoop ();
	}
}
