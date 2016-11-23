using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class Chushui : GenericUnit
{
    public override void InitializeMoveAndAttack()
    {
        Moves = CommonMovement.dir8;
        AttackMoves = CommonMovement.dir4;
    }

    public override void SpecialMove(GameController gameController)
    {
        throw new NotImplementedException();
    }
}

