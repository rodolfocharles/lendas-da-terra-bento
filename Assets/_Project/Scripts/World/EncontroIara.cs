using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Iara corrompida: o canto paralisa sem Treme. Tacaca de oferenda
/// entrega a Gota de Quartzo e abre o caminho.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class EncontroIara : MonoBehaviour
{
    public static EncontroIara Instancia { get; private set; }
    public static bool Purificada { get; private set; }
    public static event Action IaraPurificada;

    public static bool CapturaTeclaE =>
        Instancia != null && Instancia.jogadorProximo && !Purificada;

    public GameObject[] barreiras;
    public KeyCode teclaOferecer = KeyCode.E;
    public float intervaloCanto = 2.2f;
    public float duracaoParalisia = 1.1f;

    private SpriteRenderer sprite;
    private bool jogadorProximo;
    private float proximoCanto;
    private bool paralisando;
    private Color corCorrupta = new Color(0.12f, 0.35f, 0.42f);
    private Color corCalma = new Color(0.35f, 0.85f, 0.9f);

    void Awake()
    {
        Instancia = this;
        Purificada = false;
        sprite = GetComponent<SpriteRenderer>();
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Start()
    {
        if (barreiras == null || barreiras.Length == 0)
        {
            var barreira = GameObject.Find("BarreiraRioIara");
            if (barreira != null) barreiras = new[] { barreira };
        }

        AplicarEstado();
    }

    void OnDestroy()
    {
        if (Instancia == this) Instancia = null;
        if (paralisando) LiberarParalisia();
    }

    void Update()
    {
        if (!jogadorProximo || Purificada) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto
            || HudCaderno.Aberto || HudPesquisa.Aberto) return;

        if (!TemTreme() && Time.time >= proximoCanto && !paralisando)
            StartCoroutine(Cantar());

        if (paralisando) return;
        if (!Input.GetKeyDown(teclaOferecer)) return;
        TentarOferenda();
    }

    IEnumerator Cantar()
    {
        paralisando = true;
        proximoCanto = Time.time + intervaloCanto;

        var bento = FindFirstObjectByType<BentoMovimento>();
        var vida = FindFirstObjectByType<BentoVida>();
        if (bento != null) bento.ControleAtivo = false;
        if (vida != null) vida.ReceberDano(1, transform.position);

        yield return new WaitForSeconds(duracaoParalisia);
        LiberarParalisia();
        paralisando = false;
    }

    void LiberarParalisia()
    {
        var bento = FindFirstObjectByType<BentoMovimento>();
        if (bento != null && (GerenciadorDialogo.Instancia == null || !GerenciadorDialogo.Instancia.EmDialogo))
            bento.ControleAtivo = true;
    }

    void TentarOferenda()
    {
        CatalogoItensNorte.Garantir();
        var inventario = InventarioBento.Instancia ?? FindFirstObjectByType<InventarioBento>();

        if (!TemTreme())
        {
            GerenciadorDialogo.Instancia?.Iniciar(new[]
            {
                new FalaDialogo
                {
                    personagem = "Iara",
                    emocao = "canto",
                    texto = "O canto trava os musculos. Seu Zeca avisou: coma Tacaca ou Pato no Tucupi (buff Treme) antes de chegar perto."
                }
            });
            return;
        }

        if (inventario == null || !inventario.TemPorId(CatalogoItensNorte.IdTacaca))
        {
            GerenciadorDialogo.Instancia?.Iniciar(new[]
            {
                new FalaDialogo
                {
                    personagem = "Iara",
                    emocao = "corrompida",
                    texto = "O Treme segura o canto, mas ela ainda pede oferenda. Volte ao Fogao e traga Tacaca."
                }
            });
            return;
        }

        inventario.RemoverPorId(CatalogoItensNorte.IdTacaca, 1);
        inventario.Adicionar(CatalogoItensNorte.GotaDeQuartzo, 1);
        Purificada = true;
        paralisando = false;
        LiberarParalisia();
        AplicarEstado();
        CadernoCampo.Instancia?.Desbloquear(CatalogoPaginasCap1.IdIara);
        CadernoCampo.Instancia?.Desbloquear(CatalogoPaginasCap1.IdIaraPurificada);
        IaraPurificada?.Invoke();

        GerenciadorDialogo.Instancia?.Iniciar(new[]
        {
            new FalaDialogo
            {
                personagem = "Iara",
                emocao = "serena",
                texto = "O canto vira brisa. A Iara entrega uma Gota de Quartzo — abre o caminho ate o Curupira."
            },
            new FalaDialogo
            {
                personagem = "Bento",
                emocao = "respeitoso",
                texto = "Obrigado. O rio pode voltar a proteger quem pesca e quem se banha."
            }
        });

        Debug.Log("[Lendas] Iara purificada. Gota de Quartzo no inventario.");
    }

    static bool TemTreme()
    {
        var buffs = SistemaBuffs.Instancia ?? FindFirstObjectByType<SistemaBuffs>();
        return buffs != null && buffs.ResistenciaParalisiaVeneno;
    }

    void AplicarEstado()
    {
        if (sprite != null)
            sprite.color = Purificada ? corCalma : corCorrupta;

        if (barreiras == null) return;
        for (int i = 0; i < barreiras.Length; i++)
        {
            if (barreiras[i] != null) barreiras[i].SetActive(!Purificada);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<BentoMovimento>() == null) return;
        jogadorProximo = true;
        proximoCanto = Time.time + 0.35f;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<BentoMovimento>() == null) return;
        jogadorProximo = false;
        if (paralisando)
        {
            StopAllCoroutines();
            paralisando = false;
            LiberarParalisia();
        }
    }

    void OnGUI()
    {
        if (!jogadorProximo || Purificada) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto
            || HudCaderno.Aberto || HudPesquisa.Aberto) return;
        if (Camera.main == null) return;

        Vector3 tela = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.3f);
        if (tela.z < 0f) return;
        float y = Screen.height - tela.y;

        var inventario = InventarioBento.Instancia;
        bool temTacaca = inventario != null && inventario.TemPorId(CatalogoItensNorte.IdTacaca);
        string dica;
        if (!TemTreme())
            dica = "Canto! Coma Tacaca/Pato (Treme).";
        else if (temTacaca)
            dica = "[E] Oferecer Tacaca";
        else
            dica = "Treme ok. Falta Tacaca de oferenda.";

        GUI.Box(new Rect(tela.x - 140f, y - 28f, 280f, 32f), dica);
    }
}
