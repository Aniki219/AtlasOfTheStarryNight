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

    public canvasController canvasCtrl;
    public GameObject player;
    public playerController playerCtrl;
    public Transform playerHanger;

    private float playerStartX;
    private float playerStartY;
    private bool canSetPosition;

    public string currentDoorLabel = "none";

    public List<GameObject> pauseMenus;

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        instance = Resources.LoadAll<gameManager>("Managers")[0];
        SceneManager.sceneLoaded += onSceneLoad;
        instance.canSetPosition = false;
        instance.currentDoorLabel = "none";
        //Calls setPlayer
        onSceneLoad(SceneManager.GetActiveScene(), LoadSceneMode.Single);

        instance.pauseMenus = new List<GameObject>();
    }

    public void setPlayer()
    {
        instance.player = GameObject.Find("Atlas");
        instance.canvasCtrl = GameObject.Find("GameCanvas").GetComponent<canvasController>();
        instance.playerHanger = instance.player.transform.Find("AtlasSprite/Hanger");
        instance.playerCtrl = instance.player.GetComponent<playerController>();

        if (instance.canSetPosition)
        {
            instance.player.transform.position = new Vector3(playerStartX, playerStartY, 0);
            Physics2D.SyncTransforms();
            instance.player.GetComponent<characterController>().setCollidable(true);
            if (instance.playerCtrl.state == playerController.State.Wait)
            {
                playerCtrl.returnToMovement();
                canvasCtrl.doBlackout(false);
            }
        }
        instance.canSetPosition = true;
    }

    public void switchScene(string to, float startx, float starty)
    {
        playerStartX = startx;
        playerStartY = starty;
        playerCtrl.controller.setCollidable(false);
        SceneManager.LoadScene(to);
    }

    static void onSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (instance.currentDoorLabel != "none")
        {
            GameObject door = null;
            GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
            if (doors.Length == 1)
            {
                door = doors[0];
            }
            if (doors.Length > 1)
            {
                foreach (GameObject d in doors)
                {
                    if (d.GetComponent<doorController>().label == instance.currentDoorLabel)
                    {
                        door = d;
                        break;
                    }
                }
            }
            if (door != null)
            {
                instance.playerStartX = door.transform.position.x;
                instance.playerStartY = door.transform.position.y;
            }
        }
        instance.setPlayer();
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

    public void clearPersistence(string scene)
    {
        List<string> keys = new List<string>(objects.Keys);
        foreach (string key in keys)
        {
            if (key.Contains(scene))
            {
                objects[key] = true;
                Debug.Log(objects[key]);
            }
        }
    }

    public GameObject createInstance(string name, Vector3 at, Transform parent = null)
    {
        GameObject inst = Instantiate(Resources.Load<GameObject>("Prefabs/" + name), at, Quaternion.identity, parent);
        return inst;
    }
}
