using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popHeldBerry : MonoBehaviour
{
    public GameObject pickParticle;
    public AudioClip pickSound;

    public enum BerryType
    {
        Bump,
        Woosh
    }
    public BerryType berryType;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 0.5f);
    }

    private void OnDestroy()
    {
        if (pickSound) SoundManager.Instance.playClip(pickSound.name);
        gameManager.createInstance(pickParticle, transform.position);
        gameManager.Instance.playerCtrl.returnToMovement();
        if (berryType == BerryType.Bump)
        {
            gameManager.Instance.playerCtrl.bounce(9.0f);
        }

        if (berryType == BerryType.Woosh)
        {
            gameManager.Instance.playerCtrl.triggerBroomStart(true, transform.localScale.x);
        }
    }
}
