﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaHide : MonoBehaviour
{
    public LayerMask whatIsPlayer;
    public LayerMask whatIsHiddenPlayer;

    public bool hidden = false;

    private SpriteRenderer renderer;
    public float interpolateIn;
    private float lerp;
    private Color color1;
    public Color color2;

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        color1 = renderer.color;
    }

    private void Update()
    {
        if (hidden)
        {
            lerp = Mathf.Min(lerp + Time.deltaTime / interpolateIn, 1f);
        }
        else
        {
            lerp = Mathf.Max(lerp - Time.deltaTime / interpolateIn, 0f);
        }

        renderer.color = Color.Lerp(color1, color2, lerp);
    }

    public void Hide()
    {
        hidden = true;
        gameObject.layer = LayerMask.NameToLayer("PlayerHidden");
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("PlayerHidden"),
            LayerMask.NameToLayer("Enemy"),
            true
        );
    }

    public void Unhide()
    {
        hidden = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("PlayerHidden"),
            LayerMask.NameToLayer("Enemy"),
            false
        );
    }
}