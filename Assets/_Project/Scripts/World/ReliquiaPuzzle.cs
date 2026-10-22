using UnityEngine;

/// <summary>
/// Reliquia no fim do puzzle: vai para o inventario e anota no Caderno.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ReliquiaPuzzle : MonoBehaviour
{
    public ItemDados item;
    public string idPaginaCaderno;
    public int quantidade = 1;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<BentoMovimento>() == null) return;

        var inventario = InventarioBento.Instancia ?? other.GetComponent<InventarioBento>();
        if (item != null && inventario != null)
            inventario.Adicionar(item, quantidade);

        if (!string.IsNullOrEmpty(idPaginaCaderno))
            CadernoCampo.Instancia?.Desbloquear(idPaginaCaderno);

        Debug.Log("[Lendas] Reliquia do puzzle coletada.");
        Destroy(gameObject);
    }
}
