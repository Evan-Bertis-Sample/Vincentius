using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FollowOffset : MonoBehaviour
{
    public static FollowOffset main;

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
    }


    public void SetPosition(Vector3 worldPos)
    {
        transform.DOMove(worldPos, (transform.position - worldPos).magnitude * 0.25f);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void ResetPosition()
    {
        transform.DOLocalMove(Vector3.zero, transform.localPosition.magnitude * 0.25f);
    }
}
