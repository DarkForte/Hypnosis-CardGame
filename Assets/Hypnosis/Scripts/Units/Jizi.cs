using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class Jizi : GenericUnit
{
    public override void InitializeMoveAndAttack()
    {
        Moves = CommonMovement.dir8;
        AttackMoves = CommonMovement.dir8;
    }

    public override void SpecialMove(GameController gameController)
    {
        gameController.GameState = new SpecialStateJizi(gameController, this);
    }
}

