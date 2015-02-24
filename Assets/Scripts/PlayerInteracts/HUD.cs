using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUD : MonoBehaviour 
{
	//publix
	public GameObject[] deck = new GameObject[30];
	public double uoff = 0;
	public Texture bar, backbar;
	public GUITexture combar, atkbar, defbar, atkprt, defprt, cardslots;
	public Texture2D turns, stats;


	//Jack
	//So the ints are player 0,1,2,3
	// and the targets are 4,5,6,7,8
	//public int curAttacking;
	//public int curDefending;
	//I made these two varible in the manager, so just grab the number there(default to be -1 if nothing is happening)
	//--------------------//
	//privates
	private RaycastHit curcard;
	private GameObject stat, estat;
	private GameObject set1, set2, set3, set4, set5, set6, set7, set8, set9, set10, set11, set12;
	private int decksize, cdel;
	private GameObject[] discard = new GameObject[30];
	private GameObject[] cards = new GameObject[30];
	private GameObject[] hand = new GameObject[15];
	private int cardsheld = 0;
	private int cardsDealt = 0;
	private bool[] cs = new bool[3];
	private List<GameObject> p1c = new List<GameObject>(), p2c= new List<GameObject>(), p3c= new List<GameObject>(), p4c= new List<GameObject>(), t1c= new List<GameObject>(), t2c= new List<GameObject>(), t3c= new List<GameObject>(), t4c= new List<GameObject>(), t5c= new List<GameObject>();
	private bool showR = false;
	private bool combui = false;
	private bool choosing = false;
	private bool comuitext = false;
	private int attackeratk, attackerdef, defenderatk, defenderdef, bartotal, barpercent;
	
	//wyatt
	private GameManager mManager;
	//
	
	public float maxinfamy, infamy, percent;

	void Start ()
	{
		for (int j = 0; j < 5; j++)
			p1c.Add (null);
		for (int j = 0; j < 5; j++)
			p2c.Add (null);
		for (int j = 0; j < 5; j++)
			p3c.Add (null);
		for (int j = 0; j < 5; j++)
			p4c.Add (null);
		for (int j = 0; j < 5; j++)
			t1c.Add (null);
		for (int j = 0; j < 5; j++)
			t2c.Add (null);
		for (int j = 0; j < 5; j++)
			t3c.Add (null);
		for (int j = 0; j < 5; j++)
			t4c.Add (null);
		for (int j = 0; j < 5; j++)
			t5c.Add (null);

		//Compute Player stats here
		decksize = deck.Length;

		maxinfamy = 8; infamy = 0;
		stat = new GameObject();
		stat.AddComponent<GUITexture> ();
		stat.transform.localScale = Vector3.zero;
		stat.guiTexture.pixelInset = new Rect((Screen.width/2), (Screen.height/2), 200, 300);

		estat = new GameObject();
		estat.AddComponent<GUITexture> ();
		estat.transform.localScale = Vector3.zero;
		estat.guiTexture.pixelInset = new Rect((Screen.width/2), (Screen.height/2), 200, 300);

		set1 = new GameObject();
		set1.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set1.AddComponent<GUIText> ();


		set2 = new GameObject();
		set2.AddComponent<GUIText>();
		set2.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set2.guiText.pixelOffset = new Vector2 (250, 0);

		set3 = new GameObject ();
		set3.AddComponent<GUIText>();
		set3.transform.position = new Vector3(0.5f, 0.5f,  1.0f);
		set3.guiText.pixelOffset = new Vector2 (350, 0);

		set4 = new GameObject();
		set4.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set4.AddComponent<GUIText>();

		set5 = new GameObject();
		set5.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set5.AddComponent<GUIText>();

		set6 = new GameObject();
		set6.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set6.AddComponent<GUIText>();

		set7 = new GameObject();
		set7.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set7.AddComponent<GUIText>();

		set8 = new GameObject();
		set8.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set8.AddComponent<GUIText>();

		set9 = new GameObject();
		set9.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set9.AddComponent<GUIText>();

		set10 = new GameObject();
		set10.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set10.AddComponent<GUIText>();

		set11 = new GameObject();
		set11.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set11.AddComponent<GUIText>();

		set12 = new GameObject();
		set12.transform.position = new Vector3(0.5f, 0.5f, 1.0f);
		set12.AddComponent<GUIText>();

		ResetDeck ();

	}
	
	void ResetDeck()
	{
		for (int i = 0; i < hand.Length; i++) 
		{
			if (hand[i] != null)
				Destroy(hand[i]);
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
		System.Random rand = new System.Random();
		int card = rand.Next (30);
		while(true)
		{
			if (cards [card] == null)
			{
				card = rand.Next (30);
				Debug.Log(card);
			}
			else
				break;
		}
		GameObject go = GameObject.Instantiate (cards [card]) as GameObject;
		cards [card] = null;
		for(int i = 0; i < 15; ++i)
		{
			if (hand[i] == null)
			{
				hand[i] = go;
				break;
			}
		}
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
		if (mManager.curDefending == mManager.curAttacking)
		{
			mManager.curDefending = -1;
		}

		for (int l = 0; l < 5; l++)
		{
			if (p1c[l] != null)
			p1c[l].SetActive(false);
		}
		for (int l = 0; l < 5; l++)
		{
			if (p2c[l] != null)
			p2c[l].SetActive(false);
		}
		for (int l = 0; l < 5; l++)
		{
			if (p3c[l] != null)
			p3c[l].SetActive(false);
		}
		for (int l = 0; l < 5; l++)
		{
			if (p4c[l] != null)
			p4c[l].SetActive(false);
		}
		for (int l = 0; l < 5; l++)
		{
			if (t1c[l] != null)
			t1c[l].SetActive(false);
		}
		for (int l = 0; l < 5; l++)
		{
			if (t2c[l] != null)
			t2c[l].SetActive(false);
		}
		for (int l = 0; l < 5; l++)
		{
			if (t3c[l] != null)
			t3c[l].SetActive(false);
		}
		for (int l = 0; l < 5; l++)
		{
			if (t4c[l] != null)
			t4c[l].SetActive(false);
		}
		for (int l = 0; l < 5; l++)
		{
			if (t5c[l] != null)
			t5c[l].SetActive(false);
		}

		estat.guiTexture.pixelInset = new Rect(Screen.width - Screen.width, (Screen.height/2) - 150, 200, 300);
		stat.guiTexture.pixelInset = new Rect(Screen.width - 200, (Screen.height/2) - 150, 200, 300);
		stat.guiTexture.texture = stats;
		//estat.guiTexture.pixelInset = new Rect (100, 100, 100, 100);
		estat.guiTexture.texture = stats;
		//GUI.DrawTexture(new Rect((Screen.width/2) + 275, 100 , 200, 300), stats , ScaleMode.StretchToFill, true, 0.0f);
		//GUI.DrawTexture(new Rect((Screen.width/2) - 475, 100 , 200, 300), stats , ScaleMode.StretchToFill, true, 0.0f);

		if (infamy == 0)
			percent = 0;
		else if (infamy >= maxinfamy)
		{percent = 190; infamy = maxinfamy;}
		else
			percent = 190 * (infamy/maxinfamy);
		
		GUI.DrawTexture(new Rect((Screen.width/2) - 100, 20  , 200, 30), backbar , ScaleMode.StretchToFill, true, 0.0f);
		GUI.DrawTexture(new Rect((Screen.width/2) - 95, 25 , percent, 20), bar , ScaleMode.StretchToFill, true, 0.0f);


		//Infamy text
		set1.guiText.text = infamy.ToString ();
		set2.guiText.text = "/";
		set3.guiText.text = maxinfamy.ToString();
		set1.guiText.pixelOffset = new Vector2 (0, (Screen.height/2));
		set2.guiText.pixelOffset = new Vector2 (10, (Screen.height/2));
		set3.guiText.pixelOffset = new Vector2 (20, (Screen.height/2));
	
		//Players Stats
		set4.guiText.text = "Infamy:";
		set5.guiText.text = mManager.sPlayers[mManager.curAttacking].name;
		set6.guiText.text = mManager.sPlayers[mManager.curAttacking].mAttack.ToString();
		set7.guiText.text = mManager.sPlayers[mManager.curAttacking].mDefence.ToString();
		set8.guiText.text = mManager.sPlayers[mManager.curAttacking].mMovement.ToString();
		set4.guiText.pixelOffset = new Vector2 (-70, (Screen.height/2));
		set5.guiText.pixelOffset = new Vector2 ((Screen.width / 2) - 100, 0 + 120);
		set6.guiText.pixelOffset = new Vector2 ((Screen.width / 2) - 100, 0 + 65);
		set7.guiText.pixelOffset = new Vector2 ((Screen.width / 2) - 100, 0 + 20);
		set8.guiText.pixelOffset = new Vector2 ((Screen.width / 2) - 100, 0 - 25);

		//Target/Other player stats
		if (choosing)
		{
			if (GUI.Button(new Rect(50,(Screen.height/2) - 200,100, 40), "Play Card") )
			{
				choosing = false;
				if ( mManager.curDefending == 0)
				{
					if (p1c[mManager.curAttacking] ==  null)
					{
						curcard.collider.gameObject.tag = "Untagged";
						p1c[mManager.curAttacking] = Instantiate(curcard.collider.gameObject.transform.parent.gameObject) as GameObject;
						cardsheld--;
						Destroy(curcard.collider.gameObject.transform.parent.gameObject);
					}

				}
				else if ( mManager.curDefending == 1)
				{
					if (p2c[mManager.curAttacking] ==  null)
					{
						curcard.collider.gameObject.tag = "Untagged";
						p2c[mManager.curAttacking] = Instantiate(curcard.collider.gameObject.transform.parent.gameObject) as GameObject;
						cardsheld--;
						Destroy(curcard.collider.gameObject.transform.parent.gameObject);
					}
					
				}
				else if ( mManager.curDefending == 2)
				{
					if (p3c[mManager.curAttacking] ==  null)
					{
						curcard.collider.gameObject.tag = "Untagged";
						p3c[mManager.curAttacking] = Instantiate(curcard.collider.gameObject.transform.parent.gameObject) as GameObject;
						cardsheld--;
						Destroy(curcard.collider.gameObject.transform.parent.gameObject);
					}
					
				}
				else if ( mManager.curDefending == 3)
				{
					if (p4c[mManager.curAttacking] ==  null)
					{
						curcard.collider.gameObject.tag = "Untagged";
						p4c[mManager.curAttacking] = Instantiate(curcard.collider.gameObject.transform.parent.gameObject) as GameObject;
						cardsheld--;
						Destroy(curcard.collider.gameObject.transform.parent.gameObject);
					}
					
				}
				else if ( mManager.curDefending == 4)
				{
					if (t1c[mManager.curAttacking] ==  null)
					{
						curcard.collider.gameObject.tag = "Untagged";
						t1c[mManager.curAttacking] = Instantiate(curcard.collider.gameObject.transform.parent.gameObject) as GameObject;
						cardsheld--;
						Destroy(curcard.collider.gameObject.transform.parent.gameObject);
					}
					
				}
				else if ( mManager.curDefending == 5)
				{
					if (t2c[mManager.curAttacking] ==  null)
					{
						curcard.collider.gameObject.tag = "Untagged";
						t2c[mManager.curAttacking] = Instantiate(curcard.collider.gameObject.transform.parent.gameObject) as GameObject;
						cardsheld--;
						Destroy(curcard.collider.gameObject.transform.parent.gameObject);
					}
					
				}
				else if ( mManager.curDefending == 6)
				{
					if (t3c[mManager.curAttacking] ==  null)
					{
						curcard.collider.gameObject.tag = "Untagged";
						t3c[mManager.curAttacking] = Instantiate(curcard.collider.gameObject.transform.parent.gameObject) as GameObject;
						cardsheld--;
						Destroy(curcard.collider.gameObject.transform.parent.gameObject);
					}
					
				}
				else if ( mManager.curDefending == 7)
				{
					if (t4c[mManager.curAttacking] ==  null)
					{
						curcard.collider.gameObject.tag = "Untagged";
						t4c[mManager.curAttacking] = Instantiate(curcard.collider.gameObject.transform.parent.gameObject) as GameObject;
						cardsheld--;
						Destroy(curcard.collider.gameObject.transform.parent.gameObject);
					}
					
				}
				else if ( mManager.curDefending == 8)
				{
					if (t5c[mManager.curAttacking] ==  null)
					{
						curcard.collider.gameObject.tag = "Untagged";
						t5c[mManager.curAttacking] = Instantiate(curcard.collider.gameObject.transform.parent.gameObject) as GameObject;
						cardsheld--;
						Destroy(curcard.collider.gameObject.transform.parent.gameObject);
					}
					
				}

			}

			if (GUI.Button(new Rect((Screen.width) - 100,(Screen.height/2)- 200, 100, 40), "Play Card"))
			{
				choosing = false;

				if ( mManager.curAttacking == 0 )
				{

					if (p1c[mManager.curAttacking] ==  null)
					{
						curcard.collider.gameObject.tag = "Untagged";
						p1c[mManager.curAttacking] = Instantiate(curcard.collider.gameObject.transform.parent.gameObject, new Vector3(0,0,0),Quaternion.identity) as GameObject;
						cardsheld--;
						Destroy(curcard.collider.gameObject.transform.parent.gameObject);

					}
					else if (p1c[4] ==  null)
					{
						curcard.collider.gameObject.tag = "Untagged";
						p1c[4] = Instantiate(curcard.collider.gameObject.transform.parent.gameObject, new Vector3(0,0,0),Quaternion.identity) as GameObject;
						cardsheld--;
						Destroy(curcard.collider.gameObject.transform.parent.gameObject);
						
					}
				}
				else if ( mManager.curAttacking == 1)
				{
					if (p2c[mManager.curAttacking] ==  null)
					{
						curcard.collider.gameObject.tag = "Untagged";
						p2c[mManager.curAttacking] = Instantiate(curcard.collider.gameObject.transform.parent.gameObject) as GameObject;
						cardsheld--;
						Destroy(curcard.collider.gameObject.transform.parent.gameObject);
					}
					else if (p2c[4] ==  null)
					{
						curcard.collider.gameObject.tag = "Untagged";
						p2c[4] = Instantiate(curcard.collider.gameObject.transform.parent.gameObject, new Vector3(0,0,0),Quaternion.identity) as GameObject;
						cardsheld--;
						Destroy(curcard.collider.gameObject.transform.parent.gameObject);
						
					}
					
				}
				else if ( mManager.curAttacking == 2)
				{
					if (p3c[mManager.curAttacking] ==  null)
					{
						curcard.collider.gameObject.tag = "Untagged";
						p3c[mManager.curAttacking] = Instantiate(curcard.collider.gameObject.transform.parent.gameObject) as GameObject;
						cardsheld--;
						Destroy(curcard.collider.gameObject.transform.parent.gameObject);
					}
					else if (p3c[4] ==  null)
					{
						curcard.collider.gameObject.tag = "Untagged";
						p3c[4] = Instantiate(curcard.collider.gameObject.transform.parent.gameObject, new Vector3(0,0,0),Quaternion.identity) as GameObject;
						cardsheld--;
						Destroy(curcard.collider.gameObject.transform.parent.gameObject);
						
					}
					
				}
				else if ( mManager.curAttacking == 3)
				{
					if (p4c[mManager.curAttacking] ==  null)
					{
						curcard.collider.gameObject.tag = "Untagged";
						p4c[mManager.curAttacking] = Instantiate(curcard.collider.gameObject.transform.parent.gameObject) as GameObject;
						cardsheld--;
						Destroy(curcard.collider.gameObject.transform.parent.gameObject);
					}
					else if (p4c[4] ==  null)
					{
						curcard.collider.gameObject.tag = "Untagged";
						p4c[4] = Instantiate(curcard.collider.gameObject.transform.parent.gameObject, new Vector3(0,0,0),Quaternion.identity) as GameObject;
						cardsheld--;
						Destroy(curcard.collider.gameObject.transform.parent.gameObject);
						
					}
					
				}
			}
		}

		if (mManager.curDefending == -1 || mManager.curDefending == mManager.curAttacking)
		{

			set9.guiText.text = "None Selected";
			set10.guiText.text = "N/A";
			set11.guiText.text = "N/A";
			set12.guiText.text = "N/A";
		}
		else
		{
		set9.guiText.text = mManager.sPlayers[mManager.curDefending].name;
		set10.guiText.text = mManager.sPlayers[mManager.curDefending].mAttack.ToString();
		set11.guiText.text = mManager.sPlayers[mManager.curDefending].mDefence.ToString();
		set12.guiText.text = mManager.sPlayers[mManager.curDefending].mMovement.ToString();
		}
		set9.guiText.pixelOffset = new Vector2 (-(Screen.width/2) + 100, 0 + 120);
		set10.guiText.pixelOffset = new Vector2(-(Screen.width/2) + 100, 0 + 65);
		set11.guiText.pixelOffset = new Vector2(-(Screen.width/2) + 100, 0 + 20);
		set12.guiText.pixelOffset = new Vector2(-(Screen.width/2) + 100, 0 - 25);
		if (mManager.curDefending != -1)
			if (GUI.Button(new Rect(40,40,50, 30), "Attack"))
			{
				Attack();

				// infamy boost infamy = infamy+1;
			}
			
			if (!showR) 
				if (GUI.Button(new Rect(10,10,100, 20), "Deal"))
				{
					MoveDealtCard();
				}
			
			if (GUI.Button(new Rect(Screen.width - 110, 10, 100, 20), "GameOver"))
			{
				Gameover();
			}
			
			if (GUI.Button(new Rect(Screen.width - 110, 40, 100, 20), "Quit"))
			{
				Application.Quit();
			}

		Dispcards ();

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
		GameObject[] tempdeck = new GameObject[15];
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
		float offset = (float)-5.6;
		offset = offset + (float)uoff;
		
		//Find farthest card left based on how many cards total
		
		for (int i = 0; i < cardsheld; i++)
		{
			hand[i].transform.position = Camera.main.transform.position + Camera.main.transform.forward  * 6;
			hand[i].transform.position = hand[i].transform.position + Camera.main.transform.right * offset;
			hand[i].transform.position = new Vector3(hand[i].transform.position.x, hand[i].transform.position.y - 5, hand[i].transform.position.z);
			hand[i].transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
			offset = offset + (float)1.4;
		}
		
		
	}

	
	void Update()
	{
		if(PhotonNetwork.offlineMode)
		{
			if(!mManager)
			{
				mManager = GameObject.Find("GameManager").GetComponent<GameManager>(); // thats how you get infromation from the manager
				if(mManager)
				{
					//stat  updates
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
					//stat  updates
				}
			}
		}


		if (choosing) 
		{		
			Debug.Log ("choosing");
			if (Input.GetMouseButtonDown (0)) 
			{
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit)) 
				{ 
					curcard = hit;
				} 
			}
			if (Input.GetMouseButtonDown (1)) 
			{
				choosing = false;
			}
			
		}
		else
		{
			if (combui)
			{

				//if attacker is attacking
				bartotal = attackeratk + defenderdef;
				
				//import bar code from other bar
				
				//if defender is attacking
				bartotal = defenderatk + attackerdef;
			}
			
			Rearrangehand ();

			if (cdel >= 15) 
			{
				showR = true;
			}
			else
				showR = false;
			if (Input.GetKeyDown("space"))
			{
				if (mManager.curAttacking == 0)
				mManager.curAttacking = 2;
				else 
					mManager.curAttacking = 0;
			}
			if (Input.GetKeyDown("m"))
			{
					mManager.curDefending = 2;
				mManager.sPlayers[mManager.curDefending].mDefence = 2;
			
			}
			if(Input.GetMouseButtonDown(0))
			{
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit))			
				{ 
					Debug.Log("clicked it");
					
					if(hit.collider.CompareTag("Card"))
					{
						curcard = hit;
						choosing = true;
					}
				} 
			}
			
		}
		
	}


	void Dispcards()
	{
		
		//CARDS ABOVE CURRENT PLAYER

		if (mManager.curAttacking == 0)
		{
			for (int l = 0; l < 5; l++)
			{
				if (p1c[l] != null)
					p1c[l].SetActive(true);
			}
			
			if (p1c[0] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 150, Screen.height/2 + 300, 6.0f) );
				p1c[0].transform.position = p;
				
				p1c[0].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p1c[0].transform.rotation = Camera.main.transform.rotation;
			}
			if (p1c[1] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 75, Screen.height/2 + 300, 6.0f) );
				p1c[1].transform.position = p;
				
				p1c[1].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p1c[1].transform.rotation = Camera.main.transform.rotation;
			}
			if (p1c[2] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 115, Screen.height/2 + 250, 6.0f) );
				p1c[2].transform.position = p;
				
				p1c[2].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p1c[2].transform.rotation = Camera.main.transform.rotation;
			}
			if (p1c[3] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 150, Screen.height/2 + 175, 6.0f) );
				p1c[3].transform.position = p;
				
				p1c[3].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p1c[3].transform.rotation = Camera.main.transform.rotation;
			}
			if (p1c[4] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 75, Screen.height/2 + 175, 6.0f) );
				p1c[4].transform.position = p;
				
				p1c[4].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p1c[4].transform.rotation = Camera.main.transform.rotation;
			}
		}
		
		else if (mManager.curAttacking == 1)
		{
			for (int l = 0; l < 5; l++)
			{
				if (p2c[l] != null)
					p2c[l].SetActive(true);
			}
			
			if (p2c[0] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 150, Screen.height/2 + 300, 6.0f) );
				p2c[0].transform.position = p;
				
				p2c[0].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p2c[0].transform.rotation = Camera.main.transform.rotation;
			}
			if (p2c[1] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 75, Screen.height/2 + 300, 6.0f) );
				p2c[1].transform.position = p;
				
				p2c[1].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p2c[1].transform.rotation = Camera.main.transform.rotation;
			}
			if (p2c[2] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 115, Screen.height/2 + 250, 6.0f) );
				p2c[2].transform.position = p;
				
				p2c[2].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p2c[2].transform.rotation = Camera.main.transform.rotation;
			}
			if (p2c[3] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 150, Screen.height/2 + 175, 6.0f) );
				p2c[3].transform.position = p;
				
				p2c[3].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p2c[3].transform.rotation = Camera.main.transform.rotation;
			}
			if (p2c[4] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 75, Screen.height/2 + 175, 6.0f) );
				p2c[4].transform.position = p;
				
				p2c[4].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p2c[4].transform.rotation = Camera.main.transform.rotation;
			}
		}
		
		else if (mManager.curAttacking == 2)
		{
			for (int l = 0; l < 5; l++)
			{
				if (p3c[l] != null)
					p3c[l].SetActive(true);
			}
			
			if (p3c[0] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 150, Screen.height/2 + 300, 6.0f) );
				p3c[0].transform.position = p;
				
				p3c[0].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p3c[0].transform.rotation = Camera.main.transform.rotation;
			}
			if (p3c[1] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 75, Screen.height/2 + 300, 6.0f) );
				p3c[1].transform.position = p;
				
				p3c[1].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p3c[1].transform.rotation = Camera.main.transform.rotation;
			}
			if (p3c[2] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 115, Screen.height/2 + 250, 6.0f) );
				p3c[2].transform.position = p;
				
				p3c[2].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p3c[2].transform.rotation = Camera.main.transform.rotation;
			}
			if (p3c[3] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 150, Screen.height/2 + 175, 6.0f) );
				p3c[3].transform.position = p;
				
				p3c[3].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p3c[3].transform.rotation = Camera.main.transform.rotation;
			}
			if (p3c[4] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 75, Screen.height/2 + 175, 6.0f) );
				p3c[4].transform.position = p;
				
				p3c[4].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p3c[4].transform.rotation = Camera.main.transform.rotation;
			}
		}
		
		else if (mManager.curAttacking == 3)
		{
			for (int l = 0; l < 5; l++)
			{
				if (p4c[l] != null)
					p4c[l].SetActive(true);
			}
			
			if (p4c[0] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 150, Screen.height/2 + 300, 6.0f) );
				p4c[0].transform.position = p;
				
				p4c[0].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p4c[0].transform.rotation = Camera.main.transform.rotation;
			}
			if (p4c[1] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 75, Screen.height/2 + 300, 6.0f) );
				p4c[1].transform.position = p;
				
				p4c[1].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p4c[1].transform.rotation = Camera.main.transform.rotation;
			}
			if (p4c[2] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 115, Screen.height/2 + 250, 6.0f) );
				p4c[2].transform.position = p;
				
				p4c[2].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p4c[2].transform.rotation = Camera.main.transform.rotation;
			}
			if (p4c[3] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 150, Screen.height/2 + 175, 6.0f) );
				p4c[3].transform.position = p;
				
				p4c[3].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p4c[3].transform.rotation = Camera.main.transform.rotation;
			}
			if (p4c[4] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(Screen.width - 75, Screen.height/2 + 175, 6.0f) );
				p4c[4].transform.position = p;
				
				p4c[4].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p4c[4].transform.rotation = Camera.main.transform.rotation;
			}
		}
		
		
		//SHOW CARDS ABOVE DEFENDING WINDOW
		if (mManager.curDefending == 0)
		{
			for (int l = 0; l < 5; l++)
			{
				if (p1c[l] != null)
					p1c[l].SetActive(true);
			}
			
			if (p1c[0] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 300, 6.0f) );
				p1c[0].transform.position = p;
				
				p1c[0].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p1c[0].transform.rotation = Camera.main.transform.rotation;
			}
			if (p1c[1] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 300, 6.0f) );
				p1c[1].transform.position = p;
				
				p1c[1].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p1c[1].transform.rotation = Camera.main.transform.rotation;
			}
			if (p1c[2] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 115, Screen.height/2 + 250, 6.0f) );
				p1c[2].transform.position = p;
				
				p1c[2].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p1c[2].transform.rotation = Camera.main.transform.rotation;
			}
			if (p1c[3] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 175, 6.0f) );
				p1c[3].transform.position = p;
				
				p1c[3].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p1c[3].transform.rotation = Camera.main.transform.rotation;
			}
			if (p1c[4] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 175, 6.0f) );
				p1c[4].transform.position = p;
				
				p1c[4].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p1c[4].transform.rotation = Camera.main.transform.rotation;
			}
		}
		else if (mManager.curDefending == 1)
		{
			for (int l = 0; l < 5; l++)
			{
				if (p2c[l] != null)
					p2c[l].SetActive(true);
			}
			
			if (p2c[0] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 300, 6.0f) );
				p2c[0].transform.position = p;
				
				p2c[0].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p2c[0].transform.rotation = Camera.main.transform.rotation;
			}
			if (p2c[1] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 300, 6.0f) );
				p2c[1].transform.position = p;
				
				p2c[1].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p2c[1].transform.rotation = Camera.main.transform.rotation;
			}
			if (p2c[2] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 115, Screen.height/2 + 250, 6.0f) );
				p2c[2].transform.position = p;
				
				p2c[2].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p2c[2].transform.rotation = Camera.main.transform.rotation;
			}
			if (p2c[3] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 175, 6.0f) );
				p2c[3].transform.position = p;
				
				p2c[3].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p2c[3].transform.rotation = Camera.main.transform.rotation;
			}
			if (p2c[4] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 175, 6.0f) );
				p2c[4].transform.position = p;
				
				p2c[4].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p2c[4].transform.rotation = Camera.main.transform.rotation;
			}
		}
		else if (mManager.curDefending == 2)
		{
			for (int l = 0; l < 5; l++)
			{
				if (p3c[l] != null)
					p3c[l].SetActive(true);
			}
			
			if (p3c[0] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 300, 6.0f) );
				p3c[0].transform.position = p;
				
				p3c[0].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p3c[0].transform.rotation = Camera.main.transform.rotation;
			}
			if (p3c[1] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 300, 6.0f) );
				p3c[1].transform.position = p;
				
				p3c[1].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p3c[1].transform.rotation = Camera.main.transform.rotation;
			}
			if (p3c[2] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 115, Screen.height/2 + 250, 6.0f) );
				p3c[2].transform.position = p;
				
				p3c[2].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p3c[2].transform.rotation = Camera.main.transform.rotation;
			}
			if (p3c[3] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 175, 6.0f) );
				p3c[3].transform.position = p;
				
				p3c[3].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p3c[3].transform.rotation = Camera.main.transform.rotation;
			}
			if (p3c[4] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 175, 6.0f) );
				p3c[4].transform.position = p;
				
				p3c[4].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p3c[4].transform.rotation = Camera.main.transform.rotation;
			}
		}
		else if (mManager.curDefending == 3)
		{
			for (int l = 0; l < 5; l++)
			{
				if (p4c[l] != null)
					p4c[l].SetActive(true);
			}
			
			if (p4c[0] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 300, 6.0f) );
				p4c[0].transform.position = p;
				
				p4c[0].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p4c[0].transform.rotation = Camera.main.transform.rotation;
			}
			if (p4c[1] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 300, 6.0f) );
				p4c[1].transform.position = p;
				
				p4c[1].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p4c[1].transform.rotation = Camera.main.transform.rotation;
			}
			if (p4c[2] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 115, Screen.height/2 + 250, 6.0f) );
				p4c[2].transform.position = p;
				
				p4c[2].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p4c[2].transform.rotation = Camera.main.transform.rotation;
			}
			if (p4c[3] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 175, 6.0f) );
				p4c[3].transform.position = p;
				
				p4c[3].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p4c[3].transform.rotation = Camera.main.transform.rotation;
			}
			if (p4c[4] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 175, 6.0f) );
				p4c[4].transform.position = p;
				
				p4c[4].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				p4c[4].transform.rotation = Camera.main.transform.rotation;
			}
		}
		else if (mManager.curAttacking == 4)
		{
			for (int l = 0; l < 5; l++)
			{
				if (t1c[l] != null)
					t1c[l].SetActive(true);
			}
			
			if (t1c[0] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 300, 6.0f) );
				t1c[0].transform.position = p;
				
				t1c[0].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t1c[0].transform.rotation = Camera.main.transform.rotation;
			}
			if (t1c[1] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 300, 6.0f) );
				t1c[1].transform.position = p;
				
				t1c[1].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t1c[1].transform.rotation = Camera.main.transform.rotation;
			}
			if (t1c[2] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 115, Screen.height/2 + 250, 6.0f) );
				t1c[2].transform.position = p;
				
				t1c[2].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t1c[2].transform.rotation = Camera.main.transform.rotation;
			}
			if (t1c[3] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 175, 6.0f) );
				t1c[3].transform.position = p;
				
				t1c[3].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t1c[3].transform.rotation = Camera.main.transform.rotation;
			}
			if (t1c[4] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 175, 6.0f) );
				t1c[4].transform.position = p;
				
				t1c[4].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t1c[4].transform.rotation = Camera.main.transform.rotation;
			}
		}
		else if (mManager.curAttacking == 5)
		{
			for (int l = 0; l < 5; l++)
			{
				if (t2c[l] != null)
					t2c[l].SetActive(true);
			}
			
			if (t2c[0] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 300, 6.0f) );
				t2c[0].transform.position = p;
				
				t2c[0].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t2c[0].transform.rotation = Camera.main.transform.rotation;
			}
			if (t2c[1] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 300, 6.0f) );
				t2c[1].transform.position = p;
				
				t2c[1].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t2c[1].transform.rotation = Camera.main.transform.rotation;
			}
			if (t2c[2] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 115, Screen.height/2 + 250, 6.0f) );
				t2c[2].transform.position = p;
				
				t2c[2].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t2c[2].transform.rotation = Camera.main.transform.rotation;
			}
			if (t2c[3] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 175, 6.0f) );
				t2c[3].transform.position = p;
				
				t2c[3].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t2c[3].transform.rotation = Camera.main.transform.rotation;
			}
			if (t2c[4] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 175, 6.0f) );
				t2c[4].transform.position = p;
				
				t2c[4].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t2c[4].transform.rotation = Camera.main.transform.rotation;
			}
		}
		else if (mManager.curAttacking == 6)
		{
			for (int l = 0; l < 5; l++)
			{
				if (t3c[l] != null)
					t3c[l].SetActive(true);
			}
			
			if (t3c[0] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 300, 6.0f) );
				t3c[0].transform.position = p;
				
				t3c[0].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t3c[0].transform.rotation = Camera.main.transform.rotation;
			}
			if (t3c[1] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 300, 6.0f) );
				t3c[1].transform.position = p;
				
				t3c[1].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t3c[1].transform.rotation = Camera.main.transform.rotation;
			}
			if (t3c[2] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 115, Screen.height/2 + 250, 6.0f) );
				t3c[2].transform.position = p;
				
				t3c[2].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t3c[2].transform.rotation = Camera.main.transform.rotation;
			}
			if (t3c[3] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 175, 6.0f) );
				t3c[3].transform.position = p;
				
				t3c[3].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t3c[3].transform.rotation = Camera.main.transform.rotation;
			}
			if (t3c[4] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 175, 6.0f) );
				t3c[4].transform.position = p;
				
				t3c[4].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t3c[4].transform.rotation = Camera.main.transform.rotation;
			}
		}
		else if (mManager.curAttacking == 7)
		{
			for (int l = 0; l < 5; l++)
			{
				if (t4c[l] != null)
					t4c[l].SetActive(true);
			}
			
			if (t4c[0] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 300, 6.0f) );
				t4c[0].transform.position = p;
				
				t4c[0].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t4c[0].transform.rotation = Camera.main.transform.rotation;
			}
			if (t4c[1] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 300, 6.0f) );
				t4c[1].transform.position = p;
				
				t4c[1].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t4c[1].transform.rotation = Camera.main.transform.rotation;
			}
			if (t4c[2] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 115, Screen.height/2 + 250, 6.0f) );
				t4c[2].transform.position = p;
				
				t4c[2].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t4c[2].transform.rotation = Camera.main.transform.rotation;
			}
			if (t4c[3] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 175, 6.0f) );
				t4c[3].transform.position = p;
				
				t4c[3].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t4c[3].transform.rotation = Camera.main.transform.rotation;
			}
			if (t4c[4] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 175, 6.0f) );
				t4c[4].transform.position = p;
				
				t4c[4].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t4c[4].transform.rotation = Camera.main.transform.rotation;
			}
		}
		else if (mManager.curAttacking == 8)
		{
			for (int l = 0; l < 5; l++)
			{
				if (t5c[l] != null)
					t5c[l].SetActive(true);
			}
			
			if (t5c[0] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 300, 6.0f) );
				t5c[0].transform.position = p;
				
				t5c[0].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t5c[0].transform.rotation = Camera.main.transform.rotation;
			}
			if (t5c[1] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 300, 6.0f) );
				t5c[1].transform.position = p;
				
				t5c[1].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t5c[1].transform.rotation = Camera.main.transform.rotation;
			}
			if (t5c[2] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 115, Screen.height/2 + 250, 6.0f) );
				t5c[2].transform.position = p;
				
				t5c[2].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t5c[2].transform.rotation = Camera.main.transform.rotation;
			}
			if (t5c[3] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 75, Screen.height/2 + 175, 6.0f) );
				t5c[3].transform.position = p;
				
				t5c[3].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t5c[3].transform.rotation = Camera.main.transform.rotation;
			}
			if (t5c[4] != null)
			{
				Vector3 p = Camera.main.ScreenToWorldPoint( new Vector3(0 + 150, Screen.height/2 + 175, 6.0f) );
				t5c[4].transform.position = p;
				
				t5c[4].transform.localScale = new Vector3(0.1f,0.1f,0.1f);
				t5c[4].transform.rotation = Camera.main.transform.rotation;
			}
		}
	}

	void Attack()
	{
		int tempatk, tempdef, taratk, tardef;
		tempatk = mManager.sPlayers[mManager.curAttacking].mAttack;
		tempdef = mManager.sPlayers[mManager.curAttacking].mDefence;
		taratk = mManager.sPlayers[mManager.curDefending].mAttack;
		tardef = mManager.sPlayers[mManager.curDefending].mDefence;
		//When cards are defined do card stuff
		if (mManager.curAttacking == 0)
		{
			
			for (int h = 0; h < 5; h++)
			{
				Destroy(p1c[h]);
			}
		}
		else if (mManager.curAttacking == 1)
		{
			
			for (int h = 0; h < 5; h++)
			{
				Destroy(p2c[h]);
			}
		}
		else if (mManager.curAttacking == 2)
		{
			
			for (int h = 0; h < 5; h++)
			{
				Destroy(p3c[h]);
			}
		}
		else if (mManager.curAttacking == 3)
		{
			
			for (int h = 0; h < 5; h++)
			{
				Destroy(p4c[h]);
			}
		}
		
		if (mManager.curDefending == 0)
		{
			
			for (int h = 0; h < 5; h++)
			{
				Destroy(p1c[h]);
			}
		}
		else if (mManager.curDefending == 1)
		{
			
			for (int h = 0; h < 5; h++)
			{
				Destroy(p2c[h]);
			}
		}
		else if (mManager.curDefending == 2)
		{
			
			for (int h = 0; h < 5; h++)
			{
				Destroy(p3c[h]);
			}
		}
		else if (mManager.curDefending == 3)
		{
			
			for (int h = 0; h < 5; h++)
			{
				Destroy(p4c[h]);
			}
		}
		else if (mManager.curDefending == 4)
		{
			
			for (int h = 0; h < 5; h++)
			{
				Destroy(t1c[h]);
			}
		}
		else if (mManager.curDefending == 5)
		{
			
			for (int h = 0; h < 5; h++)
			{
				Destroy(t2c[h]);
			}
		}
		else if (mManager.curDefending == 6)
		{
			
			for (int h = 0; h < 5; h++)
			{
				Destroy(t3c[h]);
			}
		}
		else if (mManager.curDefending == 7)
		{
			
			for (int h = 0; h < 5; h++)
			{
				Destroy(t4c[h]);
			}
		}
		else if (mManager.curDefending == 8)
		{
			
			for (int h = 0; h < 5; h++)
			{
				Destroy(t5c[h]);
			}
		}
		
		if (tempatk > tardef)
		{
			mManager.sPlayers[mManager.curDefending].gameObject.renderer.enabled = false; 
			//Kill target.
		}
		else if (taratk > tempdef)
		{
			mManager.sPlayers[mManager.curAttacking].gameObject.renderer.enabled = false; 
			//Kill player.
		}
		else
		{
			//Do nothing.
		}
		
	}
	
}