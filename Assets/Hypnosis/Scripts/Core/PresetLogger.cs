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

    public const string SelfColor = "cyan";
    public const string EnemyColor = "red";
    public const string DefaultColor = "white";

    public void LogMove(Unit unit)
    {
        string color = GetColor(unit.isFriendUnit);
        Log(String.Format(moveText, unit.UnitName), color);
    }

    public void LogAttack(Unit attacker, Unit victim)
    {
        string color = GetColor(attacker.isFriendUnit);
        Log(String.Format(attackText, attacker.UnitName, victim.UnitName), color);
    }

    public void LogSummon(Player player, Unit unit)
    {
        string color = GetColor(player is HumanPlayer);
        Log(String.Format(summonText, "Player " + player.PlayerNumber, unit.UnitName), color);
    }

    public void LogPass(Player player)
    {
        string color = GetColor(player is HumanPlayer);
        Log(String.Format(passText, "Player " + player.PlayerNumber), color);
    }

    public void LogSpecial(Unit unit, string specialMsg)
    {
        string color = GetColor(unit.isFriendUnit);
        Log(String.Format(specialText, unit.UnitName), color, refresh:false);
        Log(specialMsg, color);
    }

    public void LogBaseDestroyed(Player player)
    {
        string color = GetColor(player is HumanPlayer);
        Log(String.Format(baseDestroyedText, "Player " + player.PlayerNumber), color);
    }

    public void LogWinner(Player winner)
    {
        Log(String.Format(winText, "Player " + winner.PlayerNumber), DefaultColor);
    }

    public void LogTie()
    {
        Log("The game is a tie!", DefaultColor);
    }

    public string GetColor(bool isLocal)
    {
        if (isLocal)
            return SelfColor;
        else
            return EnemyColor;
    }
}
