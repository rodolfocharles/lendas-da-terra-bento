using System.Collections.Generic;

/// <summary>
/// Efeitos dos pratos do Cap. 1 ao serem consumidos.
/// </summary>
public static class CatalogoEfeitosComida
{
    static readonly List<EfeitoComida> efeitos = new List<EfeitoComida>();

    public static void Garantir()
    {
        if (efeitos.Count > 0) return;

        // Tacaca: oferenda + Treme padrao
        efeitos.Add(new EfeitoComida(
            CatalogoItensNorte.IdTacaca,
            TipoBuff.Treme,
            "Treme",
            "Resist. paralisia/veneno. +25% dano espiritual da besta.",
            60f,
            1.25f,
            true));

        // Pato: mesma familia, um pouco mais longo
        efeitos.Add(new EfeitoComida(
            CatalogoItensNorte.IdPatoNoTucupi,
            TipoBuff.Treme,
            "Treme",
            "Resist. paralisia/veneno. +25% dano espiritual da besta.",
            90f,
            1.25f,
            true));
    }

    public static EfeitoComida BuscarPorItem(string idItem)
    {
        Garantir();
        if (string.IsNullOrWhiteSpace(idItem)) return null;
        for (int i = 0; i < efeitos.Count; i++)
        {
            if (efeitos[i].idItem == idItem) return efeitos[i];
        }
        return null;
    }

    public static bool EhComidaConsumivel(ItemDados item)
    {
        if (item == null || item.categoria != CategoriaItem.Comida) return false;
        return BuscarPorItem(item.id) != null;
    }
}
