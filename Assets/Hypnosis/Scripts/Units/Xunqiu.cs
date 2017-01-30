﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;

public class Xunqiu : GenericUnit
{
    const string specialMsg = "{0} transformed to {1}!";
    Unit virtualServant;
    delegate void SpecialFunc(Unit unit, GameController gameController, List<Vector2> targetSeq);

    public override void InitializeMoveAndAttack()
    {
        Moves = CommonMovement.dir4;
        AttackMoves = CommonMovement.dir4;
    }

    public override void PerformSpecialMove(GameController gameController, List<Vector2> targetSeq)
    {
        Unit servant = gameController.CellMap[targetSeq[0]].OccupyingUnit;
        virtualServant = (Unit)gameObject.AddComponent(servant.GetType());

        virtualServant.Moves = Moves;
        virtualServant.Steps = Steps;
        virtualServant.AttackMoves = AttackMoves;
        virtualServant.AttackPower = AttackPower;
        virtualServant.AttackRange = AttackPower;
        virtualServant.HP = HP;
        virtualServant.Cell = Cell;
        virtualServant.UnitName = servant.UnitName;
        virtualServant.logger = logger;

        targetSeq.RemoveAt(0);
        SpecialUsed = true;
        servant.SpecialUsed = true;
        gameController.logger.LogSpecial(this, String.Format(specialMsg, UnitName, servant.UnitName));
        virtualServant.PerformSpecialMove(gameController, targetSeq);
    }

    public override void OnTurnEnd(GameController gameController)
    {
        base.OnTurnEnd(gameController);
        if(virtualServant!=null)
        {
            Moves = virtualServant.Moves;
            Steps = virtualServant.Steps;
            AttackMoves = virtualServant.AttackMoves;
            AttackPower = virtualServant.AttackPower;
            AttackRange = virtualServant.AttackPower;
            HP = virtualServant.HP;
            Cell = virtualServant.Cell;
            Cell.OccupyingUnit = this; //If moved, the occupying unit will be the destoryed virtual servant
            Destroy(virtualServant);
        }
        virtualServant = null;
    }

    public override SpecialState GetSpecialState(GameController gameController)
    {
        return new SpecialStateXunqiu(gameController, this);
    }
}