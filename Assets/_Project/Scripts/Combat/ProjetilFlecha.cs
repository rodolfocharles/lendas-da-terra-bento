using UnityEngine;

/// <summary>
/// Flecha da Besta Espiritual (trajetoria com gravidade).
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ProjetilFlecha : MonoBehaviour
{
    public int danoBase = 1;
    public float tempoVida = 4f;
    public TipoDano tipoDano = TipoDano.Espiritual;

    private Rigidbody2D rb;
    private float multiplicador = 1f;
    private bool chegou;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    public void Disparar(Vector2 velocidade, float multiplicadorDano)
    {
        multiplicador = multiplicadorDano;
        rb.gravityScale = 1.2f;
        rb.linearVelocity = velocidade;
        Destroy(gameObject, tempoVida);
    }

    void FixedUpdate()
    {
        if (chegou || rb.linearVelocity.sqrMagnitude < 0.01f) return;
        float angulo = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angulo);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (chegou) return;
        if (other.GetComponent<BentoMovimento>() != null) return;
        if (other.GetComponentInParent<BentoMovimento>() != null) return;

        var alvo = other.GetComponent<IRecebeDano>() ?? other.GetComponentInParent<IRecebeDano>();
        if (alvo != null)
        {
            int dano = Mathf.Max(1, Mathf.RoundToInt(danoBase * multiplicador));
            alvo.ReceberDano(dano, transform.position, tipoDano);
            chegou = true;
            Destroy(gameObject);
            return;
        }

        // Chao / obstaculo solido
        if (!other.isTrigger)
        {
            chegou = true;
            Destroy(gameObject);
        }
    }
}
