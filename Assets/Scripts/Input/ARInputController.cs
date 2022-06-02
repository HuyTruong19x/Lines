using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARInputController : InputController
{
    [SerializeField]
    private GridManager _gridManager;
    private Vector2 _startPos;
    private float _smoothRotate = 5f;
    public const float MIN_SWIPE_DISTANCE = 0.2f;
    public override void InputInteract()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if(GameManager.Instance.GameMode == GAMEMODE.ARMODE)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    _startPos = touch.position;
                }

                if (touch.phase == TouchPhase.Moved)
                {
                    Vector2 endPos = touch.position;
                    Vector2 swipe = new Vector2(endPos.x - _startPos.x, endPos.y - _startPos.y);

                    if (swipe.magnitude < MIN_SWIPE_DISTANCE) // Too short swipe
                        return;

                    if (Mathf.Abs(swipe.x) > Mathf.Abs(swipe.y))
                    { // Horizontal swipe
                        if (swipe.x > 0)
                        {
                            _gridManager.RotateGameBoard(_smoothRotate);
                        }
                        else
                        {
                            _gridManager.RotateGameBoard(-_smoothRotate);
                        }
                    }

                }
            }    
            if (touch.phase == TouchPhase.Ended)
            {
                if (_cameraController == null)
                {
                    _cameraController = GameObject.FindObjectOfType<CameraController>();
                }
                Camera currentCamera = _cameraController.GetCamera();
                if (currentCamera != null)
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
}
