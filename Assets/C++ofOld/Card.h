#ifndef CARD_H
#define CARD_H

#include "SGE.h"
#include "StatusEffects.h"
using namespace SGE;

class Card
{
public:
	Card();
	~Card();

	virtual void Load(const char* cardinfo);
	virtual void Unload();
	virtual bool Update(float Update);
	virtual void Render(SVector2 position);
	
 	SRect GetBoundingBox() const	{return SRect(myPosition.x + 90, myPosition.y + 150, myPosition.x + 220, myPosition.y + 325);}
	void DealDamage(Card* Attacked);
	void UseEffect();
	
public:
	int mCostToPlay;
	int mAmmountOfDamage;  
	int mAmmountOfHealth;
	StatusEffect mStatusEffect;
private:
	bool LoadCard(const char* cardinfo);
private:
	SVector2 myPosition;
	SGE_Sprite mVisual;
	SGE_Font mHealth;
};
#endif