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
        Camera.main.transform.parent.position = new Vector3(4.03f, 5.06f, -10f);
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        Debug.Log("Kuzan pos " + Camera.main.transform.localPosition);
        Debug.Log("Kuzan rot " + Camera.main.transform.localRotation.eulerAngles);
        Debug.Log("Kuzan cal " + Camera.main.transform.localScale);
        Debug.Log("Kuzan parent p " + Camera.main.transform.parent.position);
        Debug.Log("Kuzan parent r " + Camera.main.transform.parent.rotation.eulerAngles);
        Debug.Log("Kuzan parent c" + Camera.main.transform.parent.localScale);
    }
}
