using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TileMap))]
public class TileMapMouse : MonoBehaviour {

	public int mMouseHitX;
	public int mMouseHitY;
	public Vector3 mMousePosition;

	public TileMap _tileMap;
	
	Vector3 currentTileCoord;
	//public GameObject mCube;
	public Transform selectionCube;
	
	void Start() 
	{
		_tileMap = GetComponent<TileMap>();
		mMousePosition = new Vector3(0.0f,0.0f,0.0f);
		mMouseHitX = 0;
		mMouseHitY = 0;
	}

	// Update is called once per frame
	void Update ()
	{
		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );
		RaycastHit hitInfo;
		if( collider.Raycast( ray, out hitInfo, Mathf.Infinity ) )
		{	
			selectionCube.renderer.enabled = true;
			int x = Mathf.FloorToInt( hitInfo.point.x / _tileMap.tileSize);
			int z = Mathf.FloorToInt( hitInfo.point.z / _tileMap.tileSize);
			if(Input.GetKey ("t"))
			{
				Debug.Log ("Tile: " + x + ", " + z);
			}
			currentTileCoord.x = x;
			currentTileCoord.z = z;
			mMouseHitX=x;
			mMouseHitY=z;
			mMousePosition=currentTileCoord*_tileMap.tileSize+ new Vector3(0.5f,selectionCube.position.y,0.5f);
			selectionCube.transform.position = mMousePosition;
		}
		else
		{
			//mMousePosition=currentTileCoord*_tileMap.tileSize+ new Vector3(0.5f,0.0f,0.5f);
			//selectionCube.transform.position = mMousePosition;
			selectionCube.renderer.enabled = false;
			// Hide selection cube?
		}
	}
}
