using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtension
{
    public static void Reset(this Transform transform)
    {
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
}
