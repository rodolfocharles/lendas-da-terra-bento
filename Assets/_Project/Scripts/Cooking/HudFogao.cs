using UnityEngine;

/// <summary>
/// Painel OnGUI do Fogao a Lenha (prototipo).
/// </summary>
public class HudFogao : MonoBehaviour
{
    public static bool Aberto { get; private set; }

    private static HudFogao instancia;
    private BentoMovimento movimento;
    private InventarioBento inventario;
    private string mensagem = "";
    private float mensagemAte;

    void Awake()
    {
        instancia = this;
    }

    void Start()
    {
        CatalogoReceitasNorte.Garantir();
        movimento = FindFirstObjectByType<BentoMovimento>();
        inventario = InventarioBento.Instancia ?? FindFirstObjectByType<InventarioBento>();
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
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.I))
            Fechar();
    }

    void OnGUI()
    {
        if (!Aberto) return;

        if (inventario == null)
            inventario = InventarioBento.Instancia ?? FindFirstObjectByType<InventarioBento>();

        float largura = Mathf.Min(480f, Screen.width - 40f);
        float altura = Mathf.Min(420f, Screen.height - 60f);
        float x = (Screen.width - largura) * 0.5f;
        float y = (Screen.height - altura) * 0.5f;

        GUI.Box(new Rect(x, y, largura, altura), "");

        var titulo = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold };
        var corpo = new GUIStyle(GUI.skin.label) { fontSize = 15, wordWrap = true };
        var aviso = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Italic };
        titulo.normal.textColor = Color.white;
        corpo.normal.textColor = Color.white;
        aviso.normal.textColor = new Color(1f, 0.9f, 0.5f);

        GUI.Label(new Rect(x + 16, y + 12, largura - 32, 28), "Fogao a Lenha", titulo);
        GUI.Label(new Rect(x + 16, y + 40, largura - 32, 22), "Receitas do Norte — [Esc] fechar", aviso);

        var receitas = CatalogoReceitasNorte.Receitas;
        float linhaY = y + 70f;

        for (int i = 0; i < receitas.Count; i++)
        {
            var receita = receitas[i];
            bool pode = PodePreparar(receita);

            GUI.Box(new Rect(x + 12, linhaY, largura - 24, 110f), "");
            GUI.Label(new Rect(x + 24, linhaY + 8, largura - 48, 24), receita.nomeExibicao, titulo);
            GUI.Label(new Rect(x + 24, linhaY + 34, largura - 48, 36),
                $"{receita.descricao}\n{TextoIngredientes(receita)}", corpo);

            GUI.enabled = pode;
            if (GUI.Button(new Rect(x + largura - 140, linhaY + 72, 110, 28), pode ? "Preparar" : "Falta item"))
            {
                if (TentarPreparar(receita))
                {
                    mensagem = $"Preparou {receita.nomeExibicao}!";
                    mensagemAte = Time.unscaledTime + 2.5f;
                }
            }
            GUI.enabled = true;

            linhaY += 120f;
        }

        if (!string.IsNullOrEmpty(mensagem) && Time.unscaledTime < mensagemAte)
            GUI.Label(new Rect(x + 16, y + altura - 36, largura - 32, 24), mensagem, aviso);

        if (GUI.Button(new Rect(x + largura - 100, y + 10, 80, 26), "Fechar"))
            Fechar();
    }

    bool PodePreparar(ReceitaDados receita)
    {
        CatalogoReceitasNorte.Garantir();
        if (inventario == null || receita?.ingredientes == null) return false;
        if (receita.resultado == null)
            receita.resultado = CatalogoItensNorte.PorId(receita.id);
        if (receita.resultado == null) return false;
        for (int i = 0; i < receita.ingredientes.Length; i++)
        {
            var req = receita.ingredientes[i];
            if (!inventario.TemPorId(req.idItem, req.quantidade)) return false;
        }
        return true;
    }

    bool TentarPreparar(ReceitaDados receita)
    {
        if (!PodePreparar(receita)) return false;

        for (int i = 0; i < receita.ingredientes.Length; i++)
        {
            var req = receita.ingredientes[i];
            if (!inventario.RemoverPorId(req.idItem, req.quantidade)) return false;
        }

        inventario.Adicionar(receita.resultado, receita.quantidadeResultado);
        if (receita.id == CatalogoReceitasNorte.IdTacaca)
            CadernoCampo.Instancia?.Desbloquear(CatalogoPaginasCap1.IdTacaca);
        Debug.Log($"[Lendas] Fogao: preparou {receita.nomeExibicao}");
        return true;
    }

    string TextoIngredientes(ReceitaDados receita)
    {
        if (receita.ingredientes == null || receita.ingredientes.Length == 0) return "";

        var partes = new string[receita.ingredientes.Length];
        for (int i = 0; i < receita.ingredientes.Length; i++)
        {
            var req = receita.ingredientes[i];
            int tem = inventario != null ? inventario.QuantidadePorId(req.idItem) : 0;
            string nome = NomeAmigavel(req.idItem);
            partes[i] = $"{nome} {tem}/{req.quantidade}";
        }
        return string.Join("  |  ", partes);
    }

    static string NomeAmigavel(string id)
    {
        if (id == CatalogoItensNorte.IdJambu) return "Jambu";
        if (id == CatalogoItensNorte.IdTucupi) return "Tucupi";
        if (id == CatalogoItensNorte.IdGoma) return "Goma";
        return id;
    }
}
