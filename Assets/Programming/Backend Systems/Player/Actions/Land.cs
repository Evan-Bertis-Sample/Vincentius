using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Actions/Land")]
public class Land : PlayerAction
{
    public float landTime = 0.25f;
    public GameObject landParticles;
    public Vector2 particlesOffset = new Vector2(0, -0.525f);
    public float requiredFallVelocity = -5f;

    public override bool CheckForOverrideExit(PlayerController controller)
    {
        return (controller.timeOnGround >= landTime);
    }

    public override bool CheckParameter(PlayerController controller)
    {
        return (controller.timeOnGround < landTime && controller.OnGround && controller.lastFrameVelocity.y <= requiredFallVelocity);
    }

    protected override void Initiate(PlayerController controller)
    {
        Instantiate(landParticles, (Vector2)controller.transform.position + particlesOffset, Quaternion.identity);
        //controller.canMove = false;
    }

    protected override void OnExit(PlayerController controller, bool interrupted)
    {
        //controller.canMove = true;
    }

    protected override void OnHold(PlayerController controller)
    {
        //Nothing
    }
}
