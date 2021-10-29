using UnityEngine;

public class TouchAndDestroy : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player") {
            collision.gameObject.GetComponent<NinjaStatesAnimationSound>().KillNinja();
            Debug.Log("KILL");
        }
    }
}
