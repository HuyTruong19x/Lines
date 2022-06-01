using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    
    private Ray _ray;
    private RaycastHit _hitInfo;
    private BallManager _ballManager;
    private CameraController _cameraController;

    private void Awake()
    {
        _ballManager = GameObject.FindObjectOfType<BallManager>();
        _cameraController = GameObject.FindObjectOfType<CameraController>();
    }

    private void Update()
    {
        if(GameManager.Instance.CanPlay)
        {
            InputInteract();
        }    
    }
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
    private void InputInteract()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Camera currentCamera = _cameraController.GetCamera();
            if(currentCamera != null)
            {
                _ray = currentCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(_ray, out _hitInfo))
                {
                    HandleRaycastHit(_hitInfo);
                }
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
                Camera currentCamera = _cameraController.GetCamera();
                if(currentCamera != null)
                {
                    _ray = currentCamera.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(_ray, out _hitInfo))
                    {
                        HandleRaycastHit(_hitInfo);
                    }
                }
            }    
        }    
    }
#endif
    private void HandleRaycastHit(RaycastHit i_hitInfo)
    {
        _ballManager.HandleBallMoveMent(i_hitInfo);
    }    
}
