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
    [Tooltip("Forces berry to launch objects forward only")]
    public bool forwardLaunch;

    public PickEvent pickCallback;
    [System.Serializable]
    public class PickEvent : UnityEvent<HitBox> { }

    public BroomEvent broomCallback;
    [System.Serializable]
    public class BroomEvent : UnityEvent<bool> { }

    public GameObject liftObject;
    public GameObject pickParticle;
    public AudioClip pickSound;
    public AudioClip regrowSound;

    Deformer deformer;
    BoxCollider2D col;
    [HideInInspector] public Vector3 center;

    public bool sayFwd = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        deformer = GetComponent<Deformer>();
        col = GetComponent<BoxCollider2D>();

        Vector2 scenePos = AtlasSceneManager.getPlayerCoords();
        if (scenePos.x >= 6 && scenePos.x <= 12 && scenePos.y >= -1 && scenePos.y <= 4)
        {
            setDry();
        }

        AtlasEventManager.Instance.onFlagSet += setDry;
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
            .AddForce(Vector2.Scale(new Vector2(1.25f, 1), hb.kbDir) * 150.0f);
        StartCoroutine(Picked());
    }

    IEnumerator Picked(bool playSound = true)
    {
        canPick = false;
        deformer.flashWhite(0.2f);
        if (playSound)
        {
            if (pickSound) SoundManager.Instance.playClip(pickSound.name);
            if (pickParticle) Instantiate(pickParticle, center, Quaternion.identity);
        }

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
        Vector2 fwd = getForward();
        Vector2 upwd = new Vector2(Mathf.Round(transform.up.x), Mathf.Round(transform.up.y));
        upwd *= Mathf.Round(transform.localScale.y);
        if (forwardLaunch) return getForward();
        return fwd + upwd;
    }
    public Vector3 getForward()
    {
        Vector2 myRight = (transform.parent ? transform.parent.right : transform.right);
        Vector2 fwd = new Vector2(Mathf.Round(myRight.x), Mathf.Round(myRight.y));
        return fwd * Mathf.Round(transform.localScale.x);
    }

    GameObject createBombBerry(bool atPlayer = false)
    {
        Vector3 at = center;
        Transform parent = null;
        Transform hanger = gameManager.Instance.playerHanger;
        if (atPlayer)
        {
            parent = hanger;
            at = hanger.position - Vector3.up * 0.15f;
        }
        
        return gameManager.createInstance("LevelPrefabs/Level Objects/BombBerry", at, parent);
    }

    private void Update()
    {
        center = col.bounds.center;
    }

    private void setDry()
    {
        if (
                CompareTag("BombBerryPlant") && !gameFlagsManager.Instance.checkFlag("GardenBombs") ||
                CompareTag("BumpBerryPlant") && !gameFlagsManager.Instance.checkFlag("GardenBumps") ||
                CompareTag("WooshBerryPlant") && !gameFlagsManager.Instance.checkFlag("GardenWoosh"))
        {
            anim.SetBool("Dry", true);
            col.enabled = false;
        }
        else
        {
            anim.SetBool("Dry", false);
            col.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AllyHitbox") && canPick)
        {
            AllyHitBoxController hbc = collision.GetComponent<AllyHitBoxController>();
            if (hbc.hasHit) return;

            hbc.hasHit = true && tag != "BombBerryPlant";
            gameManager.Instance.player.GetComponentInChildren<Deformer>().flashWhite();
            HitBox hb = hbc.hitbox;
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!(transform.parent && transform.parent.name == "seed") && collision.CompareTag("Player") && canPick && AtlasInputManager.getKeyPressed("Up", true))
        {
            if (gameManager.Instance.playerCtrl.canLift())
            {
                GameObject berry = gameManager.createInstance(liftObject, center);
                gameManager.Instance.playerCtrl.liftObject(berry.transform.Find("handle").gameObject);
                StartCoroutine(Picked(false));
            }
        }
    }

    private void OnDestroy()
    {
        AtlasEventManager.Instance.onFlagSet -= setDry;
    }
}
