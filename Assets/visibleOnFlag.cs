using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class visibleOnFlag : MonoBehaviour
{
    public bool visibleOn = true;
    public string flagName;

    TilemapRenderer tilemap;

    void Start()
    {
        AtlasEventManager.Instance.onFlagSet += onFlagSet;
        tilemap = GetComponent<TilemapRenderer>();
        setVisible();
    }

    void onFlagSet()
    {
        setVisible();
    }

    void setVisible()
    {
        tilemap.enabled = (gameFlagsManager.Instance.checkFlag(flagName) == visibleOn);
    }

    private void OnDestroy()
    {
        AtlasEventManager.Instance.onFlagSet -= onFlagSet;
    }
}
