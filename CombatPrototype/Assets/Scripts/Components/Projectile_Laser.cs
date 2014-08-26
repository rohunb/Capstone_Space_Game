﻿using UnityEngine;
using System.Collections;

public class Projectile_Laser : Projectile
{


    public override IEnumerator MoveProjectile()
    {
        return base.MoveProjectile();

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == GlobalTagsAndLayers.Instance.tags.enemyShipTag)
        {
            OnProjectileHit();

        }
    }


}
