//Created by Dylan Fraser
//November 3, 2014
//<<<<<<<<<<<<<<<<<<//
//Modified by Jack Ng 
//November 4, 2014

using UnityEngine;
using System.Collections;

public class TestMap : MonoBehaviour 
{
	//publics
	public Space[] mGroundFloor;
	public int mRow;
	public int mColumn;

	public GameObject mTile;

	public Player mPlayer;
	public int mTesting;

	public int index;

	private Transform mStartTransform;
	private Transform mCurrentTransform;
	
	//privates

	// Use this for initialization
	void Start () 
	{
		//mStartTransform = mTile.transform;
		//mCurrentTransform = mStartTransform;
		//mRow = 10;
		//mColumn = 10;
		//mGroundFloor = new Space[mRow*mColumn];
		//for(int y=0; y<mColumn ; ++y)
		//{
		//	for(int x=0; x<mRow ; ++x)
		//	{
		//		index=x+y*mColumn;
		//		mGroundFloor[index] = Instantiate(mTile, mCurrentTransform, mCurrentTransform.rotation) as Space;
		//		mCurrentTransform.position.x=(float)x*mTile.transform.position.x;
		//		mCurrentTransform.position.z=(float)y*mTile.transform.position.z;
		//	}
		//}
		mPlayer = gameObject.GetComponent<Player> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		//mTesting = mGroundFloor [10].mID;
	}
}
