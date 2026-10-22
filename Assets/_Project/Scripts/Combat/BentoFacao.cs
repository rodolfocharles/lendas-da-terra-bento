using UnityEngine;

/// <summary>
/// Facao de campo: combo curto (ate 2 golpes) e corte de vegetacao.
/// Ataque: botao esquerdo do mouse ou J (Fire1).
/// </summary>
[RequireComponent(typeof(BentoMovimento))]
public class BentoFacao : MonoBehaviour
{
    [Header("Dano")]
    public int danoGolpe = 1;
    public float alcance = 1.1f;
    public float alturaHitbox = 0.9f;
    public float deslocamentoFrente = 0.7f;

    [Header("Tempo")]
    public float duracaoGolpe = 0.18f;
    public float fimCombo = 0.45f;
    public float recuperacao = 0.12f;

    [Header("Camadas")]
    public LayerMask camadasAlvo = ~0;

    public bool Atacando => atacando;

    private BentoMovimento movimento;
    private BentoChicote chicote;
    private BentoBesta besta;
    private BentoVida vida;
    private bool atacando;
    private int indiceCombo;
    private float tempoProximoGolpe;
    private float tempoExpiraCombo;
    private float tempoFimAtaque;
    private readonly Collider2D[] bufferHits = new Collider2D[12];

    void Awake()
    {
        movimento = GetComponent<BentoMovimento>();
        chicote = GetComponent<BentoChicote>();
        besta = GetComponent<BentoBesta>();
        vida = GetComponent<BentoVida>();
    }

    void Update()
    {
        if (atacando && Time.time >= tempoFimAtaque)
            atacando = false;

        if (indiceCombo > 0 && Time.time > tempoExpiraCombo)
            indiceCombo = 0;

        if (!PodeAtacar()) return;
        if (!Input.GetButtonDown("Fire1") && !Input.GetKeyDown(KeyCode.J)) return;
        if (Time.time < tempoProximoGolpe) return;

        ExecutarGolpe();
    }

    bool PodeAtacar()
    {
        if (!movimento.ControleAtivo) return false;
        if (vida != null && vida.EstaMorto) return false;
        if (chicote != null && chicote.Balancando) return false;
        if (besta == null) besta = GetComponent<BentoBesta>();
        if (besta != null && besta.Mirando) return false;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return false;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto || HudCaderno.Aberto || HudPesquisa.Aberto) return false;
        return true;
    }

    void ExecutarGolpe()
    {
        indiceCombo = indiceCombo >= 2 ? 1 : indiceCombo + 1;
        atacando = true;
        tempoFimAtaque = Time.time + duracaoGolpe;
        tempoProximoGolpe = Time.time + duracaoGolpe + recuperacao;
        tempoExpiraCombo = Time.time + fimCombo;

        AplicarHits();
        Debug.Log($"[Lendas] Facao golpe {indiceCombo}");
    }

    void AplicarHits()
    {
        Vector2 origem = (Vector2)transform.position + DirecaoFrente() * deslocamentoFrente;
        var filtro = new ContactFilter2D();
        filtro.SetLayerMask(camadasAlvo);
        filtro.useTriggers = true;

        int n = Physics2D.OverlapBox(origem, new Vector2(alcance, alturaHitbox), 0f, filtro, bufferHits);
        for (int i = 0; i < n; i++)
        {
            var col = bufferHits[i];
            if (col == null || col.attachedRigidbody == GetComponent<Rigidbody2D>()) continue;
            if (col.transform.IsChildOf(transform) || col.transform == transform) continue;

            var alvo = col.GetComponent<IRecebeDano>() ?? col.GetComponentInParent<IRecebeDano>();
            if (alvo == null) continue;

            alvo.ReceberDano(danoGolpe, transform.position, TipoDano.Fisico);
        }
    }

    Vector2 DirecaoFrente()
    {
        return movimento.OlhandoDireita ? Vector2.right : Vector2.left;
    }

    void OnDrawGizmosSelected()
    {
        if (movimento == null) movimento = GetComponent<BentoMovimento>();
        bool direita = movimento == null || movimento.OlhandoDireita;
        Vector2 origem = (Vector2)transform.position + (direita ? Vector2.right : Vector2.left) * deslocamentoFrente;
        Gizmos.color = atacando ? Color.yellow : new Color(1f, 0.6f, 0.1f, 0.5f);
        Gizmos.DrawWireCube(origem, new Vector3(alcance, alturaHitbox, 0.1f));
    }
}
