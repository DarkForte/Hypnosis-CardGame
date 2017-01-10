using System;
using System.Collections;
using UnityEngine;

class HumanPlayer : Player
{
    public override void Play(GameController gamecontroller)
    {
        gamecontroller.GameState = new GameStateWaitingInput(gamecontroller, NowCards.Dequeue());
    }

    public override IEnumerator SelectCard(GameController gameController, UIController uiController)
    {
        GameObject cardInterface = uiController.CardInterface;
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

            GameObject cardPrefab = uiController.CardPrefabs.Find(card => card.GetComponent<DraggingCard>().type == nowCardType);
            GameObject cardObject = Instantiate(cardPrefab);
            DragAndDropCell dragCell = cell.gameObject.GetComponent<DragAndDropCell>();
            dragCell.PlaceItem(cardObject);
        }

        uiController.PassButton.SetActive(false);
        uiController.PickCardButton.SetActive(true);

        yield return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="selectPanel">The panel in Card Inteface</param>
    /// <param name="equipPanel">The panel under board</param>
    public void EquipCard(Transform selectPanel, Transform equipPanel)
    {
        int i = 0;
        foreach(Transform cell in selectPanel)
        {
            GameObject card = cell.GetChild(0).gameObject;
            NowCards.Enqueue(card.GetComponent<DraggingCard>().type);

            DragAndDropCell displayCell = equipPanel.GetChild(i).gameObject.GetComponent<DragAndDropCell>();
            displayCell.PlaceItem(card);
            i++;
        }
    }

}