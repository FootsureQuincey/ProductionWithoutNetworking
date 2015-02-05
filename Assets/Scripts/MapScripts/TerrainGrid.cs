using UnityEngine;
using System.Collections;

public class TerrainGrid : MonoBehaviour 
{
	//publics
	public Space[] mSpacesFloor; // array of spaces on current floor object

	public bool CheckIfShop(int ID)
	{
		return mSpacesFloor [ID].IsShop ();
	}

	public bool SpaceFull(int ID) // digs into space (using ID), checks if full
	{
		return mSpacesFloor[ID].IsFull ();
	}

	public void RemoveFromSpace(int ID) // removes marker for full space
	{
		mSpacesFloor [ID].LeaveSpace ();
	}

	public void EnterSpace(int ID) // turns on marker for full space
	{
		mSpacesFloor [ID].MoveToSpace ();
	}
}
