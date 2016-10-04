using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Pawn : GenericUnit
{
    public override void Initialize()
    {
        base.Initialize();
        SpecialUsed = true;
    }

    public override void InitializeMoveAndAttack()
    {
        Moves.Add(new Vector2(1, 0));
        Moves.Add(new Vector2(-1, 0));
        if (PlayerNumber == 0)
            Moves.Add(new Vector2(0, 1));
        else
            Moves.Add(new Vector2(0, -1));

        AttackMoves = CommonMovement.dir4;
    }

    public override void SpecialMove(GameController gameController)
    {
        
    }
}