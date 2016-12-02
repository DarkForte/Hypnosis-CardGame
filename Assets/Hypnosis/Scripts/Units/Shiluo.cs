﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class Shiluo : GenericUnit
{
    public override void InitializeMoveAndAttack()
    {
        Moves = CommonMovement.dir4;
        AttackMoves = CommonMovement.dir8;
    }

    public override void SpecialMove(GameController gameController)
    {
        gameController.GameState = new SpecialStateShiluo(gameController, this);
    }
}

