using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class equipmentPanelController : MonoBehaviour
{
    List<Material> materials;
    List<GameObject> equipmentPanels;

    public GameObject selectIcon;
    
    float easingTime = 0.25f;
    int selected;
    VectorEaser easer;

    // Start is called before the first frame update
    void OnEnable()
    {
        easer = new VectorEaser(Vector2.zero, Vector2.zero, 0);
        gameManager.setPause(gameManager.PauseType.ON);
        selected = -1;
        equipmentPanels = new List<GameObject>();
        materials = playerStatsManager.Instance.playerSkins;
        for (int i = 0; i < 8; i++)
        {
            float theta = (i * 2 * Mathf.PI / materials.Count);
            Vector2 rot = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
            Vector2 newPosition = Vector2.right + Vector2.up;
            newPosition.Scale(rot);

            GameObject panel = Instantiate(Resources.Load<GameObject>("Prefabs/UI/EquipBorder"), transform);
            panel.transform.localPosition = newPosition;

            if (materials[i])
            {
                GameObject clothesIcon = Instantiate(Resources.Load<GameObject>("Prefabs/UI/ClothesIcon"), panel.transform);
                clothesIcon.GetComponent<Image>().material = materials[i];
            }

            equipmentPanels.Add(panel);
        }
        selectIcon.transform.SetAsLastSibling();
        appear();
    }

    void Update()
    {
        Vector2 input = AtlasInputManager.getAxisState("Dpad");
        int index = -1;
        if (input.x == 0 && input.y == 0)
        {

        } else {
            float theta = Mathf.Atan2(input.y, input.x);
            theta *= Mathf.Rad2Deg;
            theta += 360.0f;
            index = (int)Mathf.Round(theta / 45.0f);
            index %= 8;
        }

        if (input.normalized != easer.end) {
            easer = new VectorEaser(selectIcon.transform.localPosition, 
                input.normalized, 
                input.magnitude == 0 ? 0.25f : 0.1f,
                Ease.OutQuart);
        }

        if (easer != null) selectIcon.transform.localPosition = easer.Update();
        if (easer.isComplete) {
            selected = index;
        }

    }

    void OnDisable()
    {
        gameManager.setPause(gameManager.PauseType.OFF);
        if (selected >= 0)
        {
            Material selectedMat = materials[selected];
            if (selectedMat != null)
            {
                playerStatsManager.Instance.swapSkin(selectedMat);
            }
        }
        selected = -1;
        foreach (Transform t in transform)
        {
            if (t.name == "SelectIcon") continue;
            Destroy(t.gameObject);
        }
    }

    void OnDestroy()
    {
        gameManager.Instance.pauseMenus.Remove(gameObject);
    }

    async void appear()
    {
        transform.localScale = Vector3.zero;
        transform.localEulerAngles = Vector3.forward * -180;

        float startTime = Time.unscaledTime;

        float duration = 0.3f;
        while (transform.localScale.x < 1.0f)
        {
            float p = (Time.unscaledTime - startTime) / duration;
            transform.localScale = Vector3.one * p;
            transform.localEulerAngles = Vector3.forward * p * 360.0f;
            await System.Threading.Tasks.Task.Delay(16);
        }

        transform.localScale = Vector3.one;
        transform.localEulerAngles = Vector3.zero;
    }
}
