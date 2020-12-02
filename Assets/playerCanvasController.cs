using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerCanvasController : MonoBehaviour
{
    public Transform equipmentPanel;

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
}
