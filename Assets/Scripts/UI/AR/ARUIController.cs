using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
public class ARUIController : MonoBehaviour
{
    private ARPlaneManager _arPlaneManager;
    private ARSession _arSession;

    [SerializeField]
    private Sprite _sprToggleOn;
    [SerializeField]
    private Sprite _sprToggleOff;
    // Start is called before the first frame update
    void Start()
    {
        _arPlaneManager = GameObject.FindObjectOfType<ARPlaneManager>();
        _arSession = GameObject.FindObjectOfType<ARSession>();
    }
    public void ToggleARMode(Toggle i_toggle)
    {
        _arSession.enabled = i_toggle.isOn;
        UpdateToggleSprite(i_toggle);
        GameManager.Instance.ChangeGameMode(i_toggle.isOn ? GAMEMODE.ARMODE : GAMEMODE.NONE);
    }
    public void TogglePlaneVisualizer(Toggle i_toggle)
    {
        _arPlaneManager.enabled = i_toggle.isOn;
        UpdateToggleSprite(i_toggle);

        if (_arPlaneManager.enabled)
        {
            SetAllPlanesActive(true);
        }
        else
        {
            SetAllPlanesActive(false);
        }
    }

    private void SetAllPlanesActive(bool value)
    {
        foreach (var plane in _arPlaneManager.trackables)
            plane.gameObject.SetActive(value);
    }

    private void UpdateToggleSprite(Toggle i_toggle)
    {
        i_toggle.GetComponentInChildren<Image>().sprite = i_toggle.isOn ? _sprToggleOn : _sprToggleOff;
    }
}
