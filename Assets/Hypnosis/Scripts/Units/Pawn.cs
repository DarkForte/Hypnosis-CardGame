using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Pawn : GenericUnit
{
    public override void InitializeMoveAndAttack()
    {
        foreach(var m in CommonMovement.front3)
        {
            if (PlayerNumber == 0)
                Moves.Add(m);
            else
                Moves.Add(-m);
        }

        AttackMoves = CommonMovement.dir4;
    }

    public override void SpecialMove(GameController gameController)
    {
        
    }
}