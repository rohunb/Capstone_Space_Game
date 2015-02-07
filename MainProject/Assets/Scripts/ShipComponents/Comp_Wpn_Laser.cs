﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Comp_Wpn_Laser : Component_Weapon
{
    [SerializeField]
    private float effectDuration = 0.4f;
    [SerializeField]
    private ParticleSystem laserImpactEffect;
    [SerializeField]
    private float lineNoise;

    private LineRenderer line;
    int length;

    public override void Init()
    {
        base.Init();

        line = GetComponentInChildren<LineRenderer>();
#if FULL_DEBUG
        if (!line)
        {
            Debug.LogError("No line renderer found");
        }
#endif

        line.enabled = false;
        //laserImpactEffect.Stop();
    }

    //public override IEnumerator Fire(Transform target, System.Action OnActivationComplete)
    public override IEnumerator Fire(ShipComponent targetComp, System.Action OnActivationComplete)
    {
        if (targetComp && targetComp.CompHP > 0.0f)
        {
            Debug.Log("Firing lasers");

            length = Mathf.RoundToInt(Vector3.Distance(targetComp.transform.position, shootPoint.position));
            float currentTime = 0.0f;
            while (currentTime <= effectDuration)
            {
                CreateBeamEffect();
                currentTime += Time.deltaTime;
                yield return null;
            }
            line.enabled = false;
            if (targetComp)
            {
                yield return StartCoroutine(targetComp.TakeDamage(damage));
            }
            else
            {
                yield return null;
            }
        }
        OnActivationComplete();
    }

    private void CreateBeamEffect()
    {
        line.enabled = true;
        line.SetVertexCount(length);
        for (int i = 0; i < length; i++)
        {
            Vector3 newPos = shootPoint.position;
            Vector3 offset = Vector3.zero;
            offset.x = newPos.x + i * shootPoint.forward.x + Random.Range(-lineNoise, lineNoise);
            offset.y = newPos.y + i * shootPoint.forward.y + Random.Range(-lineNoise, lineNoise);
            offset.z = newPos.z + i * shootPoint.forward.z + Random.Range(-lineNoise, lineNoise);
            newPos = offset;
            line.SetPosition(i, newPos);

        }
    }
}
