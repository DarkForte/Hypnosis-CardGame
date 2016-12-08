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
        if (PlayerNumber == 0)
            Moves.Add(new Vector2(0, 1));
        else
            Moves.Add(new Vector2(0, -1));

        AttackMoves = CommonMovement.dir4;
    }

    public override void PerformSpecialMove(GameController gameController, List<Vector2> targetSeq){}

    public override void SpecialMove(GameController gameController){}

    protected override void OnKillingOthers(Unit victim)
    {
        StartCoroutine(TakeUpCell(victim));
    }

    private IEnumerator TakeUpCell(Unit victim)
    {
        yield return new WaitWhile(() => isMoving);
        List<Cell> path = new List<Cell>();
        path.Add(victim.Cell);
        path.Add(Cell);
        Move(victim.Cell, path);
    }
}