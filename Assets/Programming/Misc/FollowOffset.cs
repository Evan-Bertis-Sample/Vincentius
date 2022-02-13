using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class FollowOffset : MonoBehaviour
{
    public static FollowOffset main;

    public bool lockOffset;
    public Vector3 lockPosition;

    public List<CameraRequest<Vector3>> requests = new List<CameraRequest<Vector3>>();
    public CameraRequest<Vector3> currentRequest;

    private void Awake() {
        if(main == null)
        {
            main = this;
        }
        else
        {
            Destroy(this);
        }

        DontDestroyOnLoad(this);
        lockOffset = false;
        currentRequest = null;
    }

    private void Update() {
        if (lockOffset)
        {
            transform.position = lockPosition;
        }
    }

    private void LateUpdate() 
    {
        if (requests.Count == 0) return;

        if(requests.Where(r => r.reset == false).Any())
        {
            //Setting a new frame takes priority over resetting the camera
            CameraRequest<Vector3> request = requests.Where(r => r.reset == false).First();
            SetPosition(request);
        }
        else
        {
            CameraRequest<Vector3> request = requests.Where(r => r.reset == true).First();
            SetPosition(request);
        }

        requests.Clear();
    }

    private void SetPosition(CameraRequest<Vector3> request)
    {        
        Debug.Log("Moving Camera");
        currentRequest = request;

        transform.DOKill();

        Debug.Log("Setting Position -- Is Reset? " + request.requestLock);

        lockOffset = false;

        if (request.global)
        {
            transform.DOMove(request.to, request.speed).OnComplete(() => 
            {
                //currentRequest = null;
                HandleLock(request);
            });
        }
        else
        {
            transform.DOLocalMove(request.to, request.speed).OnComplete(() => 
            {
                //currentRequest = null;
                HandleLock(request);
            });
        }
    }

    private void HandleLock(CameraRequest<Vector3> request)
    {
        if (request.requestLock)
        {
            lockOffset = true;
            lockPosition = request.lockValue;
        }
        else
        {
            lockOffset = false;
        }
    }

    public void SetPosition(Vector3 worldPos, float speed = -1f, int priority = 1)
    {
        float time = (speed <= 0) ? (transform.position - worldPos).magnitude * 0.1f : speed;
        CameraRequest<Vector3> request = new CameraRequest<Vector3>(worldPos, time, false, true, priority);

        requests.Add(request);
    }

    public void SetAndLockPosition(Vector3 worldPos, float speed = -1f, int priority = 1)
    {
        float time = (speed <= 0) ? (transform.position - worldPos).magnitude * 0.1f : speed;
        CameraRequest<Vector3> request = new CameraRequest<Vector3>(worldPos, time, false, true, priority);
        request.SetLock(worldPos);

        requests.Add(request);
    }

    public void SetLocalPosition(Vector3 localPos, float speed = -1f, int priority = 1)
    {
        float time = (speed <= 0) ? (transform.localPosition - localPos).magnitude * 0.1f : speed;
        CameraRequest<Vector3> request = new CameraRequest<Vector3>(localPos, time, false, false, priority);

        requests.Add(request);
    }

    public void SetAndLockLocalPosition(Vector3 localPos, float speed = -1f, int priority = 1)
    {
        float time = (speed <= 0) ? (transform.localPosition - localPos).magnitude * 0.1f : speed;
        CameraRequest<Vector3> request = new CameraRequest<Vector3>(localPos, time, false, false, priority);
        request.SetLock(localPos);
        requests.Add(request);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void ResetPosition()
    {
        CameraRequest<Vector3> request = new CameraRequest<Vector3>(Vector3.zero, transform.localPosition.magnitude * 0.1f, true, false);
        requests.Add(request);
    }
}
