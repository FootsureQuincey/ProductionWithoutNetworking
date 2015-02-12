//Created by Dylan Fraser
//November 3, 2014
//Updated by
//Jack Ng
//November 4, 2014
//Wyatt Gibbs
//December 10, 2014
//Jack Ng
//Jan 8th, 2015
//Rewritten by Jack Ng
//Feb 5th, 2015

//Engine usuage
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Scripts requirement
[RequireComponent(typeof(TileMap))]
[RequireComponent(typeof(TileMapMouse))]
//[RequireComponent(typeof(GameManager))]
[RequireComponent(typeof(GraphSearch))]
[RequireComponent(typeof(Graph))]
[RequireComponent(typeof(Node))]


public class Player : MonoBehaviour
{
	
	//Information needed in the game 
	public baseCharacter mCharacter;				//Character base stats
	private TileMap mTileMap;						//TileMap information
	TileMapMouse mMouse;							//Current Mouse information
	GameObject mTileMapObject;						//TileMap Object
	
	//Current Stats					
	public int mAttack;								//Current Player Attack
	public int mDefence;							//Current Player Defence
	public int mMovement;							//Current Player Movment
	public int mRange;								//Current Player Attack Range
	public int mInfamy = 0;							//Current Player Infamy
	
	//Mouse Info			   		
	private int mMouseX;							//MouseOnTile info on X
	private int mMouseY;							//MouseOnTile info on Y
	
	//Tracking current Spot//  		
	public int mPositionX;							//Current TileMap Position X
	public int mPositionY;							//Current TileMap Position Y
	
	//List to Track Graph	  		
	public List<Node>mCloseList;					//List for finding walking range
	public List<Node>mPath;							//List for the actual path for player
	public List<int>mWalkRangeIndex;
	
	//Player Loop
	public DTileMap.TileType mPlayerIndex;			//Current Player information
	
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
	public GameObject Self;							//GameObject itself
	void Start()
	{
		//
		if(mPlayerIndex==DTileMap.TileType.Floor)
		{
			mPlayerIndex = DTileMap.TileType.Player1;
		}
		//Updating the currentTileMap information
		mTileMapObject=GameObject.Find("CurrentTileMap");
		mMouse = mTileMapObject.GetComponent<TileMapMouse> ();
		mTileMap = mTileMapObject.GetComponent<TileMap>();
		mMouseX = mMouse.mMouseHitX;
		mMouseY = mMouse.mMouseHitY;
		//instantiates the objects in this object
		mMoved = false;
		//mHand = new Hand();
		//mDeck = new Deck ();
		Debug.Log ("Player Created");
		mWalkRangeIndex = new List<int> ();
		//mTileMap.MapInfo.SetTileType(0, 0, 4);
	}
	void Update()
	{
		//Manager Loop
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
		//Updating the Current Mouse Information
		//Travel (mPositionX, mPositionY);
		Debug.Log ("mPositionX = "+mPositionX);
		Debug.Log ("mPositionY = "+mPositionY);
		mMouse = mTileMapObject.GetComponent<TileMapMouse> ();
		mTileMap = mTileMapObject.GetComponent<TileMap>();
		mMouseX = mMouse.mMouseHitX;
		mMouseY = mMouse.mMouseHitY;
		//Update the whole player function
		if (Input.GetKey ("w"))
		{
			UpdatePlayer ();
		}
		//Wall building code
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
		if(Input.GetKey ("c"))
		{
			FindWalkRange();	
		}
	}
	public void FindWalkRange()
	{
		GraphSearch mSearch= new GraphSearch(mTileMap.MapInfo.mGraph);
		mSearch.RangeSearch(mPositionX, mPositionY, mMovement);
		mCloseList = mSearch.GetCloseList();
		foreach(Node i in mCloseList)
		{
			int index = i.mIndex;
			DTileMap.TileType temp = mTileMap.MapInfo.GetTileTypeIndex(index);
			if(temp==DTileMap.TileType.Floor)
			{
				mTileMap.MapInfo.SetTileTypeIndex(index,DTileMap.TileType.Walkable);
			}		
		}
	}
	public void FindAttackRange()
	{
		GraphSearch mSearch= new GraphSearch(mTileMap.MapInfo.mGraph);
		mSearch.RangeSearch(mPositionX, mPositionY, mRange);
		mCloseList = mSearch.GetCloseList();
		foreach(Node i in mCloseList)
		{
			int index = i.mIndex;
			DTileMap.TileType temp = mTileMap.MapInfo.GetTileTypeIndex(index);
			if(temp==DTileMap.TileType.Floor)
			{
				mTileMap.MapInfo.SetTileTypeIndex(index,DTileMap.TileType.Walkable);
			}		
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
			
			DTileMap.TileType temp=mTileMap.MapInfo.GetTileType(mMouseX, mMouseY);
			ResetPath();
			switch((int)temp)
			{
				//case 0:
				//	Debug.Log ("Target::Floor(out of range)");
				//	mMoved = false;
				//	break;
			case 0:
				Debug.Log ("Target::Walkable");
				Travel (mMouseX, mMouseY);
				mMoved = true;
				//ResetWalkRange();
				//mWalkRange = false;
				break;
			case 2:
				Debug.Log ("Target::Wall");
				mMoved = false;
				break;
			case 3:
				break;
			case 4:
				break;
			case 5:
				break;
			case 6:
				mManager.curAttacking = (int)mPlayerIndex -6;
				mManager.curDefending = 6-6;
				Debug.Log("Player1");
				break;
			case 7:
				mManager.curAttacking = (int)mPlayerIndex -6;
				mManager.curDefending = 7-6;
				Debug.Log("Player2");
				break;
			case 8:
				mManager.curAttacking = (int)mPlayerIndex -6;
				mManager.curDefending = 8-6;
				Debug.Log("Player3");
				break;
			case 9:
				mManager.curAttacking = (int)mPlayerIndex -6;
				mManager.curDefending = 9-6;
				Debug.Log("Player4");
				break;
			case 10:
				mManager.curAttacking = (int)mPlayerIndex -6;
				mManager.curDefending = 10-6;
				Debug.Log("Target1");
				break;
			case 11:
				mManager.curAttacking = (int)mPlayerIndex -6;
				mManager.curDefending = 11-6;
				Debug.Log("Target2");
				break;
			case 12:
				mManager.curAttacking = (int)mPlayerIndex -6;
				mManager.curDefending = 12-6;
				Debug.Log("Target3");
				break;
			default:
				//Debug.Log ("Target::Default");
				mMoved = false;
				break;
			}
		}
		return true;
	}
	void Travel(int TileX, int TileY)
	{
		mTileMap.MapInfo.SetTileType(mPositionX,mPositionY, DTileMap.TileType.Floor);
		Vector3 v3Temp = mTileMap.MapInfo.GetTileLocation(TileX, TileY);
		Move(v3Temp);
		PathFind (mPositionX, mPositionY, TileX, TileY);
		mPositionX=TileX;
		mPositionY=TileY;
		mTileMap.MapInfo.SetTileType(mPositionX,mPositionY,mPlayerIndex);
	}
	void Move(Vector3 pos)
	{
		gameObject.transform.position = pos + new Vector3(0.0f, 1.0f, 0.0f);
	}
	
	void PathFind(int startX, int startY, int endX, int endY)
	{
		GraphSearch mSearch= new GraphSearch(mTileMap.MapInfo.mGraph);
		mSearch.PathFind(startX, startY, endX, endY);
		if(mSearch.IsFound())
		{
			mCloseList = mSearch.GetCloseList();
			mPath= mSearch.GetPathList();
		}
		foreach(Node i in mPath)
		{
			mTileMap.MapInfo.SetTileTypeIndex(i.mIndex,DTileMap.TileType.Path);
		}
		
	}	
	void ResetPath()
	{
		if (mPath == null) 
		{
			return;
		}
		for (int i=0; i<mCloseList.Count; i++)
		{
			int x = mCloseList[i].mIndex;
			mTileMap.MapInfo.SetTileTypeIndex (x, DTileMap.TileType.Floor);
		}
		
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