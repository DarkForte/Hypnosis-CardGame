// ----------------------------------------------------------------------------
// <copyright file="PunTurnManager.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
//  Manager for Turn Based games, using PUN
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;
using ExitGames = ExitGames.Client.Photon.Hashtable;

/// <summary>
/// Pun turnBased Game manager. 
/// Provides an Interface (IPunTurnManagerCallbacks) for the typical turn flow and logic, between players
/// Provides Extensions for PhotonPlayer, Room and RoomInfo to feature dedicated api for TurnBased Needs
/// </summary>
public class TurnManager : PunBehaviour
{
    /// <summary>
    /// Wraps accessing the "turn" custom properties of a room.
    /// </summary>
    /// <value>The turn index</value>
    public int Turn
    {
        get { return PhotonNetwork.room.GetTurn(); }
        private set
        {

            _isOverCallProcessed = false;

            PhotonNetwork.room.SetTurn(value, true);
        }
    }

    /// <summary>
    /// The duration of the turn in seconds.
    /// </summary>
    public float TurnDuration = 20f;

    /// <summary>
    /// Gets the elapsed time in the current turn in seconds
    /// </summary>
    /// <value>The elapsed time in the turn.</value>
    public float ElapsedTimeInTurn
    {
        get { return ((float)(PhotonNetwork.ServerTimestamp - PhotonNetwork.room.GetTurnStart())) / 1000.0f; }
    }


    /// <summary>
    /// Gets the remaining seconds for the current turn. Ranges from 0 to TurnDuration
    /// </summary>
    /// <value>The remaining seconds fo the current turn</value>
    public float RemainingSecondsInTurn
    {
        get { return Mathf.Max(0f, this.TurnDuration - this.ElapsedTimeInTurn); }
    }

    /// <summary>
    /// Gets a value indicating whether the current turn is over. That is the ElapsedTimeinTurn is greater or equal to the TurnDuration
    /// </summary>
    /// <value><c>true</c> if the current turn is over; otherwise, <c>false</c>.</value>
    public bool IsOver
    {
        get { return this.RemainingSecondsInTurn <= 0f; }
    }

    /// <summary>
    /// The turn manager listener. Set this to your own script instance to catch Callbacks
    /// </summary>
    public ITurnManagerCallbacks TurnManagerListener;

    /// <summary>
    /// The turn manager event offset event message byte. Used internaly for defining data in Room Custom Properties
    /// </summary>
    public const byte TurnManagerEventOffset = 0;

    /// <summary>
    /// The Move event message byte. Used internaly for saving data in Room Custom Properties
    /// </summary>
    public const byte EvMove = 1 + TurnManagerEventOffset;

    public const byte EvCard = 2 + TurnManagerEventOffset;

    // keep track of message calls
    private bool _isOverCallProcessed = false;

    #region MonoBehaviour CallBack
    /// <summary>
    /// Register for Event Call from PhotonNetwork.
    /// </summary>
    void Start()
    {
        PhotonNetwork.OnEventCall = OnEvent;
        PhotonPeer.RegisterType(typeof(Vector2), (byte)'W', SerializeVector2, DeserializeVector2);
    }

    void Update()
    {

    }
    #endregion

    /// <summary>
    /// Tells the TurnManager to begins a new turn.
    /// </summary>
    public void BeginTurn()
    {
        Turn = this.Turn + 1; // note: this will set a property in the room, which is available to the other players.
    }


    /// <summary>
	/// Call to send an action. Optionally finish the turn, too.
	/// The move object can be anything. Try to optimize though and only send the strict minimum set of information to define the turn move.
	/// </summary>
    /// <param name="move"></param>
    public void SendMove(object move)
    {
        // along with the actual move, we have to send which turn this move belongs to
        Hashtable moveHt = new Hashtable();
        moveHt.Add("turn", Turn);
        moveHt.Add("move", move);

        byte evCode = EvMove;
        PhotonNetwork.RaiseEvent(evCode, moveHt, true, new RaiseEventOptions() { CachingOption = EventCaching.AddToRoomCache });

        // the server won't send the event back to the origin (by default). to get the event, call it locally 
        // (note: the order of events might be mixed up as we do this locally)
        //OnEvent(evCode, moveHt, PhotonNetwork.player.ID);
    }

    public void SendCard(List<CardType> cards)
    {
        PhotonNetwork.RaiseEvent(EvCard, cards.ToArray(), true, new RaiseEventOptions() { CachingOption = EventCaching.AddToRoomCache });
    }

    #region Callbacks

    /// <summary>
    /// Called by PhotonNetwork.OnEventCall registration
    /// </summary>
    /// <param name="eventCode">Event code.</param>
    /// <param name="content">Content.</param>
    /// <param name="senderId">Sender identifier.</param>
    public void OnEvent(byte eventCode, object content, int senderId)
    {
        PhotonPlayer sender = PhotonPlayer.Find(senderId);
        switch (eventCode)
        {
            case EvMove:
                {
                    Hashtable evTable = content as Hashtable;
                    int turn = (int)evTable["turn"];
                    Vector2[] move = evTable["move"] as Vector2[];
                    this.TurnManagerListener.OnPlayerMove(sender, turn, new List<Vector2>(move));

                    break;
                }
            case EvCard:
                {
                    int[] intCards = content as int[];
                    List<CardType> cards = new List<CardType>();
                    foreach (int cardNum in intCards)
                        cards.Add((CardType)cardNum);

                    this.TurnManagerListener.OnPlayerChooseCard(sender, cards);
                    break;
                }
        }
    }

    /// <summary>
    /// Called by PhotonNetwork
    /// </summary>
    /// <param name="propertiesThatChanged">Properties that changed.</param>
    public override void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
    {

        //   Debug.Log("OnPhotonCustomRoomPropertiesChanged: "+propertiesThatChanged.ToStringFull());

        if (propertiesThatChanged.ContainsKey("Turn"))
        {
            _isOverCallProcessed = false;
        }
    }

    #endregion

    private static byte[] SerializeVector2(object customobject)
    {
        Vector2 vo = (Vector2)customobject;

        byte[] bytes = new byte[2 * 4];
        int index = 0;
        Protocol.Serialize(vo.x, bytes, ref index);
        Protocol.Serialize(vo.y, bytes, ref index);
        return bytes;
    }

    private static object DeserializeVector2(byte[] bytes)
    {
        Vector2 vo = new Vector2();
        int index = 0;
        Protocol.Deserialize(out vo.x, bytes, ref index);
        Protocol.Deserialize(out vo.y, bytes, ref index);
        return vo;
    }
}

public interface ITurnManagerCallbacks
{
    /// <summary>
    /// Called when a player moved (but did not finish the turn)
    /// </summary>
    /// <param name="player">Player reference</param>
    /// <param name="turn">Turn Index</param>
    /// <param name="move">Move Object data</param>
    void OnPlayerMove(PhotonPlayer player, int turn, List<Vector2> move);

    void OnPlayerChooseCard(PhotonPlayer player, List<CardType> cards);
}