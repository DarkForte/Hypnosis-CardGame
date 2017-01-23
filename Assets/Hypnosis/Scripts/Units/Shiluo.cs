using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Shiluo : GenericUnit
{
    public override void InitializeMoveAndAttack()
    {
        Moves = CommonMovement.dir4;
        AttackMoves = CommonMovement.dir8;
    }

    public override void PerformSpecialMove(GameController gameController, List<Vector2> targetSeq)
    {
        Cell cell = gameController.CellMap[targetSeq[0]];

        Move(cell, null, gameController, log:false);
        SpecialUsed = true;

        gameController.logger.LogSpecial(this, this.UnitName + " transported to " + cell.OffsetCoord + "!");
        gameController.EndTurn();
    }

    public override void SpecialMove(GameController gameController)
    {
        gameController.GameState = new SpecialStateShiluo(gameController, this);
    }
}

