using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class FirstTargetLocked : Buff
{
    public FirstTargetLocked(int duration) : base(duration)
    {
    }

    public override void Apply(Unit unit)
    {
        unit.MarkAsFirstTargetLocked();
    }

    public override void Undo(Unit unit)
    {
        unit.UnMark();
    }
}

