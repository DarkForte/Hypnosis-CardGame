﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public int PlayerNumber;
    protected List<CardType> CardPool;
    protected int p_CardPool;
    public Queue<CardType> NowCards = new Queue<CardType>();

    /// <summary>
    /// Method is called every turn. Allows player to interact with his units.
    /// </summary>         
    public abstract void Play(GameController gameController);

    public abstract IEnumerator SelectCard(GameController gameController, UIController uiController = null);

    protected void ShuffleCardPool()
    {
        int i;
        int cnt = CardPool.Count;
        for(i=0; i<cnt; i++)
        {
            int nowPos = Utils.randomGenerator.Next(i, cnt);
            //swap CardPool[i], CardPool[nowPos]
            CardType tmp = CardPool[nowPos];
            CardPool[nowPos] = CardPool[i];
            CardPool[i] = tmp;
            i++;
        }
    }

    public void InitCardPool()
    {
        CardPool = new List<CardType>();
        int i;
        for(i=1; i<=2; i++)
        {
            CardPool.Add(CardType.YUREN);
            CardPool.Add(CardType.XIARUI);
            CardPool.Add(CardType.JIZI);
            CardPool.Add(CardType.CHUSHUI);
            CardPool.Add(CardType.SHILUO);
            CardPool.Add(CardType.YUREN);
        }

        for (i = 1; i <= 16; i++)
            CardPool.Add(CardType.ATTACK);

        for (i = 1; i <= 12; i++)
            CardPool.Add(CardType.MOVE);

        for (i = 1; i <= 5; i++)
            CardPool.Add(CardType.SPECIAL);

        ShuffleCardPool();

        p_CardPool = 0;

        return;
    }

    public Unit LockedUnitFirst(List<Unit> units)
    {
        List<Unit> myUnits = units.FindAll(unit => unit.PlayerNumber == PlayerNumber);
        return myUnits.Find(unit => unit.Buffs.Find(buff => buff is FirstTargetLocked) != null);
    }

    public CardType TakeFirstCard()
    {
        return NowCards.Dequeue();
    }
}