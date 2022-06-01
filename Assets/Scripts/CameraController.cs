using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    GameObject _camera;
    [SerializeField]
    GameObject _arCamera;
    private void OnEnable()
    {
        EventManager.Instance.RegisterEvent(GAMEEVENT.CHANGEDGAMEMODE, ChangedGameMode);
    }
    private void OnDisable()
    {
        EventManager.Instance.RemoveEvent(GAMEEVENT.CHANGEDGAMEMODE, ChangedGameMode);
    }
    private void ChangedGameMode()
    {
        if(GameManager.Instance.GameMode == GAMEMODE.ARMODE)
        {
            _arCamera.SetActive(true);
            _camera.SetActive(false);
        }
        else
        {
            _arCamera.SetActive(false);
            _camera.SetActive(true);
        }    
    }

    public Camera GetCamera()
    {
        if (_arCamera.activeSelf)
            return _arCamera.GetComponent<Camera>();
        return _camera.GetComponent<Camera>();
    }    
}
