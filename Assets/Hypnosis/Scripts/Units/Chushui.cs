using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Chushui : GenericUnit
{
    public override void InitializeMoveAndAttack()
    {
        Moves = CommonMovement.dir8;
        AttackMoves = CommonMovement.dir4;
    }

    public override void PerformSpecialMove(GameController gameController, List<Vector2> targetSeq)
    {
        throw new NotImplementedException();
    }

    public override void SpecialMove(GameController gameController)
    {
        gameController.GameState = new SpecialStateChushui(gameController, this);
    }
}

