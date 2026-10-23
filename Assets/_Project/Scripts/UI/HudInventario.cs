using UnityEngine;

/// <summary>
/// Painel OnGUI do inventario (prototipo). Tecla I abre/fecha.
/// Comida consumivel: botao Comer aplica buff (B14).
/// </summary>
public class HudInventario : MonoBehaviour
{
    public static bool Aberto { get; private set; }

    public KeyCode teclaAbrir = KeyCode.I;

    private InventarioBento inventario;
    private SistemaBuffs buffs;
    private BentoMovimento movimento;
    private Vector2 scroll;
    private string mensagem;
    private float mensagemAte;
    private ItemDados comidaPendente;

    void Start()
    {
        inventario = InventarioBento.Instancia ?? FindFirstObjectByType<InventarioBento>();
        buffs = SistemaBuffs.Instancia ?? FindFirstObjectByType<SistemaBuffs>();
        movimento = FindFirstObjectByType<BentoMovimento>();
    }

    void Update()
    {
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudFogao.Aberto || HudAlquimia.Aberto || HudCaderno.Aberto || HudPesquisa.Aberto) return;
        if (!Input.GetKeyDown(teclaAbrir)) return;

        Aberto = !Aberto;
        if (movimento != null) movimento.ControleAtivo = !Aberto;
        if (!Aberto)
        {
            mensagem = "";
            comidaPendente = null;
        }
    }

    void OnGUI()
    {
        var estiloHint = new GUIStyle(GUI.skin.label) { fontSize = 16 };
        estiloHint.normal.textColor = new Color(0.9f, 0.9f, 0.85f);
        GUI.Label(new Rect(16, 48, 320, 28), Aberto ? "[I] Fechar inventario" : "[I] Inventario", estiloHint);

        if (!Aberto) return;

        float largura = Mathf.Min(460f, Screen.width - 40f);
        float altura = Mathf.Min(400f, Screen.height - 80f);
        float x = (Screen.width - largura) * 0.5f;
        float y = (Screen.height - altura) * 0.5f;

        GUI.Box(new Rect(x, y, largura, altura), "");

        var titulo = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold };
        var corpo = new GUIStyle(GUI.skin.label) { fontSize = 16, wordWrap = true };
        var vazio = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Italic };
        var aviso = new GUIStyle(GUI.skin.label) { fontSize = 14, fontStyle = FontStyle.Italic };
        titulo.normal.textColor = Color.white;
        corpo.normal.textColor = Color.white;
        vazio.normal.textColor = new Color(0.75f, 0.75f, 0.7f);
        aviso.normal.textColor = new Color(1f, 0.9f, 0.5f);

        GUI.Label(new Rect(x + 16, y + 12, largura - 32, 28), "Bolsa de Campo", titulo);

        if (inventario == null)
            inventario = InventarioBento.Instancia ?? FindFirstObjectByType<InventarioBento>();
        if (buffs == null)
            buffs = SistemaBuffs.Instancia ?? FindFirstObjectByType<SistemaBuffs>();

        if (inventario == null || inventario.Entradas.Count == 0)
        {
            GUI.Label(new Rect(x + 16, y + 56, largura - 32, 60),
                "Bolsa vazia. Colete jambu, tucupi e goma na vila.", vazio);
            return;
        }

        float areaY = y + 48;
        float areaAltura = altura - 72;
        float alturaLinha = 64f;

        scroll = GUI.BeginScrollView(
            new Rect(x + 8, areaY, largura - 16, areaAltura),
            scroll,
            new Rect(0, 0, largura - 40, inventario.Entradas.Count * alturaLinha));

        // Copia defensiva: Remover pode alterar a lista enquanto desenhamos
        int total = inventario.Entradas.Count;
        for (int i = 0; i < total; i++)
        {
            if (i >= inventario.Entradas.Count) break;
            var entrada = inventario.Entradas[i];
            if (entrada?.item == null) continue;

            float linhaY = i * alturaLinha;
            string cat = entrada.item.categoria.ToString();
            GUI.Label(new Rect(8, linhaY, largura - 130, 24),
                $"{entrada.item.nomeExibicao}  x{entrada.quantidade}", corpo);
            GUI.Label(new Rect(8, linhaY + 22, largura - 130, 22),
                $"{cat} — {entrada.item.descricao}", vazio);

            if (CatalogoEfeitosComida.EhComidaConsumivel(entrada.item))
            {
                if (GUI.Button(new Rect(largura - 120, linhaY + 16, 72, 28), "Comer"))
                    PedirComer(entrada.item);
            }
        }

        GUI.EndScrollView();

        if (comidaPendente != null)
        {
            GUI.Box(new Rect(x + 12, y + altura - 78, largura - 24, 66f), "");
            GUI.Label(new Rect(x + 20, y + altura - 74, largura - 40, 36),
                "E o ultimo Tacaca da bolsa. Ele tambem e oferenda. Comer mesmo?", aviso);
            if (GUI.Button(new Rect(x + 20, y + altura - 38, 90, 24), "Comer"))
            {
                var item = comidaPendente;
                comidaPendente = null;
                TentarComer(item);
            }
            if (GUI.Button(new Rect(x + 118, y + altura - 38, 90, 24), "Guardar"))
                comidaPendente = null;
        }
        else if (!string.IsNullOrEmpty(mensagem) && Time.unscaledTime < mensagemAte)
            GUI.Label(new Rect(x + 16, y + altura - 28, largura - 32, 24), mensagem, aviso);
    }

    void PedirComer(ItemDados item)
    {
        if (item != null && item.id == CatalogoItensNorte.IdTacaca
            && inventario != null && inventario.QuantidadePorId(CatalogoItensNorte.IdTacaca) <= 1)
        {
            comidaPendente = item;
            return;
        }

        TentarComer(item);
    }

    void TentarComer(ItemDados item)
    {
        if (buffs == null)
            buffs = SistemaBuffs.Instancia ?? FindFirstObjectByType<SistemaBuffs>();
        if (buffs == null) return;

        if (buffs.ConsumirComida(item))
        {
            mensagem = $"Comeu {item.nomeExibicao} — buff {buffs.BuffAtual?.nomeExibicao} ativo!";
            mensagemAte = Time.unscaledTime + 2.5f;
        }
        else
        {
            mensagem = "Nao foi possivel comer.";
            mensagemAte = Time.unscaledTime + 2f;
        }
    }
}
