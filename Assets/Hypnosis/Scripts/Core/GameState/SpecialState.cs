using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class SpecialState : GameStateUnitSelected
{
    public SpecialState(GameController gameController, Unit unit) : base(gameController, unit, CardType.SPECIAL)
    {
    }

    public override void OnUnitClicked(Unit unit)
    {
        base.OnUnitClicked(unit);
        if (unit.PlayerNumber == _unit.PlayerNumber)
        {
            if (unit.SpecialUsed == false)
                unit.SpecialMove(_gameController);
        }
    }
}

