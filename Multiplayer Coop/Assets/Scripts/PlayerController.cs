﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Barebones.MasterServer;
using Barebones.Utils;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerController : NetworkBehaviour
{
    protected float distance = 10.0f;
    protected float currentX = 0.0f;
    protected float currentY = 0.0f;
    protected float sensitivityX = 0.0f;
    protected float sensitivityY = 0.0f;

    protected CharacterController _characterController;
    protected Rigidbody _rigidbody;
    protected FloorDetection floorD;

    protected float _fallVelocity;
    protected readonly float _fallVelocityMax = 20f;
    protected readonly float _forwardMaxSpeed = 15f;

    protected float _forwardSpeed;

    protected readonly float _yDeathValue = -20f;
    protected readonly float _rotationMaxVelocity = 270;

    //protected Vector3 _rotationVelocity;
    protected float _rotationVelocity;

    public GameObject playerCam;
    public SpriteRenderer Direction;

    public GameObject Shape;
    public GameObject Arrow;
    public Transform TargetPlayer;

    public float jumpHeight;

    //public Team team;

    // Use this for initialization
    protected void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _characterController.detectCollisions = false;
        _rigidbody = GetComponent<Rigidbody>();
        floorD = GetComponentInChildren<FloorDetection>();
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        Instantiate(playerCam, transform.Find("CamOrigin"));
        DecideRole(FindObjectsOfType<PlayerController>().Length);

        // Notify UI
        if (GameUi.Instance != null)
        {
            GameUi.Instance.OnPlayerSpawned(this);
        }
    }

    // Update is called once per frame
    protected void Update()
    {
        // Ignore input from other players
        if (!isLocalPlayer)
            return;

        if (TargetPlayer != null)
        {
            Debug.Log(TargetPlayer);
            _rigidbody.MovePosition((TargetPlayer.transform.position + Vector3.up * 2) + TargetPlayer.transform.forward);
            _rigidbody.MoveRotation(TargetPlayer.transform.rotation);
        }
        else
        {
            // Input
            _forwardSpeed = Input.GetAxis("Vertical") * _forwardMaxSpeed;
            _rotationVelocity = Input.GetAxis("Horizontal") * _rotationMaxVelocity;
            if (Input.GetButtonDown("Jump") && floorD.Grounded)
            {
                _rigidbody.AddForce(Mathf.Sqrt(jumpHeight * 2 * 1.1f * -Physics.gravity.y * transform.lossyScale.y) * Vector3.up, ForceMode.VelocityChange);
            }
        }
    }

    protected void FixedUpdate()
    {
        // Ignore input from other players
        if (!isLocalPlayer || TargetPlayer != null)
            return;
        UpdateMovement();
    }

    protected void UpdateMovement()
    {
        Vector3 moveDirection = transform.forward * _forwardSpeed * Time.deltaTime;

        // Movement update
        _rigidbody.MovePosition(_rigidbody.position + moveDirection);
        _rigidbody.MoveRotation(Quaternion.Euler(_rigidbody.rotation.eulerAngles + Vector3.up * _rotationVelocity * Time.deltaTime));

        // Death and "respawn"

        //_rigidbody.MovePosition(transform.position + Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward);

        //_rigidbody.MoveRotation(Quaternion.Euler(_rigidbody.rotation.eulerAngles + Vector3.up * _rotationVelocity * Time.deltaTime));
        if (_rigidbody.position.y < _yDeathValue)
            MoveToRandomSpawnPoint();
    }

    public void MoveToRandomSpawnPoint()
    {
        _rigidbody.MovePosition(new Vector3(Random.Range(0f, 4f), 10, Random.Range(0f, 4f)));
    }

    [ClientRpc]
    public void RpcReset(int i)
    {
        transform.position = Vector3.up * (20 + 2 * i);
    }

    public void ReSpawn(int i)
    {
        transform.position = Vector3.up * (20 + 2 * i);
    }

    [Command]
    public void CmdPickUp (GameObject p, GameObject g)
    {
        p.GetComponent<PlayerController>().RpcBePickedUp(g);
    }

    [Command]
    public void CmdThrow (GameObject p, float Angle, float ThrowForce)
    {
        p.GetComponent<PlayerController>().RpcBeThrown(Angle, ThrowForce);   
    }

    [ClientRpc]
    public void RpcBePickedUp (GameObject g)
    {
        TargetPlayer = g.transform;
    }

    [ClientRpc]
    public void RpcBeThrown (float Angle, float ThrowForce)
    {
        TargetPlayer = null;
        GetComponent<Rigidbody>().AddForce
        (
            (transform.forward * Mathf.Cos(Mathf.Deg2Rad * Angle) +
            transform.up * Mathf.Sin(Mathf.Deg2Rad * Angle)) * ThrowForce,
            ForceMode.VelocityChange
        );
        GetComponent<Rigidbody>().useGravity = true;
    }

    private void DecideRole(int r)
    {
        //yield return new WaitForEndOfFrame();
        if (r % 2 == 1)
        {
            GetComponent<TelepathController>().enabled = false;
            GetComponent<TelepathController>().crossHair.enabled = false;
        }
        else
        {
            GetComponent<ThrowerController>().enabled = false;
            GetComponent<PlayerTrigger>().enabled = false;
            Arrow.SetActive(false);
        }
    }
}