﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class Component_Weapon : ShipComponent
{
    public float damage;
    public float range;
    public float hullDamagePercent;
    public float armourDmgModifier;
    public float shieldDmgModifier;

    [SerializeField]
    protected Transform shootPoint;


    public virtual IEnumerator Fire(ShipComponent targetComp, Action OnActivationComplete)
    {
        Debug.Log("Weapon fire");
        yield return null;
    }

    /// /////////////////////////
    /// temp overloading for AI
    /// //////////////////////////
    public virtual IEnumerator Fire(Transform target, Action OnActivationComplete)
    {
        Debug.Log("Weapon fire");
        yield return null;
    }

    protected IEnumerator DoDamage(ShipComponent targetComp)
    {
        float componentDamage = damage * (1.0f - hullDamagePercent / 100.0f);
        
        //if comp is armour
        //  compDmg+=compDmg*armourMod%/100
        if(targetComp is Comp_Def_Shield)
        {
            componentDamage += componentDamage * shieldDmgModifier / 100.0f;
        }

        yield return StartCoroutine(targetComp.TakeDamage(componentDamage));
        yield return StartCoroutine(targetComp.ParentShip.TakeDamage(damage * hullDamagePercent / 100.0f));
    }
}
