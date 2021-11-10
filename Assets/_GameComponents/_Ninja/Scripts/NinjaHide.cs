using UnityEngine;

public class NinjaHide : MonoBehaviour
{
    public LayerMask whatIsPlayer;
    public LayerMask whatIsHiddenPlayer;

    public bool hidden = false;

    private SpriteRenderer spriteRenderer;
    public float interpolateIn;
    private float lerp;
    private Color color1;
    public Color color2;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        color1 = spriteRenderer.color;
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

        spriteRenderer.color = Color.Lerp(color1, color2, lerp);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Hider")
        {
            Hide();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Hider")
        {
            Unhide();
        }
    }
}
