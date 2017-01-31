﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using Photon;

/// <summary>
/// GameController class keeps track of the game, stores cells, units and players objects. It starts the game and makes turn transitions. 
/// It reacts to user interacting with units or cells, and raises events related to game progress. 
/// </summary>
public class GameController : PunBehaviour, ITurnManagerCallbacks
{
    
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
        get { return Players.Find(player => player.PlayerNumber == CurrentPlayerNumber); }
    }

    [HideInInspector]
    public int CurrentPlayerNumber;
    [HideInInspector]
    public int FirstPlayerNumber;
    int PlayerNumberSum;
    int RoundNumber;

    public Transform PlayersParent;
    public Transform CellParent;
    public Transform UnitsParent;

    public List<Player> Players { get; private set; }
    public List<Cell> Cells { get; private set; }
    public Dictionary<Vector2, Cell> CellMap { get; private set; }
    public List<Unit> Units { get; private set; }

    public List<GameObject> SpecialUnitPrefabs;

    public Player LocalPlayer;
    public Player RemotePlayer;

    public TurnManager TurnManager;
    public UIController uiController;

    public PresetLogger logger;


    void Start()
    {
        Players = new List<Player>();
        foreach(Transform playerTrans in PlayersParent)
        {
            Players.Add(playerTrans.GetComponent<Player>());
        }

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

        TurnManager.TurnManagerListener = this;
        GameState = new GameStateGameOver(this);
    }
    
    /// <summary>
    /// Method is called once, at the beggining of the game.
    /// </summary>
    public void StartGame()
    {
        uiController.HideConnectUI();

        NumberOfPlayers = Players.Count;
        PlayerNumberSum = 0;
        foreach (var player in PhotonNetwork.playerList)
            PlayerNumberSum += player.ID;

        FirstPlayerNumber = PlayerNumberSum - PhotonNetwork.room.masterClientId;

        var unitGenerator = GetComponent<IUnitGenerator>();
        if (unitGenerator != null)
        {
            Units = unitGenerator.SpawnUnits(CellMap);
            foreach (var unit in Units)
            {
                unit.UnitClicked += OnUnitClicked;
                unit.UnitDestroyed += OnUnitDestroyed;
                unit.UnitHighlighted += OnUnitHighlighted;
                unit.UnitDehighlighted += OnUnitDehighlighted;
                unit.logger = logger;
            }
        }
        else
            Debug.LogError("No IUnitGenerator script attached to cell grid");

        foreach (Unit unit in Units)
        {
            if (unit.PlayerNumber == 0)
                unit.PlayerNumber = LocalPlayer.PlayerNumber;
            else
                unit.PlayerNumber = RemotePlayer.PlayerNumber;
        }

        Players.ForEach(p => p.InitCardPool());
        Players.ForEach(p => p.NowCards.Clear());
        RoundNumber = 0;
        GameState = new GameStateRoundStart(this);
        uiController.SetLocalPlayer(LocalPlayer);
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
        newUnit.isFriendUnit = (CurrentPlayerNumber == LocalPlayer.PlayerNumber);
        newUnit.Cell = cell;
        cell.OccupyingUnit = newUnit;
        newUnit.UnitClicked += OnUnitClicked;
        newUnit.UnitDestroyed += OnUnitDestroyed;
        newUnit.UnitHighlighted += OnUnitHighlighted;
        newUnit.UnitDehighlighted += OnUnitDehighlighted;
        newUnit.Initialize();
        newUnit.logger = logger;
        Units.Add(newUnit);

        Units.ForEach(unit => unit.OnUnitCreate(newUnit, this));

        logger.LogSummon(CurrentPlayer, newUnit);
    }

    /// <summary>
    /// Method makes turn transitions. It is called at the end of each action.
    /// </summary>
    public void EndTurn()
    {
        uiController.EndTurn(CurrentPlayerNumber);
        if (GameState is GameStateGameOver)
            return;

        Units.ForEach(unit => unit.OnTurnEnd(this));

        GameState = new GameStateTurnChanging(this);
        CurrentPlayerNumber = PlayerNumberSum - CurrentPlayerNumber;

        if(CurrentPlayer.NowCards.Count()==0)
        {
            if(RoundNumber == 9)
            {
                JudgeResult();
                EndGame();
            }
            else
            { 
                GameState = new GameStateRoundStart(this);
                StartCoroutine(StartRound());
            }

        }
        else
        {
            uiController.GetFirstCard(CurrentPlayerNumber).Activate();
            CurrentPlayer.Play(this);
        }
    }

    [HideInInspector]
    public bool localCardReady, remoteCardReady;

    IEnumerator StartRound()
    {
        RoundNumber++;

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

        FirstPlayerNumber = PlayerNumberSum - FirstPlayerNumber;
        CurrentPlayerNumber = FirstPlayerNumber;

        logger.Log("Next round: Round " + RoundNumber);
        logger.Log(String.Format("Player {0} acts first.", CurrentPlayerNumber), logger.GetColor(CurrentPlayer is HumanPlayer));

        foreach (var player in Players)
        {
            StartCoroutine(player.SelectCard(this, uiController));
        }

        yield return new WaitUntil(() => localCardReady && remoteCardReady);


        Debug.Log("Round Start! First player = " + FirstPlayerNumber);
        logger.Log(String.Format("Round {0} start!", RoundNumber), PresetLogger.DefaultColor);

        uiController.GetFirstCard(CurrentPlayerNumber).Activate();
        CurrentPlayer.Play(this);
    }

    public void PassButtonPressed()
    {
        if(CurrentPlayer == LocalPlayer)
        {
            TurnManager.SendMove(new Vector2[0]);
            logger.LogPass(CurrentPlayer);
            EndTurn();
        }
    }

    public void CardReadyButtonPressed()
    {
        HumanPlayer humanPlayer = LocalPlayer as HumanPlayer;
        humanPlayer.EquipCard(uiController.CardInterfaceBottomPanel(), uiController.LocalEquipCardPanel());
        TurnManager.SendCard(humanPlayer.NowCards.ToList());
        localCardReady = true;
    }

    public void CancelButtonPressed()
    {
        GameState = new GameStateWaitingInput(this, CardType.SPECIAL);
    }

    private void JudgeResult()
    {
        List<Unit> bases = Units.FindAll(unit => unit is Base);
        if (bases[0].HP == bases[1].HP)
        {
            logger.LogTie();
        }
        else
        {
            int winner = bases[0].HP > bases[1].HP ? bases[0].PlayerNumber : bases[1].PlayerNumber;
            logger.LogWinner(Players.Find(p => p.PlayerNumber == winner));
        }
    }

    private void EndGame()
    {
        GameState = new GameStateGameOver(this);
    }

    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.room.playerCount == 2)
        { 
            LocalPlayer.GetComponent<Player>().PlayerNumber = PhotonNetwork.player.ID;
            RemotePlayer.GetComponent<Player>().PlayerNumber = PhotonNetwork.otherPlayers[0].ID;
            StartGame();
        }
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (PhotonNetwork.room.playerCount == 2)
        {
            LocalPlayer.GetComponent<Player>().PlayerNumber = PhotonNetwork.player.ID;
            RemotePlayer.GetComponent<Player>().PlayerNumber = PhotonNetwork.otherPlayers[0].ID;
            StartGame();
        }
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

    private void OnUnitHighlighted(object sender, EventArgs e)
    {
        GameState.OnUnitSelected(sender as Unit);
    }

    private void OnUnitDehighlighted(object sender, EventArgs e)
    {
        GameState.OnUnitDeselected(sender as Unit);
    }

    private void OnUnitClicked(object sender, EventArgs e)
    {
        GameState.OnUnitClicked(sender as Unit);
    }

    private void OnUnitDestroyed(object sender, AttackEventArgs e)
    {
        Unit destroyedUnit = sender as Unit;
        Units.Remove(destroyedUnit);

        if(destroyedUnit is Base)
        {
            Player loser = Players.Find(p => p.PlayerNumber == destroyedUnit.PlayerNumber);
            Player winner = Players.Find(p => p.PlayerNumber != destroyedUnit.PlayerNumber);
            logger.LogBaseDestroyed(loser);
            logger.LogWinner(winner);

            EndGame();
        }
        
    }

    void ITurnManagerCallbacks.OnPlayerMove(PhotonPlayer player, int turn, List<Vector2> move)
    {
        Debug.Log("Received remote message: ");
        move.ForEach(x => Debug.Log(x));
        GameState.OnReceiveNetMessage(player, move);
    }

    void ITurnManagerCallbacks.OnPlayerChooseCard(PhotonPlayer player, List<CardType> cards)
    {
        RemotePlayer remotePlayer = Players.Find(p => p is RemotePlayer) as RemotePlayer;
        cards.ForEach(card => remotePlayer.NowCards.Enqueue(card));

        uiController.ShowRemoteCard(cards);

        remoteCardReady = true;
    }
}
