using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ZonaMorte : MonoBehaviour
{
    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var vida = other.GetComponent<BentoVida>();
        if (vida != null) vida.MatarImediatamente();
    }
}
