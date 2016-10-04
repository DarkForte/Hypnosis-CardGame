using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class Invincible : Buff
{
    public Invincible(int duration) : base(duration)
    {

    }

    public override void Apply(Unit unit)
    {
        unit.MarkAsInvincible();
    }

    public override void Undo(Unit unit)
    {
        unit.UnMark();
    }
}

