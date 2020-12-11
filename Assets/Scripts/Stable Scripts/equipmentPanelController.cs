using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class equipmentPanelController : MonoBehaviour
{
    List<Material> materials;
    List<GameObject> equipmentPanels;

    public GameObject selectIcon;
    int selected;
    Vector2 selectIconVel;

    // Start is called before the first frame update
    void OnEnable()
    {
        gameManager.Instance.pauseMenus.Add(gameObject);
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
        StartCoroutine(appear());
    }

    void Update()
    {
        Vector2 input = AtlasInputManager.getAxisState("Dpad");
        if (input.x == 0 && input.y == 0)
        {
            selectIcon.transform.localPosition = Vector2.zero;
            selectIcon.SetActive(false);
            return;
        }
        selectIcon.SetActive(true);
        float theta = Mathf.Atan2(input.y, input.x);
        theta *= Mathf.Rad2Deg;
        theta += 360.0f;
        int index = (int)Mathf.Round(theta / 45.0f);
        index %= 8;
        selected = index;
        selectIcon.transform.localPosition = Vector2.SmoothDamp(
                selectIcon.transform.localPosition, 
                input.normalized, 
                ref selectIconVel, 
                0.05f);
    }

    void OnDisable()
    {
        gameManager.Instance.pauseMenus.Remove(gameObject);
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
        StopAllCoroutines();
    }

    void OnDestroy()
    {
        gameManager.Instance.pauseMenus.Remove(gameObject);
    }

    IEnumerator appear()
    {
        transform.localScale = Vector3.zero;
        transform.localEulerAngles = Vector3.forward * -180;

        float duration = 0.3f;
        while (transform.localScale.x < 1.0f)
        {
            float tstep = Time.deltaTime / duration;
            transform.localScale += Vector3.one * tstep * 1.0f;
            transform.localEulerAngles += Vector3.forward * tstep * 360.0f;
            yield return new WaitForEndOfFrame();
        }

        transform.localScale = Vector3.one;
        transform.localEulerAngles = Vector3.zero;
    }
}
