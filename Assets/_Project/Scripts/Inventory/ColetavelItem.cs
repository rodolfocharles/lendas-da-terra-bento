using UnityEngine;

/// <summary>
/// Item no mundo que o Bento coleta ao encostar (ou com E).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ColetavelItem : MonoBehaviour
{
    public ItemDados item;
    [Min(1)]
    public int quantidade = 1;
    public bool exigirTecla;
    public KeyCode teclaColetar = KeyCode.E;
    public bool destruirAoColetar = true;

    private bool jogadorProximo;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Update()
    {
        if (!exigirTecla || !jogadorProximo) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto || HudCaderno.Aberto) return;
        if (Input.GetKeyDown(teclaColetar)) TentarColetar();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<BentoMovimento>() == null) return;
        jogadorProximo = true;
        if (!exigirTecla) TentarColetar();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<BentoMovimento>() != null) jogadorProximo = false;
    }

    void TentarColetar()
    {
        if (item == null) return;

        var inventario = InventarioBento.Instancia;
        if (inventario == null)
        {
            var bento = FindFirstObjectByType<BentoMovimento>();
            if (bento != null) inventario = bento.GetComponent<InventarioBento>();
        }

        if (inventario == null) return;
        if (!inventario.Adicionar(item, quantidade)) return;

        Debug.Log($"[Lendas] Coletou {quantidade}x {item.nomeExibicao}");
        if (destruirAoColetar) Destroy(gameObject);
    }

    void OnGUI()
    {
        if (!exigirTecla || !jogadorProximo) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto || HudCaderno.Aberto) return;

        Vector3 tela = Camera.main != null
            ? Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 0.8f)
            : Vector3.zero;
        if (tela.z < 0f) return;

        float y = Screen.height - tela.y;
        string nome = item != null ? item.nomeExibicao : "item";
        GUI.Label(new Rect(tela.x - 70f, y - 10f, 140f, 24f), $"[E] Pegar {nome}");
    }
}
