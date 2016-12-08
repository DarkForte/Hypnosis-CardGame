using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Jizi : GenericUnit
{
    public override void InitializeMoveAndAttack()
    {
        Moves = CommonMovement.dir8;
        AttackMoves = CommonMovement.dir8;
    }

    public override void PerformSpecialMove(GameController gameController, List<Vector2> targetSeq)
    {
        var cellMap = gameController.CellMap;
        Unit unit = cellMap[targetSeq[0]].OccupyingUnit;

        unit.AddBuff(new FirstTargetLocked(1));
        SpecialUsed = true;
        gameController.EndTurn();
    }

    public override void SpecialMove(GameController gameController)
    {
        gameController.GameState = new SpecialStateJizi(gameController, this);
    }
}

