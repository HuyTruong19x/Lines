using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    protected Ray _ray;
    protected RaycastHit _hitInfo;
    [SerializeField]
    protected BallManager _ballManager;
    protected CameraController _cameraController;
    private void Update()
    {
        if(GameManager.Instance.CanPlay)
        {
            InputInteract();
        }    
    }
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
    public virtual void InputInteract()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(_cameraController == null)
            {
                _cameraController = GameObject.FindObjectOfType<CameraController>();
            }    
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
    public virtual void InputInteract()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Ended)
            {
                if(_cameraController == null)
                {
                    _cameraController = GameObject.FindObjectOfType<CameraController>();
                } 
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
    protected void HandleRaycastHit(RaycastHit i_hitInfo)
    {
        _ballManager.HandleBallMoveMent(i_hitInfo);
    }    
}
