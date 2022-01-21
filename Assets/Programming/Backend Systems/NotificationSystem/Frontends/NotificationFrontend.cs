using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NotificationFrontend : MonoBehaviour
{
    public abstract bool StopDisplay(Notification notification);
    public abstract void OnNotificationStart(Notification notification);
    public abstract void OnNotificationUpdate(Notification notification);
    public abstract void OnNotificationEnd(Notification notification);
    public abstract bool IsReadyForNext(Notification notification);
}
