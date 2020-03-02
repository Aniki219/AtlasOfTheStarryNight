using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followController : MonoBehaviour
{
    public AudioClip collectSound;
    public AudioClip breakSound;
    public followController following;
    public followController followedBy;
    public bool canMoveParent = true;
    public bool canCollect = true;

    playerController pc;

    Vector3 startLocation;
    Vector3 smoothVelocity;
    bool breakingChain = false;

    void Start()
    {
        pc = GetComponent<playerController>();
        startLocation = transform.position;
        following = null;
        followedBy = null;
    }

    void Update()
    {
        if (!breakingChain && pc && followedBy && pc.isGrounded())
        {
            breakingChain = true;
            Invoke("breakChain", 0.5f);
            SoundManager.Instance.playClip("Collectibles/dropStarShards");
        }

        if (canMoveParent)
        {
            Vector3 targetLocation = transform.position;
            if (!following)
            {
                targetLocation = startLocation;
                transform.position = Vector3.SmoothDamp(transform.position, targetLocation, ref smoothVelocity, 0.2f);
                if (!canCollect && Vector3.SqrMagnitude(transform.position - targetLocation) < 0.5f)
                {
                    canCollect = true;
                }
            }
            else
            {
                targetLocation = following.transform.position;

                if (Vector3.SqrMagnitude(transform.position - following.transform.position) > 0.5f)
                {
                    transform.position = Vector3.SmoothDamp(transform.position, targetLocation, ref smoothVelocity, 0.2f);
                }
            }
        }
    }

    public void breakChain()
    {
        breakingChain = false;
        if (followedBy)
        {
            followedBy.breakChain();
        }
        following = null;
        followedBy = null;
    }
}
