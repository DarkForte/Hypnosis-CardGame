using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class GameStateRoundStart : GameState
{
    public GameStateRoundStart(GameController gameController) : base(gameController)
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        int i;
        for(i=1; i<=_gameController.NumberOfPlayers; i++)
        {
            List<CardType> nowCandidates = _gameController.Players[i].DrawCards(5);

            List<CardType> nowCards = new List<CardType>();
            for(int j=1; j<=3; j++)
            {
                nowCards.Add(nowCandidates[j]);
            }

            _gameController.Players[i].NowCards = nowCards;

            _gameController.GameState = new GameStateTurnChanging(_gameController);
        }
    }
}

