using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Iara na margem, depois na agua. Sem Treme, o canto puxa o Bento
/// da canoa. Com Treme, ela sente o amuleto. Oferenda: pente, espelho e fitas.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class EncontroIara : MonoBehaviour
{
    public static EncontroIara Instancia { get; private set; }
    public static bool Purificada { get; private set; }
    public static event Action IaraPurificada;

    public static bool NaZonaDoCanto =>
        Instancia != null && !Purificada && CanoaRio.JogadorDentro && Instancia.CanoaNaZona();

    public static bool CapturaTeclaE =>
        NaZonaDoCanto && (Instancia.prontaParaOferenda || !TemTremePublico());

    static bool TemTremePublico()
    {
        return Instancia != null && TemTreme();
    }

    public GameObject[] barreiras;
    public KeyCode teclaOferecer = KeyCode.E;
    public float tempoAteOFundo = 6f;

    SpriteRenderer sprite;
    bool prontaParaOferenda;
    bool avisoCantoFeito;
    bool acalmouComTreme;
    bool puxandoAoFundo;
    bool descendoAAgua;
    float cantoAcumulado;
    Color corCorrupta = new Color(0.12f, 0.35f, 0.42f);
    Color corCalma = new Color(0.35f, 0.85f, 0.9f);
    Vector3 posicaoMargem;
    Vector3 posicaoAgua;

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
        posicaoMargem = LayoutPrototipo.Iara;
        posicaoAgua = LayoutPrototipo.IaraAgua;
        transform.position = posicaoMargem;

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
    }

    void Update()
    {
        if (Purificada || puxandoAoFundo) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto
            || HudCaderno.Aberto || HudPesquisa.Aberto) return;

        if (!CanoaRio.JogadorDentro || !CanoaNaZona())
        {
            cantoAcumulado = 0f;
            return;
        }

        if (!descendoAAgua && !prontaParaOferenda)
            StartCoroutine(DescerAAgua());

        if (TemTreme())
        {
            if (!acalmouComTreme) AcalmarPeloTreme();
        }
        else if (!puxandoAoFundo)
        {
            if (!avisoCantoFeito)
            {
                avisoCantoFeito = true;
                GerenciadorDialogo.Instancia?.Iniciar(new[]
                {
                    new FalaDialogo
                    {
                        personagem = "Iara",
                        emocao = "canto",
                        texto = "Ela desce da margem e o canto cobre o rio. Sem Treme, o corpo vai ceder e a agua abre."
                    }
                });
                return;
            }

            cantoAcumulado += Time.deltaTime;
            if (cantoAcumulado >= tempoAteOFundo)
                StartCoroutine(LevarAoFundo());
        }

        if (!TemTreme() && Input.GetKeyDown(teclaOferecer))
        {
            TentarComerAntidoto();
            return;
        }

        if (!prontaParaOferenda) return;
        if (!Input.GetKeyDown(teclaOferecer)) return;
        TentarOferenda();
    }

    bool CanoaNaZona()
    {
        var canoa = CanoaRio.Instancia;
        if (canoa == null || canoa.Afundando) return false;
        float x = canoa.transform.position.x;
        return x >= LayoutPrototipo.InicioZonaIara && x <= LayoutPrototipo.LimiteCanoaIara + 0.4f;
    }

    IEnumerator DescerAAgua()
    {
        descendoAAgua = true;
        float t = 0f;
        Vector3 inicio = transform.position;
        while (t < 1.1f)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(inicio, posicaoAgua, t / 1.1f);
            yield return null;
        }
        transform.position = posicaoAgua;
    }

    void AcalmarPeloTreme()
    {
        acalmouComTreme = true;
        prontaParaOferenda = true;
        cantoAcumulado = 0f;
        if (sprite != null) sprite.color = Color.Lerp(corCorrupta, corCalma, 0.45f);

        bool temEssencia = AmuletoAncestral.Instancia != null
            && AmuletoAncestral.Instancia.TemEssenciaDasAguas;

        var falas = new List<FalaDialogo>
        {
            new FalaDialogo
            {
                personagem = "Bento",
                emocao = "tremendo",
                texto = "O Treme percorre os musculos. O canto quebra; a Iara para na agua, atenta."
            }
        };

        if (temEssencia)
        {
            falas.Add(new FalaDialogo
            {
                personagem = "Iara",
                emocao = "reconhecendo",
                texto = "Esse tremor... e essa agua no teu peito. O Boto te enviou. Se trouxeste oferenda de respeito, o rio ainda pode ouvir."
            });
        }
        else
        {
            falas.Add(new FalaDialogo
            {
                personagem = "Iara",
                emocao = "desconfiada",
                texto = "O canto nao te leva. Ainda assim, peço licenca antiga: pente, espelho e fitas — nao comida."
            });
        }

        GerenciadorDialogo.Instancia?.Iniciar(falas.ToArray());
        CadernoCampo.Instancia?.Desbloquear(CatalogoPaginasCap1.IdIara);
    }

    IEnumerator LevarAoFundo()
    {
        puxandoAoFundo = true;
        float t = 0f;
        var canoa = CanoaRio.Instancia;
        Vector3 inicio = transform.position;
        Vector3 fundo = inicio + Vector3.down * 2.4f;
        while (t < 0.85f)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(inicio, fundo, t / 0.85f);
            yield return null;
        }

        if (canoa != null) canoa.AfogarPorCanto();
        else
        {
            var vida = FindFirstObjectByType<BentoVida>();
            if (vida != null) vida.MatarImediatamente();
        }

        transform.position = posicaoAgua;
        puxandoAoFundo = false;
        cantoAcumulado = 0f;
        avisoCantoFeito = false;
        descendoAAgua = true;
        Debug.Log("[Lendas] Iara levou o Bento ao fundo. Coma Tacaca ou Pato (Treme) e volte de canoa.");
    }

    void TentarComerAntidoto()
    {
        CatalogoItensNorte.Garantir();
        var inventario = InventarioBento.Instancia ?? FindFirstObjectByType<InventarioBento>();
        var buffs = SistemaBuffs.Instancia ?? FindFirstObjectByType<SistemaBuffs>();
        if (inventario == null || buffs == null) return;

        ItemDados prato = null;
        if (inventario.TemPorId(CatalogoItensNorte.IdTacaca))
            prato = CatalogoItensNorte.Tacaca;
        else if (inventario.TemPorId(CatalogoItensNorte.IdPatoNoTucupi))
            prato = CatalogoItensNorte.PatoNoTucupi;

        if (prato == null)
        {
            GerenciadorDialogo.Instancia?.Iniciar(new[]
            {
                new FalaDialogo
                {
                    personagem = "Bento",
                    emocao = "urgente",
                    texto = "Sem Tacaca nem Pato o canto me leva. Preciso voltar ao Fogao — o Treme e o antidoto."
                }
            });
            return;
        }

        if (buffs.ConsumirComida(prato))
            Debug.Log($"[Lendas] Comeu {prato.nomeExibicao} na canoa. Treme ativo.");
    }

    void TentarOferenda()
    {
        CatalogoItensNorte.Garantir();
        var inventario = InventarioBento.Instancia ?? FindFirstObjectByType<InventarioBento>();
        if (inventario == null) return;

        bool temFitas = inventario.TemPorId(CatalogoItensNorte.IdFitasColoridas);
        bool temEspelho = inventario.TemPorId(CatalogoItensNorte.IdEspelho);
        bool temPente = inventario.TemPorId(CatalogoItensNorte.IdPenteDeOsso);

        if (!temFitas || !temEspelho || !temPente)
        {
            string falta = TextoFalta(temFitas, temEspelho, temPente);
            GerenciadorDialogo.Instancia?.Iniciar(new[]
            {
                new FalaDialogo
                {
                    personagem = "Iara",
                    emocao = "aguardando",
                    texto = "O Treme me poupou o canto, mas a licenca ainda esta incompleta. " + falta
                },
                new FalaDialogo
                {
                    personagem = "Bento",
                    emocao = "respeitoso",
                    texto = "Pacto com o rio, nao suborno. Volto com o que a vila mandou: Nair, Lia e Seu Zeca."
                }
            });
            return;
        }

        inventario.RemoverPorId(CatalogoItensNorte.IdFitasColoridas, 1);
        inventario.RemoverPorId(CatalogoItensNorte.IdEspelho, 1);
        inventario.RemoverPorId(CatalogoItensNorte.IdPenteDeOsso, 1);
        inventario.Adicionar(CatalogoItensNorte.GotaDeQuartzo, 1);
        Purificada = true;
        prontaParaOferenda = false;
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
                texto = "Pente, espelho e fitas. Licenca aceita. A Gota de Quartzo abre o caminho ate o Curupira — e lembra: quem suja o rio responde a mim."
            },
            new FalaDialogo
            {
                personagem = "Bento",
                emocao = "respeitoso",
                texto = "Obrigado. O rio pode voltar a proteger quem pesca e quem se banha."
            }
        });

        Debug.Log("[Lendas] Iara acalmada. Gota de Quartzo no inventario.");
    }

    static string TextoFalta(bool temFitas, bool temEspelho, bool temPente)
    {
        var partes = new List<string>();
        if (!temPente) partes.Add("o pente de osso de Seu Zeca");
        if (!temEspelho) partes.Add("o espelho da Lia");
        if (!temFitas) partes.Add("as fitas da Dona Nair");
        return "Falta " + string.Join(", ", partes) + ".";
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

    void OnGUI()
    {
        if (Purificada) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto
            || HudCaderno.Aberto || HudPesquisa.Aberto) return;
        if (!CanoaRio.JogadorDentro || !CanoaNaZona()) return;

        var inventario = InventarioBento.Instancia;
        bool temTacaca = inventario != null && inventario.TemPorId(CatalogoItensNorte.IdTacaca);
        bool temPato = inventario != null && inventario.TemPorId(CatalogoItensNorte.IdPatoNoTucupi);

        if (!TemTreme())
        {
            float largura = 420f;
            float altura = 110f;
            float x = (Screen.width - largura) * 0.5f;
            float y = Screen.height - altura - 48f;
            GUI.Box(new Rect(x, y, largura, altura), "");
            var titulo = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold, wordWrap = true };
            titulo.normal.textColor = Color.white;
            GUI.Label(new Rect(x + 16, y + 10, largura - 32, 40),
                "O canto da Iara! Coma o antidoto (Treme) ou ela te leva ao fundo.", titulo);

            GUI.enabled = temTacaca;
            if (GUI.Button(new Rect(x + 16, y + 58, 150, 36), temTacaca ? "[E] Comer Tacaca" : "Sem Tacaca"))
                TentarComerAntidoto();
            GUI.enabled = temPato;
            if (GUI.Button(new Rect(x + 178, y + 58, 150, 36), temPato ? "Comer Pato" : "Sem Pato"))
            {
                var buffs = SistemaBuffs.Instancia ?? FindFirstObjectByType<SistemaBuffs>();
                if (buffs != null && CatalogoItensNorte.PatoNoTucupi != null)
                    buffs.ConsumirComida(CatalogoItensNorte.PatoNoTucupi);
            }
            GUI.enabled = true;
            return;
        }

        if (Camera.main == null) return;
        Vector3 tela = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.3f);
        if (tela.z < 0f) return;
        float gy = Screen.height - tela.y;

        string dica = !prontaParaOferenda
            ? "A Iara escuta..."
            : (inventario != null
                && inventario.TemPorId(CatalogoItensNorte.IdFitasColoridas)
                && inventario.TemPorId(CatalogoItensNorte.IdEspelho)
                && inventario.TemPorId(CatalogoItensNorte.IdPenteDeOsso)
                    ? "[E] Oferecer pente, espelho e fitas"
                    : "[E] Pedir licenca (oferenda incompleta)");

        GUI.Box(new Rect(tela.x - 170f, gy - 28f, 340f, 32f), dica);
    }
}
