using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class collectiblesController : MonoBehaviour
{
    public string uiTag = "MarsCelestium";
    public float moveSpeed = 0.75f;

    bool collected = false;
    bool canCollect = true;
    GameObject uiElement;

    string uid;
    public bool persistant = true;

    public string collectionClipPath = "collectCelestium2";

    private void Start()
    {
        uiElement = GameObject.FindGameObjectWithTag(uiTag);
    }

    public void collect()
    {
        collected = true;
        SoundManager.Instance.playClip(collectionClipPath);
    }

    private void Update()
    {
        if (collected)
        {
            if (canCollect)
            {
                canCollect = false;
                if (uiTag == "Star")
                {
                    collected = false;
                    StartCoroutine(collectStar());
                }
                collectiblesUIController controller = uiElement.transform.parent.GetComponent<collectiblesUIController>();
                controller.addOne();
            }

            cameraController cc = Camera.main.GetComponent<cameraController>();
            Vector3 screenPoint = new Vector3(0, 1, 0);
            Vector3 worldPos = Camera.main.ViewportToWorldPoint(screenPoint) + uiElement.transform.position/128.0f;
            transform.position = Vector3.MoveTowards(transform.position, worldPos, moveSpeed);

            if (Vector3.Magnitude(transform.position - worldPos) < 1f)
            {
                Destroy(gameObject);
            }
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
            transform.position = player.transform.position + Vector3.up * (1.0f + elapsedTime/4.0f);
            yield return new WaitForEndOfFrame();
        }
        

        GetComponent<ParticleSystem>().Play();
        Destroy(gameObject, 1.0f);
        yield return 1.0;
        pc.returnToMovement();
    }
}

