﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Yuren : GenericUnit
{
    public override void InitializeMoveAndAttack()
    {
        Moves = CommonMovement.dir4;
        foreach(var attack in CommonMovement.front3)
        {
            if (PlayerNumber == 0)
                AttackMoves.Add(attack);
            else
                AttackMoves.Add(-attack);
        }
    }

    public override List<Unit> GetEnemiesInRange(List<Unit> units)
    {
        List<Unit> ret = new List<Unit>();
        List<Unit> enemies = units.FindAll(unit => unit.PlayerNumber != PlayerNumber);
        foreach(var attack in AttackMoves)
        {
            Vector2 nowCell = Cell.OffsetCoord + attack;
            Unit nowTarget = enemies.Find(u => u.Cell.OffsetCoord == nowCell);
            if (nowTarget != null)
                ret.Add(nowTarget);
        }
        return ret;
    }

    public override void SpecialMove(GameController gameController)
    {
        gameController.GameState = new SpecialStateYuren(gameController, this);
    }
}

