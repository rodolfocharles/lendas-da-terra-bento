using UnityEngine;

/// <summary>
/// Diario do avo na Mesa de Pesquisa. Copiar anota no Caderno de Campo.
/// </summary>
public class HudPesquisa : MonoBehaviour
{
    public static bool Aberto { get; private set; }

    private static HudPesquisa instancia;
    private BentoMovimento movimento;
    private CadernoCampo caderno;
    private string mensagem = "";
    private float mensagemAte;

    static readonly string[] idsDiario =
    {
        CatalogoPaginasCap1.IdDiarioAvoAmuleto,
        CatalogoPaginasCap1.IdDiarioAvoCurupira
    };

    void Awake()
    {
        instancia = this;
    }

    void Start()
    {
        movimento = FindFirstObjectByType<BentoMovimento>();
        caderno = CadernoCampo.Instancia ?? FindFirstObjectByType<CadernoCampo>();
    }

    public static void Abrir()
    {
        if (instancia == null) return;
        Aberto = true;
        if (instancia.movimento != null) instancia.movimento.ControleAtivo = false;
        instancia.mensagem = "";
    }

    public static void Fechar()
    {
        if (instancia == null) return;
        Aberto = false;
        if (instancia.movimento != null) instancia.movimento.ControleAtivo = true;
    }

    void Update()
    {
        if (!Aberto) return;
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.Tab))
            Fechar();
    }

    void OnGUI()
    {
        if (!Aberto) return;

        if (caderno == null)
            caderno = CadernoCampo.Instancia ?? FindFirstObjectByType<CadernoCampo>();

        float largura = Mathf.Min(520f, Screen.width - 40f);
        float altura = Mathf.Min(420f, Screen.height - 60f);
        float x = (Screen.width - largura) * 0.5f;
        float y = (Screen.height - altura) * 0.5f;

        GUI.Box(new Rect(x, y, largura, altura), "");

        var titulo = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold };
        var corpo = new GUIStyle(GUI.skin.label) { fontSize = 15, wordWrap = true };
        var aviso = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Italic };
        titulo.normal.textColor = new Color(0.95f, 0.88f, 0.65f);
        corpo.normal.textColor = Color.white;
        aviso.normal.textColor = new Color(0.85f, 0.8f, 0.6f);

        GUI.Label(new Rect(x + 16, y + 12, largura - 32, 28), "Mesa de Pesquisa", titulo);
        GUI.Label(new Rect(x + 16, y + 40, largura - 32, 22), "Diario de campo do avo — [Esc] fechar", aviso);

        float linhaY = y + 70f;
        for (int i = 0; i < idsDiario.Length; i++)
        {
            var pagina = caderno != null ? caderno.Buscar(idsDiario[i]) : null;
            if (pagina == null) continue;

            GUI.Box(new Rect(x + 12, linhaY, largura - 24, 130f), "");
            GUI.Label(new Rect(x + 24, linhaY + 8, largura - 48, 24), pagina.titulo, titulo);
            GUI.Label(new Rect(x + 24, linhaY + 36, largura - 48, 52), pagina.texto, corpo);

            if (pagina.desbloqueada)
            {
                GUI.Label(new Rect(x + 24, linhaY + 96, 200, 24), "Ja esta no Caderno.", aviso);
            }
            else if (GUI.Button(new Rect(x + largura - 180, linhaY + 94, 150, 26), "Anotar no Caderno"))
            {
                Anotar(pagina);
            }

            linhaY += 140f;
        }

        if (!string.IsNullOrEmpty(mensagem) && Time.unscaledTime < mensagemAte)
            GUI.Label(new Rect(x + 16, y + altura - 36, largura - 32, 24), mensagem, aviso);

        if (GUI.Button(new Rect(x + largura - 100, y + 10, 80, 26), "Fechar"))
            Fechar();
    }

    void Anotar(PaginaCaderno pagina)
    {
        if (caderno == null || pagina == null) return;
        caderno.Desbloquear(pagina.id);

        if (pagina.id == CatalogoPaginasCap1.IdDiarioAvoCurupira)
        {
            caderno.Desbloquear(CatalogoPaginasCap1.IdCurupira);
            caderno.Desbloquear(CatalogoPaginasCap1.IdNovelo);
        }

        mensagem = $"Copiou '{pagina.titulo}' para o Caderno.";
        mensagemAte = Time.unscaledTime + 2.5f;
    }
}
