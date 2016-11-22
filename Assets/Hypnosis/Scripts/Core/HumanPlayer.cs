using System;
using System.Collections;
using UnityEngine;

class HumanPlayer : Player
{
    public override void Play(GameController gamecontroller)
    {
        gamecontroller.GameState = new GameStateWaitingInput(gamecontroller, NowCards.Dequeue());
    }

    public override IEnumerator SelectCard(GameController gameController)
    {
        GameObject cardInterface = gameController.CardInterface;
        Transform topPanel = cardInterface.transform.GetChild(0).Find("Top Panel");
        Transform bottomPanel = cardInterface.transform.GetChild(0).Find("Bottom Panel");
        foreach(Transform cell in topPanel)
        {
            DragAndDropCell dragCell = cell.gameObject.GetComponent<DragAndDropCell>();
            dragCell.RemoveItem();
        }
        foreach(Transform cell in bottomPanel)
        {
            DragAndDropCell dragCell = cell.gameObject.GetComponent<DragAndDropCell>();
            dragCell.RemoveItem();
        }

        foreach(Transform cell in topPanel)
        {
            CardType nowCardType = CardPool[p_CardPool];
            p_CardPool++;

            GameObject cardPrefab = gameController.CardPrefabs.Find(card => card.GetComponent<DraggingCard>().type == nowCardType);
            GameObject cardObject = Instantiate(cardPrefab);
            DragAndDropCell dragCell = cell.gameObject.GetComponent<DragAndDropCell>();
            dragCell.PlaceItem(cardObject);
        }

        gameController.PassButton.SetActive(false);
        gameController.PickCardButton.SetActive(true);

        yield return null;
    }

    public void CardReady(Transform bottomPanel, Transform displayPanel)
    {
        int i = 0;
        foreach(Transform cell in bottomPanel)
        {
            GameObject card = cell.GetChild(0).gameObject;
            NowCards.Enqueue(card.GetComponent<DraggingCard>().type);

            DragAndDropCell displayCell = displayPanel.GetChild(i).gameObject.GetComponent<DragAndDropCell>();
            displayCell.PlaceItem(card);
            i++;
        }


    }
}