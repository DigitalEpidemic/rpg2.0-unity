﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    GameObject player;

    // Use this for initialization
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        //print (player.ToString ());
    }

    // Called after all Update functions have been called
    void LateUpdate() {
        transform.position = player.transform.position;
    }

} // CameraFollow
