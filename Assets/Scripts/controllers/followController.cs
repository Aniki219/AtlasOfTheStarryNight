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

    PlayerController pc;

    Vector3 startLocation;
    Vector3 smoothVelocity;
    bool breakingChain = false;

    public Actions groundAction = Actions.Collect;

    public enum Actions {
        None,
        Collect,
        CheckFive
    }

    void Start()
    {
        pc = GetComponent<PlayerController>();
        startLocation = transform.position;
        following = null;
        followedBy = null;
    }

    IEnumerator callBreakChain(bool hurt = false, int num = 0)
    {
        yield return new WaitForSeconds(0.5f);
        breakChain(hurt, num);
    }

    void Update()
    {
        //Here we trigger to resolve what happens when the player touches the ground
        //or gets hurt
        if (!breakingChain && pc && followedBy && (pc.isGrounded() || pc.resetPosition))
        {
            breakingChain = true;

            followController fc = this;
            int num;
            for (num = 0; fc.followedBy != null; num++)
            {
                fc = fc.followedBy;
            }
            StartCoroutine(callBreakChain(pc.resetPosition, num));
        }

        //Either we follow who we are following. Or we reset to the original position
        //canMoveParent means that this followController can move the object it's attached to
        if (canMoveParent)
        {
            Vector3 targetLocation = transform.position;
            if (!following)
            {
                targetLocation = startLocation;
                transform.position = Vector3.SmoothDamp(transform.position, targetLocation, ref smoothVelocity, 0.2f);

                //We need to check the distance to the startPosition so that we can
                //reset canCollect
                if (!canCollect && Vector3.SqrMagnitude(transform.position - targetLocation) < 0.5f)
                {
                    canCollect = true;
                }
            }
            else
            {
                targetLocation = following.transform.position;

                //We use disance to space the collectibles from each other while they follow
                //the player
                if (Vector3.SqrMagnitude(transform.position - following.transform.position) > 0.5f)
                {
                    transform.position = Vector3.SmoothDamp(transform.position, targetLocation, ref smoothVelocity, 0.2f);
                }
            }
        }
    }

    public void breakChain(bool hurt = false, int chainLength = 0)
    {
        //Once the chain has started breaking we must recursively break every link
        //We use breakingChain because we have a half second coroutine delay
        breakingChain = false;
        if (followedBy)
        {
            followedBy.breakChain(hurt, chainLength);
        }
        following = null;
        followedBy = null;

        switch (groundAction)
        {
            //This is for collecting starFragments
            case Actions.CheckFive:
                if (chainLength >= 5)
                {
                    GameObject starSwirler = GameObject.Find("StarSwirler");
                    if (!starSwirler)
                    {
                        Debug.LogWarning("No star swirler!");
                    } else {
                        transform.parent = starSwirler.transform;
                        enabled = false;
                    }
                } else
                {
                    SoundManager.Instance.playClip("Collectibles/dropStarShards");
                }
                break;
            case Actions.Collect:
                if (!hurt)
                {
                    collectiblesController cc = GetComponent<collectiblesController>();
                    if (cc)
                    {
                        cc.collect();
                        enabled = false;
                    }
                }
                break;
            default:
                break;
        }
    }
}
