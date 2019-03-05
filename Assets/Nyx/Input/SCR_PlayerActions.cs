using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_PlayerActions: SCR_InputManager
{
    public SCR_InputAxis_1D move;
    public SCR_InputAxis_1D jump;
    public SCR_InputAxis_1D punch;
    public SCR_InputAxis_1D block;
    public SCR_InputAxis_1D rocketPunch;
    public SCR_InputAxis_1D menu;

    private static SCR_PlayerActions singleton = null;
    public static SCR_PlayerActions Instance
    {
        get
        {
            return singleton;
        }
    }

    private void Awake()
    {
        if (singleton)
        {
            Destroy(this);
        }
        else
        {
            singleton = this;
        }
        InitializeLists();
    }

    void Start()
    {
        move = BindInputAxis1D("Horizontal", 0.0f);
        jump = BindInputAxis1D("Vertical", 0.0f);
        punch = BindInputAxis1D("Fire1", 0.0f);
        block = BindInputAxis1D("Jump", 0.0f);
        rocketPunch = BindInputAxis1D("Fire2", 0.0f);
        menu = BindInputAxis1D("Cancel", 0.0f);
    }

    private void Update()
    {
       UpdateInputAxises();
    }
}
