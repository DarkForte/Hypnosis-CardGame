using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum CardType
{
    MOVE, ATTACK, SPECIAL, YUREN
}

public class CardHelper
{
    public static bool isBasic(CardType card)
    {
        return (int)card <= 2;
    }
    public static int convertToPrefabIndex(CardType card)
    {
        return (int)card - 3;
    }
}