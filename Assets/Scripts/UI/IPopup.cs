using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public interface IPopup
{
    void Open(UnityAction onComplete);
    void Close(UnityAction onComplete);
}
