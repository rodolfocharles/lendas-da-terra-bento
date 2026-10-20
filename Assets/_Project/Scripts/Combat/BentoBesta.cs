using UnityEngine;

/// <summary>
/// Besta Espiritual: segurar K mira (arco), soltar dispara.
/// Consome Flechas Purificadoras do inventario. Buff Treme aumenta o dano.
/// </summary>
[RequireComponent(typeof(BentoMovimento))]
public class BentoBesta : MonoBehaviour
{
    [Header("Tiro")]
    public float forcaMinima = 6f;
    public float forcaMaxima = 14f;
    public float distanciaMaxMira = 8f;
    public float cooldown = 0.35f;
    public KeyCode teclaMira = KeyCode.K;

    [Header("Visual")]
    public int pontosArco = 16;
    public float passoArco = 0.08f;

    public bool Mirando { get; private set; }

    private BentoMovimento movimento;
    private BentoChicote chicote;
    private BentoVida vida;
    private InventarioBento inventario;
    private SistemaBuffs buffs;
    private LineRenderer linhaArco;
    private float proximoTiro;
    private Vector2 direcaoMira = Vector2.right;

    void Awake()
    {
        movimento = GetComponent<BentoMovimento>();
        chicote = GetComponent<BentoChicote>();
        vida = GetComponent<BentoVida>();
        inventario = GetComponent<InventarioBento>();
        buffs = GetComponent<SistemaBuffs>();

        var arcoGo = new GameObject("LinhaMiraBesta");
        arcoGo.transform.SetParent(transform, false);
        linhaArco = arcoGo.AddComponent<LineRenderer>();
        linhaArco.positionCount = 0;
        linhaArco.startWidth = 0.04f;
        linhaArco.endWidth = 0.02f;
        linhaArco.useWorldSpace = true;
        linhaArco.startColor = new Color(0.6f, 0.85f, 1f, 0.85f);
        linhaArco.endColor = new Color(0.6f, 0.85f, 1f, 0.15f);
        linhaArco.sortingOrder = 20;
        linhaArco.enabled = false;

        // Reusa material da corda do chicote, se existir
        var linhaChicote = GetComponent<LineRenderer>();
        if (linhaChicote != null && linhaChicote.sharedMaterial != null)
            linhaArco.sharedMaterial = linhaChicote.sharedMaterial;
    }

    void Start()
    {
        if (inventario == null) inventario = InventarioBento.Instancia;
        if (buffs == null) buffs = SistemaBuffs.Instancia;
        CatalogoItensNorte.Garantir();
    }

    void Update()
    {
        if (!PodeUsarBesta())
        {
            if (Mirando) CancelarMira();
            return;
        }

        if (Input.GetKeyDown(teclaMira))
            IniciarMira();

        if (Mirando)
        {
            AtualizarMira();
            if (Input.GetKeyUp(teclaMira))
                Disparar();
        }
    }

    bool PodeUsarBesta()
    {
        if (!movimento.ControleAtivo) return false;
        if (vida != null && vida.EstaMorto) return false;
        if (chicote != null && chicote.Balancando) return false;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return false;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto) return false;
        return true;
    }

    void IniciarMira()
    {
        if (Time.time < proximoTiro) return;
        if (QuantidadeFlechas() <= 0)
        {
            Debug.Log("[Lendas] Sem flechas. Prepare na Bancada de Alquimia.");
            return;
        }

        Mirando = true;
        linhaArco.enabled = true;
        AtualizarMira();
    }

    void CancelarMira()
    {
        Mirando = false;
        linhaArco.enabled = false;
        linhaArco.positionCount = 0;
    }

    void AtualizarMira()
    {
        Vector3 mouseMundo = Camera.main != null
            ? Camera.main.ScreenToWorldPoint(Input.mousePosition)
            : transform.position + Vector3.right;
        mouseMundo.z = 0f;

        Vector2 origem = PontoDisparo();
        Vector2 paraMouse = (Vector2)mouseMundo - origem;
        if (paraMouse.sqrMagnitude < 0.01f)
            paraMouse = movimento.OlhandoDireita ? Vector2.right : Vector2.left;

        float dist = Mathf.Clamp(paraMouse.magnitude, 0.5f, distanciaMaxMira);
        direcaoMira = paraMouse.normalized;
        float forca = Mathf.Lerp(forcaMinima, forcaMaxima, dist / distanciaMaxMira);
        Vector2 velocidade = direcaoMira * forca;

        DesenharArco(origem, velocidade);
    }

    void Disparar()
    {
        if (!Mirando) return;

        Vector2 origem = PontoDisparo();
        Vector3 mouseMundo = Camera.main != null
            ? Camera.main.ScreenToWorldPoint(Input.mousePosition)
            : (Vector3)(origem + direcaoMira);
        mouseMundo.z = 0f;

        Vector2 paraMouse = (Vector2)mouseMundo - origem;
        if (paraMouse.sqrMagnitude < 0.01f) paraMouse = direcaoMira;
        float dist = Mathf.Clamp(paraMouse.magnitude, 0.5f, distanciaMaxMira);
        Vector2 velocidade = paraMouse.normalized * Mathf.Lerp(forcaMinima, forcaMaxima, dist / distanciaMaxMira);

        if (!ConsumirFlecha())
        {
            CancelarMira();
            return;
        }

        float multi = buffs != null ? buffs.MultiplicadorDanoEspiritual : 1f;
        CriarFlecha(origem, velocidade, multi);

        proximoTiro = Time.time + cooldown;
        CancelarMira();
        Debug.Log($"[Lendas] Besta: flecha disparada (x{multi:0.00} dano espiritual)");
    }

    void CriarFlecha(Vector2 origem, Vector2 velocidade, float multiplicador)
    {
        var go = new GameObject("FlechaPurificadora");
        go.transform.position = origem;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = CriarSpriteQuadrado();
        sr.color = new Color(0.55f, 0.9f, 1f, 1f);
        sr.sortingOrder = 8;
        go.transform.localScale = new Vector3(0.35f, 0.12f, 1f);

        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 1.2f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = false;

        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(1.2f, 0.8f);

        var proj = go.AddComponent<ProjetilFlecha>();
        proj.Disparar(velocidade, multiplicador);
    }

    void DesenharArco(Vector2 origem, Vector2 velocidadeInicial)
    {
        linhaArco.positionCount = pontosArco;
        Vector2 pos = origem;
        Vector2 vel = velocidadeInicial;
        float g = Physics2D.gravity.y * 1.2f;

        for (int i = 0; i < pontosArco; i++)
        {
            linhaArco.SetPosition(i, pos);
            vel.y += g * passoArco;
            pos += vel * passoArco;
        }
    }

    Vector2 PontoDisparo()
    {
        float lado = movimento.OlhandoDireita ? 0.4f : -0.4f;
        return (Vector2)transform.position + new Vector2(lado, 0.35f);
    }

    public int QuantidadeFlechas()
    {
        if (inventario == null) inventario = InventarioBento.Instancia ?? GetComponent<InventarioBento>();
        if (inventario == null) return 0;
        return inventario.QuantidadePorId(CatalogoItensNorte.IdFlechasPurificadoras);
    }

    bool ConsumirFlecha()
    {
        if (inventario == null) return false;
        return inventario.RemoverPorId(CatalogoItensNorte.IdFlechasPurificadoras, 1);
    }

    static Sprite CriarSpriteQuadrado()
    {
        var tex = Texture2D.whiteTexture;
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 4f);
    }
}
