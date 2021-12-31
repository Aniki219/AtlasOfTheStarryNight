using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public SceneReference mainmenu;
    static bool created = false;

    public GameObject PausePanel;
    public GameObject PauseMenu;
    public GameObject ControlsMenu;
    public GameObject CheatsMenu;

    public GameObject ControlSchemeArrow;
    public Transform ControlSchemes;

    public Text broomStyle;
    public Text doubleJumpText;
    public Text wallJumpText;
    public Text invulnerableText;

    string newControlScheme;

    playerController.State prevState = playerController.State.Movement;

    public enum MenuState
    {
        Closed,
        Pause,
        Cheats,
        Controls
    }
    public MenuState state = MenuState.Closed;

    void Start()
    {
        //togglePauseMenu(false);
        state = MenuState.Closed;
        toggleBroomStyle(false);
    }

    private void Update()
    {
        if (AtlasInputManager.getKeyPressed("Escape"))
        {
            if (SceneManager.GetActiveScene().name == "Main Menu")
            {
                Application.Quit();
            } else
            {
                switch (state)
                {
                    case MenuState.Closed:
                        togglePauseMenu(true);
                        break;
                    case MenuState.Pause:
                        togglePauseMenu(false);
                        break;
                    case MenuState.Controls:
                        togglePauseMenu(true);
                        break;
                    case MenuState.Cheats:
                        togglePauseMenu(true);
                        break;
                }
            }
        }
    }

    public void togglePauseMenu(bool open = true)
    {
        if (open) prevState = playerController.State.Movement;
        if (open) gameManager.Instance.playerCtrl.cutScenePrep();

        gameManager.Instance.playerCtrl.state = open ? playerController.State.Wait : prevState;

        PausePanel.SetActive(open);
        PauseMenu.SetActive(open);
        CheatsMenu.SetActive(false);
        ControlsMenu.SetActive(false);
        ControlSchemeArrow.SetActive(false);
        state = open ? MenuState.Pause : MenuState.Closed;

    }

    public void openCheats()
    {
        PauseMenu.SetActive(false);
        CheatsMenu.SetActive(true);
        ControlsMenu.SetActive(false);
        state = MenuState.Cheats;
    }

    public void openControls()
    {
        PauseMenu.SetActive(false);
        CheatsMenu.SetActive(false);
        ControlsMenu.SetActive(true);
        Transform currentControlScheme = null;
        foreach (Transform t in ControlSchemes)
        {
            if (t.GetComponent<Text>().text == AtlasInputManager.Instance.actionMap.ToString())
            {
                currentControlScheme = t;
                break;
            }
        }
        if (currentControlScheme != null)
        {
            ControlSchemeArrow.SetActive(true);
            ControlSchemeArrow.transform.localPosition = currentControlScheme.localPosition + 20 * Vector3.up - 105 * Vector3.right;
        }
        state = MenuState.Controls;
    }

    public void QuitToMenu()
    {
        GameObject atlas = GameObject.Find("Atlas");
        if (atlas != null) { Destroy(atlas); }
        playerController.created = false;
        gameManager.Instance.canSetPosition = false;
        SceneManager.LoadScene(mainmenu.ScenePath);
    }

    public static void QuitGame()
    {
        Application.Quit();
    }

    public void toggleBroomStyle(bool swap = true)
    {
        if (swap) AtlasInputManager.Instance.holdBroom ^= true;
        broomStyle.text = "Broom Style: " + 
            (AtlasInputManager.Instance.holdBroom ? "Hold" : "Toggle");
    }

    public void toggleHasDoubleJump()
    {
        gameManager.Instance.playerCtrl.hasDoubleJump ^= true;
        doubleJumpText.text = "Unlock Double Jump: " +
            (gameManager.Instance.playerCtrl.hasDoubleJump ? "on" : "off");
    }

    public void toggleHasWallJump()
    {
        gameManager.Instance.playerCtrl.hasWallJump ^= true;
        wallJumpText.text = "Unlock Wall Jump: " +
            (gameManager.Instance.playerCtrl.hasWallJump ? "on" : "off");
    }

    public void toggleInvulnerable()
    {
        gameManager.Instance.playerCtrl.invulnerable ^= true;
        invulnerableText.text = "Invulnerable: " +
            (gameManager.Instance.playerCtrl.invulnerable ? "on" : "off");
    }

    public void prepSetControlScheme(GameObject button)
    {
        ControlSchemeArrow.transform.localPosition = button.transform.localPosition + 20 * Vector3.up - 105 * Vector3.right;
        newControlScheme = button.GetComponent<Text>().text;
    }

    public void setControlScheme()
    {
        if (newControlScheme != null && newControlScheme != "")
        {
            AtlasInputManager.setControlScheme(newControlScheme);
        }
        togglePauseMenu(true);
    }
}
