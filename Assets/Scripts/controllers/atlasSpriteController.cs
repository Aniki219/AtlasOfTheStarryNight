using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class atlasSpriteController : MonoBehaviour
{
    playerController pc;
    SpriteRenderer sprite;
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
}
