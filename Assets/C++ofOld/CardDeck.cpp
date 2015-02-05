#include "CardDeck.h"
#include <iostream>
#include <fstream>
#include <sstream>

CardDeck::CardDeck()
{
	for( int i = 0; i < 30; ++i)
		mCards.push_back(new Card());
}

CardDeck::~CardDeck()
{
}

void CardDeck::Load(const char* deckInfo)
{
	if(!LoadDeck(deckInfo))
	{
		LogError("Failed to load Deck file %s", deckInfo);
		Unload();
		return;
	}
	mCardBack.Load("cardback.jpg");
}

void CardDeck::Unload()
{
	for (int i = 0; i < 30; ++i)
	{
		mCards[i]->Unload();
	}
	mCardBack.Unload();
}

void CardDeck::Update(float deltatime)
{

}
void CardDeck::Render(SVector2 renderlocation)
{
	mCardBack.SetPosition(renderlocation);
	mCardBack.Render();
}

void CardDeck::Shuffle()
{
	for(int i = 0; i < mCards.size(); ++i)
	{
		int Random = rand() % 30;
		Card* temp = mCards[Random];
		mCards[Random] = mCards[i];
		mCards[i] = temp;
	}
}


Card* CardDeck::DrawCard()
{
	Card* nextCard = nullptr;
	if(!mCards.empty())
	{
		nextCard = mCards.back();
		mCards.pop_back();
	}
	return nextCard;
}

bool CardDeck::isEmpty()
{
	return mCards.empty();
}

bool CardDeck::LoadDeck(const char* deckInfo)
{
	//FILE* pFile = nullptr;
	int count = 0;
	std::ifstream iifs(deckInfo, std::ios::in);

	if(!iifs.is_open())
	{
		return false;
	}

	//pFile = fopen( deckInfo, "r" );

	/*if (pFile == nullptr)
	{
		return false;
	}*/

	std::string buffer;
	while ( getline( iifs, buffer ) )
	{
		std::string card, temp;
		stringstream ss( buffer );
		if ( getline( ss, temp, ':' ) )
		{
			if ( getline( ss, card, ':' ) )
			{
				mCards[count]->Load(card.c_str());
				count++;
			}
		}
	}

	/*for(int i = 0; i < 30;++i)
	{
		int scanned = 0;
		char* CardToLoad;
		scanned += fscanf_s(pFile,"%*s %s", &CardToLoad);
		mCards[i]->Load(CardToLoad);
	}*/

	return true;
}