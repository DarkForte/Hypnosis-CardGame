using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Chong : GenericUnit
{
    const string specialMsg = "{0} shooted!";
    public override void InitializeMoveAndAttack()
    {
        Moves = CommonMovement.dir4;
        AttackMoves = new List<Vector2>();
        if (isFriendUnit)
            AttackMoves.Add(new Vector2(0, 1));
        else
            AttackMoves.Add(new Vector2(0, -1));
    }

    public override List<Unit> GetEnemiesInRange(Dictionary<Vector2, Cell> cellMap)
    {
        List<Unit> ret = new List<Unit>();
        Vector2 now = Cell.OffsetCoord;
        while (true)
        {
            now = now + AttackMoves[0];
            if (!cellMap.ContainsKey(now))
                break;
            else if(cellMap[now].IsTaken)
            {
                if(cellMap[now].OccupyingUnit.PlayerNumber != PlayerNumber)
                {
                    ret.Add(cellMap[now].OccupyingUnit);
                }
                break;
            }
        }
        return ret;
    }

    public override void PerformSpecialMove(GameController gameController, List<Vector2> targetSeq)
    {
        Vector2 dir = targetSeq[0] - Cell.OffsetCoord;
        Vector2 now = Cell.OffsetCoord + dir;
        while (gameController.CellMap.ContainsKey(now))
        {
            if (gameController.CellMap[now].IsTaken)
            {
                Unit unit = gameController.CellMap[now].OccupyingUnit;
                if (unit.PlayerNumber != PlayerNumber && !(unit is Base))
                {
                    DealDamage(unit, gameController, false);
                }
            }
            now += dir;
        }

        SpecialUsed = true;
        logger.LogSpecial(this, String.Format(specialMsg, UnitName));
        gameController.EndTurn();
    }

    public override void SpecialMove(GameController gameController)
    {
        gameController.GameState = new SpecialStateChong(gameController, this);
    }
}

