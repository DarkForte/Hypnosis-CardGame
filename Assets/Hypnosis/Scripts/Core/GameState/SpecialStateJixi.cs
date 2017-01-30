using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class SpecialStateJixi : SpecialState
{
    Unit targetUnit = null;
    public SpecialStateJixi(GameController gameController, Unit unit) : base(gameController, unit)
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        _unitsInRange = _unit.GetAllTargetsInRange(_gameController.CellMap);
        _unitsInRange = _unitsInRange.FindAll(unit => !(unit is Base));
        _unitsInRange.ForEach(unit => unit.MarkAsReachableEnemy());
    }

    public override void OnUnitClicked(Unit unit)
    {
        base.OnUnitClicked(unit);
        if (_unitsInRange.Contains(unit))
        {
            targetUnit = unit;
            _pathsInRange = GetPossibleDestinations(_gameController.CellMap);
            _pathsInRange.ForEach(cell => cell.MarkAsReachable());
        }

    }

    public override void OnCellClicked(Cell cell)
    {
        base.OnCellClicked(cell);
        if(_pathsInRange.Contains(cell))
        {
            Vector2[] seq = { _unit.Cell.OffsetCoord, targetUnit.Cell.OffsetCoord, cell.OffsetCoord };
            SendMove(seq);
        }
    }

    protected List<Cell> GetPossibleDestinations(Dictionary<Vector2, Cell> cellMap)
    {
        List<Cell> ret = new List<Cell>();
        foreach(Vector2 dir in CommonMovement.dir4)
        {
            Vector2 nowCell = _unit.Cell.OffsetCoord;
            Cell lastCell = null;
            while(cellMap.ContainsKey(nowCell))
            {
                if (!cellMap[nowCell].IsTaken)
                    lastCell = cellMap[nowCell];
                nowCell += dir;
            }
            if (lastCell != null)
                ret.Add(lastCell);
        }
        return ret;
    }
}

