//Created by Dylan Fraser
//Prodcution 1
//Updated by Jack Ng
//Production 1
//Updated by Jack Ng
//Jan 8th
using UnityEngine;


[RequireComponent(typeof(TileMap))]
public class DTileMap
{	
	//Jack Ng
	//Search Graph
	public Graph mGraph;
	GameObject mTileMapObject;
	TileMap mInfo;

	//>>>>>>>>>>>>>>>>>>>
	//<<<<<<<<<<<<<<<<<<<
	int size_x;
	int size_y;
	

	//public struct V2
	//{
	//	public float x;
	//	public float y;
	//}

	public Vector3[] mPositionData;

	public int[] map_data;
	
	public enum TileType
	{
		Floor,		//0
		Walkable,	//1
		Wall,		//2
		Path,		//3
		Player,		//4
		Target,		//5		
		Guard,		//6
		Shop,		//7
		Buildings	//8
	};
	public void CreateMap(int sizex, int sizey)
	{
		mTileMapObject=GameObject.Find("CurrentTileMap");

		mInfo = mTileMapObject.GetComponent<TileMap>();
		map_data = new int[size_x*size_y];
		//Graph Search Structure
		mGraph=new Graph();
		mGraph.Create (size_x, size_y);
		//Map position structure
		Vector3 startLocation;
		startLocation.x = 0.5f;
		startLocation.y = 0.5f;
		startLocation.z = 0.5f;	
		
		mPositionData = new Vector3[sizex*sizey];
		
		for(int y=0;y<sizey;y++)
		{
			for(int x=0;x<sizex;x++) 
			{
				//Setting Map
				map_data[(y*size_x)+x] = (int)TileType.Floor;
				
				
				//Setting Map position
				Vector3 currentLocation = startLocation;
				currentLocation.x += x*1.0f;
				currentLocation.z += y*1.0f;
				mPositionData[(y*size_x)+x] =currentLocation;
				//Seting Graph Search
				
				Node node = mGraph.GetNodeInfo(x,y);
				int type = map_data[(y*size_x)+x];
				int i= (y*size_x)+x;
				if(type==(int)TileType.Floor)
				{
					node.walkable = true;
				}
				else
				{
					node.walkable = false;
				}
			}
		}
		map_data [1] = (int)TileType.Wall;
	}
	public DTileMap(int sizex, int sizey)
	{
		//Map Structure
		this.size_x = sizex;
		this.size_y = sizey;
		CreateMap (sizex, sizey);
	}
	public int XYToIndex(int x, int y)
	{
		return ((y * size_x) + x);
	}
	public TileType GetTileType(int x, int y)
	{
		return (TileType)map_data[(y*size_x)+x];
	}
	public void SetTileType(int x, int y, TileType t)
	{
		map_data[(y*size_x)+x]= (int)t;

		mInfo.UpdateTexture (x,y);
	}
	public void SetTileTypeIndex(int index, TileType t)
	{
		map_data[index]= (int)t;
		int x= 0, y = 0;
		IndexToXY (index,out x, out y);
		mInfo.UpdateTexture (x,y);
	}
	public void IndexToXY(int index, out int x, out int y)
	{
		int count = 0;
		while(index>=size_x)
		{
			index-=size_x;
			count++;
		}
		y = count;
		x = index;
	}

	public Vector3 GetTileLocation(int x, int y)
	{
		return mPositionData[(y*size_x)+x];
	}
	public Vector3 GetTileLocationIndex(int index)
	{
		int x= 0, y = 0;
		IndexToXY (index, out x, out y);
		return mPositionData [(y * size_x) + x];
	}
	//public V2 GetTileXY(Vector3 location)
	//{
	//	for(int y=0;y<size_x;y++)
	//	{
	//		for(int x=0;x<size_y;x++) 
	//		{
	//			Vector3 tempPosition=mPositionData[(y*size_x)+x];
	//			if(location.x == tempPosition.x
	//			   && location.y == tempPosition.y
	//			   && location.z == tempPosition.z)
	//			{
	//				V2 tileXY;
	//				tileXY.x=x;
	//				tileXY.y=y;
	//				return tileXY;
	//			}
	//		}
	//	}
	//	V2 error;
	//	error.x = -1;
	//	error.y = -1;
	//	return error;
	//}
}
