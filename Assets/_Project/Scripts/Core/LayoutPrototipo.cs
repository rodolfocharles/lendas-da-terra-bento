using UnityEngine;

/// <summary>
/// Espalha o prototipo em faixas (Barca / Vila / Mata / Ruinas)
/// para nada ficar empilhado no mesmo pedaco de chao.
/// </summary>
public static class LayoutPrototipo
{
    public const float YChao = 0f;
    public const float XChaoInicio = -32f;
    public const float XChaoFim = 150f;

    public static readonly Vector3 Barca = new Vector3(-22f, 1.2f, 0f);
    public static readonly Vector3 Fogao = new Vector3(-23.8f, 2.25f, 0f);
    public static readonly Vector3 Alquimia = new Vector3(-22f, 2.25f, 0f);
    public static readonly Vector3 Pesquisa = new Vector3(-20.2f, 2.25f, 0f);

    public static readonly Vector3 Bento = new Vector3(-16.5f, 2f, 0f);
    public static readonly Vector3 Checkpoint = new Vector3(-16.5f, 0.85f, 0f);
    public static readonly Vector3 PistaEspectral = new Vector3(-8.5f, 2.6f, 0f);
    public static readonly Vector3 DonaNair = new Vector3(-10f, 1.4f, 0f);
    public static readonly Vector3 Jambu = new Vector3(-7.2f, 0.85f, 0f);
    public static readonly Vector3 Tucupi = new Vector3(-5.6f, 0.85f, 0f);
    public static readonly Vector3 Goma = new Vector3(-4f, 0.85f, 0f);
    public static readonly Vector3 Lia = new Vector3(6f, 1.4f, 0f);
    public static readonly Vector3 GanchoInicio = new Vector3(-2f, 3.7f, 0f);

    public static readonly Vector3 CheckpointRio = new Vector3(20f, 0.85f, 0f);
    public static readonly Vector3 Zeca = new Vector3(24f, 1.4f, 0f);
    public static readonly Vector3 CanoaCais = new Vector3(27.2f, 0.55f, 0f);
    public static readonly Vector3 BarreiraCais = new Vector3(29.0f, 2.6f, 0f);
    public static readonly Vector3 AguaRio = new Vector3(36f, -0.65f, 0f);
    public const float InicioZonaBoto = 31f;
    public const float LimiteCanoaBoto = 38.6f;
    public static readonly Vector3 BloqueioBoto = new Vector3(36f, 1.5f, 0f);
    public static readonly Vector3 BarreiraBoto = new Vector3(40.2f, 2.6f, 0f);
    public static readonly Vector3 CaisIara = new Vector3(42.4f, 0.55f, 0f);
    public const float InicioZonaIara = 41.5f;
    public const float LimiteCanoaIara = 45.8f;
    public static readonly Vector3 GanchoRio = new Vector3(38f, 3.6f, 0f);
    public static readonly Vector3 Iara = new Vector3(44.6f, 1.55f, 0f);
    public static readonly Vector3 IaraAgua = new Vector3(43.2f, 0.7f, 0f);
    public static readonly Vector3 CaisAposIara = new Vector3(47.2f, 0.55f, 0f);
    public static readonly Vector3 BarreiraIara = new Vector3(48.2f, 2.6f, 0f);
    public static readonly Vector3 JambuRio = new Vector3(38f, 0.85f, 0f);
    public static readonly Vector3 TucupiRio = new Vector3(39.4f, 0.85f, 0f);
    public static readonly Vector3 GomaRio = new Vector3(40.8f, 0.85f, 0f);

    public static readonly Vector3 Cipo = new Vector3(58f, 0.85f, 0f);
    public static readonly Vector3 Vegetacao = new Vector3(62f, 1.3f, 0f);
    public static readonly Vector3 Resina = new Vector3(66f, 0.85f, 0f);
    public static readonly Vector3 GanchoMata = new Vector3(70f, 3.6f, 0f);
    public static readonly Vector3 Capanga = new Vector3(74f, 1.3f, 0f);
    public static readonly Vector3 Sombra = new Vector3(80f, 1.3f, 0f);
    public static readonly Vector3 PlataformaEspectral = new Vector3(84f, 2.5f, 0f);

    public static readonly Vector3 Alavanca = new Vector3(92f, 1.1f, 0f);
    public static readonly Vector3 Barreira = new Vector3(100f, 1.7f, 0f);
    public static readonly Vector3 GanchoRuina = new Vector3(102f, 3.8f, 0f);
    public static readonly Vector3 PlataformaEspectral2 = new Vector3(105f, 2.85f, 0f);
    public static readonly Vector3 PistaPisos = new Vector3(106.5f, 3.65f, 0f);
    public static readonly Vector3 PisoA = new Vector3(108.5f, 3.15f, 0f);
    public static readonly Vector3 PisoB = new Vector3(111.3f, 3.15f, 0f);
    public static readonly Vector3 PisoC = new Vector3(114.1f, 3.15f, 0f);
    public static readonly Vector3 PisoChegada = new Vector3(117.5f, 3.1f, 0f);
    public static readonly Vector3 Reliquia = new Vector3(117.5f, 3.9f, 0f);

    public static readonly Vector3 BarreiraArena = new Vector3(121.5f, 2.6f, 0f);
    public static readonly Vector3 CheckpointArena = new Vector3(124f, 0.85f, 0f);
    public static readonly Vector3 FocoA = new Vector3(128f, 1.1f, 0f);
    public static readonly Vector3 SombraArena = new Vector3(131f, 1.3f, 0f);
    public static readonly Vector3 FocoB = new Vector3(134f, 1.1f, 0f);
    public static readonly Vector3 Curupira = new Vector3(140f, 1.6f, 0f);
    public static readonly Vector3 FocoC = new Vector3(144f, 1.1f, 0f);

    public static readonly Vector3 ZonaVila = new Vector3(-8f, 2f, 0f);
    public static readonly Vector3 ZonaRio = new Vector3(34f, 2f, 0f);
    public static readonly Vector3 ZonaMata = new Vector3(70f, 2f, 0f);
    public static readonly Vector3 ZonaRuinas = new Vector3(104f, 2f, 0f);
    public static readonly Vector3 ZonaArena = new Vector3(136f, 2f, 0f);

    public static void Aplicar()
    {
        AlinharChao();
        AjustarLimiteDaFase();
        AjustarZonaMorte();
        AjustarBarcaEEstacoes();

        Posicionar("CheckpointInicial", Checkpoint);
        Posicionar("PistaEspectral", PistaEspectral);
        Posicionar("DonaNair", DonaNair);
        Posicionar("Coletavel_Jambu", Jambu);
        Posicionar("Coletavel_Tucupi", Tucupi);
        Posicionar("Coletavel_Goma", Goma);
        Posicionar("NamoradaDoBoto", Lia);
        Posicionar("PescadorDaIara", Zeca);

        Posicionar("Coletavel_Cipo", Cipo);
        Posicionar("Vegetacao_Cipo", Vegetacao);
        Posicionar("Coletavel_Resina", Resina);
        Posicionar("CapangaTreino", Capanga);
        Posicionar("SombraTreino", Sombra);
        Posicionar("PlataformaEspectral", PlataformaEspectral);

        Posicionar("AlavancaChicote", Alavanca);
        Posicionar("BarreiraAlavanca", Barreira);
        Posicionar("PlataformaEspectral_2", PlataformaEspectral2);
        Posicionar("PistaPisosFalsos", PistaPisos);
        Posicionar("PisoFalso_A", PisoA);
        Posicionar("PisoFalso_B", PisoB);
        Posicionar("PisoFalso_C", PisoC);
        Posicionar("PisoChegadaRuina", PisoChegada);
        Posicionar("ReliquiaMarajoara", Reliquia);

        Posicionar("CheckpointRio", CheckpointRio);
        Posicionar("AguaRio", AguaRio);
        if (!CanoaRio.JogadorDentro)
            Posicionar("CanoaRio", CanoaCais);
        Posicionar("BarreiraCais", BarreiraCais);
        var muroCais = GameObject.Find("BarreiraCais");
        if (muroCais != null) muroCais.transform.localScale = new Vector3(0.8f, 5.2f, 1f);
        Posicionar("BloqueioBoto", BloqueioBoto);
        Posicionar("BarreiraRioBoto", BarreiraBoto);
        var muroBoto = GameObject.Find("BarreiraRioBoto");
        if (muroBoto != null) muroBoto.transform.localScale = new Vector3(0.8f, 5.2f, 1f);
        Posicionar("EncontroIara", Iara);
        Posicionar("BarreiraRioIara", BarreiraIara);
        var muroIara = GameObject.Find("BarreiraRioIara");
        if (muroIara != null) muroIara.transform.localScale = new Vector3(0.8f, 5.2f, 1f);
        Posicionar("Coletavel_Jambu_Rio", JambuRio);
        Posicionar("Coletavel_Tucupi_Rio", TucupiRio);
        Posicionar("Coletavel_Goma_Rio", GomaRio);
        Posicionar("CheckpointArena", CheckpointArena);
        Posicionar("BarreiraArenaCurupira", BarreiraArena);
        var muroArena = GameObject.Find("BarreiraArenaCurupira");
        if (muroArena != null) muroArena.transform.localScale = new Vector3(0.8f, 5.2f, 1f);
        Posicionar("FocoCorrupcao_A", FocoA);
        Posicionar("SombraArena", SombraArena);
        Posicionar("FocoCorrupcao_B", FocoB);
        if (!EncontroCurupira.Purificado)
            Posicionar("EncontroCurupira", Curupira);
        Posicionar("FocoCorrupcao_C", FocoC);

        Posicionar("Zona_Vila", ZonaVila);
        Posicionar("Zona_Rio", ZonaRio);
        Posicionar("Zona_Mata", ZonaMata);
        Posicionar("Zona_Ruinas", ZonaRuinas);
        Posicionar("Zona_Arena", ZonaArena);
        Posicionar("FaixaVila", new Vector3(ZonaVila.x, 0.12f, 0f));
        Posicionar("FaixaRio", new Vector3(ZonaRio.x, 0.12f, 0f));
        Posicionar("FaixaMata", new Vector3(ZonaMata.x, 0.12f, 0f));
        Posicionar("FaixaRuinas", new Vector3(ZonaRuinas.x, 0.12f, 0f));
        Posicionar("FaixaArena", new Vector3(ZonaArena.x, 0.12f, 0f));

        GarantirGancho("PontoGancho", GanchoInicio);
        GarantirGancho("PontoGancho_Rio", GanchoRio);
        GarantirGancho("PontoGancho_Mata", GanchoMata);
        GarantirGancho("PontoGancho_Ruina", GanchoRuina);

        var bento = Object.FindFirstObjectByType<BentoMovimento>();
        if (bento != null) AplicarPosicao(bento.gameObject, Bento);

        var vida = Object.FindFirstObjectByType<BentoVida>();
        if (vida != null) vida.DefinirCheckpoint(Checkpoint);
    }

    static void AlinharChao()
    {
        GameObject modelo = GameObject.Find("Chao");
        if (modelo == null)
        {
            foreach (var tile in GameObject.FindGameObjectsWithTag("Chao"))
            {
                if (EhPisoDePuzzle(tile)) continue;
                modelo = tile;
                break;
            }
        }

        var principal = GameObject.Find("ChaoContinuo");
        if (principal == null && modelo != null)
        {
            principal = Object.Instantiate(modelo);
            principal.name = "ChaoContinuo";
        }

        if (principal == null) return;

        principal.SetActive(true);
        principal.tag = "Chao";
        int camada = LayerMask.NameToLayer("Chao");
        if (camada >= 0) principal.layer = camada;

        float largura = XChaoFim - XChaoInicio;
        float centro = (XChaoInicio + XChaoFim) * 0.5f;
        principal.transform.SetPositionAndRotation(new Vector3(centro, YChao, 0f), Quaternion.identity);
        principal.transform.localScale = new Vector3(largura, 1f, 1f);

        for (int i = 0; i <= 8; i++)
        {
            string nome = i == 0 ? "Chao" : "Chao (" + i + ")";
            var tile = GameObject.Find(nome);
            if (tile != null && tile != principal)
                tile.SetActive(false);
        }

        foreach (var tile in GameObject.FindGameObjectsWithTag("Chao"))
        {
            if (tile == principal || EhPisoDePuzzle(tile)) continue;
            tile.SetActive(false);
        }
    }

    static bool EhPisoDePuzzle(GameObject tile)
    {
        return tile.GetComponent<PisoFalso>() != null
            || tile.GetComponent<ObjetoEspectral>() != null
            || tile.GetComponent<BarreiraPuzzle>() != null;
    }

    static void AjustarLimiteDaFase()
    {
        var limite = GameObject.Find("LimiteDaFase");
        if (limite == null) return;

        float centroX = (XChaoInicio + XChaoFim) * 0.5f;
        float largura = (XChaoFim - XChaoInicio) + 16f;
        float altura = 28f;

        var box = limite.GetComponent<BoxCollider2D>();
        if (box != null)
        {
            limite.transform.position = new Vector3(centroX, 6f, 0f);
            box.offset = Vector2.zero;
            box.size = new Vector2(largura, altura);
        }

        var poly = limite.GetComponent<PolygonCollider2D>();
        if (poly != null)
        {
            float meiaL = largura * 0.5f;
            float meiaA = altura * 0.5f;
            poly.pathCount = 1;
            poly.SetPath(0, new[]
            {
                new Vector2(-meiaL, -meiaA),
                new Vector2(-meiaL, meiaA),
                new Vector2(meiaL, meiaA),
                new Vector2(meiaL, -meiaA)
            });
        }
    }

    static void AjustarZonaMorte()
    {
        var zona = Object.FindFirstObjectByType<ZonaMorte>();
        if (zona == null) return;

        float centroX = (XChaoInicio + XChaoFim) * 0.5f;
        zona.transform.position = new Vector3(centroX, -12f, 0f);
        var col = zona.GetComponent<BoxCollider2D>();
        if (col != null) col.size = new Vector2(XChaoFim - XChaoInicio + 40f, 4f);
    }

    static void AjustarBarcaEEstacoes()
    {
        var barca = Object.FindFirstObjectByType<BarcaRio>();
        if (barca == null) return;

        SoltarSeFilho("FogaoALenha");
        SoltarSeFilho("BancadaAlquimia");
        SoltarSeFilho("MesaPesquisa");

        barca.transform.SetPositionAndRotation(Barca, Quaternion.identity);
        barca.transform.localScale = new Vector3(6.5f, 1.7f, 1f);

        if (barca.pontoInterior != null)
            barca.pontoInterior.localPosition = new Vector3(0f, 0.55f, 0f);

        if (barca.pontoSaida != null)
            barca.pontoSaida.localPosition = new Vector3(0.7f, 0.12f, 0f);

        PosicionarLivre("FogaoALenha", Fogao, new Vector3(0.75f, 0.75f, 1f));
        PosicionarLivre("BancadaAlquimia", Alquimia, new Vector3(0.75f, 0.75f, 1f));
        PosicionarLivre("MesaPesquisa", Pesquisa, new Vector3(0.8f, 0.6f, 1f));
    }

    static void GarantirGancho(string nome, Vector3 posicao)
    {
        var go = GameObject.Find(nome);
        if (go == null)
        {
            var modelo = GameObject.Find("PontoGancho");
            if (modelo == null) return;
            go = Object.Instantiate(modelo);
            go.name = nome;
        }

        go.transform.position = posicao;
    }

    static void Posicionar(string nome, Vector3 posicao)
    {
        var go = GameObject.Find(nome);
        if (go != null) AplicarPosicao(go, posicao);
    }

    static void AplicarPosicao(GameObject go, Vector3 posicao)
    {
        var rb = go.GetComponent<Rigidbody2D>();
        if (rb != null) rb.position = posicao;
        go.transform.position = posicao;
    }

    static void SoltarSeFilho(string nome)
    {
        var go = GameObject.Find(nome);
        if (go != null && go.transform.parent != null)
            go.transform.SetParent(null, true);
    }

    static void PosicionarLivre(string nome, Vector3 posicao, Vector3 escala)
    {
        var go = GameObject.Find(nome);
        if (go == null) return;
        go.transform.SetParent(null, true);
        go.transform.position = posicao;
        go.transform.localScale = escala;
    }
}

/// <summary>
/// Garante o layout no Start, depois de todos os Awake da cena.
/// </summary>
public class AplicadorLayout : MonoBehaviour
{
    void Start()
    {
        LayoutPrototipo.Aplicar();
        Destroy(gameObject);
    }
}
