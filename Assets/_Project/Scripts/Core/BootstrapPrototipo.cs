using UnityEngine;

/// <summary>
/// Garante objetos do prototipo na cena ao entrar em Play,
/// mesmo sem rodar o menu Lendas/Configurar Prototipo.
/// </summary>
public static class BootstrapPrototipo
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AoCarregarCena()
    {
        if (!Application.isPlaying) return;

        GarantirBentoComponentes();
        GarantirComponente<HudVida>("HudVida");
        GarantirComponente<HudInventario>("HudInventario");
        GarantirComponente<HudFogao>("HudFogao");
        GarantirComponente<HudAlquimia>("HudAlquimia");
        GarantirComponente<HudBuffs>("HudBuffs");
        GarantirComponente<HudMunicao>("HudMunicao");
        GarantirComponente<HudEnergiaEspiritual>("HudEnergiaEspiritual");
        GarantirComponente<CadernoCampo>("CadernoCampo");
        GarantirComponente<HudCaderno>("HudCaderno");
        GarantirComponente<HudPesquisa>("HudPesquisa");
        GarantirComponente<GerenciadorDialogo>("GerenciadorDialogo");
        GarantirCheckpoint();
        GarantirZonaMorte();
        GarantirDonaNair();
        GarantirNamoradaDoBoto();
        GarantirPescadorDaIara();
        GarantirColetaveisNorte();
        GarantirBarcaEEstacoes();
        GarantirCombatePrototipo();
        GarantirMunicaoInicial();
        GarantirObjetosEspectrais();
        GarantirBlocosCap1();
        LayoutPrototipo.Aplicar();
        if (Object.FindFirstObjectByType<AplicadorLayout>() == null)
            new GameObject("AplicadorLayout").AddComponent<AplicadorLayout>();

        Debug.Log("[Lendas] Bootstrap: layout Barca / Vila / Mata / Ruinas.");
    }

    static void GarantirBentoComponentes()
    {
        var bento = Object.FindFirstObjectByType<BentoMovimento>();
        if (bento == null) return;
        if (bento.GetComponent<BentoVida>() == null)
            bento.gameObject.AddComponent<BentoVida>();
        if (bento.GetComponent<InventarioBento>() == null)
            bento.gameObject.AddComponent<InventarioBento>();
        if (bento.GetComponent<SistemaBuffs>() == null)
            bento.gameObject.AddComponent<SistemaBuffs>();
        if (bento.GetComponent<BentoFacao>() == null)
            bento.gameObject.AddComponent<BentoFacao>();
        if (bento.GetComponent<BentoBesta>() == null)
            bento.gameObject.AddComponent<BentoBesta>();
        if (bento.GetComponent<BentoVisaoEspectral>() == null)
            bento.gameObject.AddComponent<BentoVisaoEspectral>();
        if (bento.GetComponent<AmuletoAncestral>() == null)
            bento.gameObject.AddComponent<AmuletoAncestral>();
    }

    static void GarantirObjetosEspectrais()
    {
        if (GameObject.Find("PistaEspectral") == null)
        {
            var go = CriarMarcador("PistaEspectral", LayoutPrototipo.PistaEspectral,
                new Color(0.45f, 0.75f, 1f, 0.85f), new Vector3(0.6f, 0.6f, 1f));
            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.5f, 1.5f);
            var pista = go.AddComponent<ObjetoEspectral>();
            pista.tipo = TipoObjetoEspectral.Pista;
            pista.textoPista = "Marca do avo: \"O novelo de cipo acalma o Curupira.\"";
            pista.idPaginaCaderno = CatalogoPaginasCap1.IdCurupira;
        }
        else
        {
            var pistaExistente = GameObject.Find("PistaEspectral").GetComponent<ObjetoEspectral>();
            if (pistaExistente != null) pistaExistente.idPaginaCaderno = CatalogoPaginasCap1.IdCurupira;
        }

        if (GameObject.Find("PlataformaEspectral") == null)
        {
            var go = CriarMarcador("PlataformaEspectral", LayoutPrototipo.PlataformaEspectral,
                new Color(0.4f, 0.7f, 1f, 0.75f), new Vector3(2.2f, 0.35f, 1f));
            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = false;
            col.size = new Vector2(1f, 1f);

            // Camada Chao, se existir — para o pe do Bento detectar
            int camadaChao = LayerMask.NameToLayer("Chao");
            if (camadaChao >= 0) go.layer = camadaChao;

            var plat = go.AddComponent<ObjetoEspectral>();
            plat.tipo = TipoObjetoEspectral.Plataforma;
        }
    }

    static void GarantirMunicaoInicial()
    {
        CatalogoItensNorte.Garantir();
        var inventario = InventarioBento.Instancia ?? Object.FindFirstObjectByType<InventarioBento>();
        if (inventario == null) return;
        if (inventario.QuantidadePorId(CatalogoItensNorte.IdFlechasPurificadoras) > 0) return;
        inventario.Adicionar(CatalogoItensNorte.FlechasPurificadoras, 8);
    }

    static void GarantirCombatePrototipo()
    {
        if (GameObject.Find("Vegetacao_Cipo") == null)
        {
            var go = CriarMarcador("Vegetacao_Cipo", LayoutPrototipo.Vegetacao,
                new Color(0.2f, 0.55f, 0.2f), new Vector3(1.2f, 1.6f, 1f));
            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = false;
            col.size = new Vector2(1f, 1f);
            go.AddComponent<VegetacaoCortavel>();
        }

        if (GameObject.Find("CapangaTreino") == null)
        {
            var go = CriarMarcador("CapangaTreino", LayoutPrototipo.Capanga,
                new Color(0.55f, 0.2f, 0.2f), new Vector3(1f, 1.6f, 1f));
            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = false;
            col.size = new Vector2(1f, 1f);

            var rb = go.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.gravityScale = 1.5f;

            var inimigo = go.AddComponent<InimigoSimples>();
            inimigo.vidaMaxima = 3;
            inimigo.danoContato = 1;
        }

        if (GameObject.Find("SombraTreino") == null)
        {
            var go = CriarMarcador("SombraTreino", LayoutPrototipo.Sombra,
                new Color(0.55f, 0.25f, 0.75f, 0.9f), new Vector3(1.1f, 1.4f, 1f));
            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1f, 1f);
            go.AddComponent<InimigoSombra>();
        }

        GarantirPuzzleAlavanca();
        GarantirPuzzlesIndy();
    }

    static void GarantirPuzzleAlavanca()
    {
        if (GameObject.Find("BarreiraAlavanca") == null)
        {
            var barreira = CriarMarcador("BarreiraAlavanca", LayoutPrototipo.Barreira,
                new Color(0.4f, 0.32f, 0.22f), new Vector3(0.5f, 2.4f, 1f));
            var colB = barreira.GetComponent<BoxCollider2D>();
            colB.isTrigger = false;
            colB.size = new Vector2(1f, 1f);
            int camadaChao = LayerMask.NameToLayer("Chao");
            if (camadaChao >= 0) barreira.layer = camadaChao;
            barreira.AddComponent<BarreiraPuzzle>();
        }

        if (GameObject.Find("AlavancaChicote") == null)
        {
            var go = CriarMarcador("AlavancaChicote", LayoutPrototipo.Alavanca,
                new Color(0.75f, 0.55f, 0.2f), new Vector3(0.4f, 1.2f, 1f));
            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.4f, 1.4f);
            var alavanca = go.AddComponent<AlavancaChicote>();
            var barreira = GameObject.Find("BarreiraAlavanca");
            if (barreira != null)
                alavanca.barreiras = new[] { barreira };
        }
        else
        {
            var alavanca = GameObject.Find("AlavancaChicote").GetComponent<AlavancaChicote>();
            var barreira = GameObject.Find("BarreiraAlavanca");
            if (alavanca != null && barreira != null && (alavanca.barreiras == null || alavanca.barreiras.Length == 0))
                alavanca.barreiras = new[] { barreira };
        }
    }

    static void GarantirPuzzlesIndy()
    {
        CatalogoItensNorte.Garantir();
        int camadaChao = LayerMask.NameToLayer("Chao");

        if (GameObject.Find("PlataformaEspectral_2") == null)
        {
            var go = CriarMarcador("PlataformaEspectral_2", LayoutPrototipo.PlataformaEspectral2,
                new Color(0.4f, 0.7f, 1f, 0.75f), new Vector3(1.6f, 0.32f, 1f));
            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = false;
            col.size = new Vector2(1f, 1f);
            if (camadaChao >= 0) go.layer = camadaChao;
            var plat = go.AddComponent<ObjetoEspectral>();
            plat.tipo = TipoObjetoEspectral.Plataforma;
        }

        CriarPisoPuzzle("PisoFalso_A", LayoutPrototipo.PisoA, false, camadaChao);
        CriarPisoPuzzle("PisoFalso_B", LayoutPrototipo.PisoB, true, camadaChao);
        CriarPisoPuzzle("PisoFalso_C", LayoutPrototipo.PisoC, false, camadaChao);

        if (GameObject.Find("PisoChegadaRuina") == null)
        {
            var go = CriarMarcador("PisoChegadaRuina", LayoutPrototipo.PisoChegada,
                new Color(0.5f, 0.42f, 0.3f), new Vector3(2.2f, 0.35f, 1f));
            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = false;
            col.size = new Vector2(1f, 1f);
            if (camadaChao >= 0) go.layer = camadaChao;
        }

        if (GameObject.Find("PistaPisosFalsos") == null)
        {
            var go = CriarMarcador("PistaPisosFalsos", LayoutPrototipo.PistaPisos,
                new Color(0.45f, 0.75f, 1f, 0.85f), new Vector3(0.5f, 0.5f, 1f));
            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.6f, 1.6f);
            var pista = go.AddComponent<ObjetoEspectral>();
            pista.tipo = TipoObjetoEspectral.Pista;
            pista.textoPista = "Pisos mentem. O amuleto pinta o falso de vermelho.";
        }

        if (GameObject.Find("ReliquiaMarajoara") == null)
        {
            var go = CriarMarcador("ReliquiaMarajoara", LayoutPrototipo.Reliquia,
                new Color(0.85f, 0.7f, 0.25f), new Vector3(0.45f, 0.45f, 1f));
            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.4f, 1.4f);
            var rel = go.AddComponent<ReliquiaPuzzle>();
            rel.item = CatalogoItensNorte.MarcaMarajoara;
            rel.idPaginaCaderno = CatalogoPaginasCap1.IdRuinaMarajoara;
        }
    }

    static void CriarPisoPuzzle(string nome, Vector3 posicao, bool seguro, int camadaChao)
    {
        if (GameObject.Find(nome) != null) return;

        var go = CriarMarcador(nome, posicao, new Color(0.48f, 0.4f, 0.28f), new Vector3(1.35f, 0.32f, 1f));
        var col = go.GetComponent<BoxCollider2D>();
        col.isTrigger = false;
        col.size = new Vector2(1f, 1f);
        if (camadaChao >= 0) go.layer = camadaChao;
        var piso = go.AddComponent<PisoFalso>();
        piso.seguro = seguro;
    }

    static void GarantirColetaveisNorte()
    {
        CatalogoItensNorte.Garantir();

        if (GameObject.Find("Coletavel_Jambu") == null)
            CriarColetavel("Coletavel_Jambu", LayoutPrototipo.Jambu, new Color(0.35f, 0.75f, 0.25f), CatalogoItensNorte.Jambu, 2);
        if (GameObject.Find("Coletavel_Tucupi") == null)
            CriarColetavel("Coletavel_Tucupi", LayoutPrototipo.Tucupi, new Color(0.95f, 0.8f, 0.15f), CatalogoItensNorte.Tucupi, 3);
        if (GameObject.Find("Coletavel_Goma") == null)
            CriarColetavel("Coletavel_Goma", LayoutPrototipo.Goma, new Color(0.95f, 0.95f, 0.9f), CatalogoItensNorte.GomaDeTapioca, 1);
        if (GameObject.Find("Coletavel_Cipo") == null)
            CriarColetavel("Coletavel_Cipo", LayoutPrototipo.Cipo, new Color(0.55f, 0.7f, 0.35f), CatalogoItensNorte.Cipo, 3);
        if (GameObject.Find("Coletavel_Resina") == null)
            CriarColetavel("Coletavel_Resina", LayoutPrototipo.Resina, new Color(0.7f, 0.4f, 0.15f), CatalogoItensNorte.Resina, 2);
        if (GameObject.Find("Coletavel_Jambu_Rio") == null)
            CriarColetavel("Coletavel_Jambu_Rio", LayoutPrototipo.JambuRio, new Color(0.35f, 0.75f, 0.25f), CatalogoItensNorte.Jambu, 1);
        if (GameObject.Find("Coletavel_Tucupi_Rio") == null)
            CriarColetavel("Coletavel_Tucupi_Rio", LayoutPrototipo.TucupiRio, new Color(0.95f, 0.8f, 0.15f), CatalogoItensNorte.Tucupi, 1);
        if (GameObject.Find("Coletavel_Goma_Rio") == null)
            CriarColetavel("Coletavel_Goma_Rio", LayoutPrototipo.GomaRio, new Color(0.95f, 0.95f, 0.9f), CatalogoItensNorte.GomaDeTapioca, 1);
    }

    static void CriarColetavel(string nome, Vector3 posicao, Color cor, ItemDados item, int quantidade = 1)
    {
        var go = CriarMarcador(nome, posicao, cor, new Vector3(0.55f, 0.55f, 1f));
        var col = go.GetComponent<BoxCollider2D>();
        col.size = new Vector2(1.2f, 1.2f);
        var coletavel = go.AddComponent<ColetavelItem>();
        coletavel.item = item;
        coletavel.quantidade = quantidade;
    }

    static void GarantirBarcaEEstacoes()
    {
        var barca = Object.FindFirstObjectByType<BarcaRio>();
        if (barca == null)
        {
            var barcaGo = CriarMarcador("BarcaRio", LayoutPrototipo.Barca,
                new Color(0.45f, 0.3f, 0.18f), new Vector3(6.5f, 1.7f, 1f));
            var colBarca = barcaGo.GetComponent<BoxCollider2D>();
            colBarca.size = new Vector2(1.4f, 1.6f);

            var interior = new GameObject("PontoInteriorBarca");
            interior.transform.SetParent(barcaGo.transform);
            interior.transform.localPosition = new Vector3(0f, 0.55f, 0f);

            var saida = new GameObject("PontoSaidaBarca");
            saida.transform.SetParent(barcaGo.transform);
            saida.transform.localPosition = new Vector3(0.7f, 0.12f, 0f);

            barca = barcaGo.AddComponent<BarcaRio>();
            barca.pontoInterior = interior.transform;
            barca.pontoSaida = saida.transform;
        }

        if (Object.FindFirstObjectByType<EstacaoFogao>() == null)
        {
            var fogaoGo = CriarMarcador("FogaoALenha", LayoutPrototipo.Fogao,
                new Color(0.85f, 0.25f, 0.1f), new Vector3(0.75f, 0.75f, 1f));
            var colFogao = fogaoGo.GetComponent<BoxCollider2D>();
            colFogao.size = new Vector2(1.4f, 1.4f);
            fogaoGo.AddComponent<EstacaoFogao>();
        }

        if (Object.FindFirstObjectByType<EstacaoAlquimia>() == null)
        {
            var alquimiaGo = CriarMarcador("BancadaAlquimia", LayoutPrototipo.Alquimia,
                new Color(0.55f, 0.35f, 0.85f), new Vector3(0.75f, 0.75f, 1f));
            var colAlq = alquimiaGo.GetComponent<BoxCollider2D>();
            colAlq.size = new Vector2(1.4f, 1.4f);
            alquimiaGo.AddComponent<EstacaoAlquimia>();
        }

        if (Object.FindFirstObjectByType<EstacaoPesquisa>() == null)
        {
            var pesquisaGo = CriarMarcador("MesaPesquisa", LayoutPrototipo.Pesquisa,
                new Color(0.9f, 0.82f, 0.55f), new Vector3(0.8f, 0.6f, 1f));
            var colPesq = pesquisaGo.GetComponent<BoxCollider2D>();
            colPesq.size = new Vector2(1.4f, 1.4f);
            pesquisaGo.AddComponent<EstacaoPesquisa>();
        }
    }

    static void GarantirComponente<T>(string nome) where T : Component
    {
        if (Object.FindFirstObjectByType<T>() != null) return;
        new GameObject(nome).AddComponent<T>();
    }

    static void GarantirCheckpoint()
    {
        CriarCheckpoint("CheckpointInicial", LayoutPrototipo.Checkpoint, true);
    }

    static void GarantirBlocosCap1()
    {
        CriarFaixa("FaixaVila", LayoutPrototipo.ZonaVila, 40f, new Color(0.45f, 0.4f, 0.35f, 0.55f));
        CriarFaixa("FaixaRio", LayoutPrototipo.ZonaRio, 32f, new Color(0.2f, 0.45f, 0.65f, 0.55f));
        CriarFaixa("FaixaMata", LayoutPrototipo.ZonaMata, 36f, new Color(0.25f, 0.45f, 0.22f, 0.5f));
        CriarFaixa("FaixaRuinas", LayoutPrototipo.ZonaRuinas, 32f, new Color(0.55f, 0.45f, 0.28f, 0.5f));
        CriarFaixa("FaixaArena", LayoutPrototipo.ZonaArena, 28f, new Color(0.5f, 0.22f, 0.18f, 0.55f));

        CriarZona("Zona_Vila", LayoutPrototipo.ZonaVila, "Vila", 38f);
        CriarZona("Zona_Rio", LayoutPrototipo.ZonaRio, "Rio", 30f);
        CriarZona("Zona_Mata", LayoutPrototipo.ZonaMata, "Mata", 34f);
        CriarZona("Zona_Ruinas", LayoutPrototipo.ZonaRuinas, "Ruinas", 30f);
        CriarZona("Zona_Arena", LayoutPrototipo.ZonaArena, "Arena", 26f);

        if (GameObject.Find("AguaRio") == null)
        {
            var agua = CriarMarcador("AguaRio", LayoutPrototipo.AguaRio,
                new Color(0.15f, 0.4f, 0.7f, 0.85f), new Vector3(28f, 0.7f, 1f));
            agua.GetComponent<SpriteRenderer>().sortingOrder = 0;
            var colAgua = agua.GetComponent<BoxCollider2D>();
            colAgua.enabled = false;
        }

        GarantirCanoaECais();
        GarantirEncontroBoto();
        GarantirEncontroIara();
        CriarEncontro("EncontroCurupira", LayoutPrototipo.Curupira,
            new Color(0.95f, 0.4f, 0.12f), new Vector3(1.3f, 2.1f, 1f),
            "Curupira (H01)", "Novelo nos focos + purificar Sombras.");

        CriarCheckpoint("CheckpointRio", LayoutPrototipo.CheckpointRio, false);
        CriarCheckpoint("CheckpointArena", LayoutPrototipo.CheckpointArena, false);

        CriarFoco("FocoCorrupcao_A", LayoutPrototipo.FocoA);
        CriarFoco("FocoCorrupcao_B", LayoutPrototipo.FocoB);
        CriarFoco("FocoCorrupcao_C", LayoutPrototipo.FocoC);

        if (GameObject.Find("SombraArena") == null)
        {
            var go = CriarMarcador("SombraArena", LayoutPrototipo.SombraArena,
                new Color(0.55f, 0.25f, 0.75f, 0.9f), new Vector3(1.1f, 1.4f, 1f));
            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            go.AddComponent<InimigoSombra>();
        }
    }

    static void CriarZona(string nome, Vector3 posicao, string rotulo, float largura)
    {
        if (GameObject.Find(nome) != null) return;
        var go = new GameObject(nome);
        go.transform.position = posicao;
        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(largura, 8f);
        var zona = go.AddComponent<MarcadorZona>();
        zona.nomeZona = rotulo;
    }

    static void CriarFaixa(string nome, Vector3 centro, float largura, Color cor)
    {
        if (GameObject.Find(nome) != null) return;
        var go = CriarMarcador(nome, new Vector3(centro.x, 0.12f, 0f), cor, new Vector3(largura, 0.22f, 1f));
        go.GetComponent<SpriteRenderer>().sortingOrder = 1;
        go.GetComponent<BoxCollider2D>().enabled = false;
    }

    static void GarantirCanoaECais()
    {
        GameObject barreira = GameObject.Find("BarreiraCais");
        if (barreira == null)
        {
            barreira = CriarMarcador("BarreiraCais", LayoutPrototipo.BarreiraCais,
                new Color(0.4f, 0.28f, 0.16f), new Vector3(0.8f, 5.2f, 1f));
            var colB = barreira.GetComponent<BoxCollider2D>();
            colB.isTrigger = false;
            colB.size = new Vector2(1f, 1f);
            int camadaChao = LayerMask.NameToLayer("Chao");
            if (camadaChao >= 0) barreira.layer = camadaChao;
            barreira.AddComponent<BarreiraPuzzle>();
        }

        GameObject canoa = GameObject.Find("CanoaRio");
        if (canoa == null)
        {
            canoa = CriarMarcador("CanoaRio", LayoutPrototipo.CanoaCais,
                new Color(0.55f, 0.32f, 0.16f), new Vector3(2.6f, 0.45f, 1f));
            var col = canoa.GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.1f, 1.8f);
            canoa.GetComponent<SpriteRenderer>().sortingOrder = 3;
        }

        if (canoa.GetComponent<CanoaRio>() == null)
            canoa.AddComponent<CanoaRio>();
    }

    static void GarantirEncontroBoto()
    {
        GameObject barreira = GameObject.Find("BarreiraRioBoto");
        if (barreira == null)
        {
            barreira = CriarMarcador("BarreiraRioBoto", LayoutPrototipo.BarreiraBoto,
                new Color(0.35f, 0.12f, 0.32f), new Vector3(0.8f, 5.2f, 1f));
            var colB = barreira.GetComponent<BoxCollider2D>();
            colB.isTrigger = false;
            colB.size = new Vector2(1f, 1f);
            int camadaChao = LayerMask.NameToLayer("Chao");
            if (camadaChao >= 0) barreira.layer = camadaChao;
            barreira.AddComponent<BarreiraPuzzle>();
        }

        GameObject boto = GameObject.Find("BloqueioBoto");
        if (boto == null)
        {
            boto = CriarMarcador("BloqueioBoto", LayoutPrototipo.BloqueioBoto,
                new Color(0.55f, 0.2f, 0.45f), new Vector3(1.4f, 1.6f, 1f));
            var col = boto.GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(2.2f, 1.8f);
        }
        else
        {
            var col = boto.GetComponent<BoxCollider2D>();
            if (col != null)
            {
                col.isTrigger = true;
                col.size = new Vector2(2.2f, 1.8f);
            }
        }

        var placeholder = boto.GetComponent<MarcadorEncontro>();
        if (placeholder != null) Object.Destroy(placeholder);

        var encontro = boto.GetComponent<EncontroBoto>();
        if (encontro == null) encontro = boto.AddComponent<EncontroBoto>();
        encontro.barreiras = new[] { barreira };
    }

    static void GarantirEncontroIara()
    {
        GameObject barreira = GameObject.Find("BarreiraRioIara");
        if (barreira == null)
        {
            barreira = CriarMarcador("BarreiraRioIara", LayoutPrototipo.BarreiraIara,
                new Color(0.08f, 0.28f, 0.38f), new Vector3(0.8f, 5.2f, 1f));
            var colB = barreira.GetComponent<BoxCollider2D>();
            colB.isTrigger = false;
            colB.size = new Vector2(1f, 1f);
            int camadaChao = LayerMask.NameToLayer("Chao");
            if (camadaChao >= 0) barreira.layer = camadaChao;
            barreira.AddComponent<BarreiraPuzzle>();
        }

        GameObject iara = GameObject.Find("EncontroIara");
        if (iara == null)
        {
            iara = CriarMarcador("EncontroIara", LayoutPrototipo.Iara,
                new Color(0.12f, 0.35f, 0.42f), new Vector3(1.2f, 1.7f, 1f));
            var col = iara.GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.8f, 1.4f);
        }

        var placeholder = iara.GetComponent<MarcadorEncontro>();
        if (placeholder != null) Object.Destroy(placeholder);

        var encontro = iara.GetComponent<EncontroIara>();
        if (encontro == null) encontro = iara.AddComponent<EncontroIara>();
        encontro.barreiras = new[] { barreira };
    }

    static void CriarEncontro(string nome, Vector3 posicao, Color cor, Vector3 escala, string titulo, string dica)
    {
        if (GameObject.Find(nome) != null) return;
        var go = CriarMarcador(nome, posicao, cor, escala);
        var col = go.GetComponent<BoxCollider2D>();
        col.size = new Vector2(1.6f, 1.4f);
        var enc = go.AddComponent<MarcadorEncontro>();
        enc.titulo = titulo;
        enc.dica = dica;
    }

    static void CriarCheckpoint(string nome, Vector3 posicao, bool iniciar)
    {
        if (GameObject.Find(nome) != null) return;
        var go = CriarMarcador(nome, posicao, new Color(0.2f, 0.9f, 0.3f, 0.85f), new Vector3(0.6f, 0.6f, 1f));
        var col = go.GetComponent<BoxCollider2D>();
        col.size = new Vector2(2f, 2f);
        var cp = go.AddComponent<Checkpoint>();
        cp.ativarAoIniciar = iniciar;
    }

    static void CriarFoco(string nome, Vector3 posicao)
    {
        if (GameObject.Find(nome) != null) return;
        var go = CriarMarcador(nome, posicao, new Color(0.7f, 0.2f, 0.85f, 0.9f), new Vector3(0.55f, 0.9f, 1f));
        var col = go.GetComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(1.4f, 1.4f);
        var foco = go.AddComponent<ObjetoEspectral>();
        foco.tipo = TipoObjetoEspectral.Pista;
        foco.textoPista = "Foco da corrupcao. O novelo de cipo se encaixa aqui.";
        foco.idPaginaCaderno = CatalogoPaginasCap1.IdCurupira;
    }

    static void GarantirZonaMorte()
    {
        if (Object.FindFirstObjectByType<ZonaMorte>() != null) return;

        var go = new GameObject("ZonaMorte");
        go.transform.position = new Vector3(27f, -12f, 0f);
        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(160f, 4f);
        go.AddComponent<ZonaMorte>();
    }

    static void GarantirDonaNair()
    {
        CriarNpc(
            "DonaNair",
            LayoutPrototipo.DonaNair,
            new Color(1f, 0.75f, 0.2f),
            new Vector3(1.1f, 1.8f, 1f),
            new[] { CatalogoPaginasCap1.IdTacaca },
            new[]
            {
                new FalaDialogo
                {
                    personagem = "Dona Nair",
                    emocao = "preocupada",
                    texto = "Bento! A vila esta acinzentada... o Barao enlouqueceu o Curupira e poluiu as aguas."
                },
                new FalaDialogo
                {
                    personagem = "Dona Nair",
                    emocao = "saudade",
                    texto = "Seu avo deixou o amuleto com voce por um motivo. A Visao Espectral ainda dorme nele."
                },
                new FalaDialogo
                {
                    personagem = "Dona Nair",
                    emocao = "esperanca",
                    texto = "Vou te ensinar o Tacaca: jambu, tucupi e goma. Serve de oferenda ao Boto e a Iara — e te protege da paralisia."
                }
            });
    }

    static void GarantirNamoradaDoBoto()
    {
        CriarNpc(
            "NamoradaDoBoto",
            LayoutPrototipo.Lia,
            new Color(1f, 0.45f, 0.7f),
            new Vector3(1f, 1.6f, 1f),
            new[] { CatalogoPaginasCap1.IdBoto },
            new[]
            {
                new FalaDialogo
                {
                    personagem = "Lia",
                    emocao = "triste",
                    texto = "Todas as noites um rapaz bonito vinha me encontrar na beira do igarape... Depois da fumaca, ele sumiu."
                },
                new FalaDialogo
                {
                    personagem = "Lia",
                    emocao = "confidente",
                    texto = "Os mais velhos dizem que aquele rapaz e o Boto. A poluicao nao pega nele — e espirito — mas os peixes sofrem, e isso o deixou bravo."
                },
                new FalaDialogo
                {
                    personagem = "Lia",
                    emocao = "esperanca",
                    texto = "Leva este retrato. Se voce o encontrar, mostra pra ele. Quero que lembre das noites claras. Eu ainda espero."
                }
            },
            CatalogoItensNorte.IdFotoDaLia,
            new[]
            {
                new FalaDialogo
                {
                    personagem = "Lia",
                    emocao = "esperanca",
                    texto = "Voce ja tem meu retrato. Mostra pro rapaz do rio. Eu continuo na beira, esperando as noites claras."
                }
            });
    }

    static void GarantirPescadorDaIara()
    {
        CriarNpc(
            "PescadorDaIara",
            LayoutPrototipo.Zeca,
            new Color(0.35f, 0.65f, 1f),
            new Vector3(1.1f, 1.7f, 1f),
            new[] { CatalogoPaginasCap1.IdIara, CatalogoPaginasCap1.IdBoto },
            new[]
            {
                new FalaDialogo
                {
                    personagem = "Seu Zeca",
                    emocao = "preocupado",
                    texto = "O Boto ta investindo contra tudo que flutua. Nao e a poluicao que pega nele — e entidade. Mas os peixes tao morrendo, e isso o deixou bravo e triste."
                },
                new FalaDialogo
                {
                    personagem = "Seu Zeca",
                    emocao = "serio",
                    texto = "Toma a chave do meu caico, no cais. Tres flechas purificadoras acalmam o rapaz. Depois, Tacaca de oferenda — e fala com a Lia na vila. Ele lembra dela."
                },
                new FalaDialogo
                {
                    personagem = "Seu Zeca",
                    emocao = "aviso",
                    texto = "Mais adiante ta a Iara. O canto arrasta gente pro fundo. Leva Tacaca e cuidado. So a Gota de Quartzo dela abre o caminho ate o Curupira."
                }
            },
            CatalogoItensNorte.IdChaveDaCanoa,
            new[]
            {
                new FalaDialogo
                {
                    personagem = "Seu Zeca",
                    emocao = "serio",
                    texto = "A chave ja ta com voce. Solta o caico no cais, acalma o Boto com tres flechas e nao esquece a Lia. A Iara espera mais rio abaixo."
                }
            });
    }

    static void CriarNpc(string nome, Vector3 posicao, Color cor, Vector3 escala, string[] paginasCaderno, FalaDialogo[] falas, string idItem = null, FalaDialogo[] falasApos = null)
    {
        var existente = GameObject.Find(nome);
        NpcDialogo npc;
        if (existente != null)
        {
            npc = existente.GetComponent<NpcDialogo>();
        }
        else
        {
            var go = CriarMarcador(nome, posicao, cor, escala);
            var col = go.GetComponent<BoxCollider2D>();
            col.size = new Vector2(2.2f, 1.4f);
            npc = go.AddComponent<NpcDialogo>();
        }

        if (npc == null) return;
        npc.falas = falas;
        npc.paginasAoConversar = paginasCaderno;
        npc.idItemAoConversar = idItem;
        npc.falasAposEntrega = falasApos;
    }

    static GameObject CriarMarcador(string nome, Vector3 posicao, Color cor, Vector3 escala)
    {
        var go = new GameObject(nome);
        go.transform.position = posicao;
        go.transform.localScale = escala;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = CriarSpriteSimples();
        sr.color = cor;
        sr.sortingOrder = 2;

        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        return go;
    }

    static Sprite CriarSpriteSimples()
    {
        var tex = Texture2D.whiteTexture;
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 4f);
    }
}
