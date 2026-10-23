using UnityEngine;

/// <summary>
/// Cria em runtime os itens do Cap. 1 (prototipo / fallback).
/// </summary>
public static class CatalogoItensNorte
{
    public const string IdJambu = "jambu";
    public const string IdTucupi = "tucupi";
    public const string IdGoma = "goma_tapioca";
    public const string IdTacaca = "tacaca";
    public const string IdPatoNoTucupi = "pato_tucupi";
    public const string IdCipo = "cipo";
    public const string IdResina = "resina";
    public const string IdFlechasPurificadoras = "flechas_purificadoras";
    public const string IdNoveloEncantado = "novelo_encantado";
    public const string IdMarcaMarajoara = "marca_marajoara";
    public const string IdGotaDeQuartzo = "gota_quartzo";
    public const string IdChaveDaCanoa = "chave_canoa";
    public const string IdFotoDaLia = "foto_lia";
    public const string IdFitasColoridas = "fitas_coloridas";
    public const string IdEspelho = "espelho";
    public const string IdPenteDeOsso = "pente_osso";

    public static ItemDados Jambu { get; private set; }
    public static ItemDados Tucupi { get; private set; }
    public static ItemDados GomaDeTapioca { get; private set; }
    public static ItemDados Tacaca { get; private set; }
    public static ItemDados PatoNoTucupi { get; private set; }
    public static ItemDados Cipo { get; private set; }
    public static ItemDados Resina { get; private set; }
    public static ItemDados FlechasPurificadoras { get; private set; }
    public static ItemDados NoveloEncantado { get; private set; }
    public static ItemDados MarcaMarajoara { get; private set; }
    public static ItemDados GotaDeQuartzo { get; private set; }
    public static ItemDados ChaveDaCanoa { get; private set; }
    public static ItemDados FotoDaLia { get; private set; }
    public static ItemDados FitasColoridas { get; private set; }
    public static ItemDados Espelho { get; private set; }
    public static ItemDados PenteDeOsso { get; private set; }

    public static void Garantir()
    {
        if (Jambu == null)
            Jambu = Criar(IdJambu, "Jambu", "Folha anestesiante do Norte. Ingrediente do Tacaca.", CategoriaItem.Ingrediente);
        if (Tucupi == null)
            Tucupi = Criar(IdTucupi, "Tucupi", "Caldo amarelo da mandioca brava. Base do Tacaca e do Pato no Tucupi.", CategoriaItem.Ingrediente);
        if (GomaDeTapioca == null)
            GomaDeTapioca = Criar(IdGoma, "Goma de Tapioca", "Goma que da o ponto do Tacaca.", CategoriaItem.Ingrediente);
        if (Tacaca == null)
            Tacaca = Criar(IdTacaca, "Tacaca", "Oferenda ao Boto. Beber concede Treme contra o canto da Iara. Guarde um pra oferenda.", CategoriaItem.Comida);
        if (PatoNoTucupi == null)
            PatoNoTucupi = Criar(IdPatoNoTucupi, "Pato no Tucupi", "Prato tipico do Norte. Variacao do buff Treme.", CategoriaItem.Comida);
        if (Cipo == null)
            Cipo = Criar(IdCipo, "Cipo", "Fibra da mata. Base do Novelo Encantado.", CategoriaItem.Reagente);
        if (Resina == null)
            Resina = Criar(IdResina, "Resina", "Seiva endurecida. Usada em flechas e no novelo.", CategoriaItem.Reagente);
        if (FlechasPurificadoras == null)
            FlechasPurificadoras = Criar(IdFlechasPurificadoras, "Flechas Purificadoras", "Municao da Besta Espiritual contra Sombras.", CategoriaItem.Item);
        if (NoveloEncantado == null)
            NoveloEncantado = Criar(IdNoveloEncantado, "Novelo Encantado", "Novelo de cipo ritual. Fraqueza do Curupira.", CategoriaItem.Reliquia);
        if (MarcaMarajoara == null)
            MarcaMarajoara = Criar(IdMarcaMarajoara, "Marca Marajoara", "Placa de ceramica da ruina. Pisos falsos e alavancas: o avo ja passou por aqui.", CategoriaItem.Reliquia);
        if (GotaDeQuartzo == null)
            GotaDeQuartzo = Criar(IdGotaDeQuartzo, "Gota de Quartzo", "Lagrima purificada da Iara. Abre o caminho ate a arena do Curupira.", CategoriaItem.Reliquia);
        if (ChaveDaCanoa == null)
            ChaveDaCanoa = Criar(IdChaveDaCanoa, "Chave da Canoa", "Chave que Seu Zeca deixou para o caico no cais.", CategoriaItem.Item, 1);
        if (FotoDaLia == null)
            FotoDaLia = Criar(IdFotoDaLia, "Foto da Lia", "Retrato que Lia mandou ao Boto: para ele lembrar das noites no igarape.", CategoriaItem.Reliquia, 1);
        if (FitasColoridas == null)
            FitasColoridas = Criar(IdFitasColoridas, "Fitas Coloridas", "Oferenda de respeito a Iara. Dona Nair mandou para pedir licenca ao rio.", CategoriaItem.Reliquia, 1);
        if (Espelho == null)
            Espelho = Criar(IdEspelho, "Espelho", "Oferenda a Mae d'Agua. Lia mandou para ela admirar o reflexo nas pedras.", CategoriaItem.Reliquia, 1);
        if (PenteDeOsso == null)
            PenteDeOsso = Criar(IdPenteDeOsso, "Pente de Osso", "Oferenda de pescador. Seu Zeca pede licenca a Iara antes de remar trecho fundo.", CategoriaItem.Reliquia, 1);
    }

    public static ItemDados PorId(string id)
    {
        Garantir();
        if (id == IdJambu) return Jambu;
        if (id == IdTucupi) return Tucupi;
        if (id == IdGoma) return GomaDeTapioca;
        if (id == IdTacaca) return Tacaca;
        if (id == IdPatoNoTucupi) return PatoNoTucupi;
        if (id == IdCipo) return Cipo;
        if (id == IdResina) return Resina;
        if (id == IdFlechasPurificadoras) return FlechasPurificadoras;
        if (id == IdNoveloEncantado) return NoveloEncantado;
        if (id == IdMarcaMarajoara) return MarcaMarajoara;
        if (id == IdGotaDeQuartzo) return GotaDeQuartzo;
        if (id == IdChaveDaCanoa) return ChaveDaCanoa;
        if (id == IdFotoDaLia) return FotoDaLia;
        if (id == IdFitasColoridas) return FitasColoridas;
        if (id == IdEspelho) return Espelho;
        if (id == IdPenteDeOsso) return PenteDeOsso;
        return null;
    }

    static ItemDados Criar(string id, string nome, string descricao, CategoriaItem categoria, int empilhamentoMaximo = 99)
    {
        var item = ScriptableObject.CreateInstance<ItemDados>();
        item.id = id;
        item.nomeExibicao = nome;
        item.descricao = descricao;
        item.categoria = categoria;
        item.empilhamentoMaximo = Mathf.Max(1, empilhamentoMaximo);
        item.name = $"Item_{id}";
        return item;
    }
}
