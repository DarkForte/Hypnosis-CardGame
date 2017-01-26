using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Zishu : GenericUnit
{
    const string specialMsg = "{0} turned {1} into friend unit!";
    public override SpecialState GetSpecialState(GameController gameController)
    {
        return new SpecialStateZishu(gameController, this);
    }

    public override void InitializeMoveAndAttack()
    {
        Moves = CommonMovement.dir8;
        AttackMoves = CommonMovement.dir4;
    }

    public override void PerformSpecialMove(GameController gameController, List<Vector2> targetSeq)
    {
        Unit target = gameController.CellMap[targetSeq[0]].OccupyingUnit;
        target.PlayerNumber = PlayerNumber;
        target.isFriendUnit = (PlayerNumber == gameController.LocalPlayer.PlayerNumber);
        target.InitializeHealthBar(target.isFriendUnit);
        target.HP = 1;

        logger.LogSpecial(this, String.Format(specialMsg, UnitName, target.UnitName));
        SpecialUsed = true;
        gameController.EndTurn();
    }

    public override void DealDamage(Unit target, GameController gameController, bool log = true)
    {
        base.DealDamage(target, gameController, log);

        Vector2 dir = target.Cell.OffsetCoord - Cell.OffsetCoord;
        Vector2 nextCell = target.Cell.OffsetCoord + dir;
        if (gameController.CellMap.ContainsKey(nextCell))
        {
            Unit anotherTarget = gameController.CellMap[nextCell].OccupyingUnit;
            if(anotherTarget != null && anotherTarget.isFriendUnit == false)
            {
                DealDamage(anotherTarget, gameController, log);
            }
        }
    }
}
