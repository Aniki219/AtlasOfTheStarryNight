using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class atlasSpriteController : MonoBehaviour
{
    playerController pc;
    SpriteRenderer sprite;

    public GameObject dustTrail;
    public ParticleSystem doubleJumpParticle;
    // Start is called before the first frame update
    void Start()
    {
        pc = transform.GetComponentInParent<playerController>();
        sprite = GetComponent<SpriteRenderer>();
        sprite.material = playerStatsManager.Instance.currentSkin;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void returnToMovement()
    {
        pc.returnToMovement();
    }

    public void resetAnimator()
    {
        pc.resetAnimator();
    }

    public void startBroom()
    {
        pc.startBroom();
    }

    public void createHitbox(HitBox hitBox)
    {
        pc.createHitbox(hitBox);
    }

    public void takeStep(float stepSize)
    {
        stepSize = 0.175f;
        if (Mathf.Ceil(AtlasInputManager.getAxisState("Dpad").x) != pc.facing) return;
        Vector3 velocity = stepSize * pc.facing * Vector3.right  +  .01f * Vector3.down;
        pc.controller.Move(velocity);
    }

}
