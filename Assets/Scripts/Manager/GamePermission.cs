using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif
public class GamePermission : MonoBehaviour
{
    public void RequestCameraPermission()
    {
#if UNITY_ANDROID
        if(!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
#endif
        StartCoroutine(Request());
    }
    IEnumerator Request()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        Permission.RequestUserPermission(Permission.Camera);
    }
}
