using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class Xiarui : GenericUnit
{
    public override void InitializeMoveAndAttack()
    {
        Moves = CommonMovement.dir4;
        AttackMoves = CommonMovement.dir4;
    }

    public override void SpecialMove(GameController gameController)
    {
        gameController.GameState = new SpecialStateXiarui(gameController, this);
    }
}
