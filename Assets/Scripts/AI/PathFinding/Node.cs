//Jack Ng
//Dec 5th, 2014

using System;
using UnityEngine;
public enum Direction
{
	North,	// x,y-1
	East,	// x+1, y
	South,	// x, y+1
	West,	// x-1, y
	Count
}


public class Node
{

	public Node[] mNeighbors;

	public int mIndex;

	public Node mParent;

	public bool open;
	public bool close;

	public bool walkable;

	public float g;
	public float h;

	public Node()
	{
		mNeighbors = new Node[4]; //could use Direction.count but we will need to cast it which cost one more action
		g = 0.0f;
		h = 0.0f;
	}

	public void Init()
	{
		//mNeighbors = new Node[4]; //could use Direction.count but we will need to cast it which cost one more action
	}

}
