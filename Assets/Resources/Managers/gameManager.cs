using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Managers/GameManager")]
public class gameManager : ScriptableObject
{
    private static gameManager instance;
    public static gameManager Instance { get { return instance; } }

    public pauseManager pause_manager;

    public static int numberOfStarts = 0;

    private static bool isShuttingDown = false;
    private static Dictionary<string, bool> objects = new Dictionary<string, bool>();

    public float gravity = -17.6f;
    public float maxFallVel = -15f;

    public canvasController canvasCtrl;
    public GameObject player;
    public playerController playerCtrl;
    public Transform playerHanger;

    private float playerStartX;
    private float playerStartY;
    public bool canSetPosition;

    public string currentDoorLabel = "none";

    public List<GameObject> pauseMenus;

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        numberOfStarts = 0;
        isShuttingDown = false;
        instance = Resources.LoadAll<gameManager>("Managers")[0];
        SceneManager.sceneLoaded += onSceneLoad;
        instance.canSetPosition = false;
        instance.currentDoorLabel = "none";
        //Calls setPlayer
        onSceneLoad(SceneManager.GetActiveScene(), LoadSceneMode.Single);

        instance.pauseMenus = new List<GameObject>();
        instance.pause_manager = (pauseManager)ScriptableObject.CreateInstance("pauseManager");
        instance.pause_manager.Init();

        Application.quitting += OnApplicationQuit;
    }

    public void setPlayer()
    {
        GameObject atlas = GameObject.Find("Atlas");

        if (SceneManager.GetActiveScene().name == "Main Menu") return;

        if (atlas)
        {
            instance.player = atlas;
        } else
        {
            instance.player = createInstance("RoomSetup/Atlas", Vector3.zero);
        }
        instance.playerHanger = instance.player.transform.Find("AtlasSprite/Hanger");
        instance.playerCtrl = instance.player.GetComponent<playerController>();
        
        if (GameObject.Find("GameCanvas"))
        {
            instance.canvasCtrl = GameObject.Find("GameCanvas").GetComponent<canvasController>();
        }

        if (instance.canSetPosition && instance.player)
        {
            instance.player.transform.position = new Vector3(playerStartX, playerStartY, 0);
            Physics2D.SyncTransforms();
            instance.player.GetComponent<characterController>().collisions.setCollidable(true);
            playerController.State playerState = playerCtrl.depState;
            if (instance.playerCtrl.depState == playerController.State.Wait)
            {
                playerCtrl.returnToMovement(playerState);
                canvasCtrl.doBlackout(false);
            }
        }
        instance.canSetPosition = true;   
    }

    public void switchScene(string to, float startx, float starty)
    {
        playerStartX = startx;
        playerStartY = starty;
        playerCtrl.controller.collisions.setCollidable(false);
        SceneManager.LoadScene(to);
    }

    public static void switchSceneButton(string to)
    {
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
        if (instance.playerCtrl) instance.playerCtrl.setLastSafePosition();
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
        if (key == null) return;
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
            }
        }
    }

    public static GameObject createInstance(string name, Vector3 at, Transform parent = null)
    {
        if (isShuttingDown) return null;
        GameObject inst = Instantiate(Resources.Load<GameObject>("Prefabs/" + name), at, Quaternion.identity, parent);
        return inst;
    }

    public static GameObject createInstance(GameObject prefab, Vector3 at, Transform parent = null)
    {
        if (isShuttingDown) return null;
        GameObject inst = Instantiate(prefab, at, Quaternion.identity, parent);
        return inst;
    }

    public static void OnApplicationQuit()
    {
        isShuttingDown = true;
    }
}
