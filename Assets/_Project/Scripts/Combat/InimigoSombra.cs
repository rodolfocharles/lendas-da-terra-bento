using UnityEngine;

/// <summary>
/// Sombra da corrupcao: nao se "mata" — se purifica (100% → 0%).
/// Flechas espirituais sao eficazes; facao quase nao afeta.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class InimigoSombra : MonoBehaviour, IRecebeDano
{
    [Header("Corrupcao")]
    [Range(0f, 100f)]
    public float corrupcaoMaxima = 100f;
    public float reducaoPorDanoEspiritual = 28f;
    public float reducaoPorDanoFisico = 5f;

    [Header("Contato")]
    public int danoContato = 1;
    public float cooldownDanoContato = 0.9f;

    [Header("Movimento")]
    public float amplitudeFlutuar = 0.25f;
    public float velocidadeFlutuar = 2f;

    public float CorrupcaoAtual { get; private set; }
    public bool Purificada { get; private set; }
    public float ProgressoCorrupcao => corrupcaoMaxima <= 0f ? 0f : CorrupcaoAtual / corrupcaoMaxima;

    private float proximoDanoContato;
    private SpriteRenderer sprite;
    private Color corCorrupta;
    private Vector3 posicaoBase;
    private bool sumindo;

    void Awake()
    {
        CorrupcaoAtual = corrupcaoMaxima;
        sprite = GetComponent<SpriteRenderer>();
        corCorrupta = sprite != null ? sprite.color : new Color(0.55f, 0.25f, 0.75f);
        posicaoBase = transform.position;
    }

    void Update()
    {
        if (Purificada || sumindo) return;

        float y = Mathf.Sin(Time.time * velocidadeFlutuar) * amplitudeFlutuar;
        transform.position = posicaoBase + Vector3.up * y;
    }

    public void ReceberDano(int quantidade, Vector2 origem, TipoDano tipo)
    {
        if (Purificada || sumindo || quantidade <= 0) return;
        if (tipo == TipoDano.CorteVegetacao) return;

        float reducao;
        if (tipo == TipoDano.Espiritual)
        {
            float multi = SistemaBuffs.Instancia != null
                ? SistemaBuffs.Instancia.MultiplicadorDanoEspiritual
                : 1f;
            reducao = reducaoPorDanoEspiritual * quantidade * multi;
        }
        else
        {
            // Facao: feedback fraco — jogador deve usar a Besta
            reducao = reducaoPorDanoFisico * quantidade;
            Debug.Log("[Lendas] Sombra: o facao quase nao a afeta. Use flechas purificadoras.");
        }

        CorrupcaoAtual = Mathf.Max(0f, CorrupcaoAtual - reducao);
        AtualizarCor();
        CadernoCampo.Instancia?.Desbloquear(CatalogoPaginasCap1.IdSombras);
        Debug.Log($"[Lendas] Sombra corrupcao: {CorrupcaoAtual:0}%");

        if (CorrupcaoAtual <= 0f)
            IniciarPurificacao();
    }

    void AtualizarCor()
    {
        if (sprite == null) return;
        // Quanto menor a corrupcao, mais clara / translucida
        float t = ProgressoCorrupcao;
        sprite.color = Color.Lerp(
            new Color(0.85f, 0.9f, 1f, 0.55f),
            corCorrupta,
            t);
    }

    void IniciarPurificacao()
    {
        if (Purificada) return;
        Purificada = true;
        sumindo = true;
        Debug.Log("[Lendas] Sombra purificada!");
        StartCoroutine(SumirPurificada());
    }

    System.Collections.IEnumerator SumirPurificada()
    {
        float t = 0f;
        Color inicio = sprite != null ? sprite.color : Color.white;
        while (t < 0.7f)
        {
            t += Time.deltaTime;
            if (sprite != null)
            {
                var c = inicio;
                c.a = Mathf.Lerp(inicio.a, 0f, t / 0.7f);
                sprite.color = c;
            }
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 0.3f, t / 0.7f);
            yield return null;
        }
        Destroy(gameObject);
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
        if (Purificada || sumindo) return;
        if (Time.time < proximoDanoContato) return;
        var vida = other.GetComponent<BentoVida>() ?? other.GetComponentInParent<BentoVida>();
        if (vida == null) return;

        vida.ReceberDano(danoContato, transform.position);
        proximoDanoContato = Time.time + cooldownDanoContato;
    }

    void OnGUI()
    {
        if (Purificada || Camera.main == null) return;

        Vector3 tela = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.3f);
        if (tela.z < 0f) return;

        float y = Screen.height - tela.y;
        float largura = 70f;
        float x = tela.x - largura * 0.5f;

        GUI.Box(new Rect(x, y - 8f, largura, 16f), "");
        float fill = ProgressoCorrupcao;
        // Barra roxa = corrupcao restante
        GUI.color = new Color(0.6f, 0.2f, 0.85f, 0.95f);
        GUI.DrawTexture(new Rect(x + 2f, y - 6f, (largura - 4f) * fill, 12f), Texture2D.whiteTexture);
        GUI.color = Color.white;

        var estilo = new GUIStyle(GUI.skin.label)
        {
            fontSize = 11,
            alignment = TextAnchor.MiddleCenter
        };
        estilo.normal.textColor = Color.white;
        GUI.Label(new Rect(x, y - 8f, largura, 16f), $"{CorrupcaoAtual:0}%", estilo);
    }
}
