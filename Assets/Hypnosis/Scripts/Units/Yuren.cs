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
            if (isFriendUnit)
                AttackMoves.Add(attack);
            else
                AttackMoves.Add(-attack);
        }
    }

    public override void PerformSpecialMove(GameController gameController, List<Vector2> targetSeq)
    {
        var cellMap = gameController.CellMap;
        Unit victim = cellMap[targetSeq[0]].OccupyingUnit;

        AttackPower = 10;
        DealDamage(victim, log:false);
        AttackPower = 2;
        SpecialUsed = true;

        gameController.logger.LogSpecial(this, this.UnitName + " strongly attacked " + victim.UnitName + "!");
        gameController.EndTurn();
    }

    public override void SpecialMove(GameController gameController)
    {
        gameController.GameState = new SpecialStateYuren(gameController, this);
    }
}

