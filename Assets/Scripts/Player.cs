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

    public NetworkVariable<int> PlayerIdColorBlue;
    public NetworkVariable<int> PlayerIdColorRed;
    private float inTeam = 0f;

    public List<Color> teamRedColors = new List<Color>();
    public List<Color> teamBlueColors = new List<Color>();
    /*
    private bool freeMove;
    public NetworkVariable<bool> isFreeMoveActive = new NetworkVariable<bool>();
    public NetworkVariable<int> inTeamNumber = new NetworkVariable<int>();
    public int maxSizeForATeam = 2;
    */
    void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public override void OnNetworkSpawn() {
        //isFreeMoveActive.Value = true;
    }

    void Start() {
        if (IsOwner) {
            RandomSpawn();
        }
    }

    //Spawn Random de cada player al inicio
    public void RandomSpawn() {
        if (NetworkManager.Singleton.IsServer) {
            var randomPosition = new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f));
            transform.position = randomPosition;
        } else {
            SubmitPositionRequestServerRpc();
        }
    }

    //Botón de Game Manager para tp a la zona blanca, si es el Server/Host
    public void MoveToZoneNotTeam() {
        transform.position = new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f));
    }

    //Mover a la parte central de forma random, si es Client
    [ServerRpc]
    public void SubmitPositionRequestServerRpc() {
        //if() {
            transform.position = RandomPosition();
        //}
    }

    static Vector3 RandomPosition() {
        return new Vector3(Random.Range(-5f, 5f), 1f, Random.Range(-5f, 5f));
    }

    //Función de movimiento del Player
    [ServerRpc]
    void MoveServerRpc(Vector3 dir) {
        transform.position += dir * speed * Time.deltaTime;
    }

    //Coloreador del Equipo Rojo
    [ServerRpc]
    void ColorizedRedServerRpc() {
        //GameManager.instance.redTeamPlayers.Value++;
        int idForTeamRedColor;
        idForTeamRedColor = ColorAcceptRed();
        meshRenderer.material.color = teamRedColors[idForTeamRedColor];
        PlayerIdColorRed.Value = idForTeamRedColor;
    }

    int ColorAcceptRed() {
        int idColorRed;
        bool sameColorRed;
        List<int> redColorsInUse = new List<int>();
        foreach(ulong uid in NetworkManager.Singleton.ConnectedClientsIds) {
            redColorsInUse.Add(NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<Player>().PlayerIdColorRed.Value);
        } 
        //TODO Encontrar como hace el bucle infinito aquí
        do {
            idColorRed = Random.Range(0, teamRedColors.Count);
            sameColorRed = redColorsInUse.Contains(idColorRed);
        } while (sameColorRed);
        return idColorRed;
    } 

    //Coloreador del Equipo Azul
    [ServerRpc]
    void ColorizedBlueServerRpc() {
        //GameManager.instance.blueTeamPlayers.Value++;
        int idForTeamBlueColor;
        idForTeamBlueColor = ColorAcceptBlue();
        meshRenderer.material.color = teamBlueColors[idForTeamBlueColor];
        PlayerIdColorBlue.Value = idForTeamBlueColor;
    }

    int ColorAcceptBlue() {
        int idColorBlue;
        bool sameColorBlue;
        List<int> blueColorsInUse = new List<int>();
        foreach(ulong uid in NetworkManager.Singleton.ConnectedClientsIds) {
            blueColorsInUse.Add(NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<Player>().PlayerIdColorBlue.Value);
        } 
        /*do {
            idColorBlue = Random.Range(0, teamBlueColors.Count);
            sameColorBlue = blueColorsInUse.Contains(idColorBlue);
        } while (sameColorBlue);*/
        idColorBlue = 1;
        return idColorBlue;
    }

    //Restricción de movimiento si un equipo está lleno
    /*
    [ClientRpc]
    public void SetFreeMoveClientRpc(bool freeMove, ClientRpcParams clientRpc) {
        SetToFreeMoveServerRpc(freeMove);
    }

    [ServerRpc]
    public void SetToFreeMoveServerRpc(bool canMove) {
        isFreeMoveActive.Value = canMove;
    }
    */

    void Update() {
        //freeMove = isFreeMoveActive.Value;
        if(IsOwner /*&& freeMove*/) {
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
            if(Input.GetKeyDown(KeyCode.M)) {
                SubmitPositionRequestServerRpc();
            }
            if(inTeam == 0f) {
                if(transform.position.x > 5f) {
                    inTeam = 1f;
                    ColorizedBlueServerRpc();
                } else if(transform.position.x < -5f) {
                    inTeam = 2f;
                    ColorizedRedServerRpc();
                } 
            }
        }
        //Comparador para sincronizar los colores de los Equipos
        if (meshRenderer.material.color != teamBlueColors[PlayerIdColorBlue.Value] && inTeam == 1f) {
            meshRenderer.material.color = teamBlueColors[PlayerIdColorBlue.Value];
        }
        if (meshRenderer.material.color != teamRedColors[PlayerIdColorRed.Value] && inTeam == 2f) {
            meshRenderer.material.color = teamRedColors[PlayerIdColorRed.Value];
        }
        if (transform.position.x > -5f && transform.position.x < 5f) {
            meshRenderer.material.color = notTeam;
            /*if(inTeamNumber.Value == 1) {
                GameManager.instance.redTeamPlayers.Value--;
            } else if(inTeamNumber.Value == 2) {
                GameManager.instance.blueTeamPlayers.Value--;
            }*/
            inTeam = 0f;
        }
    }
}
