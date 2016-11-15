using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class GameStateRoundStart : GameState
{
    public List<bool> cardReady = new List<bool>(1);

    public GameStateRoundStart(GameController gameController) : base(gameController)
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
    }



    public override void OnStateExit()
    {
        base.OnStateExit();
    }
}

