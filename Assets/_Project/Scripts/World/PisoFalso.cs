using System.Collections;
using UnityEngine;

/// <summary>
/// Piso de ruina: o falso desaba ao pisar. Com Visao Espectral, o falso fica vermelho.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PisoFalso : MonoBehaviour
{
    public bool seguro;
    public float atrasoDesabar = 0.18f;
    public float tempoReset = 3.5f;

    private Collider2D colisor;
    private SpriteRenderer sprite;
    private Vector3 posicaoBase;
    private Color corBase;
    private bool desabando;
    private bool jogadorEmCima;

    void Awake()
    {
        colisor = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        corBase = sprite != null ? sprite.color : new Color(0.45f, 0.38f, 0.28f);
    }

    void Start()
    {
        posicaoBase = transform.position;
        AtualizarAparencia(BentoVisaoEspectral.Ativa);
    }

    void OnEnable()
    {
        BentoVisaoEspectral.VisaoAlterada += AtualizarAparencia;
    }

    void OnDisable()
    {
        BentoVisaoEspectral.VisaoAlterada -= AtualizarAparencia;
    }

    void AtualizarAparencia(bool visaoAtiva)
    {
        if (sprite == null) return;
        if (!visaoAtiva)
        {
            sprite.color = corBase;
            return;
        }

        sprite.color = seguro
            ? new Color(0.45f, 0.85f, 0.5f, 0.95f)
            : new Color(0.95f, 0.25f, 0.2f, 0.9f);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (seguro || desabando) return;
        if (collision.collider.GetComponent<BentoMovimento>() == null
            && collision.collider.GetComponentInParent<BentoMovimento>() == null)
            return;

        if (!jogadorEmCima)
        {
            jogadorEmCima = true;
            StartCoroutine(Desabar());
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<BentoMovimento>() != null
            || collision.collider.GetComponentInParent<BentoMovimento>() != null)
            jogadorEmCima = false;
    }

    IEnumerator Desabar()
    {
        desabando = true;
        yield return new WaitForSeconds(atrasoDesabar);
        if (colisor != null) colisor.enabled = false;

        float t = 0f;
        Vector3 inicio = transform.position;
        Vector3 fim = inicio + Vector3.down * 2.5f;
        while (t < 0.45f)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(inicio, fim, t / 0.45f);
            if (sprite != null)
            {
                var c = sprite.color;
                c.a = Mathf.Lerp(1f, 0.15f, t / 0.45f);
                sprite.color = c;
            }
            yield return null;
        }

        yield return new WaitForSeconds(tempoReset);
        transform.position = posicaoBase;
        if (colisor != null) colisor.enabled = true;
        desabando = false;
        jogadorEmCima = false;
        AtualizarAparencia(BentoVisaoEspectral.Ativa);
    }
}
