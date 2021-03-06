﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControll : MonoBehaviour {

    // Use this for initialization
    public GameObject player;
    private OfflinePlayerController _script;
    private Rigidbody _rigidbody;
    private float currentY;
    private readonly float MIN_Y = -30f;
    private readonly float MAX_Y = 70f;
    // Update is called once per frame
    private void Start()
    {
        currentY = 0;
        _script = player.GetComponent<OfflinePlayerController>();
        _rigidbody = GetComponent<Rigidbody>();
    }
    void FixedUpdate () {
        if (Input.GetMouseButton(1))
        {
            currentY += Input.GetAxis("Mouse Y") * 5.0f;
            currentY = Mathf.Clamp(currentY, MIN_Y, MAX_Y);
            //Debug.Log("Mouse Y coor:" + currentY);
            transform.rotation = player.transform.rotation * Quaternion.Euler(currentY, 0, 0);
        }
    }
}
