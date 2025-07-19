using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemCleaner : MonoBehaviour
{
    void Awake()
    {
        if (FindObjectsOfType<EventSystem>().Length > 1)
        {
            Destroy(gameObject);
        }
    }
}
