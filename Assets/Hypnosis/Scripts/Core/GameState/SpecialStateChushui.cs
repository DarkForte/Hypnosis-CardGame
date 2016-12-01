using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class SpecialStateChushui : SpecialState
{
    bool choosingControl = true;
    Unit controlledUnit;

    public SpecialStateChushui(GameController gameController, Unit unit) : base(gameController, unit)
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        _unitsInRange = _gameController.Units.FindAll(u => u.PlayerNumber != _unit.PlayerNumber && u.UnitName != "Base");
        _unitsInRange.ForEach(u => u.MarkAsReachableEnemy());
    }

    public override void OnUnitClicked(Unit unit)
    {
        if(choosingControl)
        {
            base.OnUnitClicked(unit);
            if(unit.PlayerNumber != _unit.PlayerNumber) //Chose the controlling unit
            {
                controlledUnit = unit;
                _unitsInRange.ForEach(u => u.UnMarkAsReachableEnemy());
                _unitsInRange = unit.GetAllTargetsInRange(_gameController.CellMap);
                _unitsInRange.RemoveAll(u => u.UnitName == "Base");
                _unitsInRange.ForEach(u => u.MarkAsReachableEnemy());
                choosingControl = false;
            }
        }
        else
        {
            if(_unitsInRange.Contains(unit))
            {
                controlledUnit.DealDamage(unit);

                _unit.SpecialUsed = true;
                _gameController.EndTurn();
            }
            else
            {
                if(unit.PlayerNumber == controlledUnit.PlayerNumber) //Change the controlling unit
                {
                    controlledUnit = unit;
                    _unitsInRange.ForEach(u => u.UnMarkAsReachableEnemy());
                    _unitsInRange = unit.GetAllTargetsInRange(_gameController.CellMap);
                    _unitsInRange.RemoveAll(u => u.UnitName == "Base");
                    _unitsInRange.ForEach(u => u.MarkAsReachableEnemy());
                }
            }
        }

    }
}

