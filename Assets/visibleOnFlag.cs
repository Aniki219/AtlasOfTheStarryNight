using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class visibleOnFlag : MonoBehaviour
{
    public bool visibleOn = true;
    public string flagName;

    TilemapRenderer tilemap;
    TilemapCollider2D col;

    void Start()
    {
        AtlasEventManager.Instance.onFlagSet += onFlagSet;
        tilemap = GetComponent<TilemapRenderer>();
        col = GetComponent<TilemapCollider2D>();
        setVisible();
    }

    void onFlagSet()
    {
        setVisible();
    }

    void setVisible()
    {
        bool active = (gameFlagsManager.Instance.checkFlag(flagName) == visibleOn);
        if (tilemap) tilemap.enabled = active;
        if (col) col.enabled = tilemap.enabled;
        if (!tilemap && !col && !active) Destroy(gameObject);
    }

    private void OnDestroy()
    {
        AtlasEventManager.Instance.onFlagSet -= onFlagSet;
    }
}
