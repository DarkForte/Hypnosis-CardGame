using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Chushui : GenericUnit
{
    const string specialMsg = "{0} controlled {1} to attack {2}!";

    public override void InitializeMoveAndAttack()
    {
        Moves = CommonMovement.dir8;
        AttackMoves = CommonMovement.dir4;
    }

    public override void SpecialMove(GameController gameController)
    {
        gameController.GameState = new SpecialStateChushui(gameController, this);
    }

    public override void PerformSpecialMove(GameController gameController, List<Vector2> targetSeq)
    {
        Vector2 controlledPos = targetSeq[0];
        Unit controlledUnit = gameController.CellMap[controlledPos].OccupyingUnit;
        Vector2 targetPos = targetSeq[1];
        Unit targetUnit = gameController.CellMap[targetPos].OccupyingUnit;

        controlledUnit.DealDamage(targetUnit, gameController, log:false);

        SpecialUsed = true;
        gameController.logger.LogSpecial(this, String.Format(specialMsg, this.UnitName, controlledUnit.UnitName, targetUnit.UnitName));
        gameController.EndTurn();
    }
}

