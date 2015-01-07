﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FleetManager : Singleton<FleetManager>
{

    #region Fields
    #region EditorExposed
    [SerializeField]
    private int maxFleetStrength;
    #endregion EditorExposed

    public List<string> currentFleet { get; private set; }
    public int currentFleetStrength { get; private set; }

    #endregion Fields

    #region Methods
    #region Public
    #region GUIAccess
    public void AddShipToFleet(string blueprintName)
    {
        currentFleet.Add(blueprintName);
    }
    public void RemoveShipFromFleet(string blueprintName)
    {
        #if !NO_DEBUG
        if (currentFleet.Contains(blueprintName))
        {
            currentFleet.Remove(blueprintName);
        }
        else
        {
            Debug.LogError("Ship does not exist in fleet");
        }
        #else //NO_DEBUG
        currentFleet.Remove(shipBP);
        #endif
    }
    #endregion GUIAccess
    #endregion Public

    #region Private
    #region UnityCallbacks
    private void Awake()
    {
        currentFleet = new List<string>();
    }
    //private void Start()
    //{
    //    GameController.Instance.OnPreSceneChange += PreSceneChange;
    //}

    //void PreSceneChange(SceneChangeArgs args)
    //{
        
    //}
    #endregion UnityCallbacks
    #endregion Private
    #endregion Methods
}
