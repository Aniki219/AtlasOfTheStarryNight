using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BerryPlantController : MonoBehaviour
{
    Animator anim;
    public float regrowTime = 5f;
    public bool canPick = true;

    public PickEvent pickCallback;
    [System.Serializable]
    public class PickEvent : UnityEvent<HitBox> { }

    public BroomEvent broomCallback;
    [System.Serializable]
    public class BroomEvent : UnityEvent<bool> { }

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnBroomCollide()
    {
        if (!canPick) return;
        broomCallback.Invoke(true);
    }

    public void bombBerryOnBroom()
    {
        createBombBerry(true);
        StartCoroutine(Picked());
    }

    public void wooshBerryOnBroom(bool player = false)
    {
        if (player) gameManager.Instance.playerCtrl.triggerBroomStart(true, transform.localScale.x);
        StartCoroutine(Picked());
    }

    public void bombBerryCallback(HitBox hb)
    {
        GameObject bb = createBombBerry(false);
        bb.GetComponent<bombBerryController>().isSimulated();
        bb.GetComponent<Rigidbody2D>()
            .AddForce(new Vector2(1.25f * hb.kbDir.x, 1) * 150.0f);
        StartCoroutine(Picked());
    }

    IEnumerator Picked()
    {
        canPick = false;
        anim.SetTrigger("Picked");
        yield return new WaitForSeconds(regrowTime);
        anim.SetTrigger("Regrow");
        yield return new WaitForSeconds(0.5f);
        canPick = true;
        yield return 0;
    }

    GameObject createBombBerry(bool atPlayer = true)
    {
        Vector3 at = transform.position;
        Transform parent = null;
        Transform hanger = gameManager.Instance.playerHanger;
        if (atPlayer)
        {
            parent = hanger;
            at = hanger.position - Vector3.up * 0.15f;
        }
        
        return gameManager.Instance.createInstance("LevelPrefabs/Level Objects/BombBerry", at, parent);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AllyHitbox") && canPick)
        {
            HitBox hb = collision.GetComponent<AllyHitBoxController>().hitbox;
            if (hb.broom)
            {
                if (tag == "WooshBerryPlant")
                {
                    broomCallback.Invoke(true);
                }
            }
            if (hb.interactBroom)
            {
                if (tag == "BombBerryPlant")
                {
                    pickCallback.Invoke(hb);
                }
            }
        }
    }
}
