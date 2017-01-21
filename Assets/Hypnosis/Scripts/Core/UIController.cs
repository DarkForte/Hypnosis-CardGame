using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject CardInterface;
    public GameObject PassButton;
    public GameObject PickCardButton;
    public GameObject ConnectButton;
    public Transform CardPanelBottom;
    public Transform CardPanelTop;

    [HideInInspector]
    public Player localPlayer;

    public List<GameObject> CardPrefabs;

    public void SetLocalPlayer(Player localPlayer)
    { 
        this.localPlayer = localPlayer;
    }

    public void EndTurn(int currentPlayerNumber)
    {
        Transform cardPanel;
        if (currentPlayerNumber == localPlayer.PlayerNumber)
            cardPanel = CardPanelBottom.GetChild(0);
        else
            cardPanel = CardPanelTop.GetChild(0);

        foreach (Transform cell in cardPanel)
        {
            if (cell.childCount > 0)
            {
                //cell.GetChild(0).GetComponent<DraggingCard>().DeActivate();
                cell.GetComponent<DragAndDropCell>().RemoveItem();
                break;
            }
        }
    }

    public void CardReadyButtonPressed()
    {
        CardInterface.SetActive(false);
        PassButton.SetActive(true);
    }

    public void PickCardButtonPressed()
    {
        CardInterface.SetActive(true);
        PickCardButton.SetActive(false);
    }

    public Transform CardInterfaceBottomPanel()
    {
        return CardInterface.transform.GetChild(0).Find("Bottom Panel");
    }

    public Transform LocalEquipCardPanel()
    {
        return CardPanelBottom.GetChild(0);
    }

    public DraggingCard GetFirstCard(int playerNum)
    {
        Transform cardPanel;
        if (playerNum == localPlayer.PlayerNumber)
            cardPanel = CardPanelBottom.GetChild(0);
        else
            cardPanel = CardPanelTop.GetChild(0);

        foreach (Transform cell in cardPanel)
        {
            if (cell.childCount > 0)
            {
                DraggingCard card = cell.GetChild(0).GetComponent<DraggingCard>();
                return card;
            }
        }
        return null;
    }

    public void ShowRemoteCard(List<CardType> cards)
    {
        Transform topPanel = CardPanelTop.GetChild(0);
        int i = 0;
        foreach (CardType card in cards)
        {
            GameObject cardObject = Instantiate(CardPrefabs[(int)card]);
            cardObject.GetComponent<DraggingCard>().Hide();
            DragAndDropCell cell = topPanel.GetChild(i).GetComponent<DragAndDropCell>();
            cell.PlaceItem(cardObject);
            i++;
        }
    }

    public void HideInGameUI()
    {
        PassButton.SetActive(false);
        PickCardButton.SetActive(false);
    }

    public void ShowConnectUI()
    {
        ConnectButton.SetActive(true);
    }

    public void HideConnectUI()
    {
        ConnectButton.SetActive(false);
    }
}

