using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;
    public List<NotificationFrontend> frontends;
    public Dictionary<Notification, NotificationFrontend> requestedNotifications = new Dictionary<Notification, NotificationFrontend>();

    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void LateUpdate() {
        if(requestedNotifications == null)
        {
            Debug.LogError("Requested Notifications are null");
            return;
        }
        
        if(requestedNotifications.Count > 0)
        {
            Notify(requestedNotifications);
        }

        requestedNotifications.Clear();
    }

    public void RequestNotification(Notification notification, int frontendIndex)
    {
        NotificationFrontend frontend = (frontendIndex <= frontends.Count - 1) ? frontends[frontendIndex] : frontends[0];
        requestedNotifications.Add(notification, frontend);
    }

    private void Notify(Dictionary<Notification, NotificationFrontend> notifications)
    {
        //Place into queues of Notifications
        List<NotificationFrontend> frontends = notifications.Values.Distinct().ToList(); //A list of all the notifications requested
        List<List<Notification>> notificationsByFrontends = new List<List<Notification>>();

        foreach(NotificationFrontend f in frontends)
        {
            //Find notifications where the frontend is f, then get all of their notifications in a list
            List<Notification> frontendNotifications = notifications.Where(n => n.Value == f).ToDictionary(n => n.Key, n => n.Value).Keys.ToList();
            //Add this to the list of lists
            notificationsByFrontends.Add(frontendNotifications);
        }

        for (int i = 0; i < frontends.Count; i++)
        {
            StartCoroutine(DisplayNotifications(notificationsByFrontends[i], frontends[i]));
        }
    }

    private IEnumerator DisplayNotifications(List<Notification> toNotify, NotificationFrontend frontend)
    {
        foreach(Notification notification in toNotify)
        {
            frontend.OnNotificationStart(notification);

            while(!frontend.StopDisplay(notification))
            {
                //While until enough time has passed, and the frontend is not ready to end
                frontend.OnNotificationUpdate(notification);
                yield return null;
            }

            yield return new WaitForSeconds(notification.displayTime);

            //Notification is done displaying
            Debug.Log("Finished Displaying Notification");
            frontend.OnNotificationEnd(notification);

            yield return new WaitUntil(() => frontend.IsReadyForNext(notification));
        }
    }
}
