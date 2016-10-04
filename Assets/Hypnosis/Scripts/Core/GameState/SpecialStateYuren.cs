using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class SpecialStateYuren : GameStateUnitSelected
{
    public SpecialStateYuren(GameController gameController, Unit unit) : base(gameController, unit, CardType.SPECIAL)
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        _unitsInRange = _unit.GetEnemiesInRange(_gameController.Units);
        _unitsInRange.ForEach(unit => unit.MarkAsReachableEnemy());
    }

    public override void OnUnitClicked(Unit unit)
    {
        if(_unitsInRange.Contains(unit))
        {
            _unit.AttackPower = 10;
            _unit.DealDamage(unit);
            _unit.AttackPower = 2;
            _gameController.EndTurn();
        }
    }
}
