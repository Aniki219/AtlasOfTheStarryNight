using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bombBerryPlantController : MonoBehaviour
{
    Animator anim;
    public float regrowTime = 5f;
    public bool canPick = true;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnBroomCollide()
    {
        if (!canPick) return;
        createBombBerry();
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
            if (hb.interactBroom)
            {
                GameObject bb = createBombBerry(false);
                bb.GetComponent<bombBerryController>().isSimulated();
                float dx = Vector3.Distance(gameManager.Instance.player.transform.position, transform.position);
                float scale = -350 * Mathf.Pow((dx - 0.5f), 2) + 200;
                bb.GetComponent<Rigidbody2D>().AddForce(new Vector2(1.25f * hb.facing, 1) * scale);
                StartCoroutine(Picked());
            }
        }
    }
}
