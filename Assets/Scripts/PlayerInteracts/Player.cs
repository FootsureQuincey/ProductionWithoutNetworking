//Created by Dylan Fraser
//November 3, 2014

//Updated by
//Jack Ng
//November 4, 2014
//Wyatt Gibbs
//December 10, 2014
//Jack Ng
//Jan 8th, 2015

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(TileMap))]
[RequireComponent(typeof(TileMapMouse))]

[RequireComponent(typeof(GraphSearch))]
[RequireComponent(typeof(Graph))]
[RequireComponent(typeof(Node))]
public class Player : MonoBehaviour
{

	//Information needed in the game 
	public baseCharacter mCharacter;
	TileMap mTileMap;
	TileMapMouse mMouse;
	GameObject mTileMapObject;

	//Current Stats
	public uint mAttack;
	public uint mDefence;
	public uint mMovement;
	public int mRange;
	
	//Mouse Info
	private int mMouseX;
	private int mMouseY;

	//Tracking current Spot//
	public int mPositionX;
	public int mPositionY;
	
	//List to Track Graph
	public List<Node>mCloseList;
	public List<Node>mPath;
	public List<int>mWalkRangeIndex;

	//Player Loop
	bool mWalkRange;
	bool mAttackRange;
	//Wyatt//
	//stuff I am using for Game Loop
	public bool mMoved;
	public Hand mHand;
	public bool mAttacked;
	private Vector3 syncEndPosition = Vector3.zero;
	private GameManager mManager;
	//made this public so I could reference it in the Game Manager to pass to the HUD 
	//allows game loop to move forwardcurrently//
	
	public Deck mDeck;
	public GameObject Self;
	public int mInfamy = 0;

	// Use this for initialization
	void Start()
	{
		mTileMapObject=GameObject.Find("CurrentTileMap");
		mMouse = mTileMapObject.GetComponent<TileMapMouse> ();
		mTileMap = mTileMapObject.GetComponent<TileMap>();
		mMouseX = mMouse.mMouseHitX;
		mMouseY = mMouse.mMouseHitY;
		//instantiates the objects in this object
		mMoved = false;
		mWalkRange = false;
		mAttackRange = false;
		//mHand = new Hand();
		//mDeck = new Deck ();
		Debug.Log ("Player Created");
		mWalkRangeIndex = new List<int> ();
		//mTileMap.MapInfo.SetTileType(0, 0, 4);
	}

	void Update()
	{
		if(!mManager)
		{
			if(!PhotonNetwork.offlineMode)
			{

				mManager = GameObject.Find ("GameManager(Clone)").GetComponent<GameManager>();
				mManager.AddPlayer (this);//allows gamemanager to know that a new player is active
			}
			else
			{
				mManager = GameObject.Find ("GameManager").GetComponent<GameManager>();
				mManager.AddPlayer (this);//allows gamemanager to know that a new player is active
			}
		}
		mMouse = mTileMapObject.GetComponent<TileMapMouse> ();
		mTileMap = mTileMapObject.GetComponent<TileMap>();
		//Debug.Log ("Tile: " + mMouse.mMouseHitX + ", " + mMouse.mMouseHitY);
		mMouseX = mMouse.mMouseHitX;
		mMouseY = mMouse.mMouseHitY;
		if (Input.GetKey ("w"))
		{
			UpdatePlayer ();
		}
		if (Input.GetKey ("o")) 
		{
			mTileMap.MapInfo.SetTileType(mMouseX,mMouseY, DTileMap.TileType.Wall);
			Debug.Log ("Tile: " + mMouseX + "," +mMouseY);
			Node node = mTileMap.MapInfo.mGraph.GetNodeInfo(mMouseX,mMouseY);
			node.walkable=false;
		}
		if (Input.GetKey ("p")) 
		{
			mTileMap.MapInfo.SetTileType(mMouseX,mMouseY, DTileMap.TileType.Floor);
			Node node = mTileMap.MapInfo.mGraph.GetNodeInfo(mMouseX,mMouseY);
			node.walkable=true;
		}
	}

	public bool UpdatePlayer()
	{
		//if(mWalkRange==false)
		//{
		//	UpdateWalkRange (mRange);
		//}
		if (Input.GetMouseButtonDown (0))
		{
			ResetPath();
			DTileMap.TileType temp=mTileMap.MapInfo.GetTileType(mMouseX, mMouseY);
			switch((int)temp)
			{
			//case 0:
			//	Debug.Log ("Target::Floor(out of range)");
			//	mMoved = false;
			//	break;
			case 0:
				
				Debug.Log ("Target::Walkable");
				mTileMap.MapInfo.SetTileType(mPositionX,mPositionY, DTileMap.TileType.Floor);
				Vector3 v3Temp = mTileMap.MapInfo.GetTileLocation(mMouseX, mMouseY);
				Move(v3Temp);
				PathFind (mPositionX, mPositionY, mMouseX, mMouseY);
				mPositionX=mMouseX;
				mPositionY=mMouseY;
				mTileMap.MapInfo.SetTileType(mPositionX,mPositionY, DTileMap.TileType.Player);
				mMoved = true;
				//ResetWalkRange();
				mWalkRange = false;
				break;
			case 2:
				Debug.Log ("Target::Wall");
				mMoved = false;
				break;
			default:
				//Debug.Log ("Target::Default");
				mMoved = false;
				break;
			}
		}
		return true;
	}

	void Move(Vector3 pos)
	{
		gameObject.transform.position = pos + new Vector3(0.0f, 1.0f, 0.0f);
	}

	void PathFind(int startX, int startY, int endX, int endY)
	{
		GraphSearch mSearch= new GraphSearch(mTileMap.MapInfo.mGraph);
		mSearch.Run(startX, startY, endX, endY);
		if(mSearch.IsFound())
		{
			mCloseList = mSearch.GetCloseList();
			mPath= mSearch.GetPathList();
		}
		foreach(Node i in mPath)
		{
			mTileMap.MapInfo.SetTileTypeIndex(i.mIndex,DTileMap.TileType.Player);
		}
	}	
	void ResetPath()
	{
		if (mPath == null) 
		{
			return;
		}
		for (int i=0; i<mPath.Count; i++)
		{
			int x = mPath[i].mIndex;
			mTileMap.MapInfo.SetTileTypeIndex (x,DTileMap.TileType.Floor);
		}
		mPath.Clear ();
	}
	void UpdateWalkRange(int range)
	{
		mWalkRangeIndex.Clear ();
		List<int>Temp =  new List<int> ();
		for(int possibleMoveY=0;possibleMoveY<=range; possibleMoveY++)
		{
			for(int possibleMoveX = 0; possibleMoveX<=range-possibleMoveY; possibleMoveX++)
			{
				int checkX = mPositionX + possibleMoveX;
				int checkY = mPositionY + possibleMoveY; 
				if(mTileMap.MapInfo.GetTileType(checkX,checkY)==0)
				{
					if (checkX > 9) 
					{
						checkX = 9;
					}
					if(checkX<0)
					{
						checkX = 0;
					}
					if (checkY > 9) 
					{
						checkY = 9;
					}
					if(checkY<0)
					{
						checkY = 0;
					}
					mTileMap.MapInfo.SetTileType(checkX,checkY, DTileMap.TileType.Walkable);
					int index = mTileMap.MapInfo.XYToIndex(checkX,checkY);
					Temp.Add (index);
				}
				else
				{
					continue;
				}
			}
		}
		mWalkRangeIndex = Temp;
		Debug.Log(mWalkRangeIndex.Count);
		mWalkRange = true;
	}
	void ResetWalkRange()
	{
		for (int i=0; i<mWalkRangeIndex.Count; i++)
		{
			mTileMap.MapInfo.SetTileTypeIndex (mWalkRangeIndex[i],0);
		}
		mWalkRangeIndex.Clear ();
	}
	//added this to try to fix some issues
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(rigidbody.position);
		}
		else
		{
			syncEndPosition = (Vector3)stream.ReceiveNext();
			mManager.sPlayersTurn++;
		}
	}
}
