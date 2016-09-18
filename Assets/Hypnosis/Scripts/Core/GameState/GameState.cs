using System.Linq;

public abstract class GameState
{
    protected GameController _gameController;
    
    protected GameState(GameController gameController)
    {
        _gameController = gameController;
    }

    public virtual void OnUnitClicked(Unit unit)
    { }
    
    public virtual void OnCellDeselected(Cell cell)
    {
        cell.UnMark();
    }
    public virtual void OnCellSelected(Cell cell)
    {
        cell.MarkAsHighlighted();
    }
    public virtual void OnCellClicked(Cell cell)
    { }

    public virtual void OnStateEnter()
    {
        if (_gameController.Units.Select(u => u.PlayerNumber).Distinct().ToList().Count == 1)
        {
            _gameController.GameState = new CellGridStateGameOver(_gameController);
        }
    }
    public virtual void OnStateExit()
    {
    }
}