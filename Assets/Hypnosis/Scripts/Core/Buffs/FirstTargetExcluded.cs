using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class FirstTargetExcluded : Buff
{
    public FirstTargetExcluded(int duration) : base(duration)
    {
    }

    public override void Apply(Unit unit)
    {
        unit.MarkAsFirstTargetExcluded();
    }

    public override void Undo(Unit unit)
    {
        unit.UnMark();
    }
}

