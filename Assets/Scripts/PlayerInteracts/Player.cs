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
	public enum PlayerPhase
	{
		Start,
		MayBeMove,
		Move,
		Play,
		Attack,
		End
	};

	public enum  ClickPhase
	{
		Zero,
		First,		// Show Path
		Second,		// moving
		Third,		// Show Attack
		Fourth,		// attacking
	};

	public enum MovePhase
	{

	}
	//Information needed in the game 
	public BaseCharacter mCharacter;				//Character base stats
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
	public int curTarget;
	int mMouseClickPhase = 0;
	bool mClick =true;

	//Tracking current Spot//
	public int mStorePositionX;						//Previous TileMap Position X
	public int mStorePositionY;                  	//Previous TileMap Position Y
	public int mPositionX;							//Current TileMap Position X
	public int mPositionY;							//Current TileMap Position Y
	
	//List to Track Graph	  		
	public List<Node>mWalkRangeList;				//List for finding walking range
	public List<Node>mPath;							//List for the actual path for player
	public List<Node>mAttackRangeList;				//List for finding walking range
	public List<Node>mAttackList;
	//Player Loop
	public DTileMap.TileType mPlayerIndex;			//Current Player information
	private PlayerPhase mPlayerPhase;
	public bool mMoved;
	public bool mPlayed;
	public bool mTurn;


	//Wyatt: Network//

	public Hand mHand;
	public bool mAttacked;
	private Vector3 syncEndPosition = Vector3.zero;
	private GameManager mManager;
	//
	public Deck mDeck;
	public GameObject Self;							//GameObject itself
	
	void Start()
	{
		//
		if(mPlayerIndex==DTileMap.TileType.Floor)
		{
			mPlayerIndex = DTileMap.TileType.Player1;
		}

		//Connect the the TIleMap
		mTileMapObject = GameObject.Find ("CurrentTileMap");
		mTileMap = mTileMapObject.GetComponent<TileMap>();

		//Connect with the Mouse
		mMouse = mTileMapObject.GetComponent<TileMapMouse> ();
		mMouseX = mMouse.mMouseHitX;
		mMouseY = mMouse.mMouseHitY;
		//
		mPlayerPhase = PlayerPhase.Start;
		mClick = true;
		mMoved = false;
		mPlayed = false;
		mTurn = false;
		Debug.Log ("Player: Created");
	}
	void Update()
	{
		if (Input.GetMouseButton (0)) 
		{
			mMouseClickPhase++;

		}
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
		//Grabing the Current Mouse and Tile Information
		mMouse = mTileMapObject.GetComponent<TileMapMouse> ();
		mTileMap = mTileMapObject.GetComponent<TileMap>();
		mMouseX = mMouse.mMouseHitX;
		mMouseY = mMouse.mMouseHitY;
		//Put Player on Map at Starting Position
		Teleport (mPositionX, mPositionY);
		//Quick button checks

		//Update the whole player function
		if (Input.GetKey ("s"))
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
		if(Input.GetKeyDown ("r"))
		{
			FindWalkRange();	
		}
		if(Input.GetKeyUp ("r"))
		{
			ResetWalkRange ();
		}
		if(Input.GetKey ("w"))
		{
			Travel(mMouseX, mMouseY);	
		}
		if(Input.GetMouseButtonDown(0))
		{	
			if(Input.GetMouseButtonUp(0))
			{
				if(mClick)
				{
					mMouseClickPhase++;
				}
			}
		}
	}
	IEnumerator WaitAndPrint(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
	}
	
	
	public bool UpdatePlayer()
	{
		WaitAndPrint (2.0f);
		switch (mPlayerPhase)
		{
			case PlayerPhase.Start:
			UpdateStart ();
				break;
			case PlayerPhase.Move:
			UpdateMove ();
				break;
			case PlayerPhase.Attack:
				break;
			case PlayerPhase.Play:
				break;
			case PlayerPhase.End:
				break;
			case PlayerPhase.MayBeMove:
				UpdateMayBeMove();
				break;
			default:
				Debug.Log ("Player:Unknown state!");
				break;
		}
		return true;
	}
	void UpdateStart ()
	{

		Debug.Log ("Player::StateStart");
		if(mMoved==false)
		{
			FindWalkRange ();	//FInd all walkable Tiles
		}
		else
		{
			ResetWalkRange ();
			ResetPath();
		}

		if(Input.GetMouseButton(0))
		{	
			mStorePositionX = mMouseX;
			mStorePositionY = mMouseY;
			DTileMap.TileType temp=mTileMap.MapInfo.GetTileType(mStorePositionX, mStorePositionY);
				switch((int)temp)
				{
				case 0:
					Debug.Log ("Player::Floor(out of range) "+mMouseClickPhase);
					mMouseClickPhase = 0;
					mClick = true;
					break;
				case 1:
					Debug.Log ("Player::Walkable");
					if(mMoved == false)
					{
						mPlayerPhase = PlayerPhase.MayBeMove;
					}
					
					break;			
				case 2:
					Debug.Log ("Player::Path");
					break;
				case 3:
					Debug.Log ("Player::Wall");
					break;
				case 4:
					Debug.Log ("Player::Sewer");
					break;
				case 5:
					Debug.Log ("Player::Building");
					break;
				case 6:
					Debug.Log ("Player::Player1");
					break;
				case 7:
					Debug.Log ("Player::Player2");
					break;
				case 8:
					Debug.Log ("Player::Player3");
					break;
				case 9:
					Debug.Log ("Player::Player4");
					break;
				case 10:
					Debug.Log ("Player::Target1");
					break;
				case 11:
					Debug.Log ("Player::Target2");
					break;
				case 12:
					Debug.Log ("Player::Target3");
					break;
				case 13:			//Transfer to Sewer EndTurn
					Debug.Log ("Player::TrueSewer");
			
					break;
				case 14:			//Nothing Happen
					Debug.Log ("Player::TargetSpot");
					break;
			}
		}
		else if(Input.GetKey ("/"))
		{
			mPlayerPhase = PlayerPhase.End;
		}
	}
	void UpdateMove()
	{
		Debug.Log ("Player::StateMove");
		Travel (mStorePositionX, mStorePositionY);
		mMoved = true;
		mPlayerPhase = PlayerPhase.Start;
	}

	void UpdateMayBeMove()
	{
		Debug.Log ("Player::StatePath");
		if(Input.GetMouseButton(0))
		{
			DTileMap.TileType temp=mTileMap.MapInfo.GetTileType(mMouseX, mMouseY);
			if(temp==DTileMap.TileType.Walkable)
			{
				mStorePositionX = mMouseX;
				mStorePositionY = mMouseY;
			}
		}
		PathFind( mPositionX, mPositionY, mStorePositionX, mStorePositionY);
		if(mMouseY==mStorePositionY && mMouseX==mStorePositionX&& Input.GetMouseButton(1))
		{
			mPlayerPhase = PlayerPhase.Move;
		}
		if (Input.GetKey ("backspace")) 
		{
			ResetPath ();
			mPlayerPhase = PlayerPhase.Start;
		}
	}
	void UpdateEnd()
	{
		Debug.Log ("PlayerTurn Ended");
		mTurn = true;
	}
	public void FindWalkRange()
	{
		GraphSearch mSearch= new GraphSearch(mTileMap.MapInfo.mGraph);
		mSearch.RangeSearch(mPositionX, mPositionY, mMovement);
		mWalkRangeList = mSearch.GetCloseList();
		foreach(Node i in mWalkRangeList)
		{
			int index = i.mIndex;
			DTileMap.TileType temp = mTileMap.MapInfo.GetTileTypeIndex(index);
			if(temp==DTileMap.TileType.Floor)
			{
				mTileMap.MapInfo.SetTileTypeIndex(index,DTileMap.TileType.Walkable);
			}
			if(temp==DTileMap.TileType.Sewer)
			{
				mTileMap.MapInfo.SetTileTypeIndex(index,DTileMap.TileType.TrueSewer);
			}
		}
	}
	public void FindAttackRange()
	{
		GraphSearch mSearch= new GraphSearch(mTileMap.MapInfo.mGraph);
		mSearch.RangeSearch(mPositionX, mPositionY, mRange);
		mAttackRangeList = mSearch.GetCloseList();
		foreach(Node i in mAttackRangeList)
		{
			int index = i.mIndex;
			DTileMap.TileType temp = mTileMap.MapInfo.GetTileTypeIndex(index);
			if(temp==DTileMap.TileType.Floor)
			{
				mTileMap.MapInfo.SetTileTypeIndex(index,DTileMap.TileType.Walkable);
			}		
		}
	}
	void Travel(int TileX, int TileY)
	{
		mTileMap.MapInfo.SetTileType(mPositionX,mPositionY, DTileMap.TileType.Floor);
		PathFind (mPositionX, mPositionY, TileX, TileY);
		Teleport(TileX, TileY);
	}

	void Teleport(int TileX, int TileY)
	{
		Vector3 v3Temp = mTileMap.MapInfo.GetTileLocation(TileX, TileY);
		gameObject.transform.position = v3Temp + new Vector3(0.0f, 1.0f, 0.0f);
		mPositionX=TileX;
		mPositionY=TileY;
		mTileMap.MapInfo.SetTileType(mPositionX,mPositionY,mPlayerIndex);
	}
	
	void PathFind(int startX, int startY, int endX, int endY)
	{
		ResetPath ();
		GraphSearch mSearch= new GraphSearch(mTileMap.MapInfo.mGraph);
		mSearch.PathFind(startX, startY, endX, endY);
		if(mSearch.IsFound())
		{
			mPath= mSearch.GetPathList();
		}
		foreach(Node i in mPath)
		{
			mTileMap.MapInfo.SetTileTypeIndex(i.mIndex,DTileMap.TileType.Path);
		}
		mTileMap.MapInfo.SetTileTypeIndex (mPath[0].mIndex, mPlayerIndex);
	}
	//Reset all Path back to Walkable
	void ResetPath()
	{
		if (mPath == null) 
		{
			return;
		}
		for (int i=0; i<mPath.Count; i++)
		{
			int x = mPath[i].mIndex;
			DTileMap.TileType tempType = mTileMap.MapInfo.GetTileTypeIndex (x);
			if(tempType == DTileMap.TileType.Path|| tempType == mPlayerIndex)
			{
				mTileMap.MapInfo.SetTileTypeIndex (x, DTileMap.TileType.Walkable);
			}
		}

		mPath.Clear ();
	}
	void ResetWalkRange()
	{
		if (mWalkRangeList == null) 
		{
			return;
		}
		for (int i=0; i<mWalkRangeList.Count; i++)
		{
			int x = mWalkRangeList[i].mIndex;
			DTileMap.TileType tempType = mTileMap.MapInfo.GetTileTypeIndex (x);
			if(tempType == DTileMap.TileType.Walkable)
			{
				mTileMap.MapInfo.SetTileTypeIndex (x, DTileMap.TileType.Floor);
			}
		}
		//Debug.Log ("Player: Walk Range Reset");
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