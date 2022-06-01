using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
public class GamePermission : MonoBehaviour
{
    public void RequestCameraPermission(UnityAction<bool> i_callback)
    {
#if UNITY_ANDROID
        if(!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
            i_callback?.Invoke(Permission.HasUserAuthorizedPermission(Permission.Camera));
        }
        else
        {
            i_callback?.Invoke(true);
        }    
#endif
    }
}