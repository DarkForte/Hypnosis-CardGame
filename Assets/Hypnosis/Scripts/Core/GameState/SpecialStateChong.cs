using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class SpecialStateChong : SpecialState
{
    public SpecialStateChong(GameController gameController, Unit unit) : base(gameController, unit)
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        foreach(Vector2 dir in CommonMovement.dir4)
        {
            Vector2 next = _unit.Cell.OffsetCoord + dir;
            if (_gameController.CellMap.ContainsKey(next))
                _pathsInRange.Add(_gameController.CellMap[next]);
        }
        _pathsInRange.ForEach(cell => cell.MarkAsReachable());
    }

    public override void OnCellSelected(Cell cell)
    {
        if (!_pathsInRange.Contains(cell))
            return;

        cell.MarkAsPath();
        List<Unit> unitsInRange = GetEnemiesOnLine(cell);
        unitsInRange.ForEach(unit => unit.MarkAsReachableEnemy());
    }

    public override void OnCellDeselected(Cell cell)
    {
        if (_pathsInRange.Contains(cell))
        {
            cell.MarkAsReachable();

            List<Unit> unitsInRange = GetEnemiesOnLine(cell);
            unitsInRange.ForEach(unit => unit.UnMarkAsReachableEnemy());
        }
    }

    public override void OnCellClicked(Cell cell)
    {
        GetEnemiesOnLine(cell).ForEach(unit => unit.UnMarkAsReachableEnemy());
        Vector2[] seq = { _unit.Cell.OffsetCoord, cell.OffsetCoord };
        _gameController.TurnManager.SendMove(seq);
        _unit.PerformSpecialMove(_gameController, new Vector2[] { cell.OffsetCoord }.ToList());
    }

    
    public override void OnUnitSelected(Unit unit)
    {
        OnCellSelected(unit.Cell);
    }

    public override void OnUnitDeselected(Unit unit)
    {
        OnCellDeselected(unit.Cell);
    }

    public override void OnUnitClicked(Unit unit)
    {
        OnCellClicked(unit.Cell);
    }
    

    private List<Unit> GetEnemiesOnLine(Cell cell)
    {
        List<Unit> ret = new List<Unit>();

        Vector2 dir = cell.OffsetCoord - _unit.Cell.OffsetCoord;
        Vector2 now = _unit.Cell.OffsetCoord + dir;
        while (_gameController.CellMap.ContainsKey(now))
        {
            if (_gameController.CellMap[now].IsTaken)
            {
                Unit unit = _gameController.CellMap[now].OccupyingUnit;
                if (unit.PlayerNumber != _unit.PlayerNumber && !(unit is Base))
                {
                    ret.Add(_gameController.CellMap[now].OccupyingUnit);
                }
            }
            now += dir;
        }

        return ret;
    }
}

