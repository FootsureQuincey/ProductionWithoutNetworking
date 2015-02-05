using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour 
{
	public int AttackModifier;
	public int DefenceModifier;

	// Use this for initialization
	public Card(int atk, int def)
	{
		AttackModifier = atk;
		DefenceModifier = def;
	}

	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
