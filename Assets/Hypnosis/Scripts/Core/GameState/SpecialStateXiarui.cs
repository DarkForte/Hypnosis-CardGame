using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class SpecialStateXiarui : SpecialState
{
    public SpecialStateXiarui(GameController gameController, Unit unit) : base(gameController, unit)
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        _unitsInRange = _gameController.Units.FindAll(unit => unit.PlayerNumber == _unit.PlayerNumber && unit.UnitName != "Base");
        _unitsInRange.ForEach(unit => unit.MarkAsReachableEnemy());
    }

    public override void OnUnitClicked(Unit unit)
    {
        base.OnUnitClicked(unit);
        Vector2[] seq = { _unit.Cell.OffsetCoord, unit.Cell.OffsetCoord };
        SendMove(seq);
    }
}

