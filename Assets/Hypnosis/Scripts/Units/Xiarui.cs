using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Xiarui : GenericUnit
{
    public override void InitializeMoveAndAttack()
    {
        Moves = CommonMovement.dir4;
        AttackMoves = CommonMovement.dir4;
    }

    public override void PerformSpecialMove(GameController gameController, List<Vector2> targetSeq)
    {
        var cellMap = gameController.CellMap;
        Unit victim = cellMap[targetSeq[0]].OccupyingUnit;

        victim.AddBuff(new Invincible(1));
        SpecialUsed = true;

        gameController.logger.LogSpecial(this, victim.UnitName + " became invincible for the round!");
        gameController.EndTurn();
    }

    public override void SpecialMove(GameController gameController)
    {
        gameController.GameState = new SpecialStateXiarui(gameController, this);
    }
}
