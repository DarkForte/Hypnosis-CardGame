using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class PresetLogger : LogWindow
{
    const string moveText = "{0} moved!";
    const string attackText = "{0} attacked {1}!";
    const string summonText = "{0} summoned {1}!";
    const string passText = "{0} passed!";

    public void LogMove(Unit unit)
    {
        Log(String.Format(moveText, unit.UnitName));
    }

    public void LogAttack(Unit attacker, Unit victim)
    {
        Log(String.Format(attackText, attacker.UnitName, victim.UnitName));
    }

    public void LogSummon(Player player, Unit unit)
    {
        Log(String.Format(summonText, "Player " + player.PlayerNumber, unit.UnitName));
    }

    public void LogPass(Player player)
    {
        Log(String.Format(passText, "Player " + player.PlayerNumber));
    }
}
