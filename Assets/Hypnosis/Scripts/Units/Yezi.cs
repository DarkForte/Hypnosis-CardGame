using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Yezi : GenericUnit
{
    public override void InitializeMoveAndAttack()
    {
        Moves = CommonMovement.dir4;
        Vector2[] attackMoves = { new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, 1), new Vector2(-1, -1) };
        AttackMoves.AddRange(attackMoves);
    }

    public override void PerformSpecialMove(GameController gameController, List<Vector2> targetSeq)
    {
        Unit target = gameController.CellMap[targetSeq[0]].OccupyingUnit;
        target.HP = target.MaxHP;

        SpecialUsed = true;
        logger.LogSpecial(this, target.UnitName + "'s HP is recovered!");
        gameController.EndTurn();
    }

    public override void SpecialMove(GameController gameController)
    {
        gameController.GameState = new SpecialStateYezi(gameController, this);
    }
}

