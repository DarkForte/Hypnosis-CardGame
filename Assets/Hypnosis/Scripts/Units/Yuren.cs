using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Yuren : GenericUnit
{
    public override void InitializeMoveAndAttack()
    {
        Moves = CommonMovement.dir4;
        AttackMoves = CommonMovement.front3;
        if (PlayerNumber == 1)
            AttackMoves.ForEach(move => move = -move);
    }
}

