//Created by Dylan Fraser
//November 3, 2014
//----------------------//
//Changed by Jack Ng
//November 4, 2014
//-added a switch function to set up enum type
//-commented part is for tracking player and AI(not working)

using UnityEngine;
using System.Collections;

public class Space : MonoBehaviour 
{
	//publics
	public int mID;
	public int mPlayerSpot;
	public OccupyType mSetSpace;

	private Player pSpot;

	//privates
	private bool mFull = false;
	private bool mShop = false;
	//private GameObject mOccupant;
	private OccupyType mSpaceFilledWith;

	public enum OccupyType
	{
		Empty,
		Player,
		Target,
		Wall,
		NPC,
		Shop,
		Ladder
	};

	void Start()
	{
		mID = int.Parse (this.name);
		//pSpot = GetComponent<Player> ();
		//mPlayerSpot = pSpot.mCurrentSpot;
		//if (mID == mPlayerSpot)
		//{
		//	mSetSpace = (OccupyType)mPlayerSpot;
		//}
		mSpaceFilledWith = mSetSpace;

		//use if to instantiate specific space types
	}

	void Update () 
	{
		//pSpot = GetComponent<Player> ();
		//mPlayerSpot = pSpot.mCurrentSpot;
		//if (mID == mPlayerSpot)
		//{
		//	mSetSpace = (OccupyType)mPlayerSpot;
		//}
		mSpaceFilledWith = mSetSpace;

		//Switch bettween different exceptions
		switch (mSpaceFilledWith) 
		{
		case OccupyType.Empty:
			gameObject.renderer.material.color=Color.blue;
			break;
		case OccupyType.Player:
			gameObject.renderer.material.color=Color.green;
			break;
		case OccupyType.Target:
			gameObject.renderer.material.color=Color.red;
			break;
		case OccupyType.Wall:
			gameObject.renderer.material.color=Color.black;
			break;
		}
	}

	public void SetOccupant(int Occupy)
	{
		if (mSpaceFilledWith == OccupyType.Empty) 
		{
			mSpaceFilledWith = (OccupyType)Occupy;

		}
	}
	
	public void MoveToSpace() //indicates a unit has moved onto the space
	{
		mFull = true;
	}

	public void LeaveSpace() //indicates a unit has moved off of a space
	{
		mFull = false;
	}

	public bool IsFull() //checks if space is currently filled
	{
		if (mFull == true) 
		{
			return true;
		} 
		else 
		{
			return false;
		}
	}

	public bool IsShop() // checks if space is a shop
	{
		if (mShop == true) 
		{
			return true;
		} 
		else 
		{
			return false;
		}
	}	
}
