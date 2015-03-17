/*
  MothershipLaunchCutscene.cs
  Mission: Invasion
  Created by Rohun Banerji on March 16, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MothershipLaunchCutscene : MonoBehaviour
{
    [SerializeField]
    private GameObject[] canvases;
    [SerializeField]
    private Transform[] hangars;
    [SerializeField]
    private Transform launchPos;
    [SerializeField]
    private float timeToExitHangar = 2.5f;
    [SerializeField]
    private float shipMoveSpeed = 50.0f;
    [SerializeField]
    private float shipTurnSpeed = 1.0f;

    private Vector3 camMotherShipPos;
    private Quaternion camMotherShipRot;

    private IEnumerator PreCutscene()
    {
        //deactivate UI
        camMotherShipPos = Camera.main.transform.position;
        camMotherShipRot = Camera.main.transform.rotation;
        SpaceGround.Instance.Display(false);
        foreach (GameObject go in canvases)
        {
            go.SetActive(false);
        }

        yield return null;
    }
    public IEnumerator PlayCutscene(Dictionary<Transform, Vector3> ship_gridPos_Table)
    {
        yield return StartCoroutine(PreCutscene());
        foreach (var ship_gridPos in ship_gridPos_Table)
        {
            Transform shipTrans = ship_gridPos.Key;
            Vector3 destination = ship_gridPos.Value;
            shipTrans.position = hangars[0].position;
            shipTrans.rotation = hangars[0].rotation;

        }
        for (int i = 0; i < ship_gridPos_Table.Count; i++)
        {
            Transform shipTrans = ship_gridPos_Table.ElementAt(i).Key;
            Vector3 destination = ship_gridPos_Table.ElementAt(i).Value;
            yield return StartCoroutine(ExitHangar(shipTrans));
            yield return StartCoroutine(FlyToGridPos(shipTrans, destination));
            //swing back to mothership until last ship
            if (i < ship_gridPos_Table.Count - 1)
            {
                yield return StartCoroutine(CameraDirector.Instance.MoveAndRotate(camMotherShipPos, camMotherShipRot, 1.0f));
            }
            shipTrans.position = destination;
            shipTrans.rotation = Quaternion.identity;
        }

        yield return StartCoroutine(PostCutscene());
    }
    private IEnumerator PostCutscene()
    {
        //re-activate UI
        SpaceGround.Instance.Display(true);
        foreach (GameObject go in canvases)
        {
            go.SetActive(true);
        }
        yield return null;
    }
    private IEnumerator FlyToGridPos(Transform ship, Vector3 destination)
    {
        Vector3 initialPos = ship.position;
        Vector3 dirToDest = destination - ship.position;
        float dirMag = dirToDest.magnitude;
        float timeToReachDest = dirMag / shipMoveSpeed;
        //float halfPeriod = timeToReachDest * 0.5f;
        //assuming angle is the same as destination angle
        float dot = Vector3.Dot(ship.forward, dirToDest);
        bool isToTheRight = Vector3.Dot(ship.right, dirToDest) > 0.0f;
        float angle = Mathf.Acos(dot / dirMag)*Mathf.Rad2Deg;
        Debug.Log("Dot: " + dot / dirMag + " Angle: " +angle + " isRight "+isToTheRight);
        float doubleAngle = isToTheRight? angle*2.0f : angle*-2.0f;
        Quaternion halfDistRot = Quaternion.AngleAxis(doubleAngle, Vector3.up);
        Vector3 doubleAngleVec = halfDistRot*ship.forward;
        float time = 0.0f;
        while(time<1.0f)
        {
#if FULL_DEBUG
            Debug.DrawRay(initialPos, dirToDest, Color.red);
            Debug.DrawRay(ship.position, ship.forward*500.0f, Color.green);
            Debug.DrawRay(initialPos, doubleAngleVec*500.0f, Color.blue);
#endif
            if (time<=0.5f)
            {
                ship.rotation = Quaternion.Slerp(ship.rotation, halfDistRot, time*2.0f*shipTurnSpeed*Time.deltaTime);
            }
            else
            {
                ship.rotation = Quaternion.Slerp(ship.rotation, Quaternion.identity, (time*2.0f-1.0f)*shipTurnSpeed*Time.deltaTime);
            }
            ship.position += ship.forward * shipMoveSpeed * Time.deltaTime;
            time += Time.deltaTime / timeToReachDest;
            Camera.main.transform.LookAt(ship);
            yield return null;
        }
    }
    private IEnumerator ExitHangar(Transform ship)
    {
        float time = 0.0f;
        Vector3 startPos = ship.position;
        Quaternion startRot = ship.rotation;
        while (time < 1.0f)
        {
            ship.position = Vector3.Lerp(startPos, launchPos.position, time);
            ship.rotation = Quaternion.Slerp(startRot, launchPos.rotation, time);
            time += Time.deltaTime / timeToExitHangar;
            yield return null;
        }
    }

}
