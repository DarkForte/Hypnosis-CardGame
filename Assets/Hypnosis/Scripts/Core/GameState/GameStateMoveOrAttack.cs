using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class GameStateMoveOrAttack : GameStateUnitSelected
{
    public GameStateMoveOrAttack(GameController gameController, Unit unit, CardType nowAction) : base(gameController, unit, nowAction)
    {
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        if (_nowAction == CardType.MOVE)
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
        else if (_nowAction == CardType.ATTACK)
        {
            _unitsInRange = _unit.GetEnemiesInRange(_gameController.CellMap);
            _unitsInRange.ForEach(e => e.MarkAsReachableEnemy());
        }
    }

    public override void OnCellClicked(Cell cell)
    {
        base.OnCellClicked(cell);
        if (_nowAction == CardType.ATTACK)
            return;

        if (cell.IsTaken || !_pathsInRange.Contains(cell))
        {
            _gameController.GameState = new GameStateWaitingInput(_gameController, _nowAction);
            return;
        }

        if (_nowAction == CardType.MOVE)
        {
            Vector2[] moves = { _unit.Cell.OffsetCoord, cell.OffsetCoord };
            _gameController.TurnManager.SendMove(moves);

            var path = _unit.FindPath(_gameController.CellMap, cell);
            _unit.Move(cell, path, _gameController);
            _gameController.EndTurn();
        }
    }

    public override void OnUnitClicked(Unit unit)
    {
        base.OnUnitClicked(unit);
        if (unit.PlayerNumber.Equals(_unit.PlayerNumber)) //Change the 1st target
        {
            _gameController.GameState = new GameStateMoveOrAttack(_gameController, unit, _nowAction);
        }

        if (_nowAction == CardType.MOVE)
            return;

        if (_nowAction == CardType.ATTACK)
        {
            if (_unitsInRange.Contains(unit))
            {
                Vector2[] moves = { _unit.Cell.OffsetCoord, unit.Cell.OffsetCoord };
                _gameController.TurnManager.SendMove(moves);

                _unit.DealDamage(unit, _gameController);
                _gameController.EndTurn();
            }
        }
    }
}

