using UnityEngine;

public class TouchAndKill : MonoBehaviour
{
    [SerializeField] private bool killByShot;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player") {
            if (killByShot) collision.gameObject.GetComponent<NinjaStatesAnimationSound>().ShootNinja();
            else collision.gameObject.GetComponent<NinjaStatesAnimationSound>().KillNinja();
            Debug.Log("KILL");
        }
    }
}
