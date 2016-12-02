using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class SpecialStateShiluo : SpecialState
{
    public SpecialStateShiluo(GameController gameController, Unit unit) : base(gameController, unit)
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        _pathsInRange = _gameController.Cells.FindAll(cell => cell.IsTaken == false);
        _pathsInRange.ForEach(cell => cell.MarkAsReachable());
    }

    public override void OnCellClicked(Cell cell)
    {
        base.OnCellClicked(cell);
        if(_pathsInRange.Contains(cell))
        {
            _unit.Move(cell, null);
            _unit.SpecialUsed = true;
            _gameController.EndTurn();
        }
    }

    public override void OnCellSelected(Cell cell)
    {
        //Do not show path
        cell.MarkAsHighlighted();
    }
}

