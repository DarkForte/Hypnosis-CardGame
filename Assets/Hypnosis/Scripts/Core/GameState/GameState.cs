using System.Linq;
using UnityEngine;
using System.Collections.Generic;

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

    public virtual void OnUnitSelected(Unit unit) { }
    public virtual void OnUnitDeselected(Unit unit) { }

    public virtual void OnReceiveNetMessage(PhotonPlayer player, List<Vector2> targets)
    {}

    public virtual void OnStateEnter()
    {
        if (_gameController.Units.Select(u => u.PlayerNumber).Distinct().ToList().Count == 1)
        {
            _gameController.GameState = new GameStateGameOver(_gameController);
        }
    }
    public virtual void OnStateExit()
    {
    }
}