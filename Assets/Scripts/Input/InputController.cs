using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    
    private Ray _ray;
    private RaycastHit _hitInfo;

    private void Update()
    {
        if(GameManager.Instance.CanPlay)
        {
            InputInteract();
        }    
    }
#if UNITY_STANDALONE_WIN
    private void InputInteract()
    {
        if(Input.GetMouseButtonDown(0))
        {
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(_ray, out _hitInfo))
            {
                HandleRaycastHit(_hitInfo);   
            }    
        }    
    }
#else
    private void InputInteract()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Ended)
            {
                _ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(_ray, out _hitInfo))
                {
                    HandleRaycastHit(_hitInfo);
                }
            }    
        }    
    }
#endif
    private void HandleRaycastHit(RaycastHit i_hitInfo)
    {
        BallManager.Instance.HandleBallMoveMent(i_hitInfo);
    }    
}
