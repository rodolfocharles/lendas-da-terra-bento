using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Boto espiritual: a poluicao nao o contamina, mas a dor dos animais
/// o deixa incontrolavel. Fica submerso um pouco abaixo da canoa; ataca
/// com pulo e mergulho. Na agua turva, Bento precisa da Visao Espectral (V)
/// para ver a aura e acertar 3 flechas. Tacaca + foto da Lia completam a oferenda.
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
        Instancia != null && Instancia.PertoParaInteragir() && Acalmado && !CaminhoLiberado
        && CanoaRio.JogadorDentro && Instancia.CanoaNaZonaDeCombate();

    public GameObject[] barreiras;
    public KeyCode teclaOferecer = KeyCode.E;
    public int flechasParaAcalmar = 3;
    public float intervaloInvestida = 2.4f;
    public float velocidadeInvestida = 7.2f;
    public float duracaoAuraAposVisao = 4.5f;
    public const float EscalaRelativaAoBento = 1.4f;
    public float deslocamentoAbaixoDoBarco = 0.72f;
    public float alturaPulo = 1.05f;
    public float duracaoPulo = 0.72f;
    public float duracaoMergulho = 0.55f;
    public float distanciaInteracao = 4.8f;

    int flechasAcertadas;
    SpriteRenderer sprite;
    Sprite spriteBom;
    Sprite spriteTriste;
    Sprite spriteCalmo;
    Sprite spriteFeliz;
    Sprite spriteCorrupto;
    bool jogadorProximo;
    bool investindo;
    float proximaInvestida;
    float auraReveladaAte;
    float avisoSemVisaoAte;
    Color corAguaTurva = new Color(1f, 1f, 1f, 0.22f);
    Color corAuraEspectral = new Color(1f, 0.82f, 1f, 1f);
    Vector3 posicaoRepouso;

    public int FlechasAcertadas => flechasAcertadas;
    public bool EstaForaDagua => transform.position.y > YCanoa() + 0.12f;
    public bool AuraVisivel =>
        Acalmado || CaminhoLiberado || EstaForaDagua
        || BentoVisaoEspectral.Ativa || Time.time < auraReveladaAte;

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
        posicaoRepouso = LayoutPrototipo.BloqueioBoto;
        if (barreiras == null || barreiras.Length == 0)
        {
            var barreira = GameObject.Find("BarreiraRioBoto");
            if (barreira != null) barreiras = new[] { barreira };
        }

        CarregarArte();
        AplicarEstado();
    }

    void CarregarArte()
    {
        spriteBom = CarregadorPixelArt.Personagem("Boto", "Boto_Bom");
        spriteTriste = CarregadorPixelArt.Personagem("Boto", "Boto_Triste");
        spriteCalmo = CarregadorPixelArt.Personagem("Boto", "Boto_Calmo");
        spriteFeliz = CarregadorPixelArt.Personagem("Boto", "Boto_Feliz");
        spriteCorrupto = CarregadorPixelArt.Personagem("Boto", "Boto_Corrupto");

        if (sprite == null) return;
        sprite.sortingOrder = 2;
        sprite.flipX = true;
        float escala = CarregadorPixelArt.EscalaMundo * EscalaRelativaAoBento;
        transform.localScale = new Vector3(escala, escala, 1f);

        var col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.size = new Vector2(1.7f, 0.95f);
            col.offset = Vector2.zero;
        }

        transform.position = PosicaoRepouso();
    }

    void OnDestroy()
    {
        if (Instancia == this) Instancia = null;
    }

    void Update()
    {
        if (CaminhoLiberado)
        {
            if (!investindo) FlutuarNoTrechoDoRio(YEmergido(), 0.07f);
            AplicarEstado();
            return;
        }

        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto
            || HudCaderno.Aberto || HudPesquisa.Aberto) return;

        if (!Acalmado)
        {
            if (BentoVisaoEspectral.Ativa && CanoaRio.JogadorDentro && CanoaNaZonaDeCombate())
                auraReveladaAte = Time.time + duracaoAuraAposVisao;

            if (!investindo)
            {
                NedarSubmersoPertoDaCanoa();
                AplicarEstado();

                if (CanoaRio.JogadorDentro && CanoaNaZonaDeCombate()
                    && Time.time >= proximaInvestida)
                    StartCoroutine(Investir());
            }
        }
        else if (!investindo)
        {
            FlutuarNoTrechoDoRio(YEmergido(), 0.1f);
            AplicarEstado();
        }

        if (!PertoParaInteragir() || !Acalmado || !CanoaRio.JogadorDentro) return;
        if (!CanoaNaZonaDeCombate()) return;
        if (!Input.GetKeyDown(teclaOferecer)) return;
        TentarOferenda();
    }

    bool PertoParaInteragir()
    {
        if (jogadorProximo) return true;
        var canoa = CanoaRio.Instancia;
        if (canoa == null) return false;
        Vector3 delta = transform.position - canoa.transform.position;
        return Mathf.Abs(delta.x) <= distanciaInteracao && Mathf.Abs(delta.y) <= 2.4f;
    }

    bool CanoaNaZonaDeCombate()
    {
        var canoa = CanoaRio.Instancia;
        if (canoa == null || canoa.Afundando) return false;
        float x = canoa.transform.position.x;
        return x >= LayoutPrototipo.InicioZonaBoto && x <= LayoutPrototipo.LimiteCanoaBoto + 1.2f;
    }

    IEnumerator Investir()
    {
        investindo = true;

        var canoa = CanoaRio.Instancia;
        if (canoa == null)
        {
            investindo = false;
            yield break;
        }

        float lado = transform.position.x <= canoa.transform.position.x ? -1f : 1f;
        Vector3 aproximacao = new Vector3(
            canoa.transform.position.x + lado * 1.55f,
            YSubmerso(),
            0f);
        Vector3 pico = new Vector3(canoa.transform.position.x, YPulo(), 0f);
        Vector3 mergulho = new Vector3(
            canoa.transform.position.x - lado * 1.7f,
            YSubmerso(),
            0f);

        yield return MoverLinear(aproximacao, velocidadeInvestida * 0.75f, true);
        if (!PodeContinuarInvestida(canoa))
        {
            yield return EncerrarInvestida();
            yield break;
        }

        bool atingiu = false;
        yield return PercorrerArco(aproximacao, pico, mergulho, duracaoPulo, p =>
        {
            if (atingiu || canoa == null) return;
            bool noPico = p.y >= YCanoa() + alturaPulo * 0.45f
                && Mathf.Abs(p.x - canoa.transform.position.x) < 0.9f;
            if (noPico || Vector3.Distance(p, canoa.transform.position) < 0.65f)
            {
                canoa.ReceberImpacto();
                atingiu = true;
            }
        });

        if (!atingiu && PodeContinuarInvestida(canoa))
            canoa.ReceberImpacto();

        Vector3 fundo = mergulho + Vector3.down * 0.35f;
        yield return PercorrerArco(mergulho, fundo, PosicaoRepouso(), duracaoMergulho, null);
        yield return EncerrarInvestida();
    }

    bool PodeContinuarInvestida(CanoaRio canoa)
    {
        return !Acalmado && canoa != null && !canoa.Afundando && CanoaRio.JogadorDentro;
    }

    IEnumerator EncerrarInvestida()
    {
        yield return MoverLinear(PosicaoRepouso(), velocidadeInvestida * 0.65f, true);
        ResetarInclinacao();
        if (sprite != null) sprite.flipX = true;
        proximaInvestida = Time.time + intervaloInvestida;
        investindo = false;
        AplicarEstado();
    }

    IEnumerator MoverLinear(Vector3 destino, float velocidade, bool submerso)
    {
        Vector3 anterior = transform.position;
        while (Vector3.Distance(transform.position, destino) > 0.08f)
        {
            if (Acalmado) yield break;
            if (BentoVisaoEspectral.Ativa)
                auraReveladaAte = Time.time + duracaoAuraAposVisao;

            Vector3 proximo = Vector3.MoveTowards(
                transform.position, destino, velocidade * Time.deltaTime);
            AplicarInclinacao(proximo - anterior);
            transform.position = proximo;
            anterior = proximo;
            AplicarEstado();
            yield return null;
        }

        transform.position = destino;
        if (submerso) ResetarInclinacao();
    }

    IEnumerator PercorrerArco(
        Vector3 inicio, Vector3 controle, Vector3 fim, float duracao, Action<Vector3> aoMover)
    {
        float t = 0f;
        Vector3 anterior = inicio;
        transform.position = inicio;
        while (t < 1f)
        {
            if (Acalmado) yield break;
            if (BentoVisaoEspectral.Ativa)
                auraReveladaAte = Time.time + duracaoAuraAposVisao;

            t += Time.deltaTime / Mathf.Max(0.08f, duracao);
            float u = Mathf.Clamp01(t);
            Vector3 p = Bezier(inicio, controle, fim, u);
            AplicarInclinacao(p - anterior);
            transform.position = p;
            anterior = p;
            aoMover?.Invoke(p);
            AplicarEstado();
            yield return null;
        }

        transform.position = fim;
    }

    static Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        float u = 1f - t;
        return u * u * a + 2f * u * t * b + t * t * c;
    }

    float YCanoa()
    {
        return CanoaRio.Instancia != null
            ? CanoaRio.Instancia.transform.position.y
            : LayoutPrototipo.CanoaCais.y;
    }

    float YSubmerso()
    {
        return YCanoa() - deslocamentoAbaixoDoBarco;
    }

    float YPulo()
    {
        return YCanoa() + alturaPulo;
    }

    float YEmergido()
    {
        return YCanoa() - deslocamentoAbaixoDoBarco * 0.38f;
    }

    Vector3 PosicaoRepouso()
    {
        return new Vector3(posicaoRepouso.x, YSubmerso(), 0f);
    }

    Vector3 PosicaoEsperaNoRio()
    {
        return new Vector3(posicaoRepouso.x, YEmergido(), 0f);
    }

    void NedarSubmersoPertoDaCanoa()
    {
        Vector3 alvo;
        if (CanoaRio.JogadorDentro && CanoaNaZonaDeCombate() && CanoaRio.Instancia != null)
        {
            float xCanoa = CanoaRio.Instancia.transform.position.x;
            float lado = transform.position.x >= xCanoa ? 1f : -1f;
            alvo = new Vector3(xCanoa + lado * 2.5f, YSubmerso(), 0f);
            alvo.x = Mathf.Clamp(
                alvo.x,
                LayoutPrototipo.InicioZonaBoto + 0.6f,
                LayoutPrototipo.LimiteCanoaBoto);
        }
        else
        {
            alvo = PosicaoRepouso();
        }

        alvo.y += Mathf.Sin(Time.time * 2.1f) * 0.08f;
        Vector3 proximo = Vector3.MoveTowards(transform.position, alvo, 2.4f * Time.deltaTime);
        if (sprite != null && Mathf.Abs(proximo.x - transform.position.x) > 0.01f)
            sprite.flipX = proximo.x < transform.position.x;
        transform.position = proximo;
        ResetarInclinacao();
    }

    void FlutuarNoTrechoDoRio(float yBase, float amplitude)
    {
        Vector3 alvo = PosicaoEsperaNoRio();
        alvo.y = yBase + Mathf.Sin(Time.time * 1.6f) * amplitude;
        Vector3 proximo = Vector3.MoveTowards(transform.position, alvo, 2.8f * Time.deltaTime);
        if (sprite != null && CanoaRio.Instancia != null)
            sprite.flipX = CanoaRio.Instancia.transform.position.x < transform.position.x;
        transform.position = proximo;
        ResetarInclinacao();
    }

    void AplicarInclinacao(Vector3 deslocamento)
    {
        if (deslocamento.sqrMagnitude < 0.0001f) return;
        float angulo = Mathf.Atan2(deslocamento.y, deslocamento.x) * Mathf.Rad2Deg;
        if (sprite != null)
        {
            sprite.flipX = false;
            if (deslocamento.x < 0f)
            {
                sprite.flipX = true;
                angulo = Mathf.Atan2(deslocamento.y, -deslocamento.x) * Mathf.Rad2Deg;
            }
        }

        transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Clamp(angulo, -48f, 48f));
    }

    void ResetarInclinacao()
    {
        transform.rotation = Quaternion.identity;
    }

    public void ReceberDano(int quantidade, Vector2 origem, TipoDano tipo)
    {
        if (Acalmado || CaminhoLiberado || quantidade <= 0) return;
        if (tipo != TipoDano.Espiritual)
        {
            Debug.Log("[Lendas] O Boto e espirito. Facao e chicote nao o acalmam. Use flechas purificadoras.");
            return;
        }

        if (!AuraVisivel)
        {
            avisoSemVisaoAte = Time.time + 3.2f;
            Debug.Log("[Lendas] Agua turva! Use a Visao Espectral [V] para enxergar a aura do Boto antes de atirar.");
            return;
        }

        flechasAcertadas++;
        auraReveladaAte = 0f;
        avisoSemVisaoAte = 0f;
        Debug.Log($"[Lendas] Flecha no Boto ({flechasAcertadas}/{flechasParaAcalmar}).");

        if (flechasAcertadas >= flechasParaAcalmar)
            Acalmar();
        else
            AplicarEstado();
    }

    void Acalmar()
    {
        Acalmado = true;
        investindo = false;
        StopAllCoroutines();
        ResetarInclinacao();
        transform.position = PosicaoEsperaNoRio();
        AplicarEstado();
        CadernoCampo.Instancia?.Desbloquear(CatalogoPaginasCap1.IdBoto);
        BotoAcalmado?.Invoke();

        GerenciadorDialogo.Instancia?.Iniciar(new[]
        {
            new FalaDialogo
            {
                personagem = "Boto",
                emocao = "cansado",
                texto = "As flechas guiadas pelo amuleto dissipam a raiva. O Boto emerge da agua turva ao lado da canoa, ainda triste — mas ja nao tenta afundar o caico."
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
            sprite.sprite = EscolherSprite();
            sprite.sortingOrder = EstaForaDagua || Acalmado || CaminhoLiberado ? 5 : 2;
            if (CaminhoLiberado || Acalmado)
                sprite.color = Color.white;
            else if (AuraVisivel)
                sprite.color = corAuraEspectral;
            else
                sprite.color = corAguaTurva;
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

        Vector3 tela = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 0.95f);
        if (tela.z < 0f) return;
        float y = Screen.height - tela.y;

        if (!Acalmado && CanoaRio.JogadorDentro && CanoaNaZonaDeCombate())
        {
            string textoCombate;
            if (Time.time < avisoSemVisaoAte && !AuraVisivel)
                textoCombate = $"Agua turva! Use [V] Visao Espectral para ver a aura ({flechasAcertadas}/{flechasParaAcalmar})";
            else if (AuraVisivel)
                textoCombate = $"Aura visivel! [K] Flecha Purificadora ({flechasAcertadas}/{flechasParaAcalmar})";
            else
                textoCombate = $"Agua turva — [V] Ver aura do Boto ({flechasAcertadas}/{flechasParaAcalmar} flechas)";

            GUI.Box(new Rect(tela.x - 185f, y - 28f, 370f, 32f), textoCombate);
            return;
        }

        if (!PertoParaInteragir() || !Acalmado || !CanoaRio.JogadorDentro || !CanoaNaZonaDeCombate()) return;

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

    Sprite EscolherSprite()
    {
        if (CaminhoLiberado)
            return spriteFeliz != null ? spriteFeliz : spriteBom;
        if (Acalmado)
            return spriteTriste != null ? spriteTriste : spriteCalmo;
        if (spriteCorrupto != null) return spriteCorrupto;
        if (spriteCalmo != null) return spriteCalmo;
        return sprite.sprite;
    }
}
