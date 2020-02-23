using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingPlatform : MonoBehaviour
{
    public float moveSpeed = 2;
    public int currentNode = 1;

    Vector3 lastPosition;
    Vector3 velocity;
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
    }

    void FixedUpdate()
    {
        if (!nodes) { return; }

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

        checkForActors();

        transform.position += velocity;
    }

    void checkForActors()
    {
        float px = 1.0f / 32.0f;
        float hdir = Mathf.Sign(velocity.x);

        Vector2 skin = new Vector2(-2*px, 2*px);
        Vector2 boxSize = Vector2.Scale(col.size + skin, transform.localScale);
        Vector2 origin = transform.position + Vector3.up * 1*px;
        int actorMask = (1 << LayerMask.NameToLayer("Player"));
        RaycastHit2D[] actors = Physics2D.BoxCastAll(origin, boxSize, 0, hdir * Vector2.right, 0.5f * px, actorMask);

        if (actors.Length > 0)
        {
            characterController cc = actors[0].transform.GetComponent<characterController>();
            if (cc && col.enabled)
            {
                cc.AddVelocity(velocity);
            }
            //GetComponent<SpriteRenderer>().color = Color.green;
        } else
        {
            //GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    void calculateNodePositions()
    {
        nodePositions.Clear();
        foreach (Transform node in nodes) {
            nodePositions.Add(node.position);
        }
    }
}
