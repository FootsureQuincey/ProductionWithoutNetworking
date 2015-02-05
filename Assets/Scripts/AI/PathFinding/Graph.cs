//Jack Ng
//Dec 8th, 2014


//North,	// x,y-1
//East,	// x+1, y
//South,	// x, y+1
//West,	// x-1, y

using System;
using UnityEngine;

public class Graph
{

	public Node[] mNodeArray;
	public int mWidth;
	public int mHeight;

	//Constructor
	public Graph()
	{
		mNodeArray = null;
		mWidth = 0;
		mHeight = 0;
	}
	//Destructor


	//Creating array
	public void Create(int width, int height)
	{
		Destroy ();
		mNodeArray = new Node[width*height];

		mWidth = width;
		mHeight = height;
		for(int i=0; i<width*height; ++i)
		{
			mNodeArray[i] = new Node();
		}
		for (int y=0; y<height; ++y) 
		{
			for (int x=0; x<width; ++x)
			{
				int index = x + (y*mWidth);
				//mNodeArray[index].mNeighbors = new Node[4];
				////mNodeArray[index].Init();
				//mNodeArray[index].mNeighbors[0] = 1.0f;
				mNodeArray[index].mNeighbors[0]= this.GetNodeInfo( x , y - 1);;	//North
				mNodeArray[index].mNeighbors[1] = this.GetNodeInfo( x + 1 , y);	//East
				mNodeArray[index].mNeighbors[2] = this.GetNodeInfo( x , y + 1);	//South
				mNodeArray[index].mNeighbors[3] = this.GetNodeInfo( x - 1 , y);	//West
				mNodeArray[index].close		 = false;
				mNodeArray[index].mIndex = index;
			}
		}
	}
	//Destory Node
	public void Destroy()
	{
		mNodeArray =null;
		mWidth=0;
		mHeight=0;

	}

	public void ResetNodes()
	{
		for(int i = 0; i < mWidth * mHeight; ++i)
		{
			mNodeArray[i].mParent=null;
			mNodeArray[i].open=false;
			mNodeArray[i].close=false;
			mNodeArray[i].g=0.0f;
			mNodeArray[i].h=0.0f;
		}
	}

	public Node GetNodeInfo(int x, int y)
	{
		//Debug.Log ("x:" + x +",y:"+y);
		if(x >= 0 && x < mWidth &&
		   y >= 0 && y < mHeight)
		{
			int index = x + (y*mWidth);
			return mNodeArray[index];
		}
		else 
		{
			Node node = null;
			return node;
		}
	}
}
