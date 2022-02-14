using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Actions/Wall Jump")]
public class WallJump : PlayerAction
{
    public float wallCheckDistance = 2.7f;
    RaycastHit2D wallCast;
    public float jumpAngle = 45f;
    public float jumpHeight = 3;
    public LayerMask wallLayers;
    public string jumpSound = "Jump";

    public override bool CheckParameter(PlayerController controller)
    {
        Vector3 endL = new Vector3(controller.transform.position.x - wallCheckDistance, controller.transform.position.y, controller.transform.position.z);
        Vector3 endR = new Vector3(controller.transform.position.x + wallCheckDistance, controller.transform.position.y, controller.transform.position.z);
        wallCast = Physics2D.Linecast(endL, endR, wallLayers);

        if (controller.debug)
        {
            Color castColor = (wallCast.collider != null) ? Color.green : Color.red;
            Debug.DrawLine(endL, endR, castColor);

            if (wallCast.collider != null)
            {
                Vector3 wallNormal = wallCast.normal;
                Debug.Log(wallNormal);
                float t = Map(jumpAngle, 0, 90, 0, 1);
                Vector3 dir = Vector3.Lerp(((controller.transform.position.x - wallCast.point.x) < 0f) ? Vector3.left : Vector3.right, Vector3.up, t).normalized;

                Debug.DrawLine(controller.transform.position, controller.transform.position + dir * 2, Color.magenta);
            }
        }

        return (wallCast.collider != null); //Return true when the collider is not null
    }

    public override bool CheckForOverrideExit(PlayerController controller)
    {                          
        return controller.OnGround;
    }

    protected override void Initiate(PlayerController controller)
    {
        //controller.StopMovementUntilOnGround();
        Vector3 wallNormal = wallCast.normal;
        float t = Map(jumpAngle, 0, 90, 0, 1);
        Vector3 dir = Vector3.Lerp(((controller.transform.position.x - wallCast.point.x) < 0f) ? Vector3.left : Vector3.right, Vector3.up, t).normalized ;

        controller.rb.velocity = Vector2.zero;
        float jumpForce = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * controller.rb.gravityScale));
        controller.rb.AddForce(dir * jumpForce, ForceMode2D.Impulse);
        AudioManager.Instance.PlaySound(jumpSound);

    }

    protected override void OnExit(PlayerController controller, bool interrupted)
    {

    }

    protected override void OnHold(PlayerController controller)
    {
        //throw new System.NotImplementedException();
    }

    public float Map(float value, float low1, float high1, float low2, float high2)
    {
        // 1 is expected range of value
        // 2 is the new range of the value
        return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
    }
}
