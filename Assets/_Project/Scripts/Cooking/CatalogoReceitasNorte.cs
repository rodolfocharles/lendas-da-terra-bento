using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Receitas do Cap. 1 no Fogao a Lenha.
/// </summary>
public static class CatalogoReceitasNorte
{
    public const string IdTacaca = "tacaca";
    public const string IdPatoNoTucupi = "pato_tucupi";

    static readonly List<ReceitaDados> receitas = new List<ReceitaDados>();

    public static IReadOnlyList<ReceitaDados> Receitas
    {
        get
        {
            Garantir();
            return receitas;
        }
    }

    public static void Resetar()
    {
        receitas.Clear();
    }

    public static void Garantir()
    {
        CatalogoItensNorte.Garantir();
        if (CatalogoValido()) return;
        receitas.Clear();

        receitas.Add(new ReceitaDados(
            IdTacaca,
            "Tacaca",
            "Oferenda ao Boto. Buff Treme contra o canto da Iara.",
            CatalogoItensNorte.Tacaca,
            new RequisitoIngrediente(CatalogoItensNorte.IdTucupi),
            new RequisitoIngrediente(CatalogoItensNorte.IdGoma),
            new RequisitoIngrediente(CatalogoItensNorte.IdJambu),
            new RequisitoIngrediente(CatalogoItensNorte.IdCamaraoSeco)));

        receitas.Add(new ReceitaDados(
            IdPatoNoTucupi,
            "Pato no Tucupi",
            "Prato favorito da Taina na entrada da Mata (escambo). Tambem concede Treme.",
            CatalogoItensNorte.PatoNoTucupi,
            new RequisitoIngrediente(CatalogoItensNorte.IdPatoAssado),
            new RequisitoIngrediente(CatalogoItensNorte.IdTucupi, 2),
            new RequisitoIngrediente(CatalogoItensNorte.IdJambu)));
    }

    static bool CatalogoValido()
    {
        if (receitas.Count == 0) return false;
        for (int i = 0; i < receitas.Count; i++)
        {
            if (receitas[i] == null || receitas[i].resultado == null) return false;
        }
        return true;
    }
}
