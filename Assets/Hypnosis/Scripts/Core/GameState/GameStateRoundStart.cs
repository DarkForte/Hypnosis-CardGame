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
        _gameController.CardInterface.SetActive(true);


    }

    public override void OnStateExit()
    {
        base.OnStateExit();
        _gameController.CardInterface.SetActive(false);
    }
}

