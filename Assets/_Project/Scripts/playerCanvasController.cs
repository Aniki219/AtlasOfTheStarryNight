using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class playerCanvasController : MonoBehaviour
{
    public Transform equipmentPanel;
    public TextMeshProUGUI shoutoutText;

    // Start is called before the first frame update
    void Start()
    {
        equipmentPanel = transform.Find("EquipmentPanel");
    }

    // Update is called once per frame
    void Update()
    {
        equipmentPanel.gameObject.SetActive(AtlasInputManager.getKey("DisplayEquipment"));
    }

    public void Shoutout(string text) {
        TextMeshProUGUI shoutout = (TextMeshProUGUI)Instantiate(shoutoutText, transform.position, Quaternion.identity, transform);
        shoutout.text = text;
    }
}
