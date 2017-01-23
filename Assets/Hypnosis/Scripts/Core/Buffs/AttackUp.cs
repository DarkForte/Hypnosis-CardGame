using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class AttackUp : Buff
{
    public int value;
    public bool eternal;
    public AttackUp(int duration, int value, bool eternal) : base(duration)
    {
        this.value = value;
        this.eternal = eternal;
    }

    public override void Apply(Unit unit)
    {
        unit.AttackPower += value;
        unit.MarkAsAttackUp();
    }

    public override void Undo(Unit unit)
    {
        unit.AttackPower -= value;
        unit.UnMark();
    }
}

