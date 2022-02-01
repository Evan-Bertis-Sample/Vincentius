using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Actions/Wall Slide")]
public class WallSlide : PlayerAction
{
    public float wallCheckDistance = 2.7f;
    RaycastHit2D wallCast;
    public float slidingGravityScale = 0.25f;
    float originalGravityScale;

    public GameObject slideParticlesPrefab;
    private GameObject slideParticles;
    private ParticleSystem slideSystem;

    public Vector2 slideParticlesOffset = new Vector2(0, -0.525f);
    public int minEmissionRate = 0;
    public int maxEmissionRate = 50;
    public float maxSlideVelocity = 5f;
    public LayerMask wallLayers;

    public override bool CheckForOverrideExit(PlayerController controller)
    {
        return (controller.OnGround || controller.moveInput.y < 0);
    }

    public override bool CheckParameter(PlayerController controller)
    {
        Vector3 endL = new Vector3(controller.transform.position.x - wallCheckDistance, controller.transform.position.y, controller.transform.position.z);
        Vector3 endR = new Vector3(controller.transform.position.x + wallCheckDistance, controller.transform.position.y, controller.transform.position.z);
        wallCast = Physics2D.Linecast(endL, endR, wallLayers);

        if (controller.debug)
        {
            Color castColor = (wallCast.collider != null) ? Color.green : Color.red;
            Debug.DrawLine(endL, endR, castColor);
        }

        return (wallCast.collider != null && !controller.OnGround && controller.moveInput.y >= 0 && controller.rb.velocity.y <= 0); //Return true when the collider is not null
    }

    protected override void Initiate(PlayerController controller)
    {
        controller.rb.velocity = Vector2.zero;
        originalGravityScale = controller.rb.gravityScale;
        controller.rb.gravityScale = slidingGravityScale;

        //gfx.lockSpriteDir = true;

        Vector3 endL = new Vector3(controller.transform.position.x - wallCheckDistance, controller.transform.position.y, controller.transform.position.z);
        Vector3 endR = new Vector3(controller.transform.position.x + wallCheckDistance, controller.transform.position.y, controller.transform.position.z);
        wallCast = Physics2D.Linecast(endL, endR, wallLayers);

        slideParticles = Instantiate(slideParticlesPrefab, wallCast.point + slideParticlesOffset, Quaternion.identity);
        slideParticles.transform.parent = controller.transform;
        slideSystem = slideParticles.GetComponent<ParticleSystem>();

    }

    protected override void OnExit(PlayerController controller, bool interrupted)
    {
        controller.rb.gravityScale = originalGravityScale;
        PlayerGFX gfx = controller.GetComponent<PlayerGFX>();
        gfx.lockSpriteDir = false;

        if (slideParticles != null) 
        {
            slideParticles.transform.parent = null;
            slideParticles.GetComponent<ParticleSystem>()?.Stop();
            Destroy(slideParticles, 2f);
        }
    }

    protected override void OnHold(PlayerController controller)
    {
        if (controller.rb.velocity.y < -maxSlideVelocity) controller.rb.velocity =  new Vector2(controller.rb.velocity.x, -maxSlideVelocity);

        float emissionT = Mathf.InverseLerp(0, -maxSlideVelocity, controller.rb.velocity.y);
        ParticleSystem.EmissionModule emission = slideSystem.emission;
        emission.rateOverTime = Mathf.Lerp(minEmissionRate, maxEmissionRate, emissionT);

        bool flip = (controller.transform.position.x - wallCast.point.x) < 0f;

        Debug.Log($"Face {((flip) ? "left" : "right")}");
        controller.playerGFX.RequestOverride(50, flip);
    }

}
