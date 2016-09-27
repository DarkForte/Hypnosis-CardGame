﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Pawn : GenericUnit
{
    public override void InitializeMoveAndAttack()
    {
        Moves = CommonMovement.front3;
        if (PlayerNumber == 1)
            Moves.ForEach(m => m = -m);

        AttackMoves = CommonMovement.dir4;
    }


}