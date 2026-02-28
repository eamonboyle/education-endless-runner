using UnityEngine;

public class RemoveDust : MonoBehaviour
{
    public float timer = 0.2f;

    void Start()
    {
        if (timer <= 0f) timer = 0.2f;
        Invoke(nameof(Remove), timer);
    }

    void Remove()
    {
        Destroy(gameObject);
    }
}
