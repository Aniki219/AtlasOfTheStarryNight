using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AtlasHelpers
{
    public static void DebugDrawBox(Vector2 position, Vector2 size)
    {
        DebugDrawBox(position, size, Color.red);
    }

    public static void DebugDrawBox(Vector2 position, Vector2 size, Color color)
    {
        if (color == null) color = Color.red;

        Vector2 topRightCorner = position + size * 0.5f;
        Vector2 left = size * Vector2.left;
        Vector2 down = size * Vector2.down;
        Debug.DrawLine(topRightCorner, topRightCorner + down, color);
        Debug.DrawLine(topRightCorner, topRightCorner + left, color);
        Debug.DrawLine(topRightCorner + down, topRightCorner + down + left, color);
        Debug.DrawLine(topRightCorner + left, topRightCorner + down + left, color);
    }

    public static int Sign(float value)
    {
        if (value == 0) return 0;
        return value > 0 ? 1 : -1;
    }
}

public static class AtlasConstants
{
    public const float StepSize = 1.0f / 32.0f;
    public const float SkinWidth = 0.006f;
}
