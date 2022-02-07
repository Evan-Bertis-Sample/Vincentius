using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LaunchOrb : Interactable
{
    public float bobTime = 2f;
    public float bobAmplitude = 0.1f;

    public Launch launchAbility;
    public bool collected;

    public float respawnTime = 3f;
    public string collectedSound;
    public GameObject collectedParticles;
    public string collectSound = "Orb";
    private SpriteRenderer sr;

    public override void OnContact(GameObject player)
    {
        if (collected) return;
        if (launchAbility.performedInAir > 0) launchAbility.performedInAir --;
        collected = true;
        Instantiate(collectedParticles, transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySound(collectedSound);
        StartCoroutine(Respawn());
    }

    public override void OnExit(GameObject player)
    {

    }

    public override void OnHold(GameObject player)
    {

    }

    private void Start() {
        transform.DOMoveY(transform.position.y + bobAmplitude, bobTime).SetLoops(-1,LoopType.Yoyo).SetEase(Ease.InOutCubic);
        sr = GetComponent<SpriteRenderer>();
        collected = false;

        RespawnManager.OnDeath += () => {
            StopCoroutine(Respawn());
            sr.color = new Color(sr.color.r, sr.color.b, sr.color.g, 1);
            collected = false;
        };
    }

    IEnumerator Respawn()
    {
        sr.color = new Color(sr.color.r, sr.color.b, sr.color.g, 0);
        yield return new WaitForSeconds(respawnTime);
        sr.DOFade(1, 0.5f).OnComplete(() => collected = false);
    }
}
