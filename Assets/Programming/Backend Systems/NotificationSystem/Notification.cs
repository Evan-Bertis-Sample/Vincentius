using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notification
{
    public string notificationText;
    public string notificationSubText;
    public float displayTime;
    
    public Notification(string mainMessage, string subMessage = null, float time = 2)
    {
        notificationText = mainMessage;
        notificationSubText = subMessage;
        displayTime = time;
    }
}
