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
//[RequireComponent(typeof(HUD))]

public class Player : MonoBehaviour
{
	public enum PlayerPhase
	{
		Start,
		MayBeMove,
		Move,
		Play,
		Special,
		Attack,
		End
	};

	//public enum  ClickPhase
	//{
	//	Zero,
	//	First,		// Show Path
	//	Second,		// moving
	//	Third,		// Show Attack
	//	Fourth,		// attacking
	//};
	//Information needed in the game 
	public Transform[] mAttackSelect = new Transform[4];
	public int mCharacter = 0  ;				//Character base stats
	private TileMap mTileMap;						//TileMap information
	TileMapMouse mMouse;							//Current Mouse information
	GameObject mTileMapObject;						//TileMap Object
	//HUD mHud;

	//Current Stats					
	public int mAttack;								//Current Player Attack
	public int mDefence;							//Current Player Defence
	public int mMovement;							//Current Player Movment
	public int mRange;								//Current Player Attack Range
	public int mInfamy = 0;							//Current Player Infamy
	public int skillsCD = 0;
	//Mouse Info			   		
	private int mMouseX;							//MouseOnTile info on X
	private int mMouseY;							//MouseOnTile info on Y
	private DTileMap.TileType curTarget;
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
	public List<DTileMap.TileType>mAttackList;
	public List<Vector3>mAttackPosition;
	//Player Loop
	public DTileMap.TileType mPlayerIndex;			//Current Player information
	public PlayerPhase mPlayerPhase;
	public bool mMoved;
	public bool mPlayed;
	public bool mTurn;
	public bool mOnSewer;


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

		//mAttack = mCharacter;
		//mDefence = mCharacter;
		//mMovement = mCharacter;
		//mRange = mCharacter;

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
		mMouse = mTileMapObject.GetComponent<TileMapMouse>();
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
		//if(Input.GetMouseButtonDown(0))
		//{	
		//	if(Input.GetMouseButtonUp(0))
		//	{
		//		if(mClick)
		//		{
		//			mMouseClickPhase++;
		//		}
		//	}
		//}
	}
	IEnumerator WaitAndPrint(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
	}
	
	
	public bool UpdatePlayer()
	{
		switch (mPlayerPhase)
		{
			case PlayerPhase.Start:
				UpdateStart ();
				break;
			case PlayerPhase.Move:
				UpdateMove ();
				break;
			case PlayerPhase.Special:
				UpdateSpecial();
				break;
			case PlayerPhase.Attack:
				UpdateAttack();
				break;
			case PlayerPhase.Play:
				UpdatePlay ();
				break;
			case PlayerPhase.End:
				UpdateEnd ();
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
		if(Input.GetMouseButtonDown(1))
		{
			ResetWalkRange ();
			mPlayerPhase = PlayerPhase.End;
		}
		if(mMouse.cubeActive == true)
		{
			FindAttackRange();
			if(mAttackList!=null)
			{
				int count = 0;
				foreach (Vector3 i in mAttackPosition)
				{
					mAttackSelect[count].position = i;
					mAttackSelect[count].renderer.enabled = true;
					count++;
				}
			}
			else
			{
				for(int i = 0; i<4; i++)
				{
					mAttackSelect[i].renderer.enabled = false;
				}
			}
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
			
			if(Input.GetMouseButtonDown(0))
			{	
				mStorePositionX = mMouseX;
				mStorePositionY = mMouseY;
				DTileMap.TileType temp=mTileMap.MapInfo.GetTileType(mStorePositionX, mStorePositionY);
					switch(temp)
					{
					case DTileMap.TileType.Floor:
						Debug.Log ("Player::Floor(out of range) "+mMouseClickPhase);
						mMouseClickPhase = 0;
						mClick = true;
						break;
					case DTileMap.TileType.Walkable:
						Debug.Log ("Player::Walkable");
						if(mMoved == false)
						{
							mPlayerPhase = PlayerPhase.MayBeMove;
						}
						break;			
					case DTileMap.TileType.Path:
						Debug.Log ("Player::Path: Invalid");
						if(mMoved == false)
						{
							mPlayerPhase = PlayerPhase.MayBeMove;
						}
						break;
					case DTileMap.TileType.Wall:
						Debug.Log ("Player::Wall: Can't travel");
						break;
					case DTileMap.TileType.Sewer:
						Debug.Log ("Player::Sewer: out of range");
						break;
					case DTileMap.TileType.Buildings:
						Debug.Log ("Player::Building");
						break;
					case DTileMap.TileType.Player1:
						Debug.Log ("Player::Player1");
						curTarget = DTileMap.TileType.Player1;
						foreach(DTileMap.TileType i in mAttackList)
						{
							if(i == DTileMap.TileType.Player1)
							{
							mPlayerPhase = PlayerPhase.Attack;
							}
						}
						break;
					case DTileMap.TileType.Player2:
						Debug.Log ("Player::Player2");
						curTarget = DTileMap.TileType.Player2;
						foreach(DTileMap.TileType i in mAttackList)
						{
							if(i == DTileMap.TileType.Player2)
							{
								mPlayerPhase = PlayerPhase.Attack;
							}
						}
						break;
					case DTileMap.TileType.Player3:
						Debug.Log ("Player::Player3");
						curTarget = DTileMap.TileType.Player3;
						foreach(DTileMap.TileType i in mAttackList)
						{
							if(i == DTileMap.TileType.Player3)
							{
								mPlayerPhase = PlayerPhase.Attack;
							}
						}
						break;
					case DTileMap.TileType.Player4:
						Debug.Log ("Player::Player4");
						curTarget = DTileMap.TileType.Player4;
						foreach(DTileMap.TileType i in mAttackList)
						{
							if(i == DTileMap.TileType.Player4)
							{
								mPlayerPhase = PlayerPhase.Attack;
							}
						}
						break;
					case DTileMap.TileType.Target1:
						Debug.Log ("Player::Target1");
						curTarget = DTileMap.TileType.Target1;
						foreach(DTileMap.TileType i in mAttackList)
						{
							if(i == DTileMap.TileType.Target1)
							{
								mPlayerPhase = PlayerPhase.Attack;
							}
						}
						break;
					case DTileMap.TileType.Target2:
						Debug.Log ("Player::Target2");
						curTarget = DTileMap.TileType.Target2;
						foreach(DTileMap.TileType i in mAttackList)
						{
							if(i == DTileMap.TileType.Target2)
							{
								mPlayerPhase = PlayerPhase.Attack;
							}
						}
						break;
					case DTileMap.TileType.Target3:
						Debug.Log ("Player::Target3");
						curTarget = DTileMap.TileType.Target3;
						foreach(DTileMap.TileType i in mAttackList)
						{
							if(i == DTileMap.TileType.Target3)
							{
								mPlayerPhase = PlayerPhase.Attack;
							}
						}
						break;
					case DTileMap.TileType.TrueSewer:		//Transfer to Sewer EndTurn
						Debug.Log ("Player::TrueSewer");
						//TravelSewer (mStorePositionX, mStorePositionY);
						break;
				}
			}
		}
		else if(mMouse.cubeActive == false)
		{
			mPlayerPhase = PlayerPhase.Play;
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
		PathFind( mPositionX, mPositionY, mStorePositionX, mStorePositionY);
			DTileMap.TileType temp=mTileMap.MapInfo.GetTileType(mMouseX, mMouseY);
		if(temp==DTileMap.TileType.Walkable|| temp==DTileMap.TileType.Path)
			{
				mStorePositionX = mMouseX;
				mStorePositionY = mMouseY;
			}
		else if(mMouseY==mStorePositionY && mMouseX==mStorePositionX&& Input.GetMouseButtonDown(0))
		{
			mPlayerPhase = PlayerPhase.Move;
		}
		else if(Input.GetMouseButtonDown(1))
		{
			ResetPath ();
			mPlayerPhase = PlayerPhase.Start;
		}
	}
	void UpdateSpecial()
	{
		Debug.Log ("PlayerTurn::Special");
	}
	void UpdateAttack()
	{
		//mHud = mHud.GetComponent<HUD>();
		//mHud.combui = true;
		Debug.Log ("PlayerTurn::Attack");
		if(Input.GetMouseButtonDown(0))
		{
			//mHud.combui = false;
			mPlayerPhase = PlayerPhase.End;
		}

	}
	void UpdatePlay()
	{
		Debug.Log ("PlayerTurn::PlayCard");
		if(mMouse.cubeActive==true)
		{
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
		mAttackPosition.Clear ();

		GraphSearch mSearch= new GraphSearch(mTileMap.MapInfo.mGraph);
		mSearch.RangeSearch(mPositionX, mPositionY, mRange);
		mAttackRangeList = mSearch.GetCloseList();
		//int positionIndex = mTileMap.MapInfo.XYToIndex (mPositionX, mPositionY);
		mAttackRangeList.RemoveAt(0);
		foreach(Node i in mAttackRangeList)
		{
			int index = i.mIndex;
			DTileMap.TileType temp = mTileMap.MapInfo.GetTileTypeIndex(index);
			if(temp==DTileMap.TileType.Player1)
			{
				mAttackList.Add (DTileMap.TileType.Player1);
				mAttackPosition.Add ( mTileMap.MapInfo.GetTileLocationIndex (index));
			}
			else if(temp==DTileMap.TileType.Player2)
			{
				mAttackList.Add (DTileMap.TileType.Player2);
				mAttackPosition.Add ( mTileMap.MapInfo.GetTileLocationIndex (index));
			}
			else if(temp==DTileMap.TileType.Player3)
			{
				mAttackList.Add (DTileMap.TileType.Player3);
				mAttackPosition.Add ( mTileMap.MapInfo.GetTileLocationIndex (index));
			}
			else if(temp==DTileMap.TileType.Player4)
			{
				mAttackList.Add (DTileMap.TileType.Player4);
				mAttackPosition.Add ( mTileMap.MapInfo.GetTileLocationIndex (index));
			}
			else if(temp==DTileMap.TileType.Target1)
			{
				mAttackList.Add (DTileMap.TileType.Target1);
				mAttackPosition.Add ( mTileMap.MapInfo.GetTileLocationIndex (index));
			}
			else if(temp==DTileMap.TileType.Target2)
			{
				mAttackList.Add (DTileMap.TileType.Target2);
				mAttackPosition.Add ( mTileMap.MapInfo.GetTileLocationIndex (index));
			}
			else if(temp==DTileMap.TileType.Target3)
			{
				mAttackList.Add (DTileMap.TileType.Target3);
				mAttackPosition.Add ( mTileMap.MapInfo.GetTileLocationIndex (index));
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
	void TravelToSewer(int TileX, int TileY)
	{
		mTileMap.MapInfo.SetTileType(mPositionX,mPositionY, DTileMap.TileType.Floor);
		PathFind (mPositionX, mPositionY, TileX, TileY);

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
	void ResetAttackList()
	{
		if (mAttackRangeList == null) 
		{
			return;
		}
		mAttackRangeList.Clear ();
		if (mAttackList == null) 
		{
			return;
		}
		mAttackList.Clear ();
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

	void AnchorbeardActive()
	{
		ResetWalkRange ();


		int rightX = 0;
     	int rightY = 0;
     	int leftX =  0;
     	int leftY =  0;
     	int upX = 0;
     	int upY = 0;
     	int downX =  0;
     	int downY =  0;
		//Still need to discard a card
		for(int hookamount = 3; hookamount>=2; hookamount--)
		{
			rightX = mPositionX + hookamount;
         	rightY = mPositionY;
         	leftX = mPositionX - hookamount;
			leftY = mPositionY;
			upX = mPositionX;
			upY = mPositionY + hookamount;
			downX = mPositionX;
			downY = mPositionY - hookamount;

			DTileMap.TileType hookRight = mTileMap.MapInfo.GetTileType (rightX, rightY);
			DTileMap.TileType hookLeft = mTileMap.MapInfo.GetTileType (leftX, leftY);
			DTileMap.TileType hookUp = mTileMap.MapInfo.GetTileType (upX, upY);
			DTileMap.TileType hookDown = mTileMap.MapInfo.GetTileType (downX, downY);
			
			if(hookRight==DTileMap.TileType.Wall || hookRight==DTileMap.TileType.Target1 ||hookRight==DTileMap.TileType.Target2||hookRight==DTileMap.TileType.Target3 || hookRight==DTileMap.TileType.Buildings)
			{
				DTileMap.TileType Check = mTileMap.MapInfo.GetTileType (rightX-1, rightY);
				if(Check == DTileMap.TileType.Floor)
				{
					mTileMap.MapInfo.SetTileType (rightX-1, rightY, DTileMap.TileType.Walkable);
				}
			}
			if(hookLeft==DTileMap.TileType.Wall || hookLeft==DTileMap.TileType.Target1 ||hookLeft==DTileMap.TileType.Target2||hookLeft==DTileMap.TileType.Target3 || hookLeft==DTileMap.TileType.Buildings)
			{
				DTileMap.TileType Check = mTileMap.MapInfo.GetTileType (leftX+1, leftY);
				if(Check == DTileMap.TileType.Floor)
				{
					mTileMap.MapInfo.SetTileType (leftX+1, leftY, DTileMap.TileType.Walkable);
				}
			}
			if(hookUp==DTileMap.TileType.Wall || hookUp==DTileMap.TileType.Target1 ||hookUp==DTileMap.TileType.Target2||hookUp==DTileMap.TileType.Target3 || hookUp==DTileMap.TileType.Buildings)
			{
				DTileMap.TileType Check = mTileMap.MapInfo.GetTileType (upX, upY-1);
				if(Check == DTileMap.TileType.Floor)
				{
					mTileMap.MapInfo.SetTileType (upX, upY-1, DTileMap.TileType.Walkable);
				}
			}
			if(hookDown==DTileMap.TileType.Wall || hookDown==DTileMap.TileType.Target1 ||hookDown==DTileMap.TileType.Target2||hookDown==DTileMap.TileType.Target3 || hookDown==DTileMap.TileType.Buildings)
			{
				DTileMap.TileType Check = mTileMap.MapInfo.GetTileType (downX, downY-1);
				if(Check == DTileMap.TileType.Floor)
				{
					mTileMap.MapInfo.SetTileType (downX, downY+1, DTileMap.TileType.Walkable);
				}
			}
		}
		DTileMap.TileType curType = mTileMap.MapInfo.GetTileType (mMouseX, mMouseY);
		if(Input.GetMouseButtonDown(0) && curType==DTileMap.TileType.Walkable)
		{
			Travel (mMouseX, mMouseY);
			if(mTileMap.MapInfo.GetTileType (downX, downY+1)==DTileMap.TileType.Walkable)
			{
				mTileMap.MapInfo.SetTileType (downX, downY+1, DTileMap.TileType.Floor);
			}
			if(mTileMap.MapInfo.GetTileType (upX, upY-1)==DTileMap.TileType.Walkable)
			{
				mTileMap.MapInfo.SetTileType (upX, upY-1, DTileMap.TileType.Floor);
			}
			if(mTileMap.MapInfo.GetTileType (leftX+1, leftY)==DTileMap.TileType.Walkable)
			{
				mTileMap.MapInfo.SetTileType (leftX+1, leftY, DTileMap.TileType.Floor);
			}
			if(mTileMap.MapInfo.GetTileType (rightX-1, rightY)==DTileMap.TileType.Walkable)
			{
				mTileMap.MapInfo.SetTileType (rightX-1, rightY, DTileMap.TileType.Floor);
			}
			mPlayerPhase = PlayerPhase.Start;
		}
		else if(Input.GetMouseButtonDown(1))
		{

			mPlayerPhase = PlayerPhase.Start;
		}
	}
	void Thunderclap()
	{
		//Discard a card

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