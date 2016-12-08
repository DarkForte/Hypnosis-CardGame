using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Base : GenericUnit
{
    public override void InitializeMoveAndAttack()
    {
        Steps = 0; // base cannot move
    }

    public override void PerformSpecialMove(GameController gameController, List<Vector2> targetSeq)
    {
        
    }

    public override void SpecialMove(GameController gameController)
    {
        
    }
}

