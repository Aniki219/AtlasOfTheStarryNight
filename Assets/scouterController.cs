using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scouterController : MonoBehaviour
{
    public float speed = 8f;
    // Start is called before the first frame update
    void Start()
    {
        Camera.main.GetComponent<cameraController>().target = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!AtlasInputManager.getKey("Scout"))
        {
            Destroy(gameObject);
        }
        Vector2 velocity = AtlasInputManager.getAxisState("Dpad");
        transform.Translate(velocity * speed * Time.deltaTime);
    }

    private void OnDestroy()
    {
        gameManager.Instance.pause_manager.unpause(pauseManager.PauseType.Scouter);
        Camera.main.GetComponent<cameraController>().target = gameManager.Instance.player.transform;
    }
}
