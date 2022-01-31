using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FollowOffset : MonoBehaviour
{
    public static FollowOffset main;

    public bool lockOffset;
    public Vector3 lockPosition;

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
    }

    private void Update() {
        if (lockOffset)
        {
            transform.position = lockPosition;
        }
    }
    public void SetPosition(Vector3 worldPos)
    {
        lockOffset = false;
        transform.DOMove(worldPos, (transform.position - worldPos).magnitude * 0.25f);
    }

    public void SetAndLockPosition(Vector3 worldPos)
    {
        lockOffset = false;
        transform.DOMove(worldPos, (transform.position - worldPos).magnitude * 0.25f).OnComplete(() => 
        {
            lockOffset = true;
            lockPosition = worldPos;
        });
    }

    public void SetLocalPosition(Vector3 localPos)
    {
        lockOffset = false;
        transform.DOLocalMove(localPos, (transform.localPosition - localPos).magnitude * 0.25f);
    }

    public void SetAndLockLocalPosition(Vector3 localPos)
    {
        lockOffset = false;
        transform.DOLocalMove(localPos, (transform.localPosition - localPos).magnitude * 0.25f).OnComplete(() => 
        {
            lockOffset = true;
            lockPosition = localPos;
        });
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void ResetPosition()
    {
        lockOffset = false;
        transform.DOLocalMove(Vector3.zero, transform.localPosition.magnitude * 0.25f);
    }
}
