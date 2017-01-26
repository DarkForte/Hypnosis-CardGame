using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Pawn : GenericUnit
{
    public override void Initialize()
    {
        base.Initialize();
        SpecialUsed = true;
    }

    public override void InitializeMoveAndAttack()
    {
        Moves.Add(new Vector2(1, 0));
        Moves.Add(new Vector2(-1, 0));
        if (isFriendUnit)
            Moves.Add(new Vector2(0, 1));
        else
            Moves.Add(new Vector2(0, -1));

        AttackMoves = CommonMovement.dir4;
    }

    public override void PerformSpecialMove(GameController gameController, List<Vector2> targetSeq){}

    public override SpecialState GetSpecialState(GameController gameController){ return null; }

    protected override void OnKillingOthers(Unit victim, GameController gameController)
    {
        StartCoroutine(TakeUpCell(victim, gameController));
    }

    private IEnumerator TakeUpCell(Unit victim, GameController gameController)
    {
        yield return new WaitWhile(() => isMoving);
        List<Cell> path = new List<Cell>();
        path.Add(victim.Cell);
        path.Add(Cell);
        Move(victim.Cell, path, gameController);
    }
}