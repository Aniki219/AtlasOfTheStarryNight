using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Deformer), typeof(BoxCollider2D))]
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

    public GameObject pickParticle;
    public AudioClip pickSound;
    public AudioClip regrowSound;

    Deformer deformer;

    private void Start()
    {
        anim = GetComponent<Animator>();
        deformer = GetComponent<Deformer>();
    }

    public void bumpPlayer(HitBox hb)
    {
        if (hb != null && hb.name != "")
        {
            gameManager.Instance.playerCtrl.bounce(9);
        }
        StartCoroutine(Picked());
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
        if (player)
        {
            gameManager.Instance.playerCtrl.hitLag();
            Invoke("playerBroom", 0.12f);
        }
            StartCoroutine(Picked());
    }
    public void playerBroom()
    {
        gameManager.Instance.playerCtrl.triggerBroomStart(true, transform.localScale.x);
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
        deformer.flashWhite(0.2f);
        if (pickSound) SoundManager.Instance.playClip(pickSound.name);
        if (pickParticle) Instantiate(pickParticle, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.1f);
        anim.SetTrigger("Picked");
        yield return new WaitForSeconds(regrowTime);
        anim.SetTrigger("Regrow");
        yield return new WaitForSeconds(0.5f);
        if (regrowSound) SoundManager.Instance.playClip(regrowSound.name, -2, transform.position);
        canPick = true;
        yield return 0;
    }

    public Vector3 getDir()
    {
        return transform.right * transform.localScale.x + transform.up * transform.localScale.y;
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
            gameManager.Instance.player.GetComponent<Deformer>().flashWhite();
            HitBox hb = collision.GetComponent<AllyHitBoxController>().hitbox;
            if (hb.broom)
            {
                if (tag == "WooshBerryPlant")
                {
                    broomCallback.Invoke(true);
                }
                if (tag == "BumpBerryPlant")
                {
                    pickCallback.Invoke(hb);
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
