using System.Collections.Generic;
using UnityEngine;

public class GerenciadorDialogo : MonoBehaviour
{
    public static GerenciadorDialogo Instancia { get; private set; }

    public bool EmDialogo { get; private set; }

    private readonly Queue<FalaDialogo> fila = new Queue<FalaDialogo>();
    private FalaDialogo falaAtual;
    private BentoMovimento movimento;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
    }

    void Start()
    {
        movimento = FindFirstObjectByType<BentoMovimento>();
    }

    void Update()
    {
        if (!EmDialogo) return;

        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            if (movimento != null) movimento.ConsumirPuloDoQuadro();
            Avancar();
        }
    }

    public void Iniciar(IEnumerable<FalaDialogo> falas)
    {
        fila.Clear();
        foreach (var fala in falas)
        {
            if (fala != null && !string.IsNullOrWhiteSpace(fala.texto))
                fila.Enqueue(fala);
        }

        if (fila.Count == 0) return;

        EmDialogo = true;
        if (movimento != null) movimento.ControleAtivo = false;
        Avancar();
    }

    void Avancar()
    {
        if (fila.Count == 0)
        {
            Encerrar();
            return;
        }

        falaAtual = fila.Dequeue();
    }

    void Encerrar()
    {
        EmDialogo = false;
        falaAtual = null;
        if (movimento != null) movimento.ControleAtivo = true;
    }

    void OnGUI()
    {
        if (!EmDialogo || falaAtual == null) return;

        float largura = Mathf.Min(700f, Screen.width - 40f);
        float altura = 128f;
        float x = (Screen.width - largura) * 0.5f;
        float y = Screen.height - altura - 24f;

        GUI.Box(new Rect(x, y, largura, altura), "");

        var titulo = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold };
        var corpo = new GUIStyle(GUI.skin.label) { fontSize = 16, wordWrap = true };
        titulo.normal.textColor = Color.white;
        corpo.normal.textColor = Color.white;

        string cabecalho = string.IsNullOrEmpty(falaAtual.emocao)
            ? falaAtual.personagem
            : $"{falaAtual.personagem} ({falaAtual.emocao})";

        var retrato = ResolverRetrato(falaAtual);
        float recuo = 16f;
        if (retrato != null)
        {
            float lado = 88f;
            GUI.DrawTexture(new Rect(x + 12f, y + 16f, lado, lado), retrato, ScaleMode.ScaleToFit, true);
            recuo = 16f + lado + 8f;
        }

        GUI.Label(new Rect(x + recuo, y + 10, largura - recuo - 16f, 28), cabecalho, titulo);
        GUI.Label(new Rect(x + recuo, y + 40, largura - recuo - 16f, 50), falaAtual.texto, corpo);
        GUI.Label(new Rect(x + recuo, y + 90, largura - recuo - 16f, 20), "[Espaco / E / Clique] continuar", corpo);
    }

    static Texture2D ResolverRetrato(FalaDialogo fala)
    {
        if (fala == null) return null;

        string quem = fala.personagem ?? "";
        if (quem.IndexOf("Bento", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return CarregadorPixelArt.CarregarTextura("Bento/Retratos/" + RetratoBentoPorEmocao(fala.emocao) + ".png");
        if (quem.IndexOf("Nair", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return CarregadorPixelArt.CarregarTextura("DonaNair/Nair_south.png");
        if (quem.IndexOf("Lia", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return CarregadorPixelArt.CarregarTextura("Lia/Lia_south.png");
        if (quem.IndexOf("Zeca", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return CarregadorPixelArt.CarregarTextura("SeuZeca/Zeca_south.png");
        if (quem.IndexOf("Taina", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return CarregadorPixelArt.CarregarTextura("Taina/Taina_south.png");

        return null;
    }

    static string RetratoBentoPorEmocao(string emocao)
    {
        string e = (emocao ?? "").ToLowerInvariant();
        if (e.Contains("feliz") || e.Contains("alivi") || e.Contains("carinho"))
            return "Bento_Retrato_Feliz";
        if (e.Contains("surpres") || e.Contains("alerta") || e.Contains("urgent") || e.Contains("aviso"))
            return "Bento_Retrato_Surpreso";
        if (e.Contains("carisma") || e.Contains("confident"))
            return "Bento_Retrato_Carisma";
        return "Bento_Retrato_Neutro";
    }
}
