using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionHiddenCollider : MonoBehaviour
{
    CapsuleCollider2D parent;
    BoxCollider2D[] colliders;
    void Start()
    {
        colliders = GetComponents<BoxCollider2D>();
        parent = GetComponentInParent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 offsetExtent = 0.5f * parent.size;
        colliders[0].offset = parent.offset + new Vector2(offsetExtent.x, offsetExtent.y);
        colliders[1].offset = parent.offset + new Vector2(offsetExtent.x, -offsetExtent.y);
        colliders[2].offset = parent.offset + new Vector2(-offsetExtent.x, offsetExtent.y);
        colliders[3].offset = parent.offset + new Vector2(-offsetExtent.x, -offsetExtent.y);
    }
}
