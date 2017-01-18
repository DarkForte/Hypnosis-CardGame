using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon;
using UnityEngine;

class ConnectionController : PunBehaviour
{
    public string UserId;
    public string previousRoom;

    const string NickNamePlayerPrefsKey = "NickName";
    public PresetLogger logger;
    

    public void Connect()
    {
        if(!PhotonNetwork.connected)
        {
            logger.Log("Connecting...");

            if (PhotonNetwork.AuthValues == null)
            {
                PhotonNetwork.AuthValues = new AuthenticationValues();
            }

            PhotonNetwork.playerName = "UserName";
            PhotonNetwork.ConnectUsingSettings("1");

            // this way we can force timeouts by pausing the client (in editor)
            PhotonHandler.StopFallbackSendAckThread();
        }
        else
        {
            logger.Log("Rejoin room...");
            if (PhotonNetwork.inRoom)
                PhotonNetwork.LeaveRoom();
            else
                PhotonNetwork.JoinRandomRoom();
        }

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("in OnConnectedToMaster");
        // after connect 
        this.UserId = PhotonNetwork.player.userId;

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("in OnJoinedLobby");
        OnConnectedToMaster(); // this way, it does not matter if we join a lobby or not
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 2, PlayerTtl = 5000 }, null);
    }

    public override void OnJoinedRoom()
    {
        string logText = "Joined room: " + PhotonNetwork.room.name;
        Debug.Log(logText);
        logger.Log(logText);
        this.previousRoom = PhotonNetwork.room.name;

    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        this.previousRoom = null;
    }

    public override void OnConnectionFail(DisconnectCause cause)
    {
        string logMsg = "Disconnected due to: " + cause + ". this.previousRoom: " + this.previousRoom;
        Debug.Log(logMsg);
        logger.Log(logMsg);
    }
}

