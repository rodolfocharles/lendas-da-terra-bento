using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Receitas da Bancada de Alquimia (Cap. 1).
/// </summary>
public static class CatalogoReceitasAlquimia
{
    public const string IdFlechas = "flechas_purificadoras";
    public const string IdNovelo = "novelo_encantado";

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
        if (CatalogoValido()) return;
        receitas.Clear();

        receitas.Add(new ReceitaDados(
            IdFlechas,
            "Flechas Purificadoras",
            "Municao espiritual para a Besta. Produz 3 flechas.",
            CatalogoItensNorte.FlechasPurificadoras,
            3,
            new RequisitoIngrediente(CatalogoItensNorte.IdResina),
            new RequisitoIngrediente(CatalogoItensNorte.IdCipo)));

        receitas.Add(new ReceitaDados(
            IdNovelo,
            "Novelo Encantado",
            "Ritual do Curupira: novelo de cipo com resina sagrada.",
            CatalogoItensNorte.NoveloEncantado,
            1,
            new RequisitoIngrediente(CatalogoItensNorte.IdCipo, 2),
            new RequisitoIngrediente(CatalogoItensNorte.IdResina)));
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
