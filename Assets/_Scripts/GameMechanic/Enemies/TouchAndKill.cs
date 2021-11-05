using UnityEngine;

public class TouchAndKill : MonoBehaviour
{
    [SerializeField] private bool killByShot;
    SecurityParent parent;
    private void Start()
    {
        parent = GetComponent<SecurityParent>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player") {
            if (parent != null) parent.FaceRight((collision.transform.position.x - transform.position.x) > 0f);
            
            if (killByShot) collision.gameObject.GetComponent<NinjaStatesAnimationSound>().ShootNinja();
            else collision.gameObject.GetComponent<NinjaStatesAnimationSound>().KillNinja();
        }
    }
}
