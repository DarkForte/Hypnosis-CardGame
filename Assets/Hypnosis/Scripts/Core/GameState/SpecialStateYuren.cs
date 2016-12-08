using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class SpecialStateYuren : SpecialState
{
    public SpecialStateYuren(GameController gameController, Unit unit) : base(gameController, unit)
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        _unitsInRange = _unit.GetEnemiesInRange(_gameController.CellMap);
        _unitsInRange.RemoveAll(unit => unit.UnitName == "Base");

        _unitsInRange.ForEach(unit => unit.MarkAsReachableEnemy());
    }

    public override void OnUnitClicked(Unit unit)
    {
        base.OnUnitClicked(unit);

        if (_unitsInRange.Contains(unit))
        {
            _gameController.TurnManager.SendMove(new Vector2[] { _unit.Cell.OffsetCoord, unit.Cell.OffsetCoord });
            _unit.PerformSpecialMove(_gameController, new Vector2[] { unit.Cell.OffsetCoord }.ToList());
        }
    }
}
