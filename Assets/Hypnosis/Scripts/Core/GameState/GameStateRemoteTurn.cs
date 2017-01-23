﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class GameStateRemoteTurn : GameState
{
    public GameStateRemoteTurn(GameController gameController) : base(gameController)
    {
    }

    public override void OnReceiveNetMessage(PhotonPlayer player, List<Vector2> targets)
    {
        Player remotePlayer = _gameController.RemotePlayer;
        CardType nowCard = remotePlayer.TakeFirstCard();
        var cellMap = _gameController.CellMap;

        for (int i = 0; i < targets.Count; i++)
            targets[i] = new Vector2(4, 5) - targets[i];

        if (targets.Count==0)
        {
            _gameController.logger.LogPass(remotePlayer);
            _gameController.EndTurn();

        }
        else if(nowCard == CardType.MOVE)
        {
            Vector2 from = targets[0];
            Vector2 to = targets[1];
            Unit unit = cellMap[from].OccupyingUnit;
            List<Cell> path = BFSPathFinder.FindCellPath(cellMap, unit.Moves, from, to, true, false, unit.PlayerNumber);

            unit.Move(cellMap[to], path, _gameController);
            _gameController.EndTurn();

        }
        else if(nowCard == CardType.ATTACK)
        {
            Unit attacker = cellMap[targets[0]].OccupyingUnit;
            Unit victim = cellMap[targets[1]].OccupyingUnit;

            attacker.DealDamage(victim, _gameController);
            _gameController.EndTurn();

        }
        else if(nowCard == CardType.SPECIAL)
        {
            Unit performer = cellMap[targets[0]].OccupyingUnit;
            targets.RemoveAt(0);

            performer.PerformSpecialMove(_gameController, targets);
        }
        else //Summon
        {
            Vector2 place = targets[0];
            _gameController.SummonPrefab(nowCard, _gameController.CellMap[place]);
            _gameController.EndTurn();

        }
    }


}

