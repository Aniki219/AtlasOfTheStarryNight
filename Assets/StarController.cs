using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : MonoBehaviour
{
    bool canCollect = true;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
        float time = 5.0f;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerController pc = player.GetComponent<playerController>();

        pc.cutScenePrep();


        GetComponent<SpriteRenderer>().enabled = false;
        transform.position = player.transform.position + Vector3.up * 1.0f;
        yield return new WaitForSeconds(0.25f);
        GetComponent<SpriteRenderer>().enabled = true;

        cameraController cc = Camera.main.GetComponent<cameraController>();
        cc.StartShake(0.1f, time);

        float startTime = Time.time;
        float elapsedTime = 0.0f;
        int frameNum = 0;
        while (elapsedTime < time)
        {
            frameNum++;
            if (frameNum % 20 == 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    GameObject starParticle = (GameObject)Instantiate(Resources.Load<GameObject>("Prefabs/Effects/StarParticle"), transform.position, Quaternion.identity);
                    float speed = 10.0f;
                    float x = speed * Mathf.Cos(i * 2 * Mathf.PI / 5.0f);
                    float y = speed * Mathf.Sin(i * 2 * Mathf.PI / 5.0f);
                    starParticle.GetComponent<starParticleController>().velocity = new Vector3(x, y, 0);
                }
            }
            elapsedTime = Time.time - startTime;
            transform.position = player.transform.position + Vector3.up * (1.0f + elapsedTime / 4.0f);
            yield return new WaitForEndOfFrame();
        }

        GetComponent<ParticleSystem>().Play();
        Destroy(gameObject, 1.0f);
        yield return 1.0;
        pc.returnToMovement();
    }
}
