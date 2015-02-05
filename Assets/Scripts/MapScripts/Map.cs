using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour {

	//publics
	public TerrainGrid mGroundFloor;
	public TerrainGrid mSecondFloor;
	public Player[] player;

	//privates
	//private int currentPlayer;

	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetMouseButton (0)) 
		{
			//player[currentPlayer].GetComponent<Player>();
		}
	}
}
