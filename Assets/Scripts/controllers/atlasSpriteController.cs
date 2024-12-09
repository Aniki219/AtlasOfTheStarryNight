using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AtlasSpriteController : MonoBehaviour
{
    PlayerController pc;
    SpriteRenderer sprite;

    public GameObject dustTrail;
    public ParticleSystem momentumTrail;

    public float targetZRotation;
    private float currentZRotation;
    private float zSmoothing;

    // Start is called before the first frame update
    void Start()
    {
        pc = transform.GetComponentInParent<PlayerController>();
        sprite = GetComponent<SpriteRenderer>();
        sprite.material = playerStatsManager.Instance.currentSkin;
    }

    // Update is called once per frame
    void Update()
    {
        //momentumTrail.gameObject.SetActive(true);//pc.hasMomentum);

        momentumTrail.textureSheetAnimation.SetSprite(0, sprite.sprite);

        currentZRotation = Mathf.SmoothDampAngle(currentZRotation, targetZRotation, ref zSmoothing, 0.05f);

        transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, currentZRotation);
    }

    public void SetZRotation(float angle) {
        sprite.transform.rotation = Quaternion.Euler(new Vector3(sprite.transform.localEulerAngles.x,
                                                                 sprite.transform.localEulerAngles.y,
                                                                 angle));
        targetZRotation = angle;
        currentZRotation = angle;
        zSmoothing = 0;
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
        // pc.startBroom();
    }

    public void createHitbox(HitBox hitBox)
    {
        pc.createHitbox(hitBox);
    }

    public void takeStep(float stepSize)
    {
        stepSize = 0.175f;
        if (Mathf.Ceil(AtlasInputManager.getAxis("Dpad").getValue().x) != pc.facing) return;
        Vector3 velocity = stepSize * pc.facing * Vector3.right  +  .01f * Vector3.down;
        pc.controller.Move(velocity);
    }

}
