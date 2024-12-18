﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

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

    public static bool SameSign(float v1, float v2) {
        return Sign(v1) == Sign(v2);
    }

    public static async Task WaitSeconds(float seconds) {
        float startTime = Time.time;
        float endTime = Time.time + seconds;
        while(Time.time < endTime) {
            await Task.Delay(16); // one frame
        }
        await Task.Yield();
    }

    public static AnimationClip FindAnimation (Animator animator, string name) 
    {
      foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
      {
          if (clip.name == name)
          {
            return clip;
          }
      }

      return null;
    }
}

public static class AtlasConstants
{
    public const float StepSize = 1.0f / 32.0f;
    public const float SkinWidth = 0.006f;
}
