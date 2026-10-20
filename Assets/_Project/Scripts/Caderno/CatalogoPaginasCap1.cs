using System.Collections.Generic;

/// <summary>
/// Paginas iniciais do Caderno no Capitulo 1.
/// </summary>
public static class CatalogoPaginasCap1
{
    public const string IdExpedicao = "expedicao_norte";
    public const string IdTacaca = "receita_tacaca";
    public const string IdBoto = "pista_boto";
    public const string IdIara = "pista_iara";
    public const string IdCurupira = "ritual_curupira";
    public const string IdSombras = "bestiario_sombra";
    public const string IdNovelo = "ritual_novelo";

    public static List<PaginaCaderno> Criar()
    {
        return new List<PaginaCaderno>
        {
            new PaginaCaderno(
                IdExpedicao,
                "Expedicao — Amazonia",
                CategoriaPagina.Diario,
                "Cheguei de barca. A vila esta acinzentada, a agua poluida, pesca e banho proibidos. Dona Nair me espera. O amuleto do avo pulsa no peito.",
                "Bento",
                true),
            new PaginaCaderno(
                IdTacaca,
                "Receita: Tacaca",
                CategoriaPagina.Receita,
                "Jambu + tucupi + goma. Oferenda ao Boto e a Iara. Buff Treme: resist. paralisia/veneno; aumenta o dano espiritual da besta.",
                "Dona Nair"),
            new PaginaCaderno(
                IdBoto,
                "Boto-Cor-de-Rosa",
                CategoriaPagina.Pista,
                "Lia espera o 'rapaz bonito' das noites. Os mais velhos dizem que e o Boto. O rio doente o corrompeu. Tacaca como oferenda pode acalma-lo.",
                "Lia"),
            new PaginaCaderno(
                IdIara,
                "Iara",
                CategoriaPagina.Pista,
                "Protegia pescadores; agora, corrompida, arrasta gente ao fundo. Cuidado com o canto. A Gota de Quartzo dela abre o caminho ate o Curupira.",
                "Seu Zeca"),
            new PaginaCaderno(
                IdSombras,
                "Sombras da corrupcao",
                CategoriaPagina.Bestiario,
                "Espiritos roxos. Nao se mata: se purifica (corrupcao 100% → 0%). O facao quase nao afeta. Use flechas purificadoras da Besta.",
                "Observacao de campo"),
            new PaginaCaderno(
                IdCurupira,
                "Curupira — fraqueza",
                CategoriaPagina.Ritual,
                "Marca do avo: o novelo de cipo acalma o Curupira. Visao Espectral para achar os focos. Purificar Sombras enquanto a corrupcao desce.",
                "Pista espectral"),
            new PaginaCaderno(
                IdNovelo,
                "Novelo Encantado",
                CategoriaPagina.Ritual,
                "2× cipo + resina na Bancada de Alquimia. Item ritual do chefe. Sem ele, o Curupira nao se deixa purificar.",
                "Bancada de Alquimia")
        };
    }
}
