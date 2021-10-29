using UnityEngine;

public class TriggerDoor : MonoBehaviour {

    bool open = true;
    public SecurityWatch[] watchers;
    BoxCollider2D col;
	
    void Start () {
        watchers = FindObjectsOfType<SecurityWatch>();
        col = GetComponent<BoxCollider2D>();
	}
	
	void Update () {
        bool closeDoor = false;

		foreach(SecurityWatch s in watchers)
        {
            if (s.isPlayerDetected()) closeDoor = true;
        }
    }
}
