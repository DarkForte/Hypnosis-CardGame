using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class RandomAIPlayer : HumanPlayer
{
    public override IEnumerator SelectCard(GameController gameController, UIController uiController)
    {
        List<CardType> tmpCardList = new List<CardType>();
        for(int i=1; i<=5; i++)
        {
            CardType nowCard = CardPool[p_CardPool];
            p_CardPool++;

            if (i <= 3)
            {
                tmpCardList.Add(nowCard);
            }
        }

        Transform cardPanel = uiController.CardPanelTop.GetChild(0);

        for(int i=0; i<tmpCardList.Count; i++)
        {
            CardType nowCard = tmpCardList[i];
            GameObject cardPrefab = uiController.CardPrefabs[(int)nowCard];
            GameObject cardObject = Instantiate(cardPrefab);
            cardPanel.GetChild(i).GetComponent<DragAndDropCell>().PlaceItem(cardObject);

            NowCards.Enqueue(nowCard);
        }

        gameController.remoteCardReady = true;

        yield return null;
    }
}

