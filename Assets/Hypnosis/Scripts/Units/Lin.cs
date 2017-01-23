using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Lin : GenericUnit
{
    const string specialMsg = "{0}'s attack power increased eternally!";

    public override void InitializeMoveAndAttack()
    {
        Moves = CommonMovement.dir8;
        AttackMoves = CommonMovement.dir4;
    }

    public override void OnTurnEnd(GameController gameController)
    {
        base.OnTurnEnd(gameController);
        List<Unit> units = gameController.Units;
        foreach(Unit unit in units)
        {
            if(unit.PlayerNumber == PlayerNumber)
            {
                Buff buff = unit.Buffs.FindLast(b => b is AttackUp && (b as AttackUp).eternal == false);
                if (buff != null)
                    unit.RemoveBuff(buff);

                if(Vector2.Distance(unit.Cell.OffsetCoord, Cell.OffsetCoord)==1)
                {
                    unit.AddBuff(new AttackUp(Buff.ETERNAL, 1, false));
                }
            }
        } 
        
    }

    protected override void OnDestroyed(GameController gameController)
    {
        RemoveBuffFromNeighbors(gameController.CellMap);
        base.OnDestroyed(gameController);
    }

    public override void PerformSpecialMove(GameController gameController, List<Vector2> targetSeq)
    {
        Unit target = gameController.CellMap[targetSeq[0]].OccupyingUnit;
        target.AddBuff(new AttackUp(Buff.ETERNAL, 1, true));

        SpecialUsed = true;
        logger.LogSpecial(this, String.Format(specialMsg, target));
        gameController.EndTurn();
    }

    public override void SpecialMove(GameController gameController)
    {
        gameController.GameState = new SpecialStateLin(gameController, this);
    }

    private void RemoveBuffFromNeighbors(Dictionary<Vector2, Cell> cellMap)
    {
        foreach (Vector2 dir in CommonMovement.dir4)
        {
            Vector2 cood = Cell.OffsetCoord + dir;
            if (cellMap.ContainsKey(cood) && cellMap[cood].IsTaken && cellMap[cood].OccupyingUnit.PlayerNumber == PlayerNumber)
            {
                Unit unit = cellMap[cood].OccupyingUnit;
                Buff buff = unit.Buffs.FindLast(b => b is AttackUp && ((AttackUp)b).eternal == false);
                cellMap[cood].OccupyingUnit.RemoveBuff(buff);
            }
        }
    }
}

