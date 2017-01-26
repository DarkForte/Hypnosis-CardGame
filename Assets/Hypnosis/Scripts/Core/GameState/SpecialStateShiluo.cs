using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
            Vector2[] seq = { _unit.Cell.OffsetCoord, cell.OffsetCoord };
            SendMove(seq);
        }
    }

    public override void OnCellSelected(Cell cell)
    {
        //Do not show path
        cell.MarkAsHighlighted();
    }
}

