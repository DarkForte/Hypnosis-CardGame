using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class SpecialStateYezi : SpecialState
{
    public SpecialStateYezi(GameController gameController, Unit unit) : base(gameController, unit)
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        _unitsInRange = _gameController.Units.FindAll(unit => !(unit is Base));
        _unitsInRange.ForEach(unit => unit.MarkAsReachableEnemy());
    }

    public override void OnUnitClicked(Unit unit)
    {
        base.OnUnitClicked(unit);
        Vector2[] seq = { _unit.Cell.OffsetCoord, unit.Cell.OffsetCoord };
        SendMove(seq);
    }

}

