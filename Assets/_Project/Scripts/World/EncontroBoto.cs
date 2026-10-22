using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Boto espiritual: a poluicao nao o contamina, mas a dor dos animais
/// o deixa incontrolavel. Tres flechas purificadoras acalmam; Tacaca +
/// foto da Lia completam a oferenda e enviam a Essencia das Aguas ao amuleto.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class EncontroBoto : MonoBehaviour, IRecebeDano
{
    public static EncontroBoto Instancia { get; private set; }
    public static bool Acalmado { get; private set; }
    public static bool CaminhoLiberado { get; private set; }
    public static event Action BotoAcalmado;
    public static event Action CaminhoDoRioLiberado;

    public static bool CapturaTeclaE =>
        Instancia != null && Instancia.jogadorProximo && Acalmado && !CaminhoLiberado
        && CanoaRio.JogadorDentro;

    public GameObject[] barreiras;
    public KeyCode teclaOferecer = KeyCode.E;
    public int flechasParaAcalmar = 3;
    public float intervaloInvestida = 2.8f;
    public float velocidadeInvestida = 9f;

    int flechasAcertadas;
    SpriteRenderer sprite;
    bool jogadorProximo;
    bool investindo;
    float proximaInvestida;
    Color corBravo = new Color(0.55f, 0.2f, 0.45f);
    Color corCalmo = new Color(0.95f, 0.55f, 0.75f);
    Vector3 posicaoRepouso;

    public int FlechasAcertadas => flechasAcertadas;

    void Awake()
    {
        Instancia = this;
        Acalmado = false;
        CaminhoLiberado = false;
        flechasAcertadas = 0;
        sprite = GetComponent<SpriteRenderer>();
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Start()
    {
        posicaoRepouso = LayoutPrototipo.BloqueioBoto;
        if (barreiras == null || barreiras.Length == 0)
        {
            var barreira = GameObject.Find("BarreiraRioBoto");
            if (barreira != null) barreiras = new[] { barreira };
        }

        AplicarEstado();
    }

    void OnDestroy()
    {
        if (Instancia == this) Instancia = null;
    }

    void Update()
    {
        if (CaminhoLiberado) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto
            || HudCaderno.Aberto || HudPesquisa.Aberto) return;

        if (!Acalmado && CanoaRio.JogadorDentro && CanoaNaZonaDeCombate() && !investindo)
        {
            if (Time.time >= proximaInvestida)
                StartCoroutine(Investir());
        }

        if (!jogadorProximo || !Acalmado || !CanoaRio.JogadorDentro) return;
        if (!Input.GetKeyDown(teclaOferecer)) return;
        TentarOferenda();
    }

    bool CanoaNaZonaDeCombate()
    {
        var canoa = CanoaRio.Instancia;
        if (canoa == null || canoa.Afundando) return false;
        float x = canoa.transform.position.x;
        return x >= LayoutPrototipo.InicioZonaBoto && x <= LayoutPrototipo.LimiteCanoaBoto + 0.5f;
    }

    IEnumerator Investir()
    {
        investindo = true;
        proximaInvestida = Time.time + intervaloInvestida;

        var canoa = CanoaRio.Instancia;
        if (canoa == null)
        {
            investindo = false;
            yield break;
        }

        if (sprite != null) sprite.color = new Color(0.75f, 0.15f, 0.35f);

        Vector3 alvo = canoa.transform.position + Vector3.up * 0.35f;
        while (Vector3.Distance(transform.position, alvo) > 0.35f)
        {
            if (Acalmado || canoa.Afundando || !CanoaRio.JogadorDentro)
            {
                investindo = false;
                AplicarEstado();
                yield break;
            }

            transform.position = Vector3.MoveTowards(
                transform.position, alvo, velocidadeInvestida * Time.deltaTime);
            yield return null;
        }

        if (!Acalmado && CanoaRio.JogadorDentro)
            canoa.ReceberImpacto();

        Vector3 volta = posicaoRepouso;
        while (Vector3.Distance(transform.position, volta) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, volta, velocidadeInvestida * 0.7f * Time.deltaTime);
            yield return null;
        }

        transform.position = posicaoRepouso;
        investindo = false;
        AplicarEstado();
    }

    public void ReceberDano(int quantidade, Vector2 origem, TipoDano tipo)
    {
        if (Acalmado || CaminhoLiberado || quantidade <= 0) return;
        if (tipo != TipoDano.Espiritual)
        {
            Debug.Log("[Lendas] O Boto e espirito. Facao e chicote nao o acalmam. Use flechas purificadoras.");
            return;
        }

        flechasAcertadas++;
        Debug.Log($"[Lendas] Flecha no Boto ({flechasAcertadas}/{flechasParaAcalmar}).");

        if (flechasAcertadas >= flechasParaAcalmar)
            Acalmar();
        else if (sprite != null)
            sprite.color = Color.Lerp(corBravo, corCalmo, flechasAcertadas / (float)flechasParaAcalmar);
    }

    void Acalmar()
    {
        Acalmado = true;
        investindo = false;
        StopAllCoroutines();
        transform.position = posicaoRepouso;
        AplicarEstado();
        CadernoCampo.Instancia?.Desbloquear(CatalogoPaginasCap1.IdBoto);
        BotoAcalmado?.Invoke();

        GerenciadorDialogo.Instancia?.Iniciar(new[]
        {
            new FalaDialogo
            {
                personagem = "Boto",
                emocao = "cansado",
                texto = "As flechas dissipam a raiva. O Boto paira ao lado da canoa, ainda triste — a agua continua doente, mas ele ja nao tenta afundar o caico."
            },
            new FalaDialogo
            {
                personagem = "Bento",
                emocao = "sereno",
                texto = "Fica em paz. Trouxe oferenda: o Tacaca da Nair... e, se Lia me deu, o retrato dela."
            }
        });
    }

    void TentarOferenda()
    {
        CatalogoItensNorte.Garantir();
        var inventario = InventarioBento.Instancia ?? FindFirstObjectByType<InventarioBento>();
        bool temTacaca = inventario != null && inventario.TemPorId(CatalogoItensNorte.IdTacaca);
        bool temFoto = inventario != null && inventario.TemPorId(CatalogoItensNorte.IdFotoDaLia);

        if (!temTacaca && !temFoto)
        {
            GerenciadorDialogo.Instancia?.Iniciar(new[]
            {
                new FalaDialogo
                {
                    personagem = "Boto",
                    emocao = "triste",
                    texto = "Ele aceita paz, mas pede oferenda. Tacaca no Fogao — e a lembranca de Lia, na vila."
                }
            });
            return;
        }

        if (temTacaca && !temFoto)
        {
            GerenciadorDialogo.Instancia?.Iniciar(new[]
            {
                new FalaDialogo
                {
                    personagem = "Boto",
                    emocao = "saudade",
                    texto = "O aroma do Tacaca traz paz, mas ainda falta aquilo que o coracao procura. Ha alguem na vila que continua esperando por mim."
                },
                new FalaDialogo
                {
                    personagem = "Bento",
                    emocao = "compreensivo",
                    texto = "A oferenda ficou incompleta. Vou ate a Lia — o Tacaca fica comigo ate eu voltar com o retrato."
                }
            });
            return;
        }

        if (!temTacaca && temFoto)
        {
            GerenciadorDialogo.Instancia?.Iniciar(new[]
            {
                new FalaDialogo
                {
                    personagem = "Boto",
                    emocao = "saudade",
                    texto = "O retrato de Lia aquece o espirito, mas a oferenda pede tambem o Tacaca da beira do rio."
                }
            });
            return;
        }

        inventario.RemoverPorId(CatalogoItensNorte.IdTacaca, 1);
        CaminhoLiberado = true;
        AplicarEstado();
        CadernoCampo.Instancia?.Desbloquear(CatalogoPaginasCap1.IdBotoAcalmado);
        CadernoCampo.Instancia?.Desbloquear(CatalogoPaginasCap1.IdEssenciaAguas);

        var amuleto = AmuletoAncestral.Instancia ?? FindFirstObjectByType<AmuletoAncestral>();
        amuleto?.Receber(AmuletoAncestral.IdEssenciaDasAguas);
        CaminhoDoRioLiberado?.Invoke();

        GerenciadorDialogo.Instancia?.Iniciar(new[]
        {
            new FalaDialogo
            {
                personagem = "Boto",
                emocao = "aliviado",
                texto = "O rosa volta ao dorso. O Tacaca e o retrato de Lia devolvem a memoria das noites claras."
            },
            new FalaDialogo
            {
                personagem = "Boto",
                emocao = "sereno",
                texto = "Ainda nao posso curar estas aguas, mas posso confiar a voce parte da minha essencia. Quando chegar a hora, ela ajudara o rio a encontrar novamente seu caminho."
            },
            new FalaDialogo
            {
                personagem = "Bento",
                emocao = "respeitoso",
                texto = "O amuleto recebeu a energia espiritual do Boto. Sigo ate a Iara — Lia ainda espera as noites claras."
            }
        });

        Debug.Log("[Lendas] Boto acalmado. Essencia das Aguas no amuleto. O rio abriu.");
    }

    void AplicarEstado()
    {
        if (sprite != null)
        {
            if (CaminhoLiberado || Acalmado) sprite.color = corCalmo;
            else sprite.color = Color.Lerp(corBravo, corCalmo, flechasAcertadas / (float)Mathf.Max(1, flechasParaAcalmar));
        }

        if (barreiras == null) return;
        for (int i = 0; i < barreiras.Length; i++)
        {
            if (barreiras[i] != null) barreiras[i].SetActive(!CaminhoLiberado);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<BentoMovimento>() != null) jogadorProximo = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<BentoMovimento>() != null) jogadorProximo = false;
    }

    void OnGUI()
    {
        if (CaminhoLiberado) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto
            || HudCaderno.Aberto || HudPesquisa.Aberto) return;
        if (Camera.main == null) return;

        Vector3 tela = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.3f);
        if (tela.z < 0f) return;
        float y = Screen.height - tela.y;

        if (!Acalmado && CanoaRio.JogadorDentro && CanoaNaZonaDeCombate())
        {
            GUI.Box(new Rect(tela.x - 140f, y - 28f, 280f, 32f),
                $"Boto agitado  {flechasAcertadas}/{flechasParaAcalmar} flechas");
            return;
        }

        if (!jogadorProximo || !Acalmado || !CanoaRio.JogadorDentro) return;

        var inventario = InventarioBento.Instancia;
        bool temTacaca = inventario != null && inventario.TemPorId(CatalogoItensNorte.IdTacaca);
        bool temFoto = inventario != null && inventario.TemPorId(CatalogoItensNorte.IdFotoDaLia);
        string dica;
        if (temTacaca && temFoto) dica = "[E] Oferecer Tacaca e foto da Lia";
        else if (temTacaca) dica = "[E] Oferecer Tacaca (falta a foto da Lia)";
        else if (temFoto) dica = "[E] Mostrar foto (falta Tacaca)";
        else dica = "Oferenda: Tacaca + foto da Lia";

        GUI.Box(new Rect(tela.x - 150f, y - 28f, 300f, 32f), dica);
    }
}
