using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/HitBox")]
public class HitBox : ScriptableObject
{
    public Vector3 position;
    public Vector3 size = new Vector3(1, 1, 1);
    public Vector3 kbDir = new Vector3(1, 1);
    public float duration = 5.0f/60.0f;
    public float damage = 1.0f;
    public bool knockback = false;
    public bool spike = false;
    public bool bounce = false;
}
