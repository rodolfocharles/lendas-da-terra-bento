using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NpcDialogo : MonoBehaviour
{
    public KeyCode teclaInteragir = KeyCode.E;
    public FalaDialogo[] falas;
    [Tooltip("Falas depois que o item de conversa ou escambo ja foi entregue.")]
    public FalaDialogo[] falasAposEntrega;
    [Tooltip("Paginas do Caderno desbloqueadas ao conversar.")]
    public string[] paginasAoConversar;
    [Tooltip("Itens entregues uma vez ao conversar (chave, foto, oferendas, pato assado).")]
    public string[] idsItensAoConversar;

    [Header("Escambo (opcional)")]
    [Tooltip("ID do item que o NPC pede em troca (ex.: pato_tucupi).")]
    public string idItemPedidoEscambo;
    [Tooltip("ID do item que o NPC entrega ao receber o item pedido (ex.: colar_sementes).")]
    public string idItemRecompensaEscambo;
    [Tooltip("Falas exibidas no momento em que o escambo acontece.")]
    public FalaDialogo[] falasAoEscambar;

    private bool jogadorProximo;
    private bool itemJaEntregue;
    private bool escamboConcluido;
    private SpriteRenderer sprite;
    private Transform bento;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (sprite == null) return;
        if (bento == null)
        {
            var movimento = FindFirstObjectByType<BentoMovimento>();
            if (movimento == null) return;
            bento = movimento.transform;
        }

        float dx = bento.position.x - transform.position.x;
        if (Mathf.Abs(dx) < 0.15f) return;
        sprite.flipX = dx < 0f;
    }

    void Update()
    {
        if (!jogadorProximo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto || HudCaderno.Aberto || HudPesquisa.Aberto) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (Input.GetKeyDown(teclaInteragir))
        {
            AnotarCaderno();

            if (TemEscamboConfigurado())
            {
                InteragirComEscambo();
                return;
            }

            bool jaTinhaItem = EntregarItemSeHouver();
            GerenciadorDialogo.Instancia?.Iniciar(EscolherFalas(jaTinhaItem));
        }
    }

    bool TemEscamboConfigurado()
    {
        return !string.IsNullOrWhiteSpace(idItemPedidoEscambo)
            && !string.IsNullOrWhiteSpace(idItemRecompensaEscambo);
    }

    void InteragirComEscambo()
    {
        CatalogoItensNorte.Garantir();
        var inventario = InventarioBento.Instancia ?? FindFirstObjectByType<InventarioBento>();

        if (escamboConcluido || (inventario != null && inventario.TemPorId(idItemRecompensaEscambo)))
        {
            escamboConcluido = true;
            GerenciadorDialogo.Instancia?.Iniciar(EscolherFalas(true));
            return;
        }

        if (inventario != null && inventario.TemPorId(idItemPedidoEscambo))
        {
            var recompensa = CatalogoItensNorte.PorId(idItemRecompensaEscambo);
            if (recompensa != null && inventario.RemoverPorId(idItemPedidoEscambo, 1))
            {
                inventario.Adicionar(recompensa, 1);
                escamboConcluido = true;
                Debug.Log($"[Lendas] Escambo concluido com {name}: entregou {idItemPedidoEscambo} -> recebeu {idItemRecompensaEscambo}.");

                var falasTroca = (falasAoEscambar != null && falasAoEscambar.Length > 0)
                    ? falasAoEscambar
                    : EscolherFalas(true);
                GerenciadorDialogo.Instancia?.Iniciar(falasTroca);
                return;
            }
        }

        GerenciadorDialogo.Instancia?.Iniciar(falas);
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
        if (!jogadorProximo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto || HudCaderno.Aberto || HudPesquisa.Aberto) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;

        Vector3 tela = Camera.main != null
            ? Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.2f)
            : Vector3.zero;
        if (tela.z < 0f) return;

        float y = Screen.height - tela.y;

        if (TemEscamboConfigurado() && !escamboConcluido)
        {
            var inventario = InventarioBento.Instancia;
            bool podeTrocar = inventario != null && inventario.TemPorId(idItemPedidoEscambo);
            if (podeTrocar)
            {
                var pedido = CatalogoItensNorte.PorId(idItemPedidoEscambo);
                string nomePedido = pedido != null ? pedido.nomeExibicao : idItemPedidoEscambo;
                GUI.Box(new Rect(tela.x - 135f, y - 16f, 270f, 28f), $"[E] Escambo: entregar {nomePedido}");
                return;
            }
        }

        GUI.Label(new Rect(tela.x - 60f, y - 10f, 120f, 24f), "[E] Conversar");
    }

    void AnotarCaderno()
    {
        if (paginasAoConversar == null || CadernoCampo.Instancia == null) return;
        for (int i = 0; i < paginasAoConversar.Length; i++)
            CadernoCampo.Instancia.Desbloquear(paginasAoConversar[i]);
    }

    bool EntregarItemSeHouver()
    {
        if (idsItensAoConversar == null || idsItensAoConversar.Length == 0) return false;
        if (itemJaEntregue) return true;

        CatalogoItensNorte.Garantir();
        var inventario = InventarioBento.Instancia ?? FindFirstObjectByType<InventarioBento>();
        if (inventario != null)
        {
            for (int i = 0; i < idsItensAoConversar.Length; i++)
            {
                var item = CatalogoItensNorte.PorId(idsItensAoConversar[i]);
                if (item != null) inventario.Adicionar(item, 1);
            }
        }

        itemJaEntregue = true;
        return false;
    }

    FalaDialogo[] EscolherFalas(bool jaTinhaItem)
    {
        if (jaTinhaItem && falasAposEntrega != null && falasAposEntrega.Length > 0)
            return falasAposEntrega;
        return falas;
    }
}
