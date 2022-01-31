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

    public Collider2D stickZone;
    private Collider2D playerCollider;
    private Transform playerParent;

    void Start()
    {
        stickZone = (stickZone == null) ? GetComponent<Collider2D>() : stickZone;
        playerCollider = LevelManager.Instance.player.GetComponent<Collider2D>();
        playerParent = playerCollider.transform.parent;

        Sequence travelSequence = DOTween.Sequence();

        for(int i = 0; i < travelPoints.Count; i++)
        {
            travelSequence.AppendInterval(restTime);

            Vector3 current = travelPoints[i];
            Vector3 next = (i == travelPoints.Count - 1 && returnToStart) ? travelPoints[0] : travelPoints[i + 1];
            float time = (current - next).magnitude / speed;
            travelSequence.Append(transform.DOMove(next, time));
        }

        travelSequence.SetLoops(-1, loopType);
    }

    private void Update() {

        if (stickZone.IsTouching(playerCollider))
        {
            playerCollider.transform.parent = transform;
        }
        else
        {
            playerCollider.transform.parent = playerParent;
        }
    }
}
