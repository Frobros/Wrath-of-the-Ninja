using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private int numberOfColliders = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Hider")
        {
            numberOfColliders++;
            if(numberOfColliders == 4)
                collision.gameObject.GetComponentInParent<NinjaHide>().Hide();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Hider")
        {
            numberOfColliders--;
            collision.gameObject.GetComponentInParent<NinjaHide>().Unhide();
        }
    }
}
