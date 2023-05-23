using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
using Random=UnityEngine.Random;

public class Player : NetworkBehaviour {
    public float speed = 4f;

    private MeshRenderer meshRenderer; 

    void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start() {
        if (IsOwner) {
            RandomSpawn();
        }
    }

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

    [ServerRpc]
    void MoveServerRpc(Vector3 dir) {
        transform.position += dir * speed * Time.deltaTime;
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
        }
    }
}
