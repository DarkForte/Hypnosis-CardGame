using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class SpecialStateSui : SpecialState
{
    public SpecialStateSui(GameController gameController, Unit unit) : base(gameController, unit)
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        Vector2[] seq = { _unit.Cell.OffsetCoord };
        SendMove(seq);
    }
}

