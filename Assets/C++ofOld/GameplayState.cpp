#include "GameplayState.h"

GameplayState::GameplayState(GameContext& gc)
    :AppState(gc)
    ,mPlayerDeckPosition(1000,750)
    ,mOpponentDeckPosition(1000,200)
    ,mPlayerHandPosition(20,700)
    ,mOpponentHandPosition(20,-100)
    ,mOpponentBoardPosition(20, 100)
    ,mPlayerBoardPosition(20, 500)
    ,mOffset(150,0)
    ,cardFoundPlayable(false)
    ,cardFoundAttack(false)
{
    int r = rand() % 2;
    if (r < 1)
    {
        mPlayer1Turn = true;
    }
    else
    {
        mPlayer1Turn = false;
    }
}

GameplayState::~GameplayState()
{

}

void GameplayState::Load()
{
    Card* Player = new Card();
    Card* Opponent = new Card();

    Player->Load("Player.txt");
    Opponent->Load("Opponent.txt");

    mPlayerBoard.push_back(Player);
    mOpponentBoard.push_back(Opponent);
    
    mMana.Load(122, true);
    mMana.SetColor(0, 200, 255);

    mCursor.Load("sword.png");
    mDeck.Load("Deck01.txt");
    mOpponentDeck.Load("Deck01.txt");

    mDeck.Shuffle();
    mOpponentDeck.Shuffle();
    
    mEndTurn.Load("button_on.png","button_off.png");
    mEndTurn.SetPosition(800.0f,500.0f);
    mEndTurn.SetText("End Turn", 255, 255,255);

    for (int i = 0; i < 3; ++i)
    {
        mPlayerHand.push_back(mDeck.DrawCard());
        mOpponentHand.push_back(mOpponentDeck.DrawCard());
    }
    if (!mPlayer1Turn)
    {
        mPlayerHand.push_back(mDeck.DrawCard());
        mOpponentMana = 1;
        mPlayerMana = 0;
        mRemainingMana = mOpponentMana;
    }
    else
    {
        mOpponentHand.push_back(mOpponentDeck.DrawCard());
        mPlayerMana = 1;
        mOpponentMana = 0;
        mRemainingMana = mPlayerMana;
    }
    //Background.Load("Background.jpg");
}

void GameplayState::Unload()
{

}

GameStates GameplayState::Update(float deltatime)
{
    mCursor.Update(deltatime);
    //Background.Update(deltatime);
    mDeck.Update(deltatime);
    mOpponentDeck.Update(deltatime);
    mEndTurn.Update(deltatime);
    GameStates nextstate = GameStates::Invalid;
    

    CleanBoard();
    nextstate = CheckInput();
    return nextstate;
}

void GameplayState::Render()
{
    //Background.Render();
    CheckWin();
    string text = to_string(mRemainingMana);
    
    mDeck.Render(mPlayerDeckPosition);
    mOpponentDeck.Render(mOpponentDeckPosition);
    int count = 0;

    for (auto it = mPlayerHand.begin(); it != mPlayerHand.end(); ++it)
    {
        Graphics_DebugRect( (*it)->GetBoundingBox() );
        (*it)->Render(mPlayerHandPosition + (mOffset * count++));
    }
    count = 0;
    for (auto it = mOpponentHand.begin(); it != mOpponentHand.end(); ++it)
    {
        Graphics_DebugRect( (*it)->GetBoundingBox() );
        (*it)->Render(mOpponentHandPosition + (mOffset * count++));
    }
    count = 0;
    for (auto it = mPlayerBoard.begin(); it != mPlayerBoard.end(); ++it)
    {
        Graphics_DebugRect( (*it)->GetBoundingBox() );
        (*it)->Render(mPlayerBoardPosition + (mOffset * count++));
        if( (*it)->mStatusEffect == 1 || (*it)->mStatusEffect == 2 )
        {
            (*it)->mStatusEffect == 0;
        }
    }
    count = 0;
    for (auto it = mOpponentBoard.begin(); it != mOpponentBoard.end(); ++it)
    {
        Graphics_DebugRect( (*it)->GetBoundingBox() );
        (*it)->Render(mOpponentBoardPosition + (mOffset * count++));
    }
    mEndTurn.Render();
    mMana.Print(text.c_str() ,700, 100);

    mCursor.Render();
}

void GameplayState::CleanBoard()
{
    if(!mPlayerBoard.empty())
    {
        vector<Card*>::iterator deadCards[7];
        int counter = 0;
        for (auto it = mPlayerBoard.begin(); it != mPlayerBoard.end(); ++it)
        {
            if((*it)->mAmmountOfHealth <= 0)
            {
                deadCards[counter++] = it;
            }
        }
        for( int i = 0; i < counter;++i)
        {
            mPlayerBoard.erase(deadCards[i]);
        }
    }
    if(!mOpponentBoard.empty())
    {
        vector<Card*>::iterator deadCards[7];
        int counter = 0;
        for (auto it = mOpponentBoard.begin(); it != mOpponentBoard.end(); ++it)
        {
            if((*it)->mAmmountOfHealth <= 0)
            {
                deadCards[counter++] = it;
            }
        }
        for( int i = 0; i < counter;++i)
        {
            mOpponentBoard.erase(deadCards[i]);
        }
    }
}

GameStates GameplayState::CheckInput()
{
    GameStates nextstate = GameStates::Invalid;

    vector<Card*>::iterator playedCard;

    if(Input_IsMousePressed(Mouse::LBUTTON))
    {
        if(mPlayer1Turn)
        {			
            for (auto it = mPlayerHand.begin(); it != mPlayerHand.end(); ++it)
            {
                if ( PointInRect(mCursor.GetCursorPos(),(*it)->GetBoundingBox()))
                {
                    playedCard = it;
                    cardFoundPlayable = true;
                }
            }
            if(cardFoundPlayable)
            {
                if((*playedCard)->mCostToPlay <= mRemainingMana)
                {
                    mRemainingMana -= (*playedCard)->mCostToPlay;
                    if((*playedCard)->mAmmountOfHealth == 0)
                    {
                        (*playedCard)->DealDamage(mOpponentBoard[0]);
                    }
                    mPlayerBoard.push_back(*playedCard);
                    mPlayerHand.erase(playedCard);
                }
                cardFoundPlayable = false;
            }
            for (auto it = mPlayerBoard.begin(); it != mPlayerBoard.end(); ++it)
            {
                if ( PointInRect(mCursor.GetCursorPos(),(*it)->GetBoundingBox()))
                {
                    attackCard = it;
                    cardFoundAttack = true;
                }
            }
            if(cardFoundAttack)
            {
                for (auto it = mOpponentBoard.begin(); it != mOpponentBoard.end(); ++it)
                {
                    if ( PointInRect(mCursor.GetCursorPos(),(*it)->GetBoundingBox()))
                    {
                        attackedCard = it;
                        cardFoundAttack = false;
                        Damage();
                    }
                }
                
            }
        }
        else //if(!mPlayer1Turn)
        {
            for (auto it = mOpponentHand.begin(); it != mOpponentHand.end(); ++it)
            {
                if ( PointInRect(mCursor.GetCursorPos(),(*it)->GetBoundingBox()))
                {
                    playedCard = it;
                    cardFoundPlayable = true;
                }
            }
            if(cardFoundPlayable)
            {
                if((*playedCard)->mCostToPlay <= mRemainingMana)
                {
                    mRemainingMana -= (*playedCard)->mCostToPlay;
                    if((*playedCard)->mAmmountOfHealth == 0)
                    {
                        (*playedCard)->DealDamage(mPlayerBoard[0]);
                    }
                    mOpponentBoard.push_back((*playedCard));
                    mOpponentHand.erase(playedCard);
                }
                cardFoundPlayable = false;
            }
            for (auto it = mOpponentBoard.begin(); it != mOpponentBoard.end(); ++it)
            {
                if ( PointInRect(mCursor.GetCursorPos(),(*it)->GetBoundingBox()))
                {
                    attackCard = it;
                    cardFoundAttack = true;
                }
            }
            if(cardFoundAttack)
            {
                for (auto it = mPlayerBoard.begin(); it != mPlayerBoard.end(); ++it)
                {
                    if ( PointInRect(mCursor.GetCursorPos(),(*it)->GetBoundingBox()))
                    {
                        attackedCard = it;
                        cardFoundAttack = false;
                        Damage();
                    }
                }
            }
        }	
    }
    EndTurn();
    if (Input_IsKeyPressed(Keys::ESCAPE))
    {
        nextstate = GameStates::Frontend;
    }
    return nextstate;
}

void GameplayState::Damage()
{
    if((*attackCard)->mStatusEffect != Asleep)
    {
        (*attackCard)->DealDamage((*attackedCard));
        (*attackCard)->mStatusEffect = Asleep;
    }
}

void GameplayState::EndTurn()
{
    if(mEndTurn.IsPressed())
    {
        if(mPlayer1Turn)
        {
            mOpponentHand.push_back(mOpponentDeck.DrawCard());
            for(auto it : mOpponentBoard)
            {
                if(it->mStatusEffect == Asleep || it->mStatusEffect == Freeze)
                {
                    it->mStatusEffect = None;
                }
            }
            if(mOpponentMana != 10)
            {
                ++mOpponentMana;
            }
            mRemainingMana = mOpponentMana;
        }
        else
        {
            mPlayerHand.push_back(mDeck.DrawCard());
            for(auto it : mPlayerBoard)
            {
                if(it->mStatusEffect == Asleep)
                {
                    it->mStatusEffect = None;
                }
            }
            if(mPlayerMana != 10)
            {
                ++mPlayerMana;
            }
            mRemainingMana = mPlayerMana;
        }
        mPlayer1Turn = !mPlayer1Turn;
        cardFoundPlayable = false;
        cardFoundAttack = false;
    }
}
void GameplayState::CheckWin()
{
    if(mPlayer1Turn)
    {
        if(mOpponentBoard.empty() || mOpponentBoard[0]->mAmmountOfHealth <= 0)
        {
            mWinText.Print("Player 1 Wins", 500, 500);
        }
    }
    else
    {
        if(mPlayerBoard.empty() || mPlayerBoard[0]->mAmmountOfHealth <= 0)
        {
            mWinText.Print("Player 2 Wins", 500, 500);
        }
    }
}
/*
enum State
{
    DrawCard,
    SelectCard,
    Action,
    EndTurn
};
void GameplayState::Update()
{
    switch (mState)
    {
    case DrawCard:
        UpdateDrawCard();
        break;
    case SelectCard:
        UpdateSelectCard();
        break;
    case Action:
        UpdateAction();
        break;
    case EndTurn:
        UpdateEndTurn();
        break;
    }
}
*/














