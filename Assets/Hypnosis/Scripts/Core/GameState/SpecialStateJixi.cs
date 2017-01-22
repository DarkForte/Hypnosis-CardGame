using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class SpecialStateJixi : SpecialState
{
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
            Vector2[] move = { _unit.Cell.OffsetCoord, unit.Cell.OffsetCoord };
            _gameController.TurnManager.SendMove(move);
            _unit.PerformSpecialMove(_gameController, new Vector2[] { unit.Cell.OffsetCoord }.ToList());
        }

    }
}

