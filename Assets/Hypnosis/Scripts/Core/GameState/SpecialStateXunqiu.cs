using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class SpecialStateXunqiu : SpecialState
{
    Unit virtualUnit;
    public SpecialStateXunqiu(GameController gameController, Unit unit) : base(gameController, unit)
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        _unitsInRange = _gameController.Units.FindAll(unit => unit.SpecialUsed == false && !(unit is Xunqiu));
        _unitsInRange.ForEach(unit => unit.MarkAsReachableEnemy());
    }

    public override void OnUnitClicked(Unit unit)
    { 
        base.OnUnitClicked(unit);
        virtualUnit = unit;
        SpecialState nextState = unit.GetSpecialState(_gameController);
        nextState.SendFunc = Send;
        nextState._unit = _unit;
        _gameController.GameState = nextState;
    }

    /// <summary>
    /// Customized send function to modify the sequence from the unit that is controlled.
    /// </summary>
    /// <param name="param">The sequence of the controllee</param>
    void Send(Vector2[] specialSeq)
    {
        List<Vector2> list = new List<Vector2>();
        list.Add(_unit.Cell.OffsetCoord);
        list.Add(virtualUnit.Cell.OffsetCoord);

        List<Vector2> otherList = specialSeq.ToList();
        otherList.RemoveAt(0);
        list.AddRange(otherList);

        SendMove(list.ToArray());
    }
}

