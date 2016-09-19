using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public int PlayerNumber;

    List<CardType> CardPool;
    int p_CardPool;

    public List<CardType> NowCards;
    public int p_NowCards;

    /// <summary>
    /// Method is called every turn. Allows player to interact with his units.
    /// </summary>         
    public abstract void Play(GameController gameController);

    public void InitCardPool()
    {
        CardPool = new List<CardType>();
        int i;
        for(i=1; i<=Constants.CARD_POOL_SIZE; i++)
        {
            CardPool.Add(CardType.MOVE);
        }

        p_CardPool = 0;

        return;
    }

    public List<CardType> DrawCards(int number)
    {
        List<CardType> ret = new List<CardType>();
        for(int i = 1; i <= number; i++)
        {
            ret.Add(CardPool[p_CardPool]);
            p_CardPool++;
        }
        return ret;
    } 
}