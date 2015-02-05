#include "Card.h"
#include <string>

Card::Card()
{
}
Card::~Card()
{
	mVisual.Unload();
}

void Card::Load(const char* cardinfo)
{
	mHealth.Load(25);
	if(!LoadCard(cardinfo))
	{
		LogError("Failed to load Card file %s", cardinfo);
		Unload();
	}
}

void Card::Unload()
{
	mVisual.Unload();
}

bool Card::Update(float Update)
{
	return true;
}
void Card::Render(SVector2 position)
{
	std::string text = std::to_string(mAmmountOfHealth);
	myPosition = position;
	mVisual.SetPosition(myPosition);
	mVisual.Render();
	mHealth.SetColor(255,0,0);
	mHealth.Print( text.c_str() ,myPosition.x + mVisual.GetWidth() / 2 ,myPosition.y + mVisual.GetHeight() / 2);
}

void Card::DealDamage(Card* Attacked)
{
	mAmmountOfHealth -= Attacked->mAmmountOfDamage;
	Attacked->mAmmountOfHealth -= mAmmountOfDamage;
}
void Card::UseEffect()
{

}

bool Card::LoadCard(const char* cardinfo)
{
	FILE* pFile = nullptr;
	
	//fopen_s(&pFile, cardinfo, "r" ); 
	pFile = fopen(cardinfo, "rb" ); 
	if (pFile == nullptr)
	{
		return false;
	}

	int scanned = 0;
	/*char *pkey;
	char *texname;
	char buf[1024];
	fgets(buf,1024,pFile);
	pkey = strtok(buf," ");
	texname = strtok(nullptr,"\r\n");
	mVisual.Load(texname);
*/

	//scanned += fscanf_s(pFile,"%*s %s", &mVisual);

	char buf[8196] = {0};
	scanned = fscanf_s(pFile,"%*s %s", buf, 8196);
	mVisual.Load(buf);
	mVisual.SetScale(SVector2(0.5f,0.5f), true);

	scanned += fscanf_s(pFile,"%*s %d", &mCostToPlay);
	scanned += fscanf_s(pFile,"%*s %d", &mAmmountOfDamage);
	scanned += fscanf_s(pFile,"%*s %d", &mAmmountOfHealth);
	scanned += fscanf_s(pFile,"%*s %d", &mStatusEffect);


	if(scanned < 5)
	{
		fclose(pFile);
		return false;
	}
	
	fclose(pFile);
	return true;
}
