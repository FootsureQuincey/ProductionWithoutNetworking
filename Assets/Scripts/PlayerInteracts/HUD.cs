using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUD : MonoBehaviour 
{
	//publix
	public GameObject[] deck = new GameObject[10];
	public List<GUITexture> infamsprites = new List<GUITexture>();
	public double uoff = 0;
	public GUITexture bar, backbar, turns, stats;
	public GUITexture combar, atkbar, defbar, atkprt, defprt, cardslots;
	public GUIText str, def, mov, inf;
	
	//privates
	private int decksize, cdel;
	private baseCharacter ch;
	private GameObject[] discard = new GameObject[10];
	private GameObject[] cards = new GameObject[10];
	private GameObject[] hand = new GameObject[5];
	private int cardsheld = 0;
	private int cardsDealt = 0;
	private bool[] cs = new bool[3];
	private bool showR = false;
	private bool combui = false;
	private int attackeratk, attackerdef, defenderatk, defenderdef, bartotal, barpercent;
	
	//wyatt
	private GameManager mManager;
	//
	
	public float maxinfamy, infamy, percent;
	
	void Start ()
	{
		maxinfamy = 8; infamy = 0;
		//Compute Player stats here
		decksize = deck.Length;
		
		
		ResetDeck ();
	}
	
	void ResetDeck()
	{
		for (int i = 0; i < hand.Length; i++) 
		{
			hand[i] = null;	
		}
		for (int i = 0; i < discard.Length; i++) 
		{
			discard[i] = null;	
		}
		System.Array.Copy (deck, cards, cards.Length);
		showR = false;
		cdel = 0;
		cardsheld = 0;
		cardsDealt = 0;
	}
	
	void Playcard(GameObject cardd)
	{
		GameObject al;
		al = cardd.transform.parent.gameObject;
		for (int i = 0; i <discard.Length; i++)
		{
			if (discard[i] == null)
			{
				discard[i] = cardd;
			}
		}
		Destroy (al);
		//do card stuff
	}
	
	GameObject DealCard()
	{
		if (cardsheld == 5) 
		{
			return null;
			// alternately reset deck with ResetDeck();
		}
		
		System.Random rand = new System.Random();
		int card = rand.Next (10);
		while(true)
		{
			if (cards [card] == null)
			{
				card = rand.Next (10);
				Debug.Log(card);
			}
			else
				break;
		}
		GameObject go = GameObject.Instantiate (cards [card]) as GameObject;
		cards [card] = null;
		if (hand[0] == null)
			hand[0] = go;
		else if (hand[1] == null)
			hand[1] = go;
		else if (hand[2] == null)
			hand[2] = go;
		else if (hand[3] == null)
			hand[3] = go;
		else if (hand[4] == null)
			hand[4] = go;
		cardsheld++;
		cdel++;
		return go;
	}
	
	void Gameover()
	{
		ResetDeck();
		infamy = 0;
	}
	
	
	Rect ResizeGUI(Rect _rect)
	{
		float FilScreenWidth = _rect.width / 800;
		float rectWidth = FilScreenWidth * Screen.width;
		float FilScreenHeight = _rect.height / 600;
		float rectHeight = FilScreenHeight * Screen.height;
		float rectX = (_rect.x / 800) * Screen.width;
		float rectY = (_rect.y / 600) * Screen.height;
		
		return new Rect(rectX,rectY,rectWidth,rectHeight);
	}
	
	void OnGUI()
	{
		
		if (combui) 
		{
			//int screenAspectRatio = (screenWidth / screenHeight);
			//int textureAspectRatio = (textureWidth / textureHeight);
			GUI.Box(new Rect((Screen.width/2) - 180,210,100,150), ""); //attacker card slot
			GUI.Box(new Rect((Screen.width/2) + 140,40,100,150), "");	//target card slot
			GUI.Box(new Rect((Screen.width/2) + 260,40,100,150), "");//target card slot
			GUI.Box(new Rect((Screen.width/2) + 140,210,100,150), "");//target card slot
			GUI.Box(new Rect((Screen.width/2) + 260,210,100,150), "");//target card slot
			if (GUI.Button(new Rect((Screen.width/2) - 180,380,100, 25), "Fight!")) //Fight button
			{
				//Combatstuff
			}
			GUI.Box(new Rect((Screen.width/2),30,60,380), "");//Middle back bar
			GUI.Box(new Rect((Screen.width/2) + 5,35,50,185), "");//Middle Attack bar
			GUI.Box(new Rect((Screen.width/2) + 5,220,50,185), "");//Middle Defense bar
			GUI.Box(new Rect((Screen.width/2) - 20,215,100,10), "");//Middle Clash bar
			GUI.Box(new Rect((Screen.width/2) - 180,80,100,100), "");//Attacker Port box
			if(mManager)
			{
				GUI.Box(new Rect((Screen.width/2) - 180,40,100,20), mManager.CurrentPlayer().networkView.name);//Attacker name box
			}
			GUI.Box(new Rect((Screen.width/2) + 210,430,100,100), "");//Def port box
			GUI.Box(new Rect((Screen.width/2) + 210,390,100,20), "Defendername");//Def name box
		}
		else if (combui == false)
		{
			if (infamy == 0)
				percent = 0;
			else if (infamy >= maxinfamy)
			{percent = 190; infamy = maxinfamy;}
			else
				percent = 190 * (infamy/maxinfamy);
			
			GUI.DrawTexture(new Rect((Screen.width/2) - 100, (Screen.height/2) - (Screen.height/2) + 25, 200, 30), backbar.texture , ScaleMode.StretchToFill, true, 0.0f);
			GUI.DrawTexture(new Rect((Screen.width/2) - 95, (Screen.height/2) - (Screen.height/2) + 30, percent, 20), bar.texture , ScaleMode.StretchToFill, true, 0.0f);
			inf.text = "Infamy"; //Infamy text and spacing it out
			//turns
			GUI.DrawTexture(new Rect((Screen.width/2) - 470, (Screen.height/2) - 240, 200, 300), turns.texture , ScaleMode.StretchToFill, true, 0.0f);
			//stats
			GUI.DrawTexture(new Rect((Screen.width/2) + 260, (Screen.height/2) - 240, 200, 300), stats.texture , ScaleMode.StretchToFill, true, 0.0f);
			
			if (GUI.Button(new Rect(40,40,50, 30), "INFAMY BOOOST"))
			{
				infamy = infamy+1;
			}
			
			if (!showR) 
			{
				if (GUI.Button(new Rect(10,10,100, 20), "Deal"))
				{
					MoveDealtCard();
				}
			}
			else
			{
				if (GUI.Button(new Rect(10, 10, 100, 20), "Reset"))
				{
					ResetDeck();
				}
			}
			if (GUI.Button(new Rect(Screen.width - 110, 10, 100, 20), "GameOver"))
			{
				Gameover();
			}
		}
		//GameObject go = GameObject.Instantiate
		
	}
	
	void MoveDealtCard()
	{
		GameObject newCard = DealCard ();
		
		if (newCard == null)
		{
			Debug.Log("Out of Cards");
			showR = true;
			return;
		}
		float offset = 0;
		GameObject hudd = GameObject.FindGameObjectWithTag("HUD");
		newCard.transform.position = hudd.transform.position;
		newCard.transform.rotation = hudd.transform.rotation;
		newCard.transform.position = new Vector3(newCard.transform.position.x - offset, newCard.transform.position.y, newCard.transform.position.z + offset);
		//hand.Add (newCard);
		cardsDealt++;
	}
	
	void Rearrangehand()
	{
		GameObject[] tempdeck = new GameObject[5];
		int cintd = 0;
		for (int i = 0; i < hand.Length; i++) 
		{
			if (hand[i] != null)
			{
				tempdeck[cintd] = hand[i];
				cintd++;
			}
		}
		
		for (int i = 0; i < hand.Length; i++) 
		{
			hand[i] = tempdeck[i];
		}
		
		GameObject hudd = GameObject.FindGameObjectWithTag("HUD");
		float offset = (float)-.5;
		offset = offset + (float)uoff;
		if (cardsheld == 1)
		{
			offset = offset + 2;
			hand[0].transform.position = Camera.main.transform.position + Camera.main.transform.forward * 6;
			hand[0].transform.position = new Vector3(hand[0].transform.position.x, hand[0].transform.position.y - 5, hand[0].transform.position.z); 
			hand[0].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
		}
		if (cardsheld == 2)
		{
			offset = offset + (float)1.5;
			hand[0].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[0].transform.position = hand[0].transform.position + Camera.main.transform.right * (float)-.7;
			hand[0].transform.position = new Vector3(hand[0].transform.position.x, hand[0].transform.position.y - 5, hand[0].transform.position.z);
			hand[0].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + 1;
			hand[1].transform.position = Camera.main.transform.position + Camera.main.transform.forward * 6;
			hand[1].transform.position = hand[1].transform.position + Camera.main.transform.right * (float).7;
			hand[1].transform.position = new Vector3(hand[1].transform.position.x, hand[1].transform.position.y - 5, hand[1].transform.position.z);
			hand[1].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
		}
		if (cardsheld == 3)
		{
			offset = offset + 1;
			hand[0].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[0].transform.position = hand[0].transform.position + Camera.main.transform.right * (float)-1.4;
			hand[0].transform.position = new Vector3(hand[0].transform.position.x , hand[0].transform.position.y- 5, hand[0].transform.position.z );
			hand[0].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + 1;
			
			hand[1].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			
			hand[1].transform.position = new Vector3(hand[1].transform.position.x , hand[1].transform.position.y- 5, hand[1].transform.position.z );
			hand[1].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + 1;
			hand[2].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[2].transform.position = hand[2].transform.position + Camera.main.transform.right * (float)1.4;
			hand[2].transform.position = new Vector3(hand[2].transform.position.x , hand[2].transform.position.y- 5, hand[2].transform.position.z );
			hand[2].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
		}
		if (cardsheld == 4)
		{
			offset = offset + (float).5;
			hand[0].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[0].transform.position = hand[0].transform.position + Camera.main.transform.right * (float)-2.1;
			hand[0].transform.position = new Vector3(hand[0].transform.position.x , hand[0].transform.position.y - 5, hand[0].transform.position.z );
			hand[0].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + 1;
			hand[1].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[1].transform.position = hand[1].transform.position + Camera.main.transform.right * (float)-.7;
			hand[1].transform.position = new Vector3(hand[1].transform.position.x , hand[1].transform.position.y - 5, hand[1].transform.position.z );
			hand[1].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + 1;
			hand[2].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[2].transform.position = hand[2].transform.position + Camera.main.transform.right * (float).7;
			hand[2].transform.position = new Vector3(hand[2].transform.position.x , hand[2].transform.position.y - 5, hand[2].transform.position.z );
			hand[2].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + 1;
			hand[3].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[3].transform.position = hand[3].transform.position + Camera.main.transform.right * (float)2.1;
			hand[3].transform.position = new Vector3(hand[3].transform.position.x , hand[3].transform.position.y - 5, hand[3].transform.position.z );
			hand[3].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
		}
		if (cardsheld == 5)
		{
			offset = offset + (float).5;
			hand[0].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[0].transform.position = hand[0].transform.position + Camera.main.transform.right * (float)-2.8;
			hand[0].transform.position = new Vector3(hand[0].transform.position.x , hand[0].transform.position.y - 5, hand[0].transform.position.z );
			hand[0].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + 1;
			hand[1].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[1].transform.position = hand[1].transform.position + Camera.main.transform.right * (float)-1.4;
			hand[1].transform.position = new Vector3(hand[1].transform.position.x , hand[1].transform.position.y - 5, hand[1].transform.position.z );
			hand[1].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + 1;
			hand[2].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[2].transform.position = new Vector3(hand[2].transform.position.x , hand[2].transform.position.y - 5, hand[2].transform.position.z );
			hand[2].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + 1;
			hand[3].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[3].transform.position = hand[3].transform.position + Camera.main.transform.right * (float)1.4;
			hand[3].transform.position = new Vector3(hand[3].transform.position.x , hand[3].transform.position.y - 5, hand[3].transform.position.z );
			hand[3].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + 1;
			hand[4].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[4].transform.position = hand[4].transform.position + Camera.main.transform.right * (float)2.8;
			hand[4].transform.position = new Vector3(hand[4].transform.position.x , hand[4].transform.position.y - 5, hand[4].transform.position.z );
			hand[4].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
		}
		
		
	}
	
	//	void Rearrangeinfamy()
	//	{
	//		int w =24, h =24, x = -128, y = -80;
	//		for (int i=0; i < maxinfamy; i++)
	//		{
	//			transform.position = Vector3.zero;
	//			transform.localScale = Vector3.zero;
	//			infamsprites[i].pixelInset = new Rect(x,y,w,h);
	//				x = x + 24;
	//		}
	//	}
	
	void Update()
	{
		if(PhotonNetwork.offlineMode)
		{
			if(!mManager)
			{
				mManager = GameObject.Find("GameManager").GetComponent<GameManager>(); // thats how you get infromation from the manager
				if(mManager)
				{
					str.text = mManager.CurrentPlayer ().mAttack.ToString();
					def.text = mManager.CurrentPlayer ().mDefence.ToString();
					mov.text = mManager.CurrentPlayer ().mMovement.ToString();
				}
			}
		}
		else
		{
			if(!mManager)
			{
				mManager = GameObject.Find("GameManager(Clone)").GetComponent<GameManager>(); // thats how you get infromation from the manager
				if(mManager)
				{
					str.text = mManager.CurrentPlayer ().mAttack.ToString();
					def.text = mManager.CurrentPlayer ().mDefence.ToString();
					mov.text = mManager.CurrentPlayer ().mMovement.ToString();
				}
			}
		}
		if (combui)
		{
			bool killed = false;
			//if attacker is attacking
			bartotal = attackeratk + defenderdef;
			
			//import bar code from other bar
			
			//if defender is attacking
			bartotal = defenderatk + attackerdef;
		}
		str.text = infamy.ToString();
		if (Input.GetKeyDown("space"))
		{
			if (combui == false)
				combui = true;
			else
				combui = false;
		}
		
		Rearrangehand ();
		if (cdel == decksize) 
		{
			showR = true;
		}
		else
			showR = false;
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))			
			{ 
				Debug.Log("clicked it");
				
				if(hit.collider.CompareTag("Card"))
				{
					Debug.Log("hit it");
					Playcard(hit.collider.gameObject);
					cardsheld= cardsheld - 1;
					cardsDealt = cardsDealt - 1;
				}
			} 
		}
		
	}
	
	
	
}