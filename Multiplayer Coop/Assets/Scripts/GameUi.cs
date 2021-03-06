﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameUi : MonoBehaviour
{
    public Text Coins;

    public static GameUi Instance;

    public Image AutoWalkBg;

    public bool IsAutoWalk { get; private set; }
    private Color _defaultWalkBtnColor;

    void Awake()
    {
        Instance = this;
    }

    public void ToggleAutoWalk()
    {
        // Save defualt color
        if (!IsAutoWalk)
            _defaultWalkBtnColor = AutoWalkBg.color;

        IsAutoWalk = !IsAutoWalk;

        AutoWalkBg.color = IsAutoWalk ? Color.red : _defaultWalkBtnColor;
    }

    public void OnPlayerSpawned(PlayerController player)
    {

    }
}
