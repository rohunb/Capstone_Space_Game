﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ComponentType { Weapon, Defense, Power, Support }

public abstract class ShipComponent : MonoBehaviour 
{

    [SerializeField]
    private ComponentType compType;
    public ComponentType CompType
    {
        get { return compType; }
    }
    
    public int ID;
    public string componentName;
    public bool unlocked;
    public float activationCost;
    public float powerDrain;

}
