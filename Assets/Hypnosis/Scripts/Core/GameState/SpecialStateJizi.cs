using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class SpecialStateJizi : SpecialState
{
    public SpecialStateJizi(GameController gameController, Unit unit) : base(gameController, unit)
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        _unitsInRange = _gameController.Units.FindAll(unit => unit.PlayerNumber != _unit.PlayerNumber && unit.UnitName != "Base");
        _unitsInRange.ForEach(unit => unit.MarkAsReachableEnemy());
    }

    public override void OnUnitClicked(Unit unit)
    {
        base.OnUnitClicked(unit);
        if(unit.PlayerNumber != _unit.PlayerNumber)
        {
            _gameController.TurnManager.SendMove(new Vector2[] { _unit.Cell.OffsetCoord, unit.Cell.OffsetCoord });
            _unit.PerformSpecialMove(_gameController, new Vector2[] {unit.Cell.OffsetCoord}.ToList());
        }
    }
}

