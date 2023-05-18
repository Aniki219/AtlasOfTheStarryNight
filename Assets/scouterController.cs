using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class scouterController : MonoBehaviour
{
    // public float speed = 8f;

    // public GameObject textPanel;
    // pauseController selectedPc;
    // // Start is called before the first frame update
    // void Start()
    // {
    //     Camera.main.GetComponent<cameraController>().target = transform;
    // }

    // // Update is called once per frame
    // void Update()
    // {
    //     if (!AtlasInputManager.getKey("Scout"))
    //     {
    //         Destroy(gameObject);
    //     }
    //     if (selectedPc && AtlasInputManager.getKeyPressed("Jump"))
    //     {
    //         textPanel.SetActive(true);
    //         textPanel.GetComponentInChildren<TextMeshProUGUI>().text = selectedPc.scoutMessage.Replace("\\n", "\n");
    //     }
    //     Vector2 velocity = AtlasInputManager.getAxisState("Dpad");
    //     transform.Translate(velocity * speed * Time.deltaTime);
    // }

    // private void OnDestroy()
    // {
    //     gameManager.Instance.pause_manager.unpause(pauseManager.PauseType.Scouter);
    //     Camera.main.GetComponent<cameraController>().target = gameManager.Instance.player.transform;
    // }

    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     pauseController pc = collision.GetComponent<pauseController>();
    //     if (pc && pc.scoutMessage != "") { 
    //         if(selectedPc)
    //         {
    //             selectedPc.scouterDeselect();
    //         }
    //         selectedPc = collision.GetComponent<pauseController>();
    //         selectedPc.scouterSelect(); 
    //     }
    // }

    // private void OnTriggerExit2D(Collider2D collision)
    // {
    //     pauseController pc = collision.GetComponent<pauseController>();
    //     if (selectedPc && pc == selectedPc)
    //     {
    //         selectedPc.scouterDeselect();
    //         selectedPc = null;
    //         textPanel.SetActive(false);
    //         textPanel.GetComponentInChildren<TextMeshProUGUI>().text = "";
    //     }
    // }
}
