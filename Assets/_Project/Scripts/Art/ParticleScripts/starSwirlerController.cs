using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class starSwirlerController : MonoBehaviour
{
    float r = 5.0f;

    // Update is called once per frame
    void Update()
    {
        if (r <= 0.1f)
        {
            GameObject star = (GameObject)Instantiate(Resources.Load<GameObject>("Prefabs/LevelPrefabs/Collectibles/Star"), transform.position, Quaternion.identity);
            star.GetComponent<ParticleSystem>().Play();
            Destroy(gameObject);
        }
        if (transform.childCount >= 5)
        {
            r -= 2.5f * Time.deltaTime;
            for (int i = 0; i < transform.childCount; i++)
            {
                float x = r * Mathf.Cos(i * 2 * Mathf.PI / transform.childCount + transform.eulerAngles.z * Mathf.Deg2Rad);
                float y = r * Mathf.Sin(i * 2 * Mathf.PI / transform.childCount + transform.eulerAngles.z * Mathf.Deg2Rad);
                Vector3 targetLocation = transform.position + new Vector3(x, y, 0);
                Transform t = transform.GetChild(i);
                t.position = Vector3.Lerp(t.position, targetLocation, 0.2f);
            }
        }
    }
}
