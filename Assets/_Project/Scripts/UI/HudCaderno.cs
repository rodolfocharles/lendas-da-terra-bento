using UnityEngine;

/// <summary>
/// Caderno de Campo (prototipo OnGUI). Tab ou C abre/fecha — inclusive em combate.
/// </summary>
public class HudCaderno : MonoBehaviour
{
    public static bool Aberto { get; private set; }

    public KeyCode teclaAbrir = KeyCode.Tab;
    public KeyCode teclaAlternativa = KeyCode.C;

    private CadernoCampo caderno;
    private BentoMovimento movimento;
    private int indice;
    private bool controleAntes;

    void Start()
    {
        caderno = CadernoCampo.Instancia ?? FindFirstObjectByType<CadernoCampo>();
        movimento = FindFirstObjectByType<BentoMovimento>();
    }

    public static void Abrir()
    {
        var hud = FindFirstObjectByType<HudCaderno>();
        if (hud != null) hud.DefinirAberto(true);
    }

    void Update()
    {
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudFogao.Aberto || HudAlquimia.Aberto) return;
        if (HudInventario.Aberto) return;

        if (Input.GetKeyDown(teclaAbrir) || Input.GetKeyDown(teclaAlternativa))
            DefinirAberto(!Aberto);

        if (!Aberto) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DefinirAberto(false);
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            MudarPagina(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            MudarPagina(1);
    }

    void DefinirAberto(bool valor)
    {
        if (Aberto == valor) return;
        Aberto = valor;

        if (movimento == null) movimento = FindFirstObjectByType<BentoMovimento>();
        if (movimento != null)
        {
            if (Aberto)
            {
                controleAntes = movimento.ControleAtivo;
                movimento.ControleAtivo = false;
            }
            else
            {
                movimento.ControleAtivo = controleAntes;
            }
        }
    }

    void MudarPagina(int delta)
    {
        if (caderno == null || caderno.Paginas.Count == 0) return;
        indice = (indice + delta + caderno.Paginas.Count) % caderno.Paginas.Count;
    }

    void OnGUI()
    {
        var hint = new GUIStyle(GUI.skin.label) { fontSize = 16 };
        hint.normal.textColor = new Color(0.9f, 0.9f, 0.85f);
        GUI.Label(new Rect(16, Screen.height - 52, 420, 24),
            Aberto ? "[Tab/C] Fechar caderno" : "[Tab/C] Caderno de Campo", hint);

        if (!Aberto) return;

        if (caderno == null)
            caderno = CadernoCampo.Instancia ?? FindFirstObjectByType<CadernoCampo>();
        if (caderno == null || caderno.Paginas.Count == 0) return;

        indice = Mathf.Clamp(indice, 0, caderno.Paginas.Count - 1);
        var pagina = caderno.Paginas[indice];

        float largura = Mathf.Min(560f, Screen.width - 40f);
        float altura = Mathf.Min(380f, Screen.height - 80f);
        float x = (Screen.width - largura) * 0.5f;
        float y = (Screen.height - altura) * 0.5f;

        GUI.Box(new Rect(x, y, largura, altura), "");

        var titulo = new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold };
        var sub = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Italic };
        var corpo = new GUIStyle(GUI.skin.label) { fontSize = 16, wordWrap = true };
        var rodape = new GUIStyle(GUI.skin.label) { fontSize = 14 };
        titulo.normal.textColor = new Color(0.95f, 0.9f, 0.75f);
        sub.normal.textColor = new Color(0.75f, 0.7f, 0.55f);
        corpo.normal.textColor = Color.white;
        rodape.normal.textColor = new Color(0.8f, 0.8f, 0.75f);

        GUI.Label(new Rect(x + 20, y + 12, largura - 40, 28), "Caderno de Campo", titulo);

        string cat = pagina.categoria.ToString();
        GUI.Label(new Rect(x + 20, y + 44, largura - 40, 22),
            pagina.desbloqueada ? $"{cat}  ·  {pagina.fonte}" : $"{cat}  ·  rascunho", sub);

        GUI.Label(new Rect(x + 20, y + 74, largura - 40, 28),
            pagina.desbloqueada ? pagina.titulo : "Pagina em branco", titulo);

        string texto = pagina.desbloqueada
            ? pagina.texto
            : "Ainda nao anotei nada aqui. Converse, explore com a Visao Espectral ou prepare itens na Barca.";
        GUI.Label(new Rect(x + 20, y + 112, largura - 40, 180), texto, corpo);

        GUI.Label(new Rect(x + 20, y + altura - 48, largura - 40, 22),
            $"{indice + 1}/{caderno.Paginas.Count}   ·   {caderno.ContarDesbloqueadas()} anotadas   ·   [A/D] virar pagina", rodape);

        if (GUI.Button(new Rect(x + 20, y + altura - 28, 80, 22), "< Ant"))
            MudarPagina(-1);
        if (GUI.Button(new Rect(x + largura - 100, y + altura - 28, 80, 22), "Prox >"))
            MudarPagina(1);
    }
}
