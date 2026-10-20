using UnityEngine;

/// <summary>
/// Base do Cap. 1: entrar/sair da Barca e liberar as estacoes.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class BarcaRio : MonoBehaviour
{
    public static BarcaRio Instancia { get; private set; }

    public KeyCode teclaEntrar = KeyCode.E;
    public Transform pontoInterior;
    public Transform pontoSaida;
    public bool JogadorDentro { get; private set; }

    private bool jogadorProximo;
    private BentoMovimento bento;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(gameObject);
            return;
        }
        Instancia = this;
    }

    void OnDestroy()
    {
        if (Instancia == this) Instancia = null;
    }

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Update()
    {
        // So sai/entra na passagem (trigger). Perto do Fogao, o E fica com a estacao.
        if (!jogadorProximo) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto || HudCaderno.Aberto) return;
        if (EstacaoTemPrioridade()) return;
        if (!Input.GetKeyDown(teclaEntrar)) return;

        if (JogadorDentro) Sair();
        else Entrar();
    }

    static bool EstacaoTemPrioridade()
    {
        var fogao = Object.FindFirstObjectByType<EstacaoFogao>();
        if (fogao != null && fogao.JogadorProximo) return true;
        var alquimia = Object.FindFirstObjectByType<EstacaoAlquimia>();
        return alquimia != null && alquimia.JogadorProximo;
    }

    void Entrar()
    {
        if (bento == null) return;
        JogadorDentro = true;

        if (pontoInterior != null)
            bento.transform.position = pontoInterior.position;
        else
            bento.transform.position = transform.position + Vector3.up * 0.5f;

        Debug.Log("[Lendas] Entrou na Barca. Fogao (vermelho) e Alquimia (roxo).");
    }

    void Sair()
    {
        if (bento == null) return;
        JogadorDentro = false;

        if (pontoSaida != null)
            bento.transform.position = pontoSaida.position;
        else
            bento.transform.position = transform.position + Vector3.right * 2.5f;

        Debug.Log("[Lendas] Saiu da Barca.");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var mov = other.GetComponent<BentoMovimento>();
        if (mov == null) return;
        bento = mov;
        jogadorProximo = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<BentoMovimento>() == null) return;
        jogadorProximo = false;
    }

    void OnGUI()
    {
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto || HudCaderno.Aberto) return;

        if (!jogadorProximo || EstacaoTemPrioridade()) return;
        string dica = JogadorDentro ? "[E] Sair da Barca" : "[E] Entrar na Barca";

        Vector3 alvo = transform.position + Vector3.up * 1.4f;

        Vector3 tela = Camera.main != null
            ? Camera.main.WorldToScreenPoint(alvo)
            : Vector3.zero;
        if (tela.z < 0f) return;

        float y = Screen.height - tela.y;
        GUI.Label(new Rect(tela.x - 70f, y - 10f, 160f, 24f), dica);
    }
}
