using System.Collections.Generic;
using System.Linq;

abstract class GameStateUnitSelected : GameState
{
    protected Unit _unit;
    protected CardType _nowAction;
    protected List<Cell> _pathsInRange;
    protected List<Unit> _unitsInRange;


    public GameStateUnitSelected(GameController gameController, Unit unit, CardType nowAction) : base(gameController)
    {
        _unit = unit;
        _pathsInRange = new List<Cell>();
        _unitsInRange = new List<Unit>();
        _nowAction = nowAction;
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        _unit.OnUnitSelected();
    }

    public override void OnCellClicked(Cell cell)
    {
        if(_unit.isMoving)
            return;
    }

    /// <summary>
    /// Pointing at the 2nd target.
    /// </summary>
    /// <param name="unit"></param>
    public override void OnUnitClicked(Unit unit)
    {
        if (unit.Equals(_unit) || unit.isMoving)
            return;
            
    }
    public override void OnCellDeselected(Cell cell)
    {
        base.OnCellDeselected(cell);

        foreach (var _cell in _pathsInRange)
        {
            _cell.MarkAsReachable();
        }
        foreach (var _cell in _gameController.Cells.Except(_pathsInRange))
        {
            _cell.UnMark();
        }
    }
    public override void OnCellSelected(Cell cell)
    {
        base.OnCellSelected(cell);
        if (!_pathsInRange.Contains(cell)) return;
        var path = _unit.FindPath(_gameController.CellMap, cell);
        foreach (var _cell in path)
        {
            _cell.MarkAsPath();
        }
    }


    public override void OnStateExit()
    {
        _unit.OnUnitDeselected();
        foreach (var unit in _unitsInRange)
        {
            if (unit == null) continue;
            unit.UnMarkAsReachableEnemy();
        }
        foreach (var cell in _gameController.Cells)
        {
            cell.UnMark();
        }   
    }
}

