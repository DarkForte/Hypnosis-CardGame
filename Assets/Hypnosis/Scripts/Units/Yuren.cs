using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Yuren : GenericUnit
{
    public override void InitializeMoveAndAttack()
    {
        Moves = CommonMovement.dir4;
        foreach(var attack in CommonMovement.front3)
        {
            if (PlayerNumber == 0)
                AttackMoves.Add(attack);
            else
                AttackMoves.Add(-attack);
        }
    }

    public override void SpecialMove(GameController gameController)
    {
        gameController.GameState = new SpecialStateYuren(gameController, this);
    }
}

