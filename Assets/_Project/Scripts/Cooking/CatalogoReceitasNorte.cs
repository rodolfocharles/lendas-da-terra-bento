using System.Collections.Generic;

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

    public static void Garantir()
    {
        CatalogoItensNorte.Garantir();
        if (receitas.Count > 0) return;

        receitas.Add(new ReceitaDados(
            IdTacaca,
            "Tacaca",
            "Oferenda ao Boto e a Iara. Buff Treme: resist. paralisia/veneno.",
            CatalogoItensNorte.Tacaca,
            new RequisitoIngrediente(CatalogoItensNorte.IdJambu),
            new RequisitoIngrediente(CatalogoItensNorte.IdTucupi),
            new RequisitoIngrediente(CatalogoItensNorte.IdGoma)));

        receitas.Add(new ReceitaDados(
            IdPatoNoTucupi,
            "Pato no Tucupi",
            "Prato tipico do Norte. Variacao do buff Treme.",
            CatalogoItensNorte.PatoNoTucupi,
            new RequisitoIngrediente(CatalogoItensNorte.IdTucupi, 2),
            new RequisitoIngrediente(CatalogoItensNorte.IdJambu)));
    }
}
