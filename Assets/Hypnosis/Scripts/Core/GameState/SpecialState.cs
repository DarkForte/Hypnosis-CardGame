using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpecialState : GameStateUnitSelected
{
    public Action<Vector2[]> SendFunc;
         
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

    protected void SendMove(Vector2[] seq)
    {
        if(SendFunc == null)
        {
            _gameController.TurnManager.SendMove(seq);
            List<Vector2> list = seq.ToList();
            list.RemoveAt(0);
            _unit.PerformSpecialMove(_gameController, list);
        }
        else
        {
            SendFunc(seq);
        }
    }
}

