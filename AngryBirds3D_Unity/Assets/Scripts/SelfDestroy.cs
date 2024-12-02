using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 2);
    }
}
