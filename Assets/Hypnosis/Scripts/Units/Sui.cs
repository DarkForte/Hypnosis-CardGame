using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Sui : GenericUnit
{
    const string specialMsg = "{0} attacked all units around him!";

    public override void InitializeMoveAndAttack()
    {
        Vector2[] dirs = { new Vector2(0, 2), new Vector2(0, -2), new Vector2(2, 0), new Vector2(-2, 0),
            new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, -1), new Vector2(-1, 1)};

        Moves = new List<Vector2>(dirs);
        AttackMoves = CommonMovement.dir4;
    }

    public override void PerformSpecialMove(GameController gameController, List<Vector2> targetSeq)
    {
        Vector2[] dirs = {new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(1, -1),
            new Vector2(0, -1), new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(-1, 1)};

        foreach(var dir in dirs)
        {
            Vector2 next;
            if (isFriendUnit)
                next = Cell.OffsetCoord + dir;
            else
                next = Cell.OffsetCoord - dir;

            if(gameController.CellMap.ContainsKey(next) && gameController.CellMap[next].IsTaken)
            {
                DealDamage(gameController.CellMap[next].OccupyingUnit, gameController, false);
            }
        }

        SpecialUsed = true;
        gameController.logger.LogSpecial(this, String.Format(specialMsg, UnitName));
        gameController.EndTurn();
    }

    public override SpecialState GetSpecialState(GameController gameController)
    {
        return new SpecialStateSui(gameController, this);
    }
}
