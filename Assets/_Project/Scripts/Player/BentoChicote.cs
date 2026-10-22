using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(DistanceJoint2D), typeof(LineRenderer))]
[RequireComponent(typeof(BentoMovimento))]
public class BentoChicote : MonoBehaviour
{
    [Header("Alcance")]
    public float alcance = 6f;
    public string tagGancho = "Gancho";
    public KeyCode teclaChicote = KeyCode.E;

    [Header("Balanco")]
    public float forcaBalanco = 20f;
    public float velocidadeRecolher = 4f;
    public float comprimentoMinimo = 1.5f;

    [Header("Soltar")]
    public float forcaPuloSoltar = 8f;
    public float multiplicadorImpulso = 1.2f;

    [Header("Puxao (combate / puzzle)")]
    public float duracaoLinhaPuxao = 0.18f;
    public float cooldownPuxao = 0.35f;

    public bool Balancando => pontoAtual != null;

    private Rigidbody2D rb;
    private DistanceJoint2D junta;
    private LineRenderer linha;
    private BentoMovimento movimento;
    private BentoBesta besta;
    private Transform pontoAtual;
    private GameObject[] pontosGancho;
    private Transform puxaoVisual;
    private float fimPuxaoVisual;
    private float proximoPuxao;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        junta = GetComponent<DistanceJoint2D>();
        linha = GetComponent<LineRenderer>();
        movimento = GetComponent<BentoMovimento>();
        besta = GetComponent<BentoBesta>();

        junta.enabled = false;
        junta.autoConfigureDistance = false;
        junta.autoConfigureConnectedAnchor = false;
        junta.maxDistanceOnly = true;
        junta.enableCollision = true;

        linha.positionCount = 2;
        linha.useWorldSpace = true;
        linha.enabled = false;
    }

    void Start()
    {
        pontosGancho = GameObject.FindGameObjectsWithTag(tagGancho);
    }

    void Update()
    {
        if (besta == null) besta = GetComponent<BentoBesta>();
        if (besta != null && besta.Mirando) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto || HudCaderno.Aberto || HudPesquisa.Aberto) return;
        if (movimento != null && !movimento.ControleAtivo && !Balancando) return;

        bool apertouE = Input.GetKeyDown(teclaChicote);
        if (CanoaRio.JogadorDentro) return;
        if (apertouE && (EncontroBoto.CapturaTeclaE || EncontroIara.CapturaTeclaE || CanoaRio.CapturaTeclaE))
            apertouE = false;
        if (apertouE || Input.GetMouseButtonDown(1))
        {
            if (Balancando) Soltar(false);
            else TentarAgarrar();
        }

        if (!Balancando) return;

        if (Input.GetButtonDown("Jump"))
        {
            movimento.ConsumirPuloDoQuadro();
            Soltar(true);
            return;
        }

        float vertical = Input.GetAxisRaw("Vertical");
        junta.distance = Mathf.Clamp(junta.distance - vertical * velocidadeRecolher * Time.deltaTime, comprimentoMinimo, alcance);
    }

    void FixedUpdate()
    {
        if (!Balancando) return;

        Vector2 direcaoCorda = ((Vector2)pontoAtual.position - rb.position).normalized;
        Vector2 tangente = new Vector2(direcaoCorda.y, -direcaoCorda.x);
        rb.AddForce(tangente * movimento.MovimentoHorizontal * forcaBalanco);
    }

    void LateUpdate()
    {
        if (Balancando)
        {
            linha.enabled = true;
            linha.SetPosition(0, transform.position);
            linha.SetPosition(1, pontoAtual.position);
            return;
        }

        if (puxaoVisual != null && Time.time < fimPuxaoVisual)
        {
            linha.enabled = true;
            linha.SetPosition(0, transform.position);
            linha.SetPosition(1, puxaoVisual.position);
            return;
        }

        if (!Balancando && linha.enabled && puxaoVisual == null)
            linha.enabled = false;
        else if (puxaoVisual != null && Time.time >= fimPuxaoVisual)
        {
            puxaoVisual = null;
            if (!Balancando) linha.enabled = false;
        }
    }

    void TentarAgarrar()
    {
        if (TentarPuxarAlvo()) return;
        TentarBalancar();
    }

    bool TentarPuxarAlvo()
    {
        if (Time.time < proximoPuxao) return false;

        var hits = Physics2D.OverlapCircleAll(rb.position, alcance);
        IAlvoChicote melhor = null;
        float melhorScore = float.MaxValue;
        Vector2 frente = movimento != null && movimento.OlhandoDireita ? Vector2.right : Vector2.left;

        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];
            if (col == null) continue;
            if (col.transform == transform || col.transform.IsChildOf(transform)) continue;

            var alvo = col.GetComponent<IAlvoChicote>() ?? col.GetComponentInParent<IAlvoChicote>();
            if (alvo == null || !alvo.PodeSerPuxado) continue;

            Vector2 delta = (Vector2)alvo.Transforme.position - rb.position;
            float dist = delta.magnitude;
            if (dist < 0.4f || dist > alcance) continue;

            float alinhamento = Vector2.Dot(delta.normalized, frente);
            // Prefere o que esta a frente, mas aceita um pouco atras
            if (alinhamento < -0.15f) continue;

            float score = dist - alinhamento * 1.5f;
            if (score < melhorScore)
            {
                melhorScore = score;
                melhor = alvo;
            }
        }

        if (melhor == null) return false;

        melhor.AoSerPuxado(rb.position);
        puxaoVisual = melhor.Transforme;
        fimPuxaoVisual = Time.time + duracaoLinhaPuxao;
        proximoPuxao = Time.time + cooldownPuxao;
        linha.enabled = true;
        return true;
    }

    void TentarBalancar()
    {
        pontosGancho = GameObject.FindGameObjectsWithTag(tagGancho);

        Transform melhor = null;
        float menorDistancia = alcance;

        foreach (GameObject ponto in pontosGancho)
        {
            if (ponto == null) continue;

            Vector2 posicao = ponto.transform.position;
            float distancia = Vector2.Distance(rb.position, posicao);
            if (distancia <= menorDistancia && posicao.y > rb.position.y)
            {
                menorDistancia = distancia;
                melhor = ponto.transform;
            }
        }

        if (melhor == null) return;

        pontoAtual = melhor;
        junta.connectedBody = null;
        junta.connectedAnchor = melhor.position;
        junta.distance = Mathf.Max(menorDistancia, comprimentoMinimo);
        junta.enabled = true;
        linha.enabled = true;
        movimento.ControleAtivo = false;
    }

    public void Soltar(bool comPulo)
    {
        pontoAtual = null;
        junta.enabled = false;
        linha.enabled = false;
        movimento.ControleAtivo = true;
        movimento.RestaurarPulosExtras();

        if (comPulo)
        {
            Vector2 v = rb.linearVelocity;
            rb.linearVelocity = new Vector2(v.x * multiplicadorImpulso, Mathf.Max(v.y, 0f) + forcaPuloSoltar);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alcance);
    }
}
