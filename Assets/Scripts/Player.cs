using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
using Random=UnityEngine.Random;

public class Player : NetworkBehaviour {
    public float speed = 4f;

    private MeshRenderer meshRenderer; 

    private Color notTeam = Color.white;

    public NetworkVariable<int> PlayerIdColor;
    private bool inTeam = false;

    public List<Color> teamRedColors = new List<Color>();
    public List<Color> teamBlueColors = new List<Color>();

    void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start() {
        if (IsOwner) {
            RandomSpawn();
        }
    }

    //Spawn Random de cada player al inicio
    public void RandomSpawn() {
        if (NetworkManager.Singleton.IsServer) {
            var randomPosition = new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f));;
            transform.position = randomPosition;
        } else {
            SubmitPositionRequestServerRpc();
        }
    }

    [ServerRpc]
    void SubmitPositionRequestServerRpc() {
        transform.position = new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f));;
    }

    //Función de movimiento del Player
    [ServerRpc]
    void MoveServerRpc(Vector3 dir) {
        transform.position += dir * speed * Time.deltaTime;
    }

    //Posicionador al 0, 1, 0
    [ClientRpc]
    public void MoveToCenterClientRpc() {
        PositionZeroZero();
    }

    void PositionZeroZero() {
        transform.position = new Vector3(0, 1, 0);
    }

    //Coloreador del Equipo Rojo
    [ServerRpc]
    void ColorizedRedServerRpc() {
        int idForTeamRedColor;
        idForTeamRedColor = ColorAcceptRed();
        meshRenderer.material.color = teamRedColors[idForTeamRedColor];
        PlayerIdColor.Value = idForTeamRedColor;
    }

    int ColorAcceptRed() {
        int idColor;
        bool sameColor;
        List<int> colorsInUse = new List<int>();
        foreach(ulong uid in NetworkManager.Singleton.ConnectedClientsIds) {
            colorsInUse.Add(NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<Player>().PlayerIdColor.Value);
        } do {
            idColor = Random.Range(0, teamRedColors.Count);
            sameColor = colorsInUse.Contains(idColor);
        } while (sameColor);
        return idColor;
    } 

    //Coloreador del Equipo Azul
    [ServerRpc]
    void ColorizedBlueServerRpc() {
        int idForTeamBlueColor;
        idForTeamBlueColor = ColorAcceptBlue();
        meshRenderer.material.color = teamBlueColors[idForTeamBlueColor];
        PlayerIdColor.Value = idForTeamBlueColor;
    }

    int ColorAcceptBlue() {
        int idColor;
        bool sameColor;
        List<int> colorsInUse = new List<int>();
        foreach(ulong uid in NetworkManager.Singleton.ConnectedClientsIds) {
            colorsInUse.Add(NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<Player>().PlayerIdColor.Value);
        } do {
            idColor = Random.Range(0, teamBlueColors.Count);
            sameColor = colorsInUse.Contains(idColor);
        } while (sameColor);
        return idColor;
    }

    void Update() {
        if(IsOwner) {
            if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
                MoveServerRpc(Vector3.right);
            }
            if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
                MoveServerRpc(Vector3.left);
            }
            if(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
                MoveServerRpc(Vector3.forward);
            }
            if(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
                MoveServerRpc(Vector3.back);
            }
            if(Input.GetKey(KeyCode.M)) {
                PositionZeroZero();
            }
            if(inTeam == false) {
                if(transform.position.x > 5f) {
                    inTeam = true;
                    ColorizedBlueServerRpc();
                } else if(transform.position.x < -5f) {
                    inTeam = true;
                    ColorizedRedServerRpc();
                } 
            }
            if (transform.position.x > -5f && transform.position.x < 5f) {
                meshRenderer.material.color = notTeam;
                inTeam = false;
            }
        }
    }
}
