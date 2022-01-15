using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Player/Actions/Jump")]
public class Jump : PlayerAction
{
    private const float cancelForceMultiplier = 0.25f;
    public float jumpHeight = 3;
    public float jumpTime = 0.2f;
    public Vector2 particlesOffset = new Vector2(0, -0.525f);
    public GameObject jumpParticlesPrefab;

    public override bool CheckParameter(PlayerController controller)
    {
        return controller.OnGround;
    }

    public override bool CheckForOverrideExit(PlayerController controller)
    {
        return ((controller.OnGround && timeElapsed > jumpTime) || controller.rb.velocity.y <= 0);
    }

    protected override void Initiate(PlayerController controller)
    {
        //controller.rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Force);
        float jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * controller.rb.gravityScale));
        controller.rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        Instantiate(jumpParticlesPrefab, (Vector2)controller.transform.position + particlesOffset, Quaternion.identity);
    }

    protected override void OnExit(PlayerController controller, bool interrupted)
    {
        if (timeElapsed < jumpTime)
        {
            //The player did not hold the jump button - cancel the force out - makes the assumption that there was no drag or gravity
            float cancelFactor = Mathf.InverseLerp(0, jumpTime, timeElapsed);
            float jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * controller.rb.gravityScale)) * (1 - cancelFactor) * cancelForceMultiplier;
            controller.rb.AddForce(Vector2.down * jumpForce, ForceMode2D.Impulse);
        }
    }

    protected override void OnHold(PlayerController controller)
    {

    }
}
