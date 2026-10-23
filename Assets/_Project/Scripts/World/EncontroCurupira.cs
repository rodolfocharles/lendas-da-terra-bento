using System;
using UnityEngine;

/// <summary>
/// Ritual hibrido do Curupira (H01). Continua placa neste prototipo:
/// Visao Espectral → novelo nos focos → purificar Sombras → corrupcao a 0%
/// → Gota de Quartzo + Essencia das Aguas selam o ritual.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class EncontroCurupira : MonoBehaviour, IRecebeDano
{
    public static EncontroCurupira Instancia { get; private set; }
    public static bool Purificado { get; private set; }
    public static bool PortaoAberto { get; private set; }
    public static event Action CurupiraPurificado;

    public static bool CapturaTeclaE =>
        Instancia != null && !Purificado
        && (Instancia.PodeAbrirPortao || Instancia.PodeCanalizar || FocoCorrupcao.AlgumProntoParaEnrolar);

    public GameObject portaoArena;
    public KeyCode teclaUsar = KeyCode.E;
    public float corrupcaoMaxima = 100f;
    public float reducaoPorFocoPorSegundo = 5f;
    public int danoContato = 1;
    public float cooldownDanoContato = 1.1f;
    public float velocidadePatrulha = 1.6f;

    public float CorrupcaoAtual { get; private set; }
    public bool NoveloJaUsado { get; private set; }
    public bool PodeAbrirPortao =>
        !PortaoAberto && portaoArena != null && portaoArena.activeSelf && JogadorPertoDoPortao();
    public bool PodeCanalizar =>
        !Purificado && ritualIniciado && jogadorProximo && ProntoParaSelo();

    SpriteRenderer sprite;
    bool jogadorProximo;
    bool ritualIniciado;
    bool avisoForcaFeito;
    bool avisoEntradaFeito;
    float proximoDanoContato;
    float limiteEsq;
    float limiteDir;
    int direcaoPatrulha = 1;
    Color corCorrupto = new Color(0.55f, 0.12f, 0.08f);
    Color corTradicional = new Color(0.95f, 0.45f, 0.12f);

    void Awake()
    {
        Instancia = this;
        Purificado = false;
        PortaoAberto = false;
        CorrupcaoAtual = corrupcaoMaxima;
        sprite = GetComponent<SpriteRenderer>();
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Start()
    {
        limiteEsq = LayoutPrototipo.FocoA.x;
        limiteDir = LayoutPrototipo.FocoC.x;

        if (portaoArena == null)
            portaoArena = GameObject.Find("BarreiraArenaCurupira");

        AplicarEstado();
    }

    void OnDestroy()
    {
        if (Instancia == this) Instancia = null;
    }

    void Update()
    {
        if (Purificado) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto
            || HudCaderno.Aberto || HudPesquisa.Aberto) return;

        if (PodeAbrirPortao && Input.GetKeyDown(teclaUsar))
        {
            TentarAbrirPortao();
            return;
        }

        AtualizarRitual();

        if (PodeCanalizar && Input.GetKeyDown(teclaUsar))
            TentarSelarRitual();
    }

    void AtualizarRitual()
    {
        if (!PortaoAberto) return;

        var bento = FindFirstObjectByType<BentoMovimento>();
        bool naArena = bento != null && bento.transform.position.x >= LayoutPrototipo.CheckpointArena.x;
        if (naArena && !ritualIniciado)
            IniciarRitual();

        if (!ritualIniciado) return;

        Patrulhar();

        if (ContarSombrasNaArena() > 0) return;
        int enrolados = FocoCorrupcao.ContarEnrolados();
        if (enrolados <= 0) return;

        CorrupcaoAtual = Mathf.Max(0f, CorrupcaoAtual - reducaoPorFocoPorSegundo * enrolados * Time.deltaTime);
        AtualizarCor();
    }

    void IniciarRitual()
    {
        ritualIniciado = true;
        CadernoCampo.Instancia?.Desbloquear(CatalogoPaginasCap1.IdCurupira);

        if (avisoEntradaFeito) return;
        avisoEntradaFeito = true;
        GerenciadorDialogo.Instancia?.Iniciar(new[]
        {
            new FalaDialogo
            {
                personagem = "Bento",
                emocao = "alerta",
                texto = "O Curupira ainda e o menino da mata, mas a corrupcao torceu o cabelo de fogo. Nao e luta de forca."
            },
            new FalaDialogo
            {
                personagem = "Bento",
                emocao = "determinado",
                texto = "Visao Espectral nos focos, novelo de cipo, flechas nas Sombras. Enquanto elas vierem, a corrupcao nao desce. Caderno aberto se eu esquecer."
            }
        });
    }

    void Patrulhar()
    {
        Vector3 pos = transform.position;
        pos.x += direcaoPatrulha * velocidadePatrulha * Time.deltaTime;
        if (pos.x > limiteDir)
        {
            pos.x = limiteDir;
            direcaoPatrulha = -1;
        }
        else if (pos.x < limiteEsq)
        {
            pos.x = limiteEsq;
            direcaoPatrulha = 1;
        }
        transform.position = pos;
    }

    public void TentarEnrolar(FocoCorrupcao foco)
    {
        if (foco == null || foco.Enrolado || Purificado) return;

        CatalogoItensNorte.Garantir();
        var inventario = InventarioBento.Instancia ?? FindFirstObjectByType<InventarioBento>();
        if (!NoveloJaUsado)
        {
            if (inventario == null || !inventario.TemPorId(CatalogoItensNorte.IdNoveloEncantado))
            {
                GerenciadorDialogo.Instancia?.Iniciar(new[]
                {
                    new FalaDialogo
                    {
                        personagem = "Bento",
                        emocao = "pensativo",
                        texto = "O foco pede o novelo. 2 cipos e resina na Bancada de Alquimia — o diario do avo ja tinha dito."
                    }
                });
                return;
            }

            inventario.RemoverPorId(CatalogoItensNorte.IdNoveloEncantado, 1);
            NoveloJaUsado = true;
            CadernoCampo.Instancia?.Desbloquear(CatalogoPaginasCap1.IdNovelo);
        }

        foco.Enrolar();
        InvocarSombra(foco.transform.position + Vector3.up * 0.4f);
        CadernoCampo.Instancia?.Desbloquear(CatalogoPaginasCap1.IdCurupira);
        CadernoCampo.Instancia?.Desbloquear(CatalogoPaginasCap1.IdSombras);

        int enrolados = FocoCorrupcao.ContarEnrolados();
        Debug.Log($"[Lendas] Novelo no foco ({enrolados}/3). Uma Sombra brotou.");
    }

    void TentarAbrirPortao()
    {
        CatalogoItensNorte.Garantir();
        var inventario = InventarioBento.Instancia ?? FindFirstObjectByType<InventarioBento>();
        bool temGota = inventario != null && inventario.TemPorId(CatalogoItensNorte.IdGotaDeQuartzo);

        if (!temGota)
        {
            GerenciadorDialogo.Instancia?.Iniciar(new[]
            {
                new FalaDialogo
                {
                    personagem = "Bento",
                    emocao = "respeitoso",
                    texto = "A mata nao abre. A Iara disse: a Gota de Quartzo e a licenca ate o Curupira."
                }
            });
            return;
        }

        PortaoAberto = true;
        if (portaoArena != null) portaoArena.SetActive(false);
        CadernoCampo.Instancia?.Desbloquear(CatalogoPaginasCap1.IdIaraPurificada);

        GerenciadorDialogo.Instancia?.Iniciar(new[]
        {
            new FalaDialogo
            {
                personagem = "Bento",
                emocao = "sereno",
                texto = "A Gota brilha e o cipozal cede. A arena do Curupira fica adiante — o quartzo fica comigo ate o selo do ritual."
            }
        });
    }

    void TentarSelarRitual()
    {
        if (!ProntoParaSelo()) return;

        CatalogoItensNorte.Garantir();
        var inventario = InventarioBento.Instancia ?? FindFirstObjectByType<InventarioBento>();
        bool temGota = inventario != null && inventario.TemPorId(CatalogoItensNorte.IdGotaDeQuartzo);
        var amuleto = AmuletoAncestral.Instancia ?? FindFirstObjectByType<AmuletoAncestral>();
        bool temEssencia = amuleto != null && amuleto.TemEssenciaDasAguas;

        if (!temGota || !temEssencia)
        {
            GerenciadorDialogo.Instancia?.Iniciar(new[]
            {
                new FalaDialogo
                {
                    personagem = "Bento",
                    emocao = "urgente",
                    texto = TextoFaltaSelo(temGota, temEssencia)
                }
            });
            return;
        }

        inventario.RemoverPorId(CatalogoItensNorte.IdGotaDeQuartzo, 1);
        Purificado = true;
        CorrupcaoAtual = 0f;
        transform.position = LayoutPrototipo.Curupira;
        AplicarEstado();

        amuleto.Receber(AmuletoAncestral.IdPassoInvertido);
        CadernoCampo.Instancia?.Desbloquear(CatalogoPaginasCap1.IdCurupira);
        CadernoCampo.Instancia?.Desbloquear(CatalogoPaginasCap1.IdCurupiraPurificado);
        CadernoCampo.Instancia?.Desbloquear(CatalogoPaginasCap1.IdPassoInvertido);
        CurupiraPurificado?.Invoke();

        GerenciadorDialogo.Instancia?.Iniciar(new[]
        {
            new FalaDialogo
            {
                personagem = "Curupira",
                emocao = "aliviado",
                texto = "O fogo do cabelo volta a ser cabelo. Os pes invertidos acham o chao da mata. O Barao me usou contra o meu rio."
            },
            new FalaDialogo
            {
                personagem = "Curupira",
                emocao = "respeito",
                texto = "A Gota e a Essencia das Aguas fecharam o ciclo. Leva o Passo Invertido: a mata te mostra o caminho falso e o verdadeiro."
            },
            new FalaDialogo
            {
                personagem = "Bento",
                emocao = "respeitoso",
                texto = "O amuleto recebeu o Passo Invertido. A vila ainda espera a cor — mas o guarda da mata ja nao e inimigo."
            }
        });

        Debug.Log("[Lendas] Curupira purificado. Passo Invertido no amuleto.");
    }

    static string TextoFaltaSelo(bool temGota, bool temEssencia)
    {
        if (!temGota && !temEssencia)
            return "A corrupcao cedeu, mas o selo pede a Gota de Quartzo e a Essencia das Aguas no amuleto.";
        if (!temGota)
            return "Falta a Gota de Quartzo da Iara para selar o ritual.";
        return "O amuleto precisa da Essencia das Aguas do Boto para concluir.";
    }

    bool ProntoParaSelo()
    {
        return FocoCorrupcao.ContarEnrolados() >= 3
            && ContarSombrasNaArena() == 0
            && CorrupcaoAtual <= 0.5f;
    }

    public void ReceberDano(int quantidade, Vector2 origem, TipoDano tipo)
    {
        if (Purificado || quantidade <= 0) return;
        if (tipo == TipoDano.CorteVegetacao) return;

        if (avisoForcaFeito) return;
        avisoForcaFeito = true;
        GerenciadorDialogo.Instancia?.Iniciar(new[]
        {
            new FalaDialogo
            {
                personagem = "Bento",
                emocao = "alerta",
                texto = "Nao cai por forca. O diario do avo: novelo nos focos, purificar Sombras, deixar a corrupcao descer."
            }
        });
    }

    int ContarSombrasNaArena()
    {
        var sombras = FindObjectsByType<InimigoSombra>(FindObjectsSortMode.None);
        int n = 0;
        float inicioArena = LayoutPrototipo.CheckpointArena.x - 2f;
        for (int i = 0; i < sombras.Length; i++)
        {
            if (sombras[i] == null || sombras[i].Purificada) continue;
            if (sombras[i].transform.position.x >= inicioArena) n++;
        }
        return n;
    }

    void InvocarSombra(Vector3 posicao)
    {
        var go = new GameObject("SombraRitual");
        go.transform.position = posicao;
        go.transform.localScale = new Vector3(1.1f, 1.4f, 1f);

        var sr = go.AddComponent<SpriteRenderer>();
        if (sprite != null) sr.sprite = sprite.sprite;
        sr.color = new Color(0.55f, 0.25f, 0.75f, 0.9f);
        sr.sortingOrder = 2;

        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(1f, 1f);

        var sombra = go.AddComponent<InimigoSombra>();
        sombra.DefinirCorrupcaoInicial(70f);
    }

    bool JogadorPertoDoPortao()
    {
        if (portaoArena == null || !portaoArena.activeSelf) return false;
        var bento = FindFirstObjectByType<BentoMovimento>();
        if (bento == null) return false;
        return Vector2.Distance(bento.transform.position, portaoArena.transform.position) < 2.4f;
    }

    void AplicarEstado()
    {
        if (sprite != null)
            sprite.color = Purificado ? corTradicional : Color.Lerp(corTradicional, corCorrupto, CorrupcaoAtual / Mathf.Max(1f, corrupcaoMaxima));

        if (Purificado && portaoArena != null)
            portaoArena.SetActive(false);
    }

    void AtualizarCor()
    {
        if (sprite == null || Purificado) return;
        float t = corrupcaoMaxima <= 0f ? 0f : CorrupcaoAtual / corrupcaoMaxima;
        sprite.color = Color.Lerp(corTradicional, corCorrupto, t);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<BentoMovimento>() != null) jogadorProximo = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<BentoMovimento>() != null) jogadorProximo = false;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (Purificado) return;
        if (!ritualIniciado) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (Time.time < proximoDanoContato) return;
        var vida = other.GetComponent<BentoVida>() ?? other.GetComponentInParent<BentoVida>();
        if (vida == null) return;
        vida.ReceberDano(danoContato, transform.position);
        proximoDanoContato = Time.time + cooldownDanoContato;
    }

    void OnGUI()
    {
        if (Purificado) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto
            || HudCaderno.Aberto || HudPesquisa.Aberto) return;

        if (PodeAbrirPortao)
            DesenharDicaPortao();

        if (!ritualIniciado) return;
        DesenharBarraCorrupcao();

        if (!PodeCanalizar || Camera.main == null) return;
        Vector3 tela = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.4f);
        if (tela.z < 0f) return;
        float y = Screen.height - tela.y;
        GUI.Box(new Rect(tela.x - 170f, y - 28f, 340f, 32f), "[E] Selar ritual (Gota + Essencia)");
    }

    void DesenharDicaPortao()
    {
        if (portaoArena == null || Camera.main == null) return;
        Vector3 tela = Camera.main.WorldToScreenPoint(portaoArena.transform.position + Vector3.up * 0.4f);
        if (tela.z < 0f) return;
        float y = Screen.height - tela.y;

        var inventario = InventarioBento.Instancia;
        bool temGota = inventario != null && inventario.TemPorId(CatalogoItensNorte.IdGotaDeQuartzo);
        string dica = temGota
            ? "[E] Apresentar a Gota de Quartzo"
            : "A mata tranca o caminho. Falta a Gota da Iara.";
        GUI.Box(new Rect(tela.x - 160f, y - 28f, 320f, 32f), dica);
    }

    void DesenharBarraCorrupcao()
    {
        float largura = 280f;
        float x = (Screen.width - largura) * 0.5f;
        float y = 18f;
        GUI.Box(new Rect(x, y, largura, 44f), "");

        var titulo = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        titulo.normal.textColor = new Color(1f, 0.85f, 0.7f);
        int focos = FocoCorrupcao.ContarEnrolados();
        int sombras = ContarSombrasNaArena();
        string estado = sombras > 0
            ? $"Sombras {sombras}  ·  a corrupcao nao desce"
            : (focos > 0 ? $"Focos {focos}/3  ·  corrupcao descendo" : "Enrolar o novelo nos focos [V]");
        GUI.Label(new Rect(x, y + 2f, largura, 18f), "Curupira — " + estado, titulo);

        float fill = corrupcaoMaxima <= 0f ? 0f : CorrupcaoAtual / corrupcaoMaxima;
        GUI.color = new Color(0.55f, 0.12f, 0.55f, 0.95f);
        GUI.DrawTexture(new Rect(x + 8f, y + 24f, (largura - 16f) * fill, 14f), Texture2D.whiteTexture);
        GUI.color = Color.white;

        var pct = new GUIStyle(GUI.skin.label) { fontSize = 11, alignment = TextAnchor.MiddleCenter };
        pct.normal.textColor = Color.white;
        GUI.Label(new Rect(x, y + 22f, largura, 16f), $"{CorrupcaoAtual:0}%", pct);
    }
}
