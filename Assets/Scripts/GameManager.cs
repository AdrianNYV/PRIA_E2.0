using Unity.Netcode;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    /*
    public NetworkVariable<int> redTeamPlayers = new NetworkVariable<int>();
    public NetworkVariable<int> blueTeamPlayers = new NetworkVariable<int>();
    public NetworkVariable<int>  someTeamIsFull = new NetworkVariable<int>();
    public int maxSizeForATeamGameManager = 2;
    */
    void Awake() {
        instance = this;
    }

    void Start() {
        //someTeamIsFull.Value = 0;
    }
    
    void OnGUI() {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer) {
            StartButtons();
        } else {
            StatusLabels();
            SubmitCentralPosition();
        }
        GUILayout.EndArea();
    }

    void StartButtons() {
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();   
    }

    void StatusLabels() {
        var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
        GUILayout.Label("Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
    }

    static void SubmitCentralPosition() {
        if (GUILayout.Button(NetworkManager.Singleton.IsServer ? "Mover al Inicio" : "Request Mover al Inicio")) {
            if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient ) {
                foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
                    NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<Player>().MoveToZoneNotTeam();
            } else {
                var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
                var player = playerObject.GetComponent<Player>();
                player.SubmitPositionRequestServerRpc();
            }
        }
    }
    /*
    public void CheckIfOneTeamIsFull() {
        if(redTeamPlayers.Value >= maxSizeForATeamGameManager) {
            someTeamIsFull.Value = 1;
        } else if(blueTeamPlayers.Value >= maxSizeForATeamGameManager) {
            someTeamIsFull.Value = 2;
        } else {
            someTeamIsFull.Value = 0;
        }
        FreeMovementYesOrNot();
    }

    public void FreeMovementYesOrNot() {
        List<NetworkObject> playersWithFreeMovement = new List<NetworkObject>();
        foreach(ulong uid in NetworkManager.Singleton.ConnectedClientsIds) {
            if(someTeamIsFull.Value != 0) {
                if(NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<Player>().inTeamNumber.Value != someTeamIsFull.Value) {
                    playersWithFreeMovement.Add(NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid));
                } 
            } else {
                playersWithFreeMovement.Add(NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid));
            }
            ClientRpcParams clientRpcParams = new ClientRpcParams();
            foreach(NetworkObject player in playersWithFreeMovement) {
                player.GetComponent<Player>().SetFreeMoveClientRpc(someTeamIsFull.Value == 0, clientRpcParams);
            }
        }
    }
    */
}
