﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class SpaceGround : Singleton<SpaceGround>, IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    public delegate void GroundClick(Vector3 worldPosition);
    public event GroundClick OnGroundClick = new GroundClick((Vector3) => { });

    public delegate void GroundHold(Vector3 worldPosition);
    public event GroundHold OnGroundHold = new GroundHold((Vector3) => { });

    private bool holding = false;


    /// <summary>
    /// Called the pointer is released on the space ground
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        holding = false;
    }

    /// <summary>
    /// Called when the pointer is clicked on the space ground. Raises the OnGroundClick event with the worldPosition of where on the ground the click happened
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            holding = true;
            OnGroundClick(eventData.worldPosition);
        }
    }
    
    /// <summary>
    /// Called when the pointer is dragged on the space ground. Raises the OnGroundClick event with the worldPosition of where on the ground the drag happened
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnGroundClick(eventData.worldPosition);
        }
    }


    //private void Start()
    //{
    //    //InputManager.Instance.RegisterMouseButtonsHold(MouseDown, MouseButton.Left);
    //}

    //void MouseDown(MouseButton btn)
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit hit;
    //    if (Physics.Raycast(ray, out hit, 1000.0f, 1 << TagsAndLayers.SpaceGroundLayer))
    //    {
    //        holding = true;
    //        OnGroundClick(hit.point);
    //    }
    //}


}
