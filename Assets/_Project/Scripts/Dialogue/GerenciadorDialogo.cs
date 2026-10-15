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
        float altura = 120f;
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

        GUI.Label(new Rect(x + 16, y + 10, largura - 32, 28), cabecalho, titulo);
        GUI.Label(new Rect(x + 16, y + 40, largura - 32, 50), falaAtual.texto, corpo);
        GUI.Label(new Rect(x + 16, y + 90, largura - 32, 20), "[Espaco / E / Clique] continuar", corpo);
    }
}
