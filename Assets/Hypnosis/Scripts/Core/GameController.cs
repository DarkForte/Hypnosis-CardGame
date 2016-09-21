using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

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
    public int CurrentPlayerNumber { get; private set; }
    public int FirstPlayerNumber;

    public Transform PlayersParent;
    public Transform CellParent;

    public List<Player> Players { get; private set; }
    public List<Cell> Cells { get; private set; }
    public List<Unit> Units { get; private set; }

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
        for (int i = 0; i < CellParent.childCount; i++)
        {
            var cell = CellParent.GetChild(i).gameObject.GetComponent<Cell>();
            if (cell != null)
                Cells.Add(cell);
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
        StartRound();
    }

    public void StartRound()
    {
        foreach(var player in Players)
        {
            List<CardType> nowCandidates = player.DrawCards(5);

            List<CardType> nowCards = new List<CardType>();
            for (int j = 0; j < 3; j++)
            {
                nowCards.Add(nowCandidates[j]);
            }

            player.NowCards = nowCards;
            player.p_NowCards = 0;
        }
        FirstPlayerNumber = 1 - FirstPlayerNumber;
        CurrentPlayerNumber = FirstPlayerNumber;
        Debug.Log("Round Start! First player = " + FirstPlayerNumber);
        Players[CurrentPlayerNumber].Play(this);
    }

    /// <summary>
    /// Method makes turn transitions. It is called at the end of each action.
    /// </summary>
    public void EndTurn()
    {
        GameState = new GameStateTurnChanging(this);

        CurrentPlayerNumber = (CurrentPlayerNumber + 1) % NumberOfPlayers;

        if(Players[CurrentPlayerNumber].p_NowCards == 3)
        {
            GameState = new GameStateRoundStart(this);
            StartRound();
        }
        else
        {
            Players[CurrentPlayerNumber].Play(this);
        }
    }
}
