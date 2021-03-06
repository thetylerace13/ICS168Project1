﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MeshRenderer))]
public class Interactible : NetworkBehaviour {
    #region Attributes
    public static List<GameObject> prefabs;     // A list of game objects to spawn for a team
    public bool canBePickedUp;                  // Whether the object can be picked up
    [SerializeField]
    protected Team teamID;                      // The team that this object is for
    protected bool set;                         // Whether the object is set or not
    protected Rigidbody _rigidbody;             // The Rigidbody component attached
    protected MeshRenderer _meshRenderer;       // The Mesh Renderer component attached
    #endregion

    #region Properties
    // Get and private set for teamID
    public Team TeamID
    {
        get
        {
            return teamID;
        }
        protected set
        {
            teamID = value;
        }
    }
    #endregion

    #region Event Functions
    // Awake is called before Start
    private void Awake ()
	{
        set = false;
        canBePickedUp = true;
        _rigidbody = GetComponent<Rigidbody>();
        _meshRenderer = GetComponent<MeshRenderer>();
	}

	// Use this for initialization
	private void Start () 
	{
		
	}
	
	// Update is called once per frame
	private void Update () 
	{

	}
	#endregion
	
	#region Methods
    // Set the object parameters
    public void SetObject (Team team)
    {
        if (!set)
        {
            set = true;
            TeamID = team;
            _meshRenderer.ChangeColor(team.Color);
        }
        else
        {
            Debug.LogWarning("Object is already set.");
        }
    }
	#endregion
	
	#region Coroutines
	
	#endregion
}
