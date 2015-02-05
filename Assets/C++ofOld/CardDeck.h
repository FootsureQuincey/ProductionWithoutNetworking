#include "Card.h"
#include <vector>

using namespace std;

class CardDeck
{
public:
	CardDeck();
	~CardDeck();

	void Load(const char* deckInfo);
	void Unload();
	void Update(float deltatime);
	void Render(SVector2 renderlocation);

	void Shuffle();
	Card* DrawCard();

	bool isEmpty();
private:
	bool LoadDeck(const char* deckInfo);
private:
	vector< Card* > mCards;
	SGE_Sprite mCardBack;
};