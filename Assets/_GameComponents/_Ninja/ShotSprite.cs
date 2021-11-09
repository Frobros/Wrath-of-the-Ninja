using UnityEngine;

public class ShotSprite : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    public void ShootNinja()
    {
        spriteRenderer.enabled = true;
    }
}
