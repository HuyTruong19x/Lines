using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : Singleton<VFXManager>
{
    public void TriggerVFX(VFXMode i_vfx, Vector3 i_position)
    {
        GameObject vfx = ObjectPool.Instance.TakeObject(GetVFXString(i_vfx));
        vfx.transform.position = i_position;
        vfx.SetActive(true);
    }    

    private string GetVFXString(VFXMode i_vfxMode)
    {
        if (i_vfxMode == VFXMode.CONFETTI)
            return "confetti";
        return "";
    }    
}

public enum VFXMode
{
    CONFETTI
}
