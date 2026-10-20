using UnityEngine;

/// <summary>
/// Inimigo com vida (placeholder de Capanga). Contato causa dano ao Bento.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class InimigoSimples : MonoBehaviour, IRecebeDano
{
    [Header("Vida")]
    public int vidaMaxima = 3;
    public int danoContato = 1;
    public float cooldownDanoContato = 0.8f;

    [Header("Knockback")]
    public float forcaEmpurrao = 4f;

    private int vidaAtual;
    private float proximoDanoContato;
    private SpriteRenderer sprite;
    private Color corOriginal;

    void Awake()
    {
        vidaAtual = vidaMaxima;
        sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) corOriginal = sprite.color;
    }

    public void ReceberDano(int quantidade, Vector2 origem, TipoDano tipo)
    {
        if (vidaAtual <= 0 || quantidade <= 0) return;
        if (tipo == TipoDano.CorteVegetacao) return;

        vidaAtual -= quantidade;
        Debug.Log($"[Lendas] {name} recebeu {quantidade} de dano ({vidaAtual}/{vidaMaxima})");

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 dir = ((Vector2)transform.position - origem).normalized;
            if (dir.sqrMagnitude < 0.01f) dir = Vector2.right;
            rb.linearVelocity = dir * forcaEmpurrao;
        }

        if (sprite != null)
            StartCoroutine(Piscar());

        if (vidaAtual <= 0)
        {
            Debug.Log($"[Lendas] {name} derrotado.");
            Destroy(gameObject);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        TentarDanoContato(collision.collider);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        TentarDanoContato(other);
    }

    void TentarDanoContato(Collider2D other)
    {
        if (Time.time < proximoDanoContato) return;
        var vida = other.GetComponent<BentoVida>() ?? other.GetComponentInParent<BentoVida>();
        if (vida == null) return;

        vida.ReceberDano(danoContato, transform.position);
        proximoDanoContato = Time.time + cooldownDanoContato;
    }

    System.Collections.IEnumerator Piscar()
    {
        if (sprite == null) yield break;
        sprite.color = Color.white;
        yield return new WaitForSeconds(0.08f);
        sprite.color = corOriginal;
    }
}
