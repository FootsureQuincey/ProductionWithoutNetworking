 using UnityEngine;
using System.Collections;

public class GameClient : MonoBehaviour 
{
	public GameObject player1Prefab;
	public GameObject player2Prefab;
	public GameObject player3Prefab;
	public GameObject player4Prefab;
	public GameObject mGameManager;

	private GameManager mManager;
	private int playersInRoom = 0;
	private GameObject[] players;
	// Use this for initialization
	void Awake()
	{
		// in case we started this demo with the wrong scene being active, simply load the menu scene
		if (!PhotonNetwork.connected)
		{
			Application.LoadLevel(GameMenu.SceneNameMenu);
			return;
		}
		if(PhotonNetwork.isMasterClient)
		{
			mGameManager = PhotonNetwork.Instantiate(mGameManager.name, transform.position, Quaternion.identity, 0);
			mManager = mGameManager.GetComponent<GameManager>();
		}
	}
	void Start () 
	{
		// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
		if(PhotonNetwork.isMasterClient)
		{
			if (players == null)
			{
				players = GameObject.FindGameObjectsWithTag("Player");		
				foreach (Object player in players) 
				{
					playersInRoom++;
				}
			}
			if(playersInRoom == 0)
			{
				PhotonNetwork.Instantiate(player1Prefab.name, transform.position, Quaternion.identity, 0);
			}
			else if(playersInRoom == 1)
			{
				PhotonNetwork.Instantiate(player2Prefab.name, transform.position, Quaternion.identity, 0);
			}
			else if(playersInRoom == 2)
			{
				PhotonNetwork.Instantiate(player3Prefab.name, transform.position, Quaternion.identity, 0);
			}
			else if(playersInRoom == 3)
			{
				PhotonNetwork.Instantiate(player4Prefab.name, transform.position, Quaternion.identity, 0);
			}
		}
	}
	// Update is called once per frame
	void Update () 
	{
		if(!mManager)
		{
			if (players == null)
			{
				players = GameObject.FindGameObjectsWithTag("Player");		
				foreach (Object player in players) 
				{
					playersInRoom++;
				}
			}
			mGameManager = GameObject.Find("GameManager(Clone)");
			mManager = mGameManager.GetComponent<GameManager>();
			if(playersInRoom == 0)
			{
				PhotonNetwork.Instantiate(player1Prefab.name, transform.position, Quaternion.identity, 0);
			}
			else if(playersInRoom == 1)
			{
				PhotonNetwork.Instantiate(player2Prefab.name, transform.position, Quaternion.identity, 0);
			}
			else if(playersInRoom == 2)
			{
				PhotonNetwork.Instantiate(player3Prefab.name, transform.position, Quaternion.identity, 0);
			}
			else if(playersInRoom == 3)
			{
				PhotonNetwork.Instantiate(player4Prefab.name, transform.position, Quaternion.identity, 0);
			}
		}
		else //if(mManager)
		{
			if(mManager.sPlayersInRoom > 0)
			{
				mManager.GameLoop ();
			}
		}
	}

	void OnGUI()
	{
		if(mManager)
		{
			if(mManager.sPlayersTurn < mManager.sPlayersInRoom)
			{
				GUI.TextArea(new Rect(10,400,100 ,50),"Players Turn " + (mManager.sPlayersTurn+1).ToString());
			}
			else
			{
				GUI.TextArea(new Rect(10,400,100 ,50),"AI Turn");
				Debug.Log(mManager.sPlayersTurn.ToString());
			}
			
			if(GUI.Button(new Rect (10, 500, 100, 50), "End Turn"))
			{
				mManager.sPlayersTurn++;
			}
		}
	}
}
