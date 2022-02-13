using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovingPlatform : MonoBehaviour
{
    public List<Vector3> travelPoints = new List<Vector3>();
    public float speed = 1f;
    public float restTime = 0.5f;

    public LoopType loopType = LoopType.Incremental;
    public bool returnToStart;
    public Sequence travelSequence;

    public Collider2D stickZone;
    private Collider2D playerCollider;
    private Transform playerParent;

    public bool playerAttached;
    public bool waitOnLastTravelPoint = true;
    public bool waitForPlayer = false;
    public bool setAsChild = true;
    public bool restartIfPlayerFall = false;

    void Start()
    {
        stickZone = (stickZone == null) ? GetComponent<Collider2D>() : stickZone;
        playerCollider = LevelManager.Instance.player.GetComponent<Collider2D>();
        playerParent = playerCollider.transform.parent;
        playerAttached = false;

        travelSequence = DOTween.Sequence();

        for(int i = 0; i < travelPoints.Count; i++)
        {
            Vector3 current = travelPoints[i];
            Vector3 next;
            if (i == travelPoints.Count - 1)
            {
                if (waitOnLastTravelPoint) travelSequence.AppendInterval(restTime);

                if (returnToStart)
                {
                    next = travelPoints[0];
                }
                else
                {
                    break;
                }
            }
            else
            {
                travelSequence.AppendInterval(restTime);
                next = travelPoints[i + 1];
            }
            float time = (current - next).magnitude / speed;
            travelSequence.Append(transform.DOMove(next, time));
        }

        travelSequence.SetLoops(-1, loopType);

        if (waitForPlayer)
        {
            travelSequence?.Pause();
        }
    }

    private void Update() {

        if (stickZone.IsTouching(playerCollider))
        {
            Debug.Log("On Platform");
            if (setAsChild) playerCollider.gameObject.transform.parent = transform;
            playerAttached = true;

            if (waitForPlayer)
            {
                travelSequence?.Play();
            }
        }
        else if (playerAttached)
        {
            if (setAsChild) playerCollider.gameObject.transform.parent = playerParent;
            playerAttached = false;
            
            if (restartIfPlayerFall)
            {
                travelSequence.Restart();
            }

            if (waitForPlayer)
            {
                travelSequence?.Pause();
            }
        }
    }
}
