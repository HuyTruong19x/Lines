using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : Singleton<VFXManager>
{
    public void TriggerVFX(string i_vfx, Vector3 i_position)
    {
        GameObject vfx = ObjectPool.Instance.TakeObject("confetti");
        vfx.SetActive(true);
        vfx.transform.position = i_position;
    }    
}
