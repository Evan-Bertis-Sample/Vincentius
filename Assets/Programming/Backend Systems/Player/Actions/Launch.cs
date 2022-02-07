using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

//The camera zoom has been disabled to work with the pixel perfect camera component (original zoom == zoom amount)
//To fix, just changee zoom amount in inspector

[CreateAssetMenu(menuName = "Player/Actions/Launch")]
public class Launch : PlayerAction
{
    [Header("Quests Required")]
    public Quest quest;

    [Header("Mechanic Values")]
    public float launchStrength = 2f;
    public float slowTimeScale = 0.5f;
    public float slowGravityScale = 0.2f;
    public float chargeTime = 1;
    public float timesInAir = 1;
    public float performedInAir;
    public float originalGravityScale;
    Vector2 initMousePos;

    [Header("Zoom Settings")]
    public float zoomAmount = 2f;
    public float zoomInTime = 5f;
    public float zoomOutTime = 1f;
    public float originalZoom;
    Tween zoomTween;
    Camera cam;
    CinemachineBrain brain;
    CinemachineVirtualCamera vcam;

    [Header("Bow Settings")]
    public Sprite[] bowSprites;
    public Material spriteMaterial;
    SpriteRenderer bowSr;
    public AnimationCurve bowEaseCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float minBowDistance = 0.5f;
    public float maxBowDistance = 1.5f;
    public float minBowSize = 0f;
    public float maxBowSize = 1f;
    public float shrinkTime = 0.5f;
    public float wobbleSpeed = 2f;
    public float wobbleAmplitude = 5f;

    private GameObject bowObject;
    private PixelRotate pxRotBowObject;
    private Tween shrinkTween;

    [Header("Player Settings")]
    public Sprite[] playerSprites; //From facing up (0 degrees) to facing down (180 degrees)
    public GameObject cloudPrefab;
    private GameObject clouds;
    public Vector2 cloudOffset;
    public Color ableSleeveColor;
    public Color unableSleeveColor;

    [Header("Indicator Settings")]
    public Sprite indicatorSprite;
    SpriteRenderer indicatorSr;
    public Color indicatorColor;
    GameObject indicatorObject;

    [Header("Sound Settings")]
    public string drawNoise;
    public string releaseNoise;
    public string fullyChargedNoise;

    Vector2 launchDir;

    public override void Start()
    {
        cam = Camera.main;
        brain = cam.GetComponent<CinemachineBrain>();
        originalZoom = (brain.ActiveVirtualCamera as CinemachineVirtualCamera).m_Lens.OrthographicSize;

        CreateObjects();

        performedInAir = 0;
    }
    

    private void CreateObjects()
    {
        bowObject = new GameObject("Bow Object");
        bowSr = bowObject.AddComponent<SpriteRenderer>();
        bowSr.sprite = bowSprites[0];
        bowSr.sortingLayerName = "Objects";
        bowSr.sortingOrder = 5;
        bowSr.material = spriteMaterial;
        pxRotBowObject = bowObject.AddComponent<PixelRotate>();
        bowObject.SetActive(false);

        indicatorObject = new GameObject("Indicator Object");
        indicatorSr = indicatorObject.AddComponent<SpriteRenderer>();
        indicatorSr.sprite = indicatorSprite;
        indicatorSr.material = spriteMaterial;
        indicatorSr.color = indicatorColor;
        indicatorSr.sortingLayerName = "Objects";
        indicatorSr.sortingOrder = 5;
        indicatorObject.SetActive(false);
    }

    public override bool CheckForOverrideExit(PlayerController controller)
    {
        return false;
    }

    public override bool CheckParameter(PlayerController controller)
    {
        if (QuestManager.Instance.GetQuestStatus(quest) == false && !controller.debug)
        {
            controller.playerGFX.sr.sharedMaterial.SetColor("New_Sleeve_Color", ableSleeveColor);
            return false;
        } 
        
        if (controller.OnGround) performedInAir = 0;
        bool param = (performedInAir < timesInAir);

        if (param)
        {
            controller.playerGFX.sr.sharedMaterial.SetColor("New_Sleeve_Color", ableSleeveColor);
            //Debug.Log("Green");
        }

        return param;
    }

    protected override void Initiate(PlayerController controller)
    {
        //Handle Mechanics
        performedInAir++;
        initMousePos = GetMousePos();
        Time.timeScale = slowTimeScale;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        originalGravityScale = controller.rb.gravityScale;
        controller.rb.gravityScale = slowGravityScale;
        controller.rb.velocity = Vector2.zero;
        controller.canMove = false;

        vcam = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
        if (shrinkTween == null)
        {
            originalZoom = vcam.m_Lens.OrthographicSize;
        }
        else if (!shrinkTween.IsPlaying())
        {
            originalZoom = vcam.m_Lens.OrthographicSize;
        }

        if (bowObject == null || indicatorObject == null) CreateObjects();

        //Handle Camera Zoom
        /*
        zoomTween?.Kill();
        if (originalZoom < zoomAmount) zoomTween = DOTween.To(() => vcam.m_Lens.OrthographicSize, x => vcam.m_Lens.OrthographicSize = x, zoomAmount, zoomInTime).SetEase(Ease.InOutBack);
        */
        if (originalZoom < zoomAmount) CameraZoom.Main.ZoomCamera(zoomAmount, zoomInTime);
        shrinkTween?.Kill();

        //Handle Objects
        if (!controller.OnGround)
        {
            clouds = Instantiate(cloudPrefab, (Vector2)controller.transform.position + cloudOffset, Quaternion.identity);
        }

        //Handle Sound
        AudioManager.Instance.PlaySound(drawNoise);
    }

    protected override void OnExit(PlayerController controller, bool interrupted)
    {
        //Handle Mechanics
        Time.timeScale = 1f;
        controller.rb.gravityScale = originalGravityScale;
        controller.canMove = true;
        if (!interrupted)
        {
            float additionalLaunchStrength = Mathf.Lerp(1f, 1.5f, launchDir.y);
            controller.rb.AddForce(launchDir * launchStrength * additionalLaunchStrength * CalculateCharge(), ForceMode2D.Impulse);
        }
        controller.playerGFX.sr.sharedMaterial.SetColor("New_Sleeve_Color", unableSleeveColor);

        //Handle Camera Zoom
        /*
        zoomTween?.Kill();
        zoomTween = DOTween.To(() => vcam.m_Lens.OrthographicSize, x => vcam.m_Lens.OrthographicSize = x, originalZoom, zoomOutTime).SetEase(Ease.InOutBack);
        */
        if (originalZoom < zoomAmount) CameraZoom.Main.ResetZoom(zoomOutTime);

        //Handle Objects
        shrinkTween = bowObject.transform.DOScale(Vector3.zero, shrinkTime).SetEase(Ease.InOutBack).OnComplete(() => bowObject.SetActive(false));
        indicatorObject.SetActive(false);
        if (clouds != null)
        {
            clouds?.GetComponent<ParticleSystem>().Stop();
            Destroy(clouds, 2f);
        }

        //Handle Sound
        AudioManager.Instance.StopSound(drawNoise);
        AudioManager.Instance.PlaySound(releaseNoise);
    }

    protected override void OnHold(PlayerController controller)
    {
        Vector2 mousePos = GetMousePos();
        launchDir = ((Vector2)initMousePos - mousePos).normalized;
        float t = CalculateCharge();
        float theta = Mathf.Atan2(launchDir.y, launchDir.x) * Mathf.Rad2Deg;

        HandlePlayerVisuals(controller, theta);
        HandleBowVisuals(controller, t, theta);

        if(t == 1 && CalculateCharge(timeElapsed - Time.deltaTime) != 1)
        {
            AudioManager.Instance.PlaySound(fullyChargedNoise);
        }
    }

    private Vector2 GetMousePos()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void HandlePlayerVisuals(PlayerController controller, float theta)
    {
        float shiftedTheta = theta;
        if (theta <= 90 && theta >= -90)
        {
            //Quadrant 1 & 4
            shiftedTheta = Map(-90, 90, 0, 180, -theta);
        }
        else if (theta > 90 && theta <= 180)
        {
            //Quadrant 2
            shiftedTheta = Map(90, 180, 0, -90, theta);
        }
        else if (theta >= -180 && theta < -90)
        {
            //Quadrant 3
            shiftedTheta = Map(-180, -90, -90, -180, theta);
        }

        int spriteIndex = Mathf.RoundToInt(Map(0, 180, 0, playerSprites.Length - 1, Mathf.Abs(shiftedTheta)));
        Sprite newPlayerSprite = playerSprites[spriteIndex];

        controller.playerGFX.RequestOverride(newPlayerSprite, 50, (shiftedTheta < 0));
    }

    private void HandleBowVisuals(PlayerController controller, float t, float theta)
    {
        bowObject.SetActive(true);
        //Calculate Size
        float size = Mathf.LerpUnclamped(minBowSize, maxBowSize, bowEaseCurve.Evaluate(t));
        //Calculate Position
        float bowDistance = Mathf.LerpUnclamped(minBowDistance, maxBowDistance, bowEaseCurve.Evaluate(t));
        //Find bowSprite
        int spriteIndex = (int)Mathf.Lerp(0, bowSprites.Length - 1, t);
        Sprite newBowSprite = bowSprites[spriteIndex];

        //Apply changes
        Vector2 bowPos = (launchDir * bowDistance) + (Vector2)controller.transform.position;
        Vector3 bowSize = new Vector3(size, size, size);
        bowObject.transform.position = bowPos;
        bowObject.transform.localScale = bowSize;
        bowSr.sprite = newBowSprite;

        indicatorObject.SetActive(true);
        indicatorObject.transform.position = (-launchDir * bowDistance) + (Vector2)controller.transform.position;

        //Add wobble
        float angleOffset = 0;
        if (t >= 1)
        {
            float dif = timeElapsed - chargeTime; //Gives time since fully charged
            angleOffset = Mathf.Sin(dif * wobbleSpeed) * wobbleAmplitude;
        }
        bowObject.transform.rotation = Quaternion.Euler(0, 0, theta + angleOffset);
    }

    float CalculateCharge()
    {
        return Mathf.InverseLerp(0, chargeTime, timeElapsed);
    }

    float CalculateCharge(float timeAtMoment)
    {
        return Mathf.InverseLerp(0, chargeTime, timeAtMoment);
    }

    float Map(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {

        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }
}
