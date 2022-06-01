using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARManager : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;
    private ARPlaneManager _arPlaneManager;
    private ARSession _arSession;

    private void Awake()
    {
        _arPlaneManager = GameObject.FindObjectOfType<ARPlaneManager>();
        _arSession = GameObject.FindObjectOfType<ARSession>();
    }   
    public void TurnOnOffARSession(bool i_isTurnOn)
    {
        _arSession.enabled = i_isTurnOn;
    }    
    public void TurnOnOffVisualize(bool i_isTurnOn)
    {
        _arPlaneManager.enabled = i_isTurnOn;
        foreach (var plane in _arPlaneManager.trackables)
            plane.gameObject.SetActive(i_isTurnOn);
    }    
}
