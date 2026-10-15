using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BentoMovimento : MonoBehaviour
{
    [Header("Configuracoes de Movimento")]
    public float velocidade = 7f;
    public float aceleracaoChao = 80f;
    public float aceleracaoAr = 25f;
    public float forcaPulo = 10f;

    [Header("Tolerancias de Pulo")]
    public float tempoCoyote = 0.1f;
    public float tempoBufferPulo = 0.12f;

    [Header("Verificacao do Chao")]
    public Transform checadorChao;
    public float raioChecagem = 0.2f;
    public LayerMask camadaChao;

    public bool NoChao => noChao;
    public float MovimentoHorizontal => movimentoHorizontal;
    public bool OlhandoDireita => olhandoDireita;
    public bool ControleAtivo { get; set; } = true;

    private Rigidbody2D rb;
    private bool noChao;
    private float movimentoHorizontal;
    private float contadorCoyote;
    private float contadorBufferPulo;
    private bool podeCortarPulo;
    private bool olhandoDireita = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movimentoHorizontal = Input.GetAxisRaw("Horizontal");

        contadorCoyote = noChao ? tempoCoyote : contadorCoyote - Time.deltaTime;
        contadorBufferPulo = Input.GetButtonDown("Jump") ? tempoBufferPulo : contadorBufferPulo - Time.deltaTime;

        if (!ControleAtivo)
        {
            podeCortarPulo = false;
        }
        else if (contadorBufferPulo > 0f && contadorCoyote > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forcaPulo);
            contadorBufferPulo = 0f;
            contadorCoyote = 0f;
            podeCortarPulo = true;
        }

        // Pulo variavel (se soltar o espaco antes, pula mais baixo)
        if (Input.GetButtonUp("Jump") && podeCortarPulo && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            podeCortarPulo = false;
        }

        AtualizarDirecao();
    }

    void FixedUpdate()
    {
        if (checadorChao != null)
        {
            noChao = Physics2D.OverlapCircle(checadorChao.position, raioChecagem, camadaChao);
        }

        if (!ControleAtivo) return;

        float alvo = movimentoHorizontal * velocidade;
        float aceleracao = noChao ? aceleracaoChao : aceleracaoAr;
        float novoX = Mathf.MoveTowards(rb.linearVelocity.x, alvo, aceleracao * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(novoX, rb.linearVelocity.y);
    }

    void AtualizarDirecao()
    {
        if ((movimentoHorizontal > 0f && !olhandoDireita) || (movimentoHorizontal < 0f && olhandoDireita))
        {
            olhandoDireita = !olhandoDireita;
            Vector3 escala = transform.localScale;
            escala.x *= -1f;
            transform.localScale = escala;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (checadorChao != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(checadorChao.position, raioChecagem);
        }
    }
}
