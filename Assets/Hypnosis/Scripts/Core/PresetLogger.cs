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
    const string specialText = "{0}'s special move!";
    const string baseDestroyedText = "{0}'s base is destroyed!";
    const string winText = "{0} wins the game!";

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

    public void LogSpecial(Unit unit, string specialMsg)
    {
        Log(String.Format(specialText, unit.UnitName));
        Log(specialMsg);
    }

    public void LogBaseDestroyed(Player player)
    {
        Log(String.Format(baseDestroyedText, "Player " + player.PlayerNumber));
    }

    public void LogWinner(Player winner)
    {
        Log(String.Format(winText, "Player " + winner.PlayerNumber));
    }

    public void LogTie()
    {
        Log("The game is a tie!");
    }
}
