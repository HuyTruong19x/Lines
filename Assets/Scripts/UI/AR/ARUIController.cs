using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
public class ARUIController : MonoBehaviour
{
    private ARManager _arManager;


    [SerializeField]
    private Sprite _sprToggleOn;
    [SerializeField]
    private Sprite _sprToggleOff;
    // Start is called before the first frame update
    void Start()
    {
        _arManager = GameObject.FindObjectOfType<ARManager>();
    }
    public void ToggleARMode(Toggle i_toggle)
    {
        _arManager.TurnOnOffARSession(i_toggle.isOn);
        UpdateToggleSprite(i_toggle);
        GameManager.Instance.ChangeGameMode(i_toggle.isOn ? GAMEMODE.ARMODE : GAMEMODE.NONE);
    }
    public void TogglePlaneVisualizer(Toggle i_toggle)
    {
        UpdateToggleSprite(i_toggle);
        _arManager.TurnOnOffVisualize(i_toggle.isOn);
    }

    private void UpdateToggleSprite(Toggle i_toggle)
    {
        i_toggle.GetComponentInChildren<Image>().sprite = i_toggle.isOn ? _sprToggleOn : _sprToggleOff;
    }

    public void Check()
    {
        GridManager grid = GameObject.FindObjectOfType<GridManager>();
        Debug.Log("Grid parrent : " + grid.GetTile(new Vector2Int(0, 0)).gameObject.transform.parent.rotation.eulerAngles);
    }
}
