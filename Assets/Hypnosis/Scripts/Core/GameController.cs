﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

/// <summary>
/// GameController class keeps track of the game, stores cells, units and players objects. It starts the game and makes turn transitions. 
/// It reacts to user interacting with units or cells, and raises events related to game progress. 
/// </summary>
public class GameController : MonoBehaviour
{
    public event EventHandler GameEnded;
    
    private GameState _gameState;//The grid delegates some of its behaviours to gameState object.
    public GameState GameState
    {
        private get
        {
            return _gameState;
        }
        set
        {
            if(_gameState != null)
                _gameState.OnStateExit();
            _gameState = value;
            _gameState.OnStateEnter();
        }
    }

    public int NumberOfPlayers { get; private set; }

    public Player CurrentPlayer
    {
        get { return Players[CurrentPlayerNumber]; }
    }

    [HideInInspector]
    public int CurrentPlayerNumber;
    public int FirstPlayerNumber;

    public Transform PlayersParent;
    public Transform CellParent;
    public Transform UnitsParent;
    public GameObject CardInterface;
    public GameObject PassButton;
    public GameObject PickCardButton;
    public Transform CardPanelBottom;
    public Transform CardPanelTop;

    public List<Player> Players { get; private set; }
    public List<Cell> Cells { get; private set; }
    public Dictionary<Vector2, Cell> CellMap { get; private set; }
    public List<Unit> Units { get; private set; }

    public List<GameObject> SpecialUnitPrefabs;
    public List<GameObject> CardPrefabs;

    void Start()
    {
        Players = new List<Player>();
        for (int i = 0; i < PlayersParent.childCount; i++)
        {
            var player = PlayersParent.GetChild(i).GetComponent<Player>();
            if (player != null)
                Players.Add(player);
            else
                Debug.LogError("Invalid object in Players Parent game object");
        }

        Players.Sort( (p1, p2) => (p1.PlayerNumber < p2.PlayerNumber)? -1:1 );

        NumberOfPlayers = Players.Count;
        FirstPlayerNumber = NumberOfPlayers -1;

        Cells = new List<Cell>();
        CellMap = new Dictionary<Vector2, Cell>();
        for (int i = 0; i < CellParent.childCount; i++)
        {
            var cell = CellParent.GetChild(i).gameObject.GetComponent<Cell>();
            if (cell != null)
            {
                Cells.Add(cell);
                CellMap[cell.OffsetCoord] = cell;
            }
            else
                Debug.LogError("Invalid object in cells paretn game object");
        }
      
        foreach (var cell in Cells)
        {
            cell.CellClicked += OnCellClicked;
            cell.CellHighlighted += OnCellHighlighted;
            cell.CellDehighlighted += OnCellDehighlighted;
        }
             
        var unitGenerator = GetComponent<IUnitGenerator>();
        if (unitGenerator != null)
        {
            Units = unitGenerator.SpawnUnits(Cells);
            foreach (var unit in Units)
            {
                unit.UnitClicked += OnUnitClicked;
                unit.UnitDestroyed += OnUnitDestroyed;
            }
        }
        else
            Debug.LogError("No IUnitGenerator script attached to cell grid");
        
        StartGame();
    }

    private void OnCellDehighlighted(object sender, EventArgs e)
    {
        GameState.OnCellDeselected(sender as Cell);
    }
    private void OnCellHighlighted(object sender, EventArgs e)
    {
        GameState.OnCellSelected(sender as Cell);
    } 
    private void OnCellClicked(object sender, EventArgs e)
    {
        GameState.OnCellClicked(sender as Cell);
    }

    private void OnUnitClicked(object sender, EventArgs e)
    {
        GameState.OnUnitClicked(sender as Unit);
    }
    private void OnUnitDestroyed(object sender, AttackEventArgs e)
    {
        Units.Remove(sender as Unit);
        var totalPlayersAlive = Units.Select(u => u.PlayerNumber).Distinct().ToList(); //Checking if the game is over
        if (totalPlayersAlive.Count == 1)
        {
            if(GameEnded != null)
                GameEnded.Invoke(this, new EventArgs());
        }
    }
    
    /// <summary>
    /// Method is called once, at the beggining of the game.
    /// </summary>
    public void StartGame()
    {
        Players.ForEach(p => p.InitCardPool());
        GameState = new GameStateRoundStart(this);
        StartCoroutine(StartRound());
    }

    public void SummonPrefab(CardType card, Cell cell)
    {
        GameObject summonPrefab = SpecialUnitPrefabs[CardHelper.convertToPrefabIndex(card)];
        var newObject = Instantiate(summonPrefab);
        newObject.transform.position = cell.transform.position;
        newObject.transform.parent = UnitsParent;

        Unit newUnit = newObject.GetComponent<Unit>();
        newUnit.PlayerNumber = CurrentPlayerNumber;
        newUnit.Cell = cell;
        cell.OccupyingUnit = newUnit;
        newUnit.UnitClicked += OnUnitClicked;
        newUnit.UnitDestroyed += OnUnitDestroyed;
        newUnit.Initialize();
        Units.Add(newUnit);
    }

    /// <summary>
    /// Method makes turn transitions. It is called at the end of each action.
    /// </summary>
    public void EndTurn()
    {
        GameState = new GameStateTurnChanging(this);

        Transform cardPanel;
        if (CurrentPlayerNumber == 0)
            cardPanel = CardPanelBottom.GetChild(0);
        else
            cardPanel = CardPanelTop.GetChild(0);

        foreach(Transform cell in cardPanel)
        {
            if(cell.childCount>0)
            {
                //cell.GetChild(0).GetComponent<DraggingCard>().DeActivate();
                cell.GetComponent<DragAndDropCell>().RemoveItem();
                break;
            }
        }

        CurrentPlayerNumber = (CurrentPlayerNumber + 1) % NumberOfPlayers;

        if(Players[CurrentPlayerNumber].NowCards.Count()==0)
        {
            GameState = new GameStateRoundStart(this);
            StartCoroutine(StartRound());
        }
        else
        {
            GetFirstCard(CurrentPlayerNumber).Activate();
            Players[CurrentPlayerNumber].Play(this);
        }
    }

    [HideInInspector]
    public bool localCardReady, remoteCardReady;
    IEnumerator StartRound()
    {
        localCardReady = false;
        remoteCardReady = false;

        foreach (var unit in Units)
        {
            if (unit.Buffs.Count > 0)
            {
                foreach (var buff in unit.Buffs)
                {
                    buff.Duration--;
                }
                List<Buff> buffToRemove = unit.Buffs.FindAll(buff => buff.Duration == 0);
                foreach (var buff in buffToRemove)
                {
                    unit.RemoveBuff(buff);
                }
            }
        }

        foreach (var player in Players)
        {
            StartCoroutine(player.SelectCard(this));
        }

        yield return new WaitUntil(() => localCardReady && remoteCardReady);

        FirstPlayerNumber = 1 - FirstPlayerNumber;
        CurrentPlayerNumber = FirstPlayerNumber;
        Debug.Log("Round Start! First player = " + FirstPlayerNumber);

        GetFirstCard(CurrentPlayerNumber).Activate();
        Players[CurrentPlayerNumber].Play(this);
    }

    public void CardReadyButtonPressed()
    {
        HumanPlayer humanPlayer = Players[0] as HumanPlayer;
        humanPlayer.CardReady(CardInterface.transform.GetChild(0).Find("Bottom Panel"), CardPanelBottom.GetChild(0));
        CardInterface.SetActive(false);
        PassButton.SetActive(true);
        localCardReady = true;

    }

    public void PickCardButtonPressed()
    {
        CardInterface.SetActive(true);
        PickCardButton.SetActive(false);
    }

    protected DraggingCard GetFirstCard(int playerNum)
    {
        Transform cardPanel;
        if (playerNum == 0)
            cardPanel = CardPanelBottom.GetChild(0);
        else
            cardPanel = CardPanelTop.GetChild(0);

        foreach(Transform cell in cardPanel)
        {
            if(cell.childCount>0)
            {
                DraggingCard card = cell.GetChild(0).GetComponent<DraggingCard>();
                return card;
            }
        }
        return null;
    }
}
