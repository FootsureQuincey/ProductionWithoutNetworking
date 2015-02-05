//added functionality throughout the entire class
//Wyatt
//DID everything else
//Logan

using UnityEngine;
using System.Collections;

public class Deck : MonoBehaviour
{
	public int decksize = 30;
	//privates
	public Card[] deck; //DONT FUCKIN TOUCH
	private int cardsused;
	//publics
	public Card[] discard; //DONT FUCKIN TOUCH

	// Use this for initialization
	void Start () 
	{
		discard = new Card[decksize];
		deck = new Card[decksize];
		for (int c = 0; c < decksize; c++)
		deck [c] = new Card (1, 1);
	}

	void Shuffle () //NEEDS WORK WITH DISCARD PILE #done
	{
		for (int i = 0; i < discard.Length; ++i) 
		{
			deck[deck.Length] = discard[i];
			discard[i] = null;
		}

		for(int i = 0; i < deck.Length; ++i)
		{
			int random = (int)(Random.value % deck.Length);
			Card temp = deck[i];
			deck[i] = deck[random];
			deck[random] = temp;

		}
		cardsused = 0;
	}

	Card Draw()
	{
		if (cardsused == deck.Length)
		Shuffle ();
		// WES WAY  throw new UnityException ("NO CARDS LEFT IN DECK");
		cardsused++;
		return deck [cardsused - 1];

	}

	bool isEmpty()
	{
		if (cardsused == deck.Length)
						return true;
				else
						return false;
	}



//	void Regain ()
//	{
//		//In case of regain discarded cards
//	}
//	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
