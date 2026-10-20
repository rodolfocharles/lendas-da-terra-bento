using System;

/// <summary>
/// Uma pilha de itens no inventario do Bento.
/// </summary>
[Serializable]
public class EntradaInventario
{
    public ItemDados item;
    public int quantidade;

    public EntradaInventario(ItemDados item, int quantidade)
    {
        this.item = item;
        this.quantidade = quantidade;
    }
}
