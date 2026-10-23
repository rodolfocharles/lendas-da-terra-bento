using System.Collections.Generic;

/// <summary>
/// Paginas iniciais do Caderno no Capitulo 1.
/// </summary>
public static class CatalogoPaginasCap1
{
    public const string IdExpedicao = "expedicao_norte";
    public const string IdTacaca = "receita_tacaca";
    public const string IdBoto = "pista_boto";
    public const string IdBotoAcalmado = "boto_acalmado";
    public const string IdEssenciaAguas = "essencia_aguas";
    public const string IdIara = "pista_iara";
    public const string IdIaraPurificada = "iara_purificada";
    public const string IdCurupira = "ritual_curupira";
    public const string IdCurupiraPurificado = "curupira_purificado";
    public const string IdPassoInvertido = "passo_invertido";
    public const string IdSombras = "bestiario_sombra";
    public const string IdNovelo = "ritual_novelo";
    public const string IdDiarioAvoAmuleto = "diario_avo_amuleto";
    public const string IdDiarioAvoCurupira = "diario_avo_curupira";
    public const string IdRuinaMarajoara = "pista_ruina";

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
                "Jambu + tucupi + goma. Dona Nair entrega o primeiro. Bebe um contra o canto da Iara (Treme); guarda outro pra oferenda ao Boto.",
                "Dona Nair"),
            new PaginaCaderno(
                IdBoto,
                "Boto-Cor-de-Rosa",
                CategoriaPagina.Pista,
                "Entidade espiritual: a poluicao nao o contamina, mas a dor dos animais o deixa bravo e incontrolavel. Seu Zeca empresta a canoa. Tres flechas purificadoras acalmam; a oferenda pede Tacaca e o retrato de Lia.",
                "Lia e Seu Zeca"),
            new PaginaCaderno(
                IdBotoAcalmado,
                "Boto acalmado",
                CategoriaPagina.Pista,
                "O Tacaca e a foto de Lia devolveram o rosa ao Boto. O rio ainda esta doente, mas ele abriu o igarape e enviou a Essencia das Aguas ao amuleto.",
                "Bento"),
            new PaginaCaderno(
                IdEssenciaAguas,
                "Essencia das Aguas",
                CategoriaPagina.Ritual,
                "Energia espiritual do Boto no amuleto. Nao ocupa a bolsa. No selo do Curupira, combina com a Gota de Quartzo da Iara.",
                "Amuleto"),
            new PaginaCaderno(
                IdIara,
                "Iara",
                CategoriaPagina.Pista,
                "Mae d'Agua. Sem Treme, o canto puxa da canoa. Oferenda de licenca: pente de osso (Zeca), espelho (Lia) e fitas (Nair). A Gota de Quartzo abre o caminho ate o Curupira.",
                "Vila"),
            new PaginaCaderno(
                IdIaraPurificada,
                "Gota de Quartzo",
                CategoriaPagina.Ritual,
                "O Treme quebrou o canto. Pente, espelho e fitas pediram licenca. A Iara sentiu a Essencia das Aguas e entregou a Gota de Quartzo.",
                "Bento"),
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
                "Marca do avo: o novelo de cipo acalma o Curupira. Visao Espectral para achar os focos. Purificar Sombras enquanto a corrupcao desce. A Gota abre a arena; Gota + Essencia selam o ritual.",
                "Pista espectral"),
            new PaginaCaderno(
                IdCurupiraPurificado,
                "Curupira purificado",
                CategoriaPagina.Pista,
                "O novelo nos focos, as Sombras e o selo da Gota com a Essencia das Aguas devolveram a forma tradicional: menino, cabelo de fogo, pes invertidos.",
                "Bento"),
            new PaginaCaderno(
                IdPassoInvertido,
                "Passo Invertido",
                CategoriaPagina.Ritual,
                "Fragmento do Curupira no amuleto. Ainda nao usei o passo: a mata deve mostrar caminhos falsos e fazer a vegetacao crescer como plataforma.",
                "Amuleto"),
            new PaginaCaderno(
                IdNovelo,
                "Novelo Encantado",
                CategoriaPagina.Ritual,
                "2× cipo + resina na Bancada de Alquimia. Item ritual do chefe. Sem ele, o Curupira nao se deixa purificar.",
                "Bancada de Alquimia"),
            new PaginaCaderno(
                IdDiarioAvoAmuleto,
                "Diario do avo — o amuleto",
                CategoriaPagina.Diario,
                "O amuleto nao e arma: e um olho. Segure a visao ate o mundo de papel mostrar o que o Barão escondeu. As marcas azuis sao pistas que eu deixei pra voce.",
                "Diario do avo"),
            new PaginaCaderno(
                IdDiarioAvoCurupira,
                "Diario do avo — Curupira",
                CategoriaPagina.Ritual,
                "O Curupira nao cai por forca. Enrole o novelo de cipo encantado nos focos da corrupcao. Enquanto as Sombras vierem, a corrupcao nao desce. Purifique, nao extermine.",
                "Diario do avo"),
            new PaginaCaderno(
                IdRuinaMarajoara,
                "Ruina — pisos e alavanca",
                CategoriaPagina.Pista,
                "Pisos identicos: so o amuleto mostra o verdadeiro (verde) e o falso (vermelho). A alavanca do chicote abre a grade. O avo deixou a marca Marajoara no fim.",
                "Reliquia da ruina")
        };
    }
}
