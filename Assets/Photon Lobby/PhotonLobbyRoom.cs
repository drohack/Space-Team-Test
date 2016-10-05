﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkerInGame.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using OVRTouchSample;

public class PhotonLobbyRoom : Photon.MonoBehaviour
{
    public static readonly string SceneNameGame = "Game";

    private bool isOtherPlayerJoining = false;

    public Transform playerPrefab;
    public Transform player1Spawn;
    public Transform player2Spawn;
    public Transform player3Spawn;
    public Transform player4Spawn;

    [SerializeField]
    Console_Text_Script p1_ConsoleText;
    [SerializeField]
    Console_Text_Script p2_ConsoleText;
    [SerializeField]
    Console_Text_Script p3_ConsoleText;
    [SerializeField]
    Console_Text_Script p4_ConsoleText;

    [SerializeField]
    Players_Text_Script p1_PlayersText;
    [SerializeField]
    Players_Text_Script p2_PlayersText;
    [SerializeField]
    Players_Text_Script p3_PlayersText;
    [SerializeField]
    Players_Text_Script p4_PlayersText;

    [SerializeField]
    Transform p1_LeaveButtonTransform;
    [SerializeField]
    Transform p2_LeaveButtonTransform;
    [SerializeField]
    Transform p3_LeaveButtonTransform;
    [SerializeField]
    Transform p4_LeaveButtonTransform;

    [SerializeField]
    Transform p1_ReadyLeverTransform;
    [SerializeField]
    Transform p2_ReadyLeverTransform;
    [SerializeField]
    Transform p3_ReadyLeverTransform;
    [SerializeField]
    Transform p4_ReadyLeverTransform;

    GameObject p1_Ready_Lever;
    GameObject p2_Ready_Lever;
    GameObject p3_Ready_Lever;
    GameObject p4_Ready_Lever;

    private bool isP1Ready = false;
    private bool isP2Ready = false;
    private bool isP3Ready = false;
    private bool isP4Ready = false;

    public void Awake()
    {
        // in case we started this demo with the wrong scene being active, simply load the menu scene
        if (!PhotonNetwork.connected)
        {
            SceneManager.LoadScene(PhotonLobby.SceneNameMenu);
            return;
        }
        
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.InstantiateSceneObject("LobbyRoom/Leave_Button", p1_LeaveButtonTransform.position, p1_LeaveButtonTransform.rotation, 0, null);
            PhotonNetwork.InstantiateSceneObject("LobbyRoom/Leave_Button", p2_LeaveButtonTransform.position, p2_LeaveButtonTransform.rotation, 0, null);
            PhotonNetwork.InstantiateSceneObject("LobbyRoom/Leave_Button", p3_LeaveButtonTransform.position, p3_LeaveButtonTransform.rotation, 0, null);
            PhotonNetwork.InstantiateSceneObject("LobbyRoom/Leave_Button", p4_LeaveButtonTransform.position, p4_LeaveButtonTransform.rotation, 0, null);

            object[] data1 = new object[1] { 0 };
            object[] data2 = new object[1] { 1 };
            object[] data3 = new object[1] { 2 };
            object[] data4 = new object[1] { 3 };

            p1_Ready_Lever = PhotonNetwork.InstantiateSceneObject("LobbyRoom/Ready_Lever", p1_ReadyLeverTransform.position, p1_ReadyLeverTransform.rotation, 0, data1);
            p2_Ready_Lever = PhotonNetwork.InstantiateSceneObject("LobbyRoom/Ready_Lever", p2_ReadyLeverTransform.position, p2_ReadyLeverTransform.rotation, 0, data2);
            p3_Ready_Lever = PhotonNetwork.InstantiateSceneObject("LobbyRoom/Ready_Lever", p3_ReadyLeverTransform.position, p3_ReadyLeverTransform.rotation, 0, data3);
            p4_Ready_Lever = PhotonNetwork.InstantiateSceneObject("LobbyRoom/Ready_Lever", p4_ReadyLeverTransform.position, p4_ReadyLeverTransform.rotation, 0, data4);
        }

        //Disable Main Camera (we will be using the OvrRigPhoton's camera
        GameObject.Find("Main Camera").SetActive(false);
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(isP1Ready);
            stream.SendNext(isP2Ready);
            stream.SendNext(isP3Ready);
            stream.SendNext(isP4Ready);
        }
        else
        {
            //Network player, receive data
            isP1Ready = (bool)stream.ReceiveNext();
            isP2Ready = (bool)stream.ReceiveNext();
            isP3Ready = (bool)stream.ReceiveNext();
            isP4Ready = (bool)stream.ReceiveNext();
        }
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Return to Lobby"))
        {
            PhotonNetwork.LeaveRoom();  // we will load the menu level when we successfully left the room
        }
    }

    public void OnMasterClientSwitched(PhotonPlayer player)
    {
        Debug.Log("OnMasterClientSwitched: " + player);

        string message;
        InRoomChat chatComponent = GetComponent<InRoomChat>();  // if we find a InRoomChat component, we print out a short message

        if (chatComponent != null)
        {
            // to check if this client is the new master...
            if (player.isLocal)
            {
                message = "You are Master Client now.";

                // Get all of the Ready_Levers
                foreach(GameObject readyLever in GameObject.FindGameObjectsWithTag("Ready_Lever"))
                {
                    Ready_Lever_Script readyLeverScript = readyLever.GetComponent<Ready_Lever_Script>();
                    if (readyLeverScript.playerPosition == 0)
                        p1_Ready_Lever = readyLever;
                    else if (readyLeverScript.playerPosition == 1)
                        p2_Ready_Lever = readyLever;
                    else if (readyLeverScript.playerPosition == 2)
                        p3_Ready_Lever = readyLever;
                    else if (readyLeverScript.playerPosition == 3)
                        p4_Ready_Lever = readyLever;
                }
            }
            else
            {
                message = player.name + " is Master Client now.";
            }


            chatComponent.AddLine(message); // the Chat method is a RPC. as we don't want to send an RPC and neither create a PhotonMessageInfo, lets call AddLine()
        }
    }

    public void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom (local)");

        PhotonNetwork.player.customProperties.Clear();

        // back to main menu
        SceneManager.LoadScene(PhotonLobby.SceneNameMenu);
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("OnDisconnectedFromPhoton");

        PhotonNetwork.player.customProperties.Clear();

        // back to main menu
        SceneManager.LoadScene(PhotonLobby.SceneNameMenu);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("OnPhotonInstantiate " + info.sender);    // you could use this info to store this or react
    }

    public void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("OnPhotonPlayerConnected: " + newPlayer);
        //Set new players pPos and update the pPosOccupied (if masterclient)
        if (PhotonNetwork.isMasterClient)
        {
            //wait till you've finished adding the other player to join the room
            StartCoroutine(UpdatePlayerWhenJoined(newPlayer));
        }
    }

    private System.Collections.IEnumerator UpdatePlayerWhenJoined(PhotonPlayer newPlayer)
    {
        while (isOtherPlayerJoining)
        {
            yield return null;
        }

        isOtherPlayerJoining = true;
        int newPlayerPos = 0;
        bool[] playerPosOccupied = (bool[])PhotonNetwork.room.customProperties[PhotonConstants.pPosOccupied];
        //Find the first open spot in pPosOccupied and set the new player's position
        for (int i = 0; i < playerPosOccupied.Length; i++)
        {
            if (playerPosOccupied[i] == false)
            {
                playerPosOccupied[i] = true;
                newPlayerPos = i;
                break;
            }
        }

        //Update the room's pPosOccupied
        Hashtable ht1 = new Hashtable() { { PhotonConstants.pPosOccupied, playerPosOccupied } };
        PhotonNetwork.room.SetCustomProperties(ht1);
        //Update new player's pPos
        Hashtable ht2 = new Hashtable() { { PhotonConstants.pPos, newPlayerPos } };
        newPlayer.SetCustomProperties(ht2);
        isOtherPlayerJoining = false;

        UpdatePlayerText();
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.Log("OnPlayerDisconneced: " + otherPlayer);

        if (PhotonNetwork.isMasterClient)
        {
            //Open up player position
            if (otherPlayer.customProperties.ContainsKey(PhotonConstants.pPos) && PhotonNetwork.room.customProperties.ContainsKey(PhotonConstants.pPosOccupied))
            {
                int otherPlayerPos = (int)otherPlayer.customProperties[PhotonConstants.pPos];
                bool[] playerPosOccupied = (bool[])PhotonNetwork.room.customProperties[PhotonConstants.pPosOccupied];
                playerPosOccupied[otherPlayerPos] = false;
                Hashtable ht = new Hashtable() { { PhotonConstants.pPosOccupied, playerPosOccupied } };
                PhotonNetwork.room.SetCustomProperties(ht);
                otherPlayer.customProperties.Clear();
            }

            //Reset all ready players
            isP1Ready = false;
            isP2Ready = false;
            isP3Ready = false;
            isP4Ready = false;

            p1_Ready_Lever.GetPhotonView().RPC("RPCLowerReadyLever", PhotonTargets.All);
            p2_Ready_Lever.GetPhotonView().RPC("RPCLowerReadyLever", PhotonTargets.All);
            p3_Ready_Lever.GetPhotonView().RPC("RPCLowerReadyLever", PhotonTargets.All);
            p4_Ready_Lever.GetPhotonView().RPC("RPCLowerReadyLever", PhotonTargets.All);

            UpdatePlayerText();
        }
    }

    public void OnFailedToConnectToPhoton()
    {
        Debug.Log("OnFailedToConnectToPhoton");

        // back to main menu
        SceneManager.LoadScene(PhotonLobby.SceneNameMenu);
    }

    void OnLevelWasLoaded(int level)
    {
        //wait till you've finished adding the other player to join the room
        StartCoroutine(SpawnOvrRigPhoton());
        
    }

    private System.Collections.IEnumerator SpawnOvrRigPhoton()
    {
        while (!PhotonNetwork.player.customProperties.ContainsKey(PhotonConstants.pPos))
        {
            yield return null;
        }

        //Find which position your player is in
        if (PhotonNetwork.player.customProperties.ContainsKey(PhotonConstants.pPos))
        {
            int playerPosition = (int)PhotonNetwork.player.customProperties[PhotonConstants.pPos];
            Debug.Log("name: " + PhotonNetwork.player.name + "; pPos: " + playerPosition);

            //Get transform of your position
            Transform currentPlayerTransform = player1Spawn;
            if (playerPosition == 0)
                currentPlayerTransform = player1Spawn;
            else if (playerPosition == 1)
                currentPlayerTransform = player2Spawn;
            else if (playerPosition == 2)
                currentPlayerTransform = player3Spawn;
            else if (playerPosition == 3)
                currentPlayerTransform = player4Spawn;

            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            Transform ovrRigPhoton = PhotonNetwork.Instantiate(this.playerPrefab.name, currentPlayerTransform.position, currentPlayerTransform.rotation, 0).transform;

            // Set your name
            ovrRigPhoton.name = ovrRigPhoton.name + "-" + PhotonNetwork.playerName;

            string roomName = " " + PhotonNetwork.room.name;
            p1_ConsoleText.photonView.RPC("RpcTypeText", PhotonTargets.All, roomName);
            p2_ConsoleText.photonView.RPC("RpcTypeText", PhotonTargets.All, roomName);
            p3_ConsoleText.photonView.RPC("RpcTypeText", PhotonTargets.All, roomName);
            p4_ConsoleText.photonView.RPC("RpcTypeText", PhotonTargets.All, roomName);

            UpdatePlayerText();
        }
    }

    private void UpdatePlayerText()
    {
        if(PhotonNetwork.isMasterClient)
        {
            CheckIfAllPlayersReady();

            string text = "";
            string p1_name = "";
            string p2_name = "";
            string p3_name = "";
            string p4_name = "";

            foreach(PhotonPlayer player in PhotonNetwork.playerList)
            {
                int playerPosition = (int)player.customProperties[PhotonConstants.pPos];

                if (playerPosition == 0)
                    p1_name = player.name;
                else if (playerPosition == 1)
                    p2_name = player.name;
                else if (playerPosition == 2)
                    p3_name = player.name;
                else if (playerPosition == 3)
                    p4_name = player.name;
            }

            text = "Player 1: " + p1_name;
            if (isP1Ready)
                text += " ✓";
            text += System.Environment.NewLine;
            text += "Player 2: " + p2_name;
            if (isP2Ready)
                text += " ✓";
            text += System.Environment.NewLine;
            text += "Player 3: " + p3_name;
            if (isP3Ready)
                text += " ✓";
            text += System.Environment.NewLine;
            text += "Player 4: " + p4_name;
            if (isP4Ready)
                text += " ✓";

            p1_PlayersText.photonView.RPC("RpcUpdateText", PhotonTargets.All, text);
            p2_PlayersText.photonView.RPC("RpcUpdateText", PhotonTargets.All, text);
            p3_PlayersText.photonView.RPC("RpcUpdateText", PhotonTargets.All, text);
            p4_PlayersText.photonView.RPC("RpcUpdateText", PhotonTargets.All, text);
        }
    }

    private void CheckIfAllPlayersReady()
    {
        if (PhotonNetwork.isMasterClient)
        {
            bool[] playerPosOccupied = (bool[])PhotonNetwork.room.customProperties[PhotonConstants.pPosOccupied];

            if (playerPosOccupied != null && playerPosOccupied.Length == 4)
            {
                if (((playerPosOccupied[0] && isP1Ready) || !playerPosOccupied[0])
                    && ((playerPosOccupied[1] && isP2Ready) || !playerPosOccupied[1])
                    && ((playerPosOccupied[2] && isP3Ready) || !playerPosOccupied[2])
                    && ((playerPosOccupied[3] && isP4Ready) || !playerPosOccupied[3]))
                {
                    PhotonNetwork.LoadLevel(SceneNameGame); //Start Game
                }
            }
        }
    }

    public void SendReadyCommand(bool isReady, int playerPosition)
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (playerPosition == 0)
                isP1Ready = isReady;
            else if (playerPosition == 1)
                isP2Ready = isReady;
            else if (playerPosition == 2)
                isP3Ready = isReady;
            else if (playerPosition == 3)
                isP4Ready = isReady;

            UpdatePlayerText();
        }
    }
}