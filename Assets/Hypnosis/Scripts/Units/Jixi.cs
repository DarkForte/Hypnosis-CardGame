using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Jixi : GenericUnit
{
    const string specialMsg = "{0} threw {1} away!";

    public override void InitializeMoveAndAttack()
    {
        Moves = CommonMovement.dir4;
        AttackMoves = CommonMovement.dir4;
    }

    public override List<Cell> GetAvailableDestinations(Dictionary<Vector2, Cell> cellMap)
    {
        List<Cell> ret = new List<Cell>();
        List<Vector2> dirs = CommonMovement.dir4;
        foreach(Vector2 dir in dirs)
        {
            Vector2 now = Cell.OffsetCoord;
            while(true)
            {
                Vector2 next = now + dir;
                if(!cellMap.ContainsKey(next) || cellMap[next].IsTaken)
                {
                    break;
                }
                now = next;
            }
            if(now != Cell.OffsetCoord)
            {
                ret.Add(cellMap[now]);
            }
        }
        return ret;
    }

    public override void OnMoveFinished(List<Cell> path, GameController gameController)
    {
        AddBuff(new FirstTargetExcluded(1));
    }

    public override void PerformSpecialMove(GameController gameController, List<Vector2> targetSeq)
    {
        var cellMap = gameController.CellMap;
        Unit unit = cellMap[targetSeq[0]].OccupyingUnit;
        Cell destCell = cellMap[targetSeq[1]];

        Vector2 dir = (destCell.OffsetCoord - Cell.OffsetCoord).normalized;
        List<Cell> path = new List<Cell>();
        path.Add(unit.Cell);

        Vector2 dest = Cell.OffsetCoord;
        while (true)
        {
            path.Add(cellMap[dest]);
            if (dest == destCell.OffsetCoord)
                break;

            Vector2 next = dest + dir;
            dest = next;
        }

        path.Reverse();

        float oringialSpeed = unit.MovementSpeed;
        unit.MovementSpeed = 5;
        unit.Move(cellMap[dest], path, gameController, log: false);
        unit.MovementSpeed = oringialSpeed;

        SpecialUsed = true;
        logger.LogSpecial(this, String.Format(specialMsg, this.UnitName, unit.UnitName));
        gameController.EndTurn();
    }

    public override SpecialState GetSpecialState(GameController gameController)
    {
        return new SpecialStateJixi(gameController, this);
    }
}

