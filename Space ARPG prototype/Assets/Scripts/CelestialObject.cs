﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CelestialObjectType {GALAXY, STAR, PLANET, MOON}

public class CelestialObject : MonoBehaviour 
{
    public Transform orbitAroundThis;
    public float rotateSpeed;

    private static int numCelestialObjs;

    public static CelestialObject currSelectedObject = null;

    public Material glowMaterial;
    public Material blankMaterial;

    public GameObject Atmosphere;

    private Texture texture;

    private int ID;

    public static bool objectSelected;

    public CelestialObjectType type;

    private Transform playerShipLocation;

    private CelestialObject thisScript;

    public bool selectable = true;
    private bool acceptInput = true;

    public string myName = "";
    public string discoveryDate = "";
    public string composedOf = "";
    public string whyInteresting = "";

    public float myDistanceToShip;

    public int mass;
    public int radius;
    private Camera cam;

	void Start () 
    {
        ID = ++numCelestialObjs;
        myDistanceToShip = 1000000000.0f;
        SystemLog.addMessage("Celestial Object " + ID + ":" + name + " has been initialized");
        cam = Camera.main;
        playerShipLocation = GameObject.Find("PlayerShip").transform;
        objectSelected = false;
        InputManager.Instance.OnMouseClick += OnMouseClick;
        thisScript = gameObject.GetComponent<CelestialObject>();
	}

	
	void FixedUpdate () 
    {
        transform.RotateAround(orbitAroundThis.position, Vector3.up, rotateSpeed * Time.deltaTime * FatherTime.timeRate);
	}


    void OnMouseClick(MouseEventArgs args)
    {
        if (acceptInput && args.button == 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.collider.tag == "CelestialObject" && hit.collider.GetComponent<CelestialObject>().ID == ID)
            {
                if (currSelectedObject != null)
                {
                    currSelectedObject.renderer.material = blankMaterial;
                }

                currSelectedObject = hit.collider.GetComponent<CelestialObject>();
                objectSelected = true;
                SystemLog.addMessage(name + " was selected");
                DisplayInfo();
                acceptInput = false;

                texture = renderer.material.mainTexture;
                renderer.material = glowMaterial;
                renderer.material.mainTexture = texture;

                switch (type)
                {
                    case CelestialObjectType.STAR:
                        CameraManager.getInstance().changeZoomLevel(CameraManager.FAR_ZOOM);
                        break;
                    case CelestialObjectType.PLANET:
                        CameraManager.getInstance().changeZoomLevel(CameraManager.MED_ZOOM);
                        break;
                    case CelestialObjectType.MOON:
                        CameraManager.getInstance().changeZoomLevel(CameraManager.CLOSE_ZOOM);
                        break;
                    default:
                        break;
                }

                StartCoroutine(WaitBeforeInput(1.0f));
            }
        } 
    }

    IEnumerator WaitBeforeInput(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        acceptInput = true;
    }

    void DisplayInfo() 
    {
        Info.EnableMe();
        Info.ResetInfo();
        Info.addMessage("Celestial Object " + ID + ": of type: " + type + " named: " + myName + " has a mass of " + mass.ToString() + ".0 kg and a diameter of " + (radius * 2).ToString() + ".0 m. Discovered in the year " + discoveryDate + " it is composed mainly of " + composedOf + " and has attracted galactic attention because of " + whyInteresting);
        Info.SetInfoToWindow();
    }
}
