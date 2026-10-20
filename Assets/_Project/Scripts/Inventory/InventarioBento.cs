using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bolsa do Bento: itens, ingredientes e reagentes.
/// </summary>
public class InventarioBento : MonoBehaviour
{
    public static InventarioBento Instancia { get; private set; }

    [SerializeField] private List<EntradaInventario> entradas = new List<EntradaInventario>();

    public event Action InventarioAlterado;
    public IReadOnlyList<EntradaInventario> Entradas => entradas;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(this);
            return;
        }
        Instancia = this;
    }

    void OnDestroy()
    {
        if (Instancia == this) Instancia = null;
    }

    public int Quantidade(ItemDados item)
    {
        if (item == null) return 0;
        return QuantidadePorId(item.id);
    }

    public int QuantidadePorId(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return 0;
        for (int i = 0; i < entradas.Count; i++)
        {
            if (entradas[i].item != null && entradas[i].item.id == id)
                return entradas[i].quantidade;
        }
        return 0;
    }

    public bool Tem(ItemDados item, int quantidade = 1)
    {
        return Quantidade(item) >= quantidade;
    }

    public bool TemPorId(string id, int quantidade = 1)
    {
        return QuantidadePorId(id) >= quantidade;
    }

    public bool Adicionar(ItemDados item, int quantidade = 1)
    {
        if (item == null || !item.IdValido || quantidade <= 0) return false;

        int max = Mathf.Max(1, item.empilhamentoMaximo);
        var entrada = BuscarEntrada(item.id);

        if (entrada != null)
        {
            int cabe = max - entrada.quantidade;
            if (cabe <= 0) return false;
            entrada.quantidade += Mathf.Min(quantidade, cabe);
        }
        else
        {
            entradas.Add(new EntradaInventario(item, Mathf.Min(quantidade, max)));
        }

        InventarioAlterado?.Invoke();
        return true;
    }

    public bool Remover(ItemDados item, int quantidade = 1)
    {
        if (item == null) return false;
        return RemoverPorId(item.id, quantidade);
    }

    public bool RemoverPorId(string id, int quantidade = 1)
    {
        if (string.IsNullOrWhiteSpace(id) || quantidade <= 0) return false;

        var entrada = BuscarEntrada(id);
        if (entrada == null || entrada.quantidade < quantidade) return false;

        entrada.quantidade -= quantidade;
        if (entrada.quantidade <= 0) entradas.Remove(entrada);

        InventarioAlterado?.Invoke();
        return true;
    }

    public void Limpar()
    {
        if (entradas.Count == 0) return;
        entradas.Clear();
        InventarioAlterado?.Invoke();
    }

    EntradaInventario BuscarEntrada(string id)
    {
        for (int i = 0; i < entradas.Count; i++)
        {
            if (entradas[i].item != null && entradas[i].item.id == id)
                return entradas[i];
        }
        return null;
    }
}
