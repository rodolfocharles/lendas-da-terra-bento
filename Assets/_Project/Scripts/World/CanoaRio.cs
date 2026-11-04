using UnityEngine;

/// <summary>
/// Caico de madeira no cais. Precisa da chave de Seu Zeca.
/// Bento navega com A/D; o Boto pode afundar a canoa em 3 impactos.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class CanoaRio : MonoBehaviour
{
    public static CanoaRio Instancia { get; private set; }
    public static bool JogadorDentro { get; private set; }

    public static bool CapturaTeclaE
    {
        get
        {
            if (Instancia == null) return false;
            if (!JogadorDentro) return Instancia.EstaProximo();
            return Instancia.PodeSair()
                && !EncontroBoto.CapturaTeclaE
                && !EncontroIara.CapturaTeclaE;
        }
    }

    public KeyCode teclaUsar = KeyCode.E;
    public float velocidade = 4.2f;
    public int impactosMaximos = 3;
    public Transform pontoAssento;
    public Transform pontoSaida;

    public int ImpactosRestantes { get; private set; }
    public bool Afundando { get; private set; }

    Vector3 posicaoCais;
    bool jogadorProximo;
    BentoMovimento bento;
    Rigidbody2D rbBento;
    Collider2D colBento;
    bool colBentoEraTrigger;
    RigidbodyType2D tipoOriginal;
    BentoVida vida;
    BentoBesta bestaBento;

    void Awake()
    {
        Instancia = this;
        JogadorDentro = false;
        Afundando = false;
        ImpactosRestantes = impactosMaximos;
        posicaoCais = transform.position;
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Start()
    {
        posicaoCais = LayoutPrototipo.CanoaCais;
        if (pontoAssento == null)
        {
            var assento = new GameObject("PontoAssentoCanoa");
            assento.transform.SetParent(transform);
            pontoAssento = assento.transform;
        }
        pontoAssento.localPosition = new Vector3(0f, 0.28f, 0f);

        if (pontoSaida == null)
        {
            var saida = new GameObject("PontoSaidaCanoa");
            saida.transform.SetParent(transform);
            saida.transform.localPosition = new Vector3(0f, 0.85f, 0f);
            pontoSaida = saida.transform;
        }

        vida = FindFirstObjectByType<BentoVida>();
        if (vida != null)
        {
            vida.Morreu += AoBentoMorrer;
            vida.Revivesceu += AoBentoReviver;
        }
    }

    void OnDestroy()
    {
        if (Instancia == this)
        {
            if (JogadorDentro) LiberarBento();
            Instancia = null;
            JogadorDentro = false;
        }

        if (vida != null)
        {
            vida.Morreu -= AoBentoMorrer;
            vida.Revivesceu -= AoBentoReviver;
        }
    }

    void Update()
    {
        if (Afundando) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto
            || HudCaderno.Aberto || HudPesquisa.Aberto) return;

        if (JogadorDentro)
        {
            if (PodeSair() && !EncontroBoto.CapturaTeclaE && !EncontroIara.CapturaTeclaE
                && Input.GetKeyDown(teclaUsar))
                Sair();
            return;
        }

        SincronizarCaisComBento();

        if (!EstaProximo()) return;
        if (!Input.GetKeyDown(teclaUsar)) return;
        TentarEntrar();
    }

    void FixedUpdate()
    {
        if (!JogadorDentro || Afundando || bento == null) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;

        float h = (bestaBento != null && bestaBento.Mirando) ? 0f : Input.GetAxisRaw("Horizontal");
        Vector3 p = transform.position;
        p.x += h * velocidade * Time.fixedDeltaTime;
        p.x = Mathf.Clamp(p.x, LimiteEsquerdo(), LimiteDireito());
        p.y = posicaoCais.y;
        transform.position = p;

        Vector3 assento = pontoAssento != null
            ? pontoAssento.position
            : transform.position + Vector3.up * 0.28f;
        bento.transform.position = assento;
        if (rbBento != null) rbBento.linearVelocity = Vector2.zero;
    }

    void TentarEntrar()
    {
        CatalogoItensNorte.Garantir();
        var inventario = InventarioBento.Instancia ?? FindFirstObjectByType<InventarioBento>();
        if (inventario == null || !inventario.TemPorId(CatalogoItensNorte.IdChaveDaCanoa))
        {
            GerenciadorDialogo.Instancia?.Iniciar(new[]
            {
                new FalaDialogo
                {
                    personagem = "Bento",
                    emocao = "observando",
                    texto = "O caico de Seu Zeca esta amarrado. A chave caiu na lama do cais — preciso falar com ele e usar a Visao Espectral [V] na margem."
                }
            });
            return;
        }

        Entrar();
    }

    void Entrar()
    {
        bento = FindFirstObjectByType<BentoMovimento>();
        if (bento == null) return;

        rbBento = bento.GetComponent<Rigidbody2D>();
        colBento = bento.GetComponent<Collider2D>();
        if (rbBento != null)
        {
            tipoOriginal = rbBento.bodyType;
            rbBento.linearVelocity = Vector2.zero;
            rbBento.bodyType = RigidbodyType2D.Kinematic;
        }

        if (colBento != null)
        {
            colBentoEraTrigger = colBento.isTrigger;
            colBento.isTrigger = true;
        }

        bestaBento = bento.GetComponent<BentoBesta>();
        JogadorDentro = true;
        Debug.Log("[Lendas] Bento entrou na canoa.");
    }

    void Sair()
    {
        if (bento == null) return;
        bool caisLeste = EncontroIara.Purificada
            && Mathf.Abs(transform.position.x - LayoutPrototipo.CaisAposIara.x) < 1.6f;

        Vector3 caisDestino = caisLeste ? LayoutPrototipo.CaisAposIara : LayoutPrototipo.CanoaCais;
        transform.position = caisDestino;

        Vector3 saida = caisLeste
            ? new Vector3(LayoutPrototipo.CaisAposIara.x + 0.6f, LayoutPrototipo.CaisAposIara.y + 0.85f, 0f)
            : new Vector3(LayoutPrototipo.CanoaCais.x - 0.6f, LayoutPrototipo.CanoaCais.y + 0.85f, 0f);

        LiberarBento();
        if (rbBento != null)
        {
            rbBento.linearVelocity = Vector2.zero;
            rbBento.position = saida;
        }
        bento.transform.position = saida;
        Debug.Log("[Lendas] Bento saiu da canoa.");
    }

    public void ReceberImpacto()
    {
        if (!JogadorDentro || Afundando || EncontroBoto.Acalmado) return;

        ImpactosRestantes = Mathf.Max(0, ImpactosRestantes - 1);
        Debug.Log($"[Lendas] Canoa atingida. Restam {ImpactosRestantes} impactos.");

        if (ImpactosRestantes <= 0)
            StartCoroutine(Afundar());
    }

    public void AfogarPorCanto()
    {
        if (!JogadorDentro || Afundando) return;
        StartCoroutine(Afundar());
    }

    public void RestaurarNoCais()
    {
        StopAllCoroutines();
        Afundando = false;
        ImpactosRestantes = impactosMaximos;
        if (JogadorDentro) LiberarBento();
        if (bento == null) bento = FindFirstObjectByType<BentoMovimento>();
        bool ladoLeste = bento != null
            && EncontroIara.Purificada
            && bento.transform.position.x >= LayoutPrototipo.BarreiraCaisLeste.x;
        transform.position = ladoLeste ? LayoutPrototipo.CaisAposIara : posicaoCais;
    }

    bool EstaProximo()
    {
        if (jogadorProximo) return true;
        if (bento == null) bento = FindFirstObjectByType<BentoMovimento>();
        if (bento == null) return false;
        return Vector2.Distance(transform.position, bento.transform.position) <= 2.2f;
    }

    void SincronizarCaisComBento()
    {
        if (JogadorDentro || Afundando) return;
        if (bento == null) bento = FindFirstObjectByType<BentoMovimento>();
        if (bento == null) return;

        float bx = bento.transform.position.x;

        // Impede travessia do rio a pe em qualquer sentido
        if (bx > LayoutPrototipo.BarreiraCais.x && bx < LayoutPrototipo.BarreiraCaisLeste.x)
        {
            float meioRio = (LayoutPrototipo.BarreiraCais.x + LayoutPrototipo.BarreiraCaisLeste.x) * 0.5f;
            bool devolverLeste = EncontroIara.Purificada && bx >= meioRio;
            Vector3 margem = devolverLeste
                ? new Vector3(LayoutPrototipo.CaisAposIara.x + 0.6f, LayoutPrototipo.CaisAposIara.y + 0.85f, 0f)
                : new Vector3(LayoutPrototipo.CanoaCais.x - 0.6f, LayoutPrototipo.CanoaCais.y + 0.85f, 0f);
            var rb = bento.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.position = margem;
            }
            bento.transform.position = margem;
            bx = margem.x;
        }

        if (EncontroIara.Purificada && bx >= LayoutPrototipo.BarreiraCaisLeste.x)
        {
            if (Mathf.Abs(transform.position.x - LayoutPrototipo.CaisAposIara.x) > 0.05f)
                transform.position = LayoutPrototipo.CaisAposIara;
        }
        else if (bx <= LayoutPrototipo.BarreiraCais.x)
        {
            if (Mathf.Abs(transform.position.x - LayoutPrototipo.CanoaCais.x) > 0.05f)
                transform.position = LayoutPrototipo.CanoaCais;
        }
    }

    System.Collections.IEnumerator Afundar()
    {
        Afundando = true;
        float t = 0f;
        Vector3 inicio = transform.position;
        Vector3 fundo = inicio + Vector3.down * 2.2f;
        while (t < 0.7f)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(inicio, fundo, t / 0.7f);
            if (bento != null)
                bento.transform.position = transform.position + Vector3.up * 0.2f;
            yield return null;
        }

        var alvo = vida != null ? vida : FindFirstObjectByType<BentoVida>();
        LiberarBento();
        if (alvo != null) alvo.MatarImediatamente();
        RestaurarNoCais();
    }

    void AoBentoMorrer()
    {
        if (JogadorDentro) LiberarBento();
    }

    void AoBentoReviver()
    {
        RestaurarNoCais();
    }

    void LiberarBento()
    {
        if (rbBento != null) rbBento.bodyType = tipoOriginal;
        if (colBento != null) colBento.isTrigger = colBentoEraTrigger;
        JogadorDentro = false;
        Afundando = false;
    }

    public bool PodeSair()
    {
        if (!JogadorDentro || Afundando) return false;
        if (EncontroIara.NaZonaDoCanto) return false;
        float x = transform.position.x;
        if (Mathf.Abs(x - LayoutPrototipo.CanoaCais.x) < 1.6f) return true;
        return EncontroIara.Purificada
            && Mathf.Abs(x - LayoutPrototipo.CaisAposIara.x) < 1.6f;
    }

    static float LimiteEsquerdo()
    {
        return LayoutPrototipo.CanoaCais.x;
    }

    static float LimiteDireito()
    {
        if (EncontroIara.Purificada) return LayoutPrototipo.CaisAposIara.x;
        if (EncontroBoto.CaminhoLiberado) return LayoutPrototipo.LimiteCanoaIara;
        return LayoutPrototipo.LimiteCanoaBoto;
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
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto
            || HudCaderno.Aberto || HudPesquisa.Aberto) return;
        if (Camera.main == null) return;

        if (JogadorDentro)
        {
            GUI.Box(new Rect(16f, 170f, 260f, 36f),
                EncontroBoto.Acalmado
                    ? "Canoa: A/D navegar"
                    : $"Canoa: {ImpactosRestantes}/{impactosMaximos}  |  A/D navegar");
            if (PodeSair() && !EncontroBoto.CapturaTeclaE && !EncontroIara.CapturaTeclaE)
            {
                Vector3 telaSair = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.4f);
                if (telaSair.z >= 0f)
                {
                    float ys = Screen.height - telaSair.y;
                    GUI.Box(new Rect(telaSair.x - 80f, ys - 28f, 160f, 28f), "[E] Sair da canoa");
                }
            }
            return;
        }

        if (!EstaProximo()) return;
        Vector3 tela = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.2f);
        if (tela.z < 0f) return;
        float y = Screen.height - tela.y;

        var inventario = InventarioBento.Instancia;
        bool temChave = inventario != null && inventario.TemPorId(CatalogoItensNorte.IdChaveDaCanoa);
        string dica = temChave
            ? "[E] Entrar na canoa"
            : "Canoa trancada. Fale com Seu Zeca e use [V] no cais.";
        GUI.Box(new Rect(tela.x - 155f, y - 28f, 310f, 32f), dica);
    }
}
