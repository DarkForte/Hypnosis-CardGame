using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class Base : GenericUnit
{
    public override SpecialState GetSpecialState(GameController gameController)
    {
        return null;
    }

    public override void Initialize()
    {
        base.Initialize();
        SpecialUsed = true;
    }
    public override void InitializeMoveAndAttack()
    {
        Steps = 0; // base cannot move
    }

    public override void PerformSpecialMove(GameController gameController, List<Vector2> targetSeq)
    {
        
    }


}

