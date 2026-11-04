using System.Collections;
using UnityEngine;

/// <summary>
/// Item no mundo que o Bento coleta ao encostar (ou com E).
/// Reagentes da mata (cipo, resina, flechas) reaparecem para nao travar o prototipo.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ColetavelItem : MonoBehaviour
{
    public ItemDados item;
    [Min(1)]
    public int quantidade = 1;
    public bool exigirTecla;
    public KeyCode teclaColetar = KeyCode.E;
    public bool destruirAoColetar = true;
    [Tooltip("Se verdadeiro, o item some e volta depois de alguns segundos.")]
    public bool reaparece;
    public float segundosParaReaparecer = 8f;

    public bool PodeColetarComE => exigirTecla && jogadorProximo && !escondido;

    bool jogadorProximo;
    bool escondido;
    Collider2D colisor;
    SpriteRenderer sprite;

    void Awake()
    {
        colisor = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Update()
    {
        if (escondido) return;
        if (!exigirTecla || !jogadorProximo) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto || HudCaderno.Aberto || HudPesquisa.Aberto) return;
        if (Input.GetKeyDown(teclaColetar)) TentarColetar();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (escondido) return;
        if (other.GetComponent<BentoMovimento>() == null) return;
        jogadorProximo = true;
        if (!exigirTecla) TentarColetar();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (escondido || exigirTecla) return;
        if (other.GetComponent<BentoMovimento>() == null) return;
        TentarColetar();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<BentoMovimento>() != null) jogadorProximo = false;
    }

    void TentarColetar()
    {
        if (escondido || item == null) return;

        var inventario = InventarioBento.Instancia;
        if (inventario == null)
        {
            var bento = FindFirstObjectByType<BentoMovimento>();
            if (bento != null) inventario = bento.GetComponent<InventarioBento>();
        }

        if (inventario == null) return;
        if (!inventario.Adicionar(item, quantidade)) return;

        BentoAnimacao.TocarInteracaoDoJogador();
        Debug.Log($"[Lendas] Coletou {quantidade}x {item.nomeExibicao}");

        if (reaparece)
        {
            StartCoroutine(EsconderEReaparecer());
            return;
        }

        if (destruirAoColetar) Destroy(gameObject);
    }

    public void MostrarDeNovo()
    {
        StopAllCoroutines();
        escondido = false;
        jogadorProximo = false;
        if (colisor != null) colisor.enabled = true;
        if (sprite != null) sprite.enabled = true;
        gameObject.SetActive(true);
    }

    IEnumerator EsconderEReaparecer()
    {
        escondido = true;
        jogadorProximo = false;
        if (colisor != null) colisor.enabled = false;
        if (sprite != null) sprite.enabled = false;
        yield return new WaitForSeconds(Mathf.Max(1f, segundosParaReaparecer));
        MostrarDeNovo();
    }

    void OnGUI()
    {
        if (escondido || !exigirTecla || !jogadorProximo) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto || HudCaderno.Aberto || HudPesquisa.Aberto) return;

        Vector3 tela = Camera.main != null
            ? Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 0.8f)
            : Vector3.zero;
        if (tela.z < 0f) return;

        float y = Screen.height - tela.y;
        string nome = item != null ? item.nomeExibicao : "item";
        GUI.Label(new Rect(tela.x - 70f, y - 10f, 140f, 24f), $"[E] Pegar {nome}");
    }
}
