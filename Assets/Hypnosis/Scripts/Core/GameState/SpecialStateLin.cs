using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class SpecialStateLin : SpecialState
{
    public SpecialStateLin(GameController gameController, Unit unit) : base(gameController, unit)
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        _unitsInRange = _unit.GetAllTargetsInRange(_gameController.CellMap).FindAll(unit => unit.PlayerNumber == _unit.PlayerNumber);
        _unitsInRange.ForEach(unit => unit.MarkAsReachableEnemy());
    }

    public override void OnUnitClicked(Unit unit)
    {
        base.OnUnitClicked(unit);
        if (!_unitsInRange.Contains(unit))
            return;

        Vector2[] seq = { _unit.Cell.OffsetCoord, unit.Cell.OffsetCoord };
        _gameController.TurnManager.SendMove(seq);
        _unit.PerformSpecialMove(_gameController, new Vector2[] {unit.Cell.OffsetCoord}.ToList());
    }
}
