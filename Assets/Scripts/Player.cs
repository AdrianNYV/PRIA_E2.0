using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class Player : NetworkBehaviour {
    public float speed = 4f;

    private MeshRenderer meshRenderer; 

    void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
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
