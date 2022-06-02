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
            var callbacks = new PermissionCallbacks();
            callbacks.PermissionGranted += (permisionStr) =>
            {
                Debug.Log($"Permission {permisionStr} Granted");
                i_callback?.Invoke(true);
            };
            callbacks.PermissionDenied += (permisionStr) =>
            {
                Debug.Log($"Permission {permisionStr} Denied");
                i_callback?.Invoke(false);
            };
            callbacks.PermissionDeniedAndDontAskAgain += (permisionStr) =>
            {
                Debug.Log($"Permission {permisionStr} Denied and Dont Ask Again");
                i_callback?.Invoke(false);
            };
            Permission.RequestUserPermission(Permission.Camera, callbacks);
        }
        else
        {
            i_callback?.Invoke(true);
        }    
#endif
    }

}