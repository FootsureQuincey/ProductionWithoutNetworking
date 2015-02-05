#ifndef INCLUDED_GAMEPLAYSTATE_H
#define INCLUDED_GAMEPLAYSTATE_H

#include "AppState.h"
#include "CardDeck.h"
class GameplayState : public AppState
{
public:
	GameplayState(GameContext& gc);
	virtual ~GameplayState();

	virtual void Load();
	virtual void Unload();
	virtual GameStates Update(float deltatime);
	virtual void Render();

	void CleanBoard();
	GameStates CheckInput();
	void Damage();
	void EndTurn();
	void CheckWin();

private:
	CardDeck mDeck;
	CardDeck mOpponentDeck;
	SGE_Cursor mCursor;
	//SGE_Sprite Background;
	SVector2 mPlayerDeckPosition;
	SVector2 mOpponentDeckPosition;
	SVector2 mPlayerHandPosition;
	SVector2 mOpponentHandPosition;
	SVector2 mPlayerBoardPosition;
	SVector2 mOpponentBoardPosition;
	vector<Card *> mPlayerHand;
	vector<Card *> mOpponentHand;
	vector<Card *> mPlayerBoard;
	vector<Card *> mOpponentBoard;
	vector<Card*>::iterator attackCard;
	vector<Card*>::iterator attackedCard;
	SVector2 mOffset;
	SGE_Button mEndTurn;
	SGE_Font mMana;
	SGE_Font mWinText;
	int mPlayerMana;
	int mPlayerHealth;
	int mOpponentHealth;
	int mOpponentMana;
	int mRemainingMana;
	bool mPlayer1Turn;
	bool cardFoundPlayable;
	bool cardFoundAttack;
};

#endif //#ifndef INCLUDED_TILE_H