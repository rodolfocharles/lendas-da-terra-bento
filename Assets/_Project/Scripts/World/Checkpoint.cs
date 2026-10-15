using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    public bool ativarAoIniciar;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Start()
    {
        if (ativarAoIniciar)
        {
            var bento = FindFirstObjectByType<BentoVida>();
            if (bento != null) bento.DefinirCheckpoint(transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var vida = other.GetComponent<BentoVida>();
        if (vida == null) return;
        vida.DefinirCheckpoint(transform.position);
    }
}
