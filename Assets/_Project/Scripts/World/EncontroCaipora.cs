using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Encontro da Mata: Caipora montado em seu queixada (guardiao da caca).
/// Etapa 1: Quebrar as 2 jaulas da fauna, derrotar o Capanga cacador e usar
/// a Visao Espectral (V) para enxergar o rastro do assobio na folhagem e acertar 2 flechas purificadoras.
/// Etapa 2: Oferenda de licenca (E) com Colar de Sementes (escambo da Taina) + Favo de Mel.
/// Entrega a Fibra do Caipora (para o Novelo Encantado) + 3 flechas e abre o caminho das Ruinas.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class EncontroCaipora : MonoBehaviour, IRecebeDano
{
    public static EncontroCaipora Instancia { get; private set; }
    public static bool Acalmado { get; private set; }
    public static bool CaminhoLiberado { get; private set; }
    public static event Action CaiporaAcalmado;
    public static event Action CaminhoDaMataLiberado;

    public static bool CapturaTeclaE =>
        Instancia != null && Instancia.jogadorProximo && Acalmado && !CaminhoLiberado;

    public GameObject[] barreiras;
    public KeyCode teclaOferecer = KeyCode.E;
    public int flechasParaAcalmar = 2;
    public float intervaloInvestida = 3.1f;
    public float velocidadeInvestida = 8.5f;
    public float duracaoAuraAposVisao = 4.5f;
    public int danoInvestida = 1;

    int flechasAcertadas;
    SpriteRenderer sprite;
    BentoVida vidaBento;
    bool jogadorProximo;
    bool investindo;
    float proximaInvestida;
    float auraReveladaAte;
    float avisoSemVisaoAte;
    Color corFolhagem = new Color(0.18f, 0.38f, 0.16f, 0.24f);
    Color corAuraEspectral = new Color(0.45f, 1f, 0.55f, 0.98f);
    Color corCalmo = new Color(0.35f, 0.85f, 0.35f, 1f);
    Vector3 posicaoRepouso;

    public int FlechasAcertadas => flechasAcertadas;
    public bool AuraVisivel =>
        Acalmado || CaminhoLiberado || BentoVisaoEspectral.Ativa || Time.time < auraReveladaAte;

    void Awake()
    {
        Instancia = this;
        Acalmado = false;
        CaminhoLiberado = false;
        flechasAcertadas = 0;
        auraReveladaAte = 0f;
        sprite = GetComponent<SpriteRenderer>();
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Start()
    {
        posicaoRepouso = LayoutPrototipo.Caipora;
        if (barreiras == null || barreiras.Length == 0)
        {
            var barreira = GameObject.Find("BarreiraMataCaipora");
            if (barreira != null) barreiras = new[] { barreira };
        }

        JaulaFauna.JaulaAberta += AoAbrirJaula;
        vidaBento = FindFirstObjectByType<BentoVida>();
        if (vidaBento != null)
        {
            vidaBento.Morreu += AoBentoMorrer;
            vidaBento.Revivesceu += AoBentoReviver;
        }

        AplicarEstado();
    }

    void OnDestroy()
    {
        JaulaFauna.JaulaAberta -= AoAbrirJaula;
        if (vidaBento != null)
        {
            vidaBento.Morreu -= AoBentoMorrer;
            vidaBento.Revivesceu -= AoBentoReviver;
        }
        if (Instancia == this) Instancia = null;
    }

    void AoBentoMorrer()
    {
        investindo = false;
        StopAllCoroutines();
        transform.position = posicaoRepouso;
        AplicarEstado();
    }

    void AoBentoReviver()
    {
        if (Acalmado || CaminhoLiberado) return;
        GarantirFlechasParaLuta();
        proximaInvestida = Time.time + intervaloInvestida;
    }

    void AoAbrirJaula(JaulaFauna _)
    {
        VerificarCondicaoAcalmar();
    }

    void Update()
    {
        if (CaminhoLiberado) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto
            || HudCaderno.Aberto || HudPesquisa.Aberto) return;

        if (!Acalmado)
        {
            if (JogadorNaZonaDeCombate())
            {
                GarantirFlechasParaLuta();
                if (BentoVisaoEspectral.Ativa)
                    auraReveladaAte = Time.time + duracaoAuraAposVisao;
            }

            VerificarCondicaoAcalmar();
            if (Acalmado) return;

            if (!investindo)
                AplicarEstado();

            if (JogadorNaZonaDeCombate() && !investindo && flechasAcertadas < flechasParaAcalmar)
            {
                if (Time.time >= proximaInvestida)
                    StartCoroutine(Investir());
            }
        }

        if (!jogadorProximo || !Acalmado) return;
        if (!Input.GetKeyDown(teclaOferecer)) return;
        TentarOferenda();
    }

    bool JogadorNaZonaDeCombate()
    {
        var bento = FindFirstObjectByType<BentoMovimento>();
        if (bento == null) return false;
        float x = bento.transform.position.x;
        return x >= LayoutPrototipo.InicioZonaCaipora && x <= LayoutPrototipo.BarreiraCaipora.x + 0.5f;
    }

    static bool CapangaCacadorDerrotado()
    {
        var capanga = GameObject.Find("CapangaTreino");
        return capanga == null || !capanga.activeInHierarchy;
    }

    int FlechasFaltandoParaAcalmar()
    {
        return Mathf.Max(0, flechasParaAcalmar - flechasAcertadas);
    }

    void GarantirFlechasParaLuta()
    {
        int precisa = FlechasFaltandoParaAcalmar();
        if (precisa <= 0) return;

        CatalogoItensNorte.Garantir();
        var inventario = InventarioBento.Instancia ?? FindFirstObjectByType<InventarioBento>();
        if (inventario == null || CatalogoItensNorte.FlechasPurificadoras == null) return;

        int tem = inventario.QuantidadePorId(CatalogoItensNorte.IdFlechasPurificadoras);
        int falta = precisa - tem;
        if (falta <= 0) return;

        inventario.Adicionar(CatalogoItensNorte.FlechasPurificadoras, falta);
        RepositorColetaveis.GarantirReagentesDaMata();
        Debug.Log($"[Lendas] Sem munição na clareira: +{falta} flecha(s). Cipó e resina voltaram na mata.");
    }

    void VerificarCondicaoAcalmar()
    {
        if (Acalmado || CaminhoLiberado) return;
        if (flechasAcertadas < flechasParaAcalmar) return;
        if (!JaulaFauna.TodasLibertadas) return;
        if (!CapangaCacadorDerrotado()) return;

        Acalmar();
    }

    IEnumerator Investir()
    {
        investindo = true;
        proximaInvestida = Time.time + intervaloInvestida;

        var bento = FindFirstObjectByType<BentoMovimento>();
        if (bento == null)
        {
            investindo = false;
            yield break;
        }

        if (sprite != null)
        {
            sprite.color = AuraVisivel
                ? corAuraEspectral
                : new Color(0.28f, 0.52f, 0.2f, 0.6f);
        }

        float alvoX = Mathf.Max(LayoutPrototipo.InicioZonaCaipora + 1f, bento.transform.position.x - 0.5f);
        Vector3 alvo = new Vector3(alvoX, posicaoRepouso.y, 0f);
        bool causouDano = false;

        while (Vector3.Distance(transform.position, alvo) > 0.25f)
        {
            if (Acalmado || !JogadorNaZonaDeCombate())
            {
                investindo = false;
                AplicarEstado();
                yield break;
            }

            if (BentoVisaoEspectral.Ativa)
                auraReveladaAte = Time.time + duracaoAuraAposVisao;

            if (sprite != null && AuraVisivel)
                sprite.color = corAuraEspectral;

            transform.position = Vector3.MoveTowards(
                transform.position, alvo, velocidadeInvestida * Time.deltaTime);

            if (!causouDano && bento != null && Vector2.Distance(transform.position, bento.transform.position) < 1.15f)
            {
                var vida = bento.GetComponent<BentoVida>();
                if (vida != null)
                {
                    vida.ReceberDano(danoInvestida, transform.position);
                    causouDano = true;
                }
            }

            yield return null;
        }

        Vector3 volta = posicaoRepouso;
        while (Vector3.Distance(transform.position, volta) > 0.1f)
        {
            if (BentoVisaoEspectral.Ativa)
                auraReveladaAte = Time.time + duracaoAuraAposVisao;

            transform.position = Vector3.MoveTowards(
                transform.position, volta, velocidadeInvestida * 0.75f * Time.deltaTime);
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
            Debug.Log("[Lendas] O Caipora e guardiao espiritual. Use flechas purificadoras com Visao Espectral [V].");
            return;
        }

        if (!AuraVisivel)
        {
            avisoSemVisaoAte = Time.time + 3.2f;
            Debug.Log("[Lendas] Assobio na mata fechada! Use a Visao Espectral [V] para ver o rastro do Caipora.");
            return;
        }

        flechasAcertadas = Mathf.Min(flechasParaAcalmar, flechasAcertadas + 1);
        auraReveladaAte = 0f;
        avisoSemVisaoAte = 0f;
        Debug.Log($"[Lendas] Flecha purificadora no Caipora ({flechasAcertadas}/{flechasParaAcalmar}).");

        VerificarCondicaoAcalmar();
        if (!Acalmado)
            AplicarEstado();
    }

    void Acalmar()
    {
        Acalmado = true;
        investindo = false;
        StopAllCoroutines();
        transform.position = posicaoRepouso;
        AplicarEstado();
        CadernoCampo.Instancia?.Desbloquear(CatalogoPaginasCap1.IdCaipora);
        CaiporaAcalmado?.Invoke();

        GerenciadorDialogo.Instancia?.Iniciar(new[]
        {
            new FalaDialogo
            {
                personagem = "Caipora",
                emocao = "atento",
                texto = "O assobio cessa na clareira. O Caipora acaricia o queixada liberto das jaulas do Barao e encara Bento em silencio."
            },
            new FalaDialogo
            {
                personagem = "Bento",
                emocao = "respeitoso",
                texto = "Os bichos estao soltos e o cacador do Barao ja nao incomoda. Trouxe a licenca da mata: o Colar de Sementes da Taina e o Favo de Mel."
            }
        });
    }

    void TentarOferenda()
    {
        CatalogoItensNorte.Garantir();
        var inventario = InventarioBento.Instancia ?? FindFirstObjectByType<InventarioBento>();
        if (inventario == null) return;

        bool temColar = inventario.TemPorId(CatalogoItensNorte.IdColarDeSementes);
        bool temMel = inventario.TemPorId(CatalogoItensNorte.IdFavoDeMel);

        if (!temColar || !temMel)
        {
            var faltas = new List<string>();
            if (!temColar) faltas.Add("o Colar de Sementes (escambo com a Taina por Pato no Tucupi)");
            if (!temMel) faltas.Add("o Favo de Mel da mata");

            GerenciadorDialogo.Instancia?.Iniciar(new[]
            {
                new FalaDialogo
                {
                    personagem = "Caipora",
                    emocao = "aguardando",
                    texto = "O queixada bate o casco no toco. A licenca da mata ainda pede " + string.Join(" e ", faltas) + "."
                }
            });
            return;
        }

        inventario.RemoverPorId(CatalogoItensNorte.IdColarDeSementes, 1);
        inventario.RemoverPorId(CatalogoItensNorte.IdFavoDeMel, 1);
        inventario.Adicionar(CatalogoItensNorte.FibraDoCaipora, 1);
        inventario.Adicionar(CatalogoItensNorte.FlechasPurificadoras, 3);

        CaminhoLiberado = true;
        AplicarEstado();
        CadernoCampo.Instancia?.Desbloquear(CatalogoPaginasCap1.IdCaiporaAcalmado);
        CaminhoDaMataLiberado?.Invoke();

        GerenciadorDialogo.Instancia?.Iniciar(new[]
        {
            new FalaDialogo
            {
                personagem = "Caipora",
                emocao = "sereno",
                texto = "Quem quebra armadilha e honra o toco da mata tem passagem livre. Leva esta Fibra do Caipora — sem ela, nenhum novelo prende a furia do Curupira."
            },
            new FalaDialogo
            {
                personagem = "Bento",
                emocao = "respeitoso",
                texto = "Obrigado, guardiao. Com a Fibra do Caipora, o cipo e a resina, posso tecer o Novelo Encantado na Barca e seguir pelas Ruinas."
            }
        });

        Debug.Log("[Lendas] Caipora apaziguado. Recebeu Fibra do Caipora + 3 flechas. Caminho para as Ruinas liberado.");
    }

    void AplicarEstado()
    {
        if (sprite != null)
        {
            if (CaminhoLiberado || Acalmado)
                sprite.color = corCalmo;
            else if (AuraVisivel)
                sprite.color = Color.Lerp(corAuraEspectral, corCalmo, flechasAcertadas / (float)Mathf.Max(1, flechasParaAcalmar));
            else
                sprite.color = corFolhagem;
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

        Vector3 tela = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.35f);
        if (tela.z < 0f) return;
        float y = Screen.height - tela.y;

        if (!Acalmado && JogadorNaZonaDeCombate())
        {
            string texto;
            if (flechasAcertadas >= flechasParaAcalmar)
            {
                bool jaulasOk = JaulaFauna.TodasLibertadas;
                bool capangaOk = CapangaCacadorDerrotado();
                if (!jaulasOk && !capangaOk)
                    texto = $"Quebre as jaulas ({JaulaFauna.JaulasLibertadas}/{JaulaFauna.TotalJaulas}) e derrote o Capanga!";
                else if (!jaulasOk)
                    texto = $"Falta libertar os bichos das jaulas ({JaulaFauna.JaulasLibertadas}/{JaulaFauna.TotalJaulas})!";
                else
                    texto = "Derrote o Capanga cacador na mata para acalmar o Caipora!";
            }
            else if (Time.time < avisoSemVisaoAte && !AuraVisivel)
                texto = $"Assobio na mata! Use [V] Visao Espectral ({flechasAcertadas}/{flechasParaAcalmar} flechas)";
            else if (AuraVisivel)
                texto = $"Rastro visivel! [K] Flecha ({flechasAcertadas}/{flechasParaAcalmar}) · Jaulas {JaulaFauna.JaulasLibertadas}/{JaulaFauna.TotalJaulas}";
            else
                texto = $"Caipora na folhagem — [V] Ver rastro ({flechasAcertadas}/{flechasParaAcalmar}) · Jaulas {JaulaFauna.JaulasLibertadas}/{JaulaFauna.TotalJaulas}";

            GUI.Box(new Rect(tela.x - 205f, y - 28f, 410f, 32f), texto);
            GUI.Box(new Rect(tela.x - 205f, y + 8f, 410f, 24f),
                "Sem flecha? O ponto verde da clareira reabastece. Cipó e resina voltam na mata.");
            return;
        }

        if (!jogadorProximo || !Acalmado) return;

        var inventario = InventarioBento.Instancia;
        bool temColar = inventario != null && inventario.TemPorId(CatalogoItensNorte.IdColarDeSementes);
        bool temMel = inventario != null && inventario.TemPorId(CatalogoItensNorte.IdFavoDeMel);

        string dica;
        if (temColar && temMel) dica = "[E] Oferecer Colar de Sementes e Favo de Mel";
        else if (temColar) dica = "[E] Oferecer Colar (falta Favo de Mel na mata)";
        else if (temMel) dica = "[E] Oferecer Favo de Mel (falta Colar da Taina)";
        else dica = "Oferenda: Colar de Sementes (Taina) + Favo de Mel";

        GUI.Box(new Rect(tela.x - 175f, y - 28f, 350f, 32f), dica);
    }
}
