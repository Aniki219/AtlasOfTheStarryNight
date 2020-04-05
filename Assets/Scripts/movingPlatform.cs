using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class movingPlatform : MonoBehaviour
{
    public float moveSpeed = 2;
    public int currentNode = 1;

    Vector3 lastPosition;
    public Vector3 velocity;
    Transform nodes;
    List<Vector3> nodePositions;

    BoxCollider2D col;

    float maxDistanceDelta = 0.05f;

    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        velocity = Vector3.zero;
        lastPosition = transform.position;
        nodePositions = new List<Vector3>();
        nodes = transform.Find("Nodes");
        calculateNodePositions();
        if (!nodes || nodes.childCount > 0)
        {
            transform.position = nodePositions[currentNode];
        }
    }

    void FixedUpdate()
    {
        if (!nodes || nodes.childCount < 2) { return; }

        Vector3 targetPos = nodePositions[currentNode];

        if (Vector3.Distance(transform.position, targetPos) > maxDistanceDelta)
        {
            velocity = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime) - transform.position;
        } else
        {
            currentNode++;
            if (currentNode >= nodePositions.Count)
            {
                currentNode = 0;
            }
        }

        if (!col.isTrigger)
        {
            checkForActors();
        }

        transform.position += velocity;
    }

    void checkForActors()
    {
        //Set up math stuff
        float px = 1.0f / 32.0f;
        float hdir = Mathf.Sign(velocity.x);

        //We need vertical skin so that the platform can grab actors ontop and pull them down
        Vector2 skin = new Vector2(0*px, 2*px);
        Vector2 boxSize = Vector2.Scale(col.size + skin, transform.localScale);

        //The box origin moves left/right so that it doesn't pull objects
        Vector2 origin = transform.position + Vector3.up * 1*px + Vector3.right * 1*px * Math.Sign(velocity.x);
        int actorMask = (1 << LayerMask.NameToLayer("Player"));
        RaycastHit2D[] actors = Physics2D.BoxCastAll(origin, boxSize, 0, hdir * Vector2.right, 0.5f * px, actorMask);

        foreach (RaycastHit2D actor in actors)
        {
            characterController cc = actor.transform.GetComponent<characterController>();
            if (cc && col.enabled)
            {
                cc.AddVelocity(velocity);
            }
        }
    }

    void calculateNodePositions()
    {
        nodePositions.Clear();
        foreach (Transform node in nodes) {
            nodePositions.Add(node.position);
        }
    }

    public Vector3 getVelocity()
    {
        return velocity * 1/(Time.deltaTime);
    }
}
