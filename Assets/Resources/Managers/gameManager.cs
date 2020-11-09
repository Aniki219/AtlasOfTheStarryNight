using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

[CreateAssetMenu(menuName = "Managers/GameManager")]
public class gameManager : ScriptableObject
{
    private static gameManager instance;
    public static gameManager Instance { get { return instance; } }

    static Dictionary<string, bool> objects = new Dictionary<string, bool>();
    public float gravity = -17.6f;
    public float maxFallVel = -15f;

    public GameObject player;
    public playerController playerCtrl;
    public Transform playerHanger;

    private float playerStartX;
    private float playerStartY;
    private bool playerStartJump;
    private bool canSetPosition;

    public string currentDoorLabel = "none";

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        instance = Resources.LoadAll<gameManager>("Managers")[0];
        SceneManager.sceneLoaded += setGameObjects;
        instance.setPlayer();
        instance.playerStartJump = false;
        instance.canSetPosition = false;
        instance.currentDoorLabel = "none";
    }

    public void setPlayer()
    {
        instance.player = GameObject.Find("Atlas");
        instance.playerHanger = instance.player.transform.Find("Atlas/AtlasSprite/Hanger");
        instance.playerCtrl = instance.player.GetComponent<playerController>();

        if (instance.canSetPosition)
        {
            player.transform.position = new Vector3(playerStartX, playerStartY, 0);
            if (instance.playerStartJump)
            {
                playerCtrl.bounce(8.0f);
                playerStartJump = false;
            }
        }
        instance.canSetPosition = true;
    }

    public void switchScene(string to, float startx, float starty, bool startJump = false)
    {
        playerStartX = startx;
        playerStartY = starty;
        playerStartJump = startJump;
        SceneManager.LoadScene(to);
    }

    static void setGameObjects(Scene scene, LoadSceneMode mode)
    {
        
    }

    public bool checkObjectKey(string key)
    {
        //foreach (KeyValuePair<string, bool> keyValues in objects)
        //{
        //    Debug.Log(keyValues.Key + " : " + keyValues.Value);
        //}
        if (objects.ContainsKey(key)) { return objects[key]; }
        objects.Add(key, true);
        return true;
    }

    public void setObjectKey(string key, bool value)
    {
        if (objects.ContainsKey(key)) {
            objects[key] = value;
        } else {
            objects.Add(key, value);
        }
    }

    public GameObject createInstance(string name, Vector3 at, Transform parent = null)
    {
        GameObject inst = Instantiate(Resources.Load<GameObject>("Prefabs/" + name), at, Quaternion.identity, parent);
        return inst;
    }
}
