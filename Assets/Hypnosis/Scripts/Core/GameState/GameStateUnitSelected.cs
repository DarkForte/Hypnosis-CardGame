using System.Collections.Generic;
using System.Linq;

class GameStateUnitSelected : GameState
{
    private Unit _unit;
    private CardType _nowAction;
    private List<Cell> _pathsInRange;
    private List<Unit> _unitsInRange;


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

        if(_nowAction == CardType.MOVE)
        {
            _pathsInRange = _unit.GetAvailableDestinations(_gameController.CellMap);
            var cellsNotInRange = _gameController.Cells.Except(_pathsInRange);

            foreach (var cell in cellsNotInRange)
            {
                cell.UnMark();
            }
            foreach (var cell in _pathsInRange)
            {
                cell.MarkAsReachable();
            }
        }
        else if(_nowAction == CardType.ATTACK)
        {
            foreach (var currentUnit in _gameController.Units)
            {
                if (currentUnit.PlayerNumber.Equals(_unit.PlayerNumber))
                    continue;

                if (_unit.IsUnitAttackable(currentUnit, _unit.Cell))
                {
                    currentUnit.SetState(new UnitStateMarkedAsReachableEnemy(currentUnit));
                    _unitsInRange.Add(currentUnit);
                }
            }
        }
    }

    public override void OnCellClicked(Cell cell)
    {
        if(_unit.isMoving)
            return;
        if (_nowAction == CardType.ATTACK)
            return;

        if(cell.IsTaken)
        {
            _gameController.GameState = new GameStateWaitingInput(_gameController, _nowAction);
            return;
        }
            
        if(!_pathsInRange.Contains(cell))
        {
            _gameController.GameState = new GameStateWaitingInput(_gameController, _nowAction);
        }
        else
        {
            var path = _unit.FindPath(_gameController.CellMap, cell);
            _unit.Move(cell,path);
            _gameController.EndTurn();
        }
    }
    public override void OnUnitClicked(Unit unit)
    {
        if (_nowAction != CardType.ATTACK)
            return;

        if (unit.Equals(_unit) || unit.isMoving)
            return;

        if (_unitsInRange.Contains(unit))
        {
            _unit.DealDamage(unit);
            _gameController.EndTurn();
        }

        if (unit.PlayerNumber.Equals(_unit.PlayerNumber))
        {
            _gameController.GameState = new GameStateUnitSelected(_gameController, unit, _nowAction);
        }
            
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
            unit.SetState(new UnitStateNormal(unit));
        }
        foreach (var cell in _gameController.Cells)
        {
            cell.UnMark();
        }   
    }
}

