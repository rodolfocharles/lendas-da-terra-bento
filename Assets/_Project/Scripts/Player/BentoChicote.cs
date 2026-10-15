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

    public bool Balancando => pontoAtual != null;

    private Rigidbody2D rb;
    private DistanceJoint2D junta;
    private LineRenderer linha;
    private BentoMovimento movimento;
    private Transform pontoAtual;
    private GameObject[] pontosGancho;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        junta = GetComponent<DistanceJoint2D>();
        linha = GetComponent<LineRenderer>();
        movimento = GetComponent<BentoMovimento>();

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
        if (Input.GetKeyDown(teclaChicote) || Input.GetMouseButtonDown(1))
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
        if (!Balancando) return;

        linha.SetPosition(0, transform.position);
        linha.SetPosition(1, pontoAtual.position);
    }

    void TentarAgarrar()
    {
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
