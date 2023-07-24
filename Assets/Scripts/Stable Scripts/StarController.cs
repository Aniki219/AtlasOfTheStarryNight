using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : MonoBehaviour
{
    bool canCollect = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canCollect && collision.CompareTag("Player"))
        {
            canCollect = false;
            StartCoroutine(collectStar());
        }
    }

    IEnumerator collectStar()
    {
        float time = 3.0f;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerController pc = player.GetComponent<PlayerController>();

        pc.pauseAnimator();
        pc.cutScenePrep();


        GetComponent<SpriteRenderer>().enabled = false;
        transform.position = player.transform.position + Vector3.up * 1.0f;
        yield return new WaitForSeconds(0.25f);
        GetComponent<SpriteRenderer>().enabled = true;

        cameraController cc = Camera.main.GetComponent<cameraController>();
        cc.StartShake(0.1f, time);

        float startTime = Time.time;
        float elapsedTime = 0.0f;

        while (elapsedTime < time)
        {
            transform.GetComponentInChildren<ParticleSystem>().Play();
            elapsedTime = Time.time - startTime;
            transform.position = player.transform.position + Vector3.up * (1.0f + elapsedTime / 4.0f);
            yield return new WaitForEndOfFrame();
        }

        GetComponent<ParticleSystem>().Play();
        Destroy(gameObject, 1.0f);
        yield return 1.0;
        GetComponent<persistance>().MarkRemoved();
        gameManager.numberOfStarts++;
        pc.returnToMovement();
    }
}
