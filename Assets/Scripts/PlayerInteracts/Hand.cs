using UnityEngine;
using System.Collections;

public class Hand : MonoBehaviour 
{
	private ArrayList hand;
	public readonly int numberofcards;

	//Wyatt
	public bool PlayedCard;

	// Use this for initialization
	public Hand()
	{
		hand = new ArrayList();
	}
	public void clear()
	{
		hand.Clear ();
	}

	void Start ()
	{
		PlayedCard = false;
	}

	void addCard(Card card)	// TO ADD CARDS CALL addcard(Draw());
	{
		if (card == null)
		throw new UnityException ("Cannot add null card");
		hand.Add(card);

	}

	void playCard(Card card)
	{
		Deck disc = GetComponent<Deck> ();
		disc.discard [disc.discard.Length] = card;
		PlayedCard = true;
		hand.Remove(card);
		//Do playcard stuff

	}

	int count()
	{
		return hand.Count;
	}

	void Showhand ()
	{
		//Display players hand
	}

	// Update is called once per frame
	void Update ()
	{
	
	}
}
