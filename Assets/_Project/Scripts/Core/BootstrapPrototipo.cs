using UnityEngine;

/// <summary>
/// Garante objetos do prototipo na cena ao entrar em Play,
/// mesmo sem rodar o menu Lendas/Configurar Prototipo.
/// </summary>
public static class BootstrapPrototipo
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void AoReiniciarPlay()
    {
        CarregadorPixelArt.LimparCache();
        CatalogoItensNorte.Resetar();
        CatalogoReceitasNorte.Resetar();
        CatalogoReceitasAlquimia.Resetar();
    }

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
        GarantirTainaMata();
        GarantirColetaveisNorte();
        GarantirBarcaEEstacoes();
        GarantirCombatePrototipo();
        GarantirMunicaoInicial();
        GarantirObjetosEspectrais();
        GarantirBlocosCap1();
        LayoutPrototipo.Aplicar();
        AplicarArtePersonagens();
        if (Object.FindFirstObjectByType<AplicadorLayout>() == null)
            new GameObject("AplicadorLayout").AddComponent<AplicadorLayout>();

        Debug.Log("[Lendas] Bootstrap: layout Barca / Vila / Mata / Ruinas.");
    }

    static void AplicarArtePersonagens()
    {
        AplicarSpriteNpc("DonaNair", "DonaNair", "Nair", "DonaNair/Animacoes/Nair_Idle_east_64x64_4f.png");
        AplicarSpriteNpc("NamoradaDoBoto", "Lia", "Lia", null);
        AplicarSpriteNpc("PescadorDaIara", "SeuZeca", "Zeca", null);
        AplicarSpriteNpc("TainaMata", "Taina", "Taina", "Taina/Animacoes/Taina_Idle_east_64x64_4f.png");
        AplicarSpriteBoto();
        AplicarSpriteIara();
    }

    static void AplicarSpriteBoto()
    {
        var go = GameObject.Find("BloqueioBoto");
        if (go == null) return;

        var sr = go.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        var sprite = CarregadorPixelArt.Personagem("Boto", "Boto_Corrupto");
        if (sprite == null) return;

        sr.sprite = sprite;
        sr.color = Color.white;
        sr.flipX = true;
        sr.sortingOrder = 2;
        float escala = CarregadorPixelArt.EscalaMundo * EncontroBoto.EscalaRelativaAoBento;
        go.transform.localScale = new Vector3(escala, escala, 1f);

        var col = go.GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.size = new Vector2(1.7f, 0.95f);
            col.offset = Vector2.zero;
            col.isTrigger = true;
        }
    }

    static void AplicarSpriteIara()
    {
        var go = GameObject.Find("EncontroIara");
        if (go == null) return;

        var sr = go.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        var sprite = CarregadorPixelArt.Personagem("Iara", "Iara_Bom");
        if (sprite == null) return;

        sr.sprite = sprite;
        sr.color = Color.white;
        sr.flipX = true;
        sr.sortingOrder = 5;
        float escala = CarregadorPixelArt.EscalaMundo * EncontroIara.EscalaRelativaAoNpc;
        go.transform.localScale = new Vector3(escala, escala, 1f);

        var col = go.GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.size = new Vector2(1.55f, 1.55f);
            col.offset = Vector2.zero;
            col.isTrigger = true;
        }
    }

    static void AplicarSpriteNpc(string nomeObjeto, string pasta, string prefixo, string folhaIdle)
    {
        var go = GameObject.Find(nomeObjeto);
        if (go == null) return;

        var sr = go.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        var sprite = CarregadorPixelArt.Personagem(pasta, prefixo + "_east");
        if (sprite == null) return;

        sr.sprite = sprite;
        sr.color = Color.white;
        sr.flipX = true;
        sr.sortingOrder = 3;
        float escala = CarregadorPixelArt.EscalaMundo;
        go.transform.localScale = new Vector3(escala, escala, 1f);

        var col = go.GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.size = new Vector2(1.1f, 1f);
            col.offset = Vector2.zero;
        }

        if (string.IsNullOrEmpty(folhaIdle)) return;

        var anim = go.GetComponent<NpcAnimacaoIdle>();
        if (anim == null) anim = go.AddComponent<NpcAnimacaoIdle>();
        anim.quadros = CarregadorPixelArt.Folha(folhaIdle, 64, 64, 4);
        anim.fps = 5f;
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
        if (bento.GetComponent<BentoAnimacao>() == null)
            bento.gameObject.AddComponent<BentoAnimacao>();
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
            go.AddComponent<PlataformaPassavel>();
        }
        else
        {
            GarantirPlataformaPassavel(GameObject.Find("PlataformaEspectral"));
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

        GarantirJaulaFauna("JaulaFauna_A", LayoutPrototipo.JaulaFaunaA, "Filhote de Queixada");
        GarantirJaulaFauna("JaulaFauna_B", LayoutPrototipo.JaulaFaunaB, "Arara da Mata");

        GarantirPuzzleAlavanca();
        GarantirPuzzlesIndy();
    }

    static void GarantirJaulaFauna(string nome, Vector3 posicao, string nomeAnimal)
    {
        if (GameObject.Find(nome) != null) return;
        var go = CriarMarcador(nome, posicao,
            new Color(0.32f, 0.34f, 0.38f, 0.95f), new Vector3(1.15f, 1.25f, 1f));
        var col = go.GetComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(1.4f, 1.4f);
        var jaula = go.AddComponent<JaulaFauna>();
        jaula.nomeAnimal = nomeAnimal;
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
            go.AddComponent<PlataformaPassavel>();
        }
        else
        {
            GarantirPlataformaPassavel(GameObject.Find("PlataformaEspectral_2"));
        }

        CriarPisoPuzzle("PisoFalso_A", LayoutPrototipo.PisoA, false, camadaChao);
        CriarPisoPuzzle("PisoFalso_B", LayoutPrototipo.PisoB, true, camadaChao);
        CriarPisoPuzzle("PisoFalso_C", LayoutPrototipo.PisoC, false, camadaChao);

        GameObject chegada = GameObject.Find("PisoChegadaRuina");
        if (chegada == null)
        {
            chegada = CriarMarcador("PisoChegadaRuina", LayoutPrototipo.PisoChegada,
                new Color(0.5f, 0.42f, 0.3f), new Vector3(2.2f, 0.35f, 1f));
            var col = chegada.GetComponent<BoxCollider2D>();
            col.isTrigger = false;
            col.size = new Vector2(1f, 1f);
            if (camadaChao >= 0) chegada.layer = camadaChao;
        }
        GarantirPlataformaPassavel(chegada);

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
        GameObject go = GameObject.Find(nome);
        if (go == null)
        {
            go = CriarMarcador(nome, posicao, new Color(0.48f, 0.4f, 0.28f), new Vector3(1.35f, 0.32f, 1f));
            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = false;
            col.size = new Vector2(1f, 1f);
            if (camadaChao >= 0) go.layer = camadaChao;
            var piso = go.AddComponent<PisoFalso>();
            piso.seguro = seguro;
        }

        GarantirPlataformaPassavel(go);
    }

    static void GarantirPlataformaPassavel(GameObject go)
    {
        if (go == null) return;
        var passavel = go.GetComponent<PlataformaPassavel>();
        if (passavel == null) passavel = go.AddComponent<PlataformaPassavel>();
        passavel.Aplicar();
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
        if (GameObject.Find("Coletavel_Camarao") == null)
            CriarColetavel("Coletavel_Camarao", LayoutPrototipo.Camarao, new Color(0.95f, 0.5f, 0.35f), CatalogoItensNorte.CamaraoSeco, 1);
        GarantirColetavelReaparece("Coletavel_Cipo", LayoutPrototipo.Cipo, new Color(0.55f, 0.7f, 0.35f), CatalogoItensNorte.Cipo, 3);
        GarantirColetavelReaparece("Coletavel_Resina", LayoutPrototipo.Resina, new Color(0.7f, 0.4f, 0.15f), CatalogoItensNorte.Resina, 2);
        if (GameObject.Find("Coletavel_Jambu_Rio") == null)
            CriarColetavel("Coletavel_Jambu_Rio", LayoutPrototipo.JambuRio, new Color(0.35f, 0.75f, 0.25f), CatalogoItensNorte.Jambu, 2);
        if (GameObject.Find("Coletavel_Tucupi_Rio") == null)
            CriarColetavel("Coletavel_Tucupi_Rio", LayoutPrototipo.TucupiRio, new Color(0.95f, 0.8f, 0.15f), CatalogoItensNorte.Tucupi, 2);
        if (GameObject.Find("Coletavel_Goma_Rio") == null)
            CriarColetavel("Coletavel_Goma_Rio", LayoutPrototipo.GomaRio, new Color(0.95f, 0.95f, 0.9f), CatalogoItensNorte.GomaDeTapioca, 1);
        if (GameObject.Find("Coletavel_Camarao_Rio") == null)
            CriarColetavel("Coletavel_Camarao_Rio", LayoutPrototipo.CamaraoRio, new Color(0.95f, 0.5f, 0.35f), CatalogoItensNorte.CamaraoSeco, 1);
        if (GameObject.Find("Coletavel_PatoAssado") == null)
            CriarColetavel("Coletavel_PatoAssado", LayoutPrototipo.PatoAssadoVila, new Color(0.75f, 0.45f, 0.2f), CatalogoItensNorte.PatoAssado, 1);
        if (GameObject.Find("Coletavel_FavoMel") == null)
            CriarColetavel("Coletavel_FavoMel", LayoutPrototipo.FavoDeMel, new Color(1f, 0.78f, 0.15f), CatalogoItensNorte.FavoDeMel, 1);
        GarantirColetavelReaparece("Coletavel_Flechas_Clareira", LayoutPrototipo.FlechasClareira, new Color(0.55f, 0.85f, 1f), CatalogoItensNorte.FlechasPurificadoras, 2);
    }

    static void GarantirColetavelReaparece(string nome, Vector3 posicao, Color cor, ItemDados item, int quantidade)
    {
        var go = GameObject.Find(nome);
        if (go == null)
        {
            CriarColetavel(nome, posicao, cor, item, quantidade, true);
            return;
        }

        var coletavel = go.GetComponent<ColetavelItem>();
        if (coletavel == null) coletavel = go.AddComponent<ColetavelItem>();
        coletavel.item = item;
        coletavel.quantidade = quantidade;
        coletavel.reaparece = true;
        coletavel.destruirAoColetar = false;
        coletavel.MostrarDeNovo();
    }

    static void CriarColetavel(string nome, Vector3 posicao, Color cor, ItemDados item, int quantidade = 1, bool reaparece = false)
    {
        var go = CriarMarcador(nome, posicao, cor, new Vector3(0.55f, 0.55f, 1f));
        var col = go.GetComponent<BoxCollider2D>();
        col.size = new Vector2(1.2f, 1.2f);
        var coletavel = go.AddComponent<ColetavelItem>();
        coletavel.item = item;
        coletavel.quantidade = quantidade;
        coletavel.reaparece = reaparece;
        coletavel.destruirAoColetar = !reaparece;
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
        CriarFaixa("FaixaRio", LayoutPrototipo.ZonaRio, LayoutPrototipo.LarguraFaixaRio, new Color(0.2f, 0.45f, 0.65f, 0.55f));
        CriarFaixa("FaixaMata", LayoutPrototipo.ZonaMata, 36f, new Color(0.25f, 0.45f, 0.22f, 0.5f));
        CriarFaixa("FaixaRuinas", LayoutPrototipo.ZonaRuinas, 32f, new Color(0.55f, 0.45f, 0.28f, 0.5f));
        CriarFaixa("FaixaArena", LayoutPrototipo.ZonaArena, 28f, new Color(0.5f, 0.22f, 0.18f, 0.55f));

        CriarZona("Zona_Vila", LayoutPrototipo.ZonaVila, "Vila", 38f);
        CriarZona("Zona_Rio", LayoutPrototipo.ZonaRio, "Rio", LayoutPrototipo.LarguraZonaRio);
        CriarZona("Zona_Mata", LayoutPrototipo.ZonaMata, "Mata", 34f);
        CriarZona("Zona_Ruinas", LayoutPrototipo.ZonaRuinas, "Ruinas", 30f);
        CriarZona("Zona_Arena", LayoutPrototipo.ZonaArena, "Arena", 26f);

        if (GameObject.Find("AguaRio") == null)
        {
            var agua = CriarMarcador("AguaRio", LayoutPrototipo.AguaRio,
                new Color(0.15f, 0.4f, 0.7f, 0.85f), new Vector3(LayoutPrototipo.LarguraAguaRio, 0.7f, 1f));
            agua.GetComponent<SpriteRenderer>().sortingOrder = 0;
            var colAgua = agua.GetComponent<BoxCollider2D>();
            colAgua.enabled = false;
        }

        GarantirCanoaECais();
        GarantirEncontroBoto();
        GarantirEncontroIara();
        GarantirEncontroCaipora();
        GarantirEncontroCurupira();

        CriarCheckpoint("CheckpointRio", LayoutPrototipo.CheckpointRio, false);
        CriarCheckpoint("CheckpointMata", LayoutPrototipo.CheckpointMata, false);
        CriarCheckpoint("CheckpointCaipora", LayoutPrototipo.CheckpointCaipora, false, 2, "Clareira do Caipora");
        CriarCheckpoint("CheckpointArena", LayoutPrototipo.CheckpointArena, false);

        GarantirFoco("FocoCorrupcao_A", LayoutPrototipo.FocoA);
        GarantirFoco("FocoCorrupcao_B", LayoutPrototipo.FocoB);
        GarantirFoco("FocoCorrupcao_C", LayoutPrototipo.FocoC);

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
        int camadaChao = LayerMask.NameToLayer("Chao");

        GameObject barreira = GameObject.Find("BarreiraCais");
        if (barreira == null)
        {
            barreira = CriarMarcador("BarreiraCais", LayoutPrototipo.BarreiraCais,
                new Color(0.4f, 0.28f, 0.16f), new Vector3(0.8f, 5.2f, 1f));
            var colB = barreira.GetComponent<BoxCollider2D>();
            colB.isTrigger = false;
            colB.size = new Vector2(1f, 6f);
            colB.offset = new Vector2(0f, 2f);
            if (camadaChao >= 0) barreira.layer = camadaChao;
            barreira.AddComponent<BarreiraPuzzle>();
        }

        GameObject barreiraLeste = GameObject.Find("BarreiraCaisLeste");
        if (barreiraLeste == null)
        {
            barreiraLeste = CriarMarcador("BarreiraCaisLeste", LayoutPrototipo.BarreiraCaisLeste,
                new Color(0.4f, 0.28f, 0.16f), new Vector3(0.8f, 5.2f, 1f));
            var colBL = barreiraLeste.GetComponent<BoxCollider2D>();
            colBL.isTrigger = false;
            colBL.size = new Vector2(1f, 6f);
            colBL.offset = new Vector2(0f, 2f);
            if (camadaChao >= 0) barreiraLeste.layer = camadaChao;
            barreiraLeste.AddComponent<BarreiraPuzzle>();
        }

        GameObject canoa = GameObject.Find("CanoaRio");
        if (canoa == null)
        {
            canoa = CriarMarcador("CanoaRio", LayoutPrototipo.CanoaCais,
                new Color(0.55f, 0.32f, 0.16f), new Vector3(2.6f, 0.45f, 1f));
            var col = canoa.GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.4f, 1.8f);
            canoa.GetComponent<SpriteRenderer>().sortingOrder = 3;
        }
        else
        {
            var col = canoa.GetComponent<BoxCollider2D>();
            if (col != null)
            {
                col.isTrigger = true;
                col.size = new Vector2(1.4f, 1.8f);
            }
        }

        if (canoa.GetComponent<CanoaRio>() == null)
            canoa.AddComponent<CanoaRio>();

        CatalogoItensNorte.Garantir();
        var inv = InventarioBento.Instancia ?? Object.FindFirstObjectByType<InventarioBento>();
        bool jaTemChave = inv != null && inv.TemPorId(CatalogoItensNorte.IdChaveDaCanoa);
        if (!jaTemChave && GameObject.Find("Coletavel_ChaveCanoa") == null)
        {
            var chaveGo = CriarMarcador("Coletavel_ChaveCanoa", LayoutPrototipo.ChaveCanoaCais,
                new Color(0.55f, 0.9f, 1f, 0.95f), new Vector3(0.5f, 0.5f, 1f));
            var colChave = chaveGo.GetComponent<BoxCollider2D>();
            colChave.isTrigger = true;
            colChave.size = new Vector2(1.4f, 1.4f);
            chaveGo.GetComponent<SpriteRenderer>().sortingOrder = 4;

            var coletavel = chaveGo.AddComponent<ColetavelItem>();
            coletavel.item = CatalogoItensNorte.ChaveDaCanoa;
            coletavel.quantidade = 1;

            var espectral = chaveGo.AddComponent<ObjetoEspectral>();
            espectral.tipo = TipoObjetoEspectral.Pista;
            espectral.textoPista = "Brilho na lama: Chave da Canoa";
        }
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
            col.size = new Vector2(2.4f, 2.2f);
        }
        else
        {
            var col = iara.GetComponent<BoxCollider2D>();
            if (col != null)
            {
                col.isTrigger = true;
                col.size = new Vector2(2.4f, 2.2f);
            }
        }

        var placeholder = iara.GetComponent<MarcadorEncontro>();
        if (placeholder != null) Object.Destroy(placeholder);

        var encontro = iara.GetComponent<EncontroIara>();
        if (encontro == null) encontro = iara.AddComponent<EncontroIara>();
        encontro.barreiras = new[] { barreira };

        GarantirVitoriaRegia();
    }

    static void GarantirVitoriaRegia()
    {
        GameObject folha = GameObject.Find("VitoriaRegiaIara");
        if (folha == null)
        {
            folha = CriarMarcador("VitoriaRegiaIara", LayoutPrototipo.VitoriaRegiaIara,
                Color.white, Vector3.one);
        }

        var sr = folha.GetComponent<SpriteRenderer>();
        if (sr == null) sr = folha.AddComponent<SpriteRenderer>();

        var sprite = CarregadorPixelArt.Personagem("Iara", "VitoriaRegia");
        if (sprite != null)
        {
            sr.sprite = sprite;
            sr.color = Color.white;
        }
        else
            sr.color = new Color(0.18f, 0.55f, 0.28f);

        sr.sortingOrder = 1;
        folha.transform.localScale = new Vector3(
            CarregadorPixelArt.EscalaMundo * 1.45f,
            CarregadorPixelArt.EscalaMundo * 0.32f,
            1f);

        var col = folha.GetComponent<BoxCollider2D>();
        if (col != null) col.enabled = false;
    }

    static void GarantirEncontroCaipora()
    {
        GameObject barreira = GameObject.Find("BarreiraMataCaipora");
        if (barreira == null)
        {
            barreira = CriarMarcador("BarreiraMataCaipora", LayoutPrototipo.BarreiraCaipora,
                new Color(0.18f, 0.38f, 0.15f), new Vector3(0.8f, 5.2f, 1f));
            var colB = barreira.GetComponent<BoxCollider2D>();
            colB.isTrigger = false;
            colB.size = new Vector2(1f, 1f);
            int camadaChao = LayerMask.NameToLayer("Chao");
            if (camadaChao >= 0) barreira.layer = camadaChao;
            barreira.AddComponent<BarreiraPuzzle>();
        }

        GameObject caipora = GameObject.Find("EncontroCaipora");
        if (caipora == null)
        {
            caipora = CriarMarcador("EncontroCaipora", LayoutPrototipo.Caipora,
                new Color(0.25f, 0.58f, 0.22f), new Vector3(1.35f, 1.65f, 1f));
            var col = caipora.GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(2.2f, 1.9f);
        }
        else
        {
            var col = caipora.GetComponent<BoxCollider2D>();
            if (col != null)
            {
                col.isTrigger = true;
                col.size = new Vector2(2.2f, 1.9f);
            }
        }

        var placeholder = caipora.GetComponent<MarcadorEncontro>();
        if (placeholder != null) Object.Destroy(placeholder);

        var encontro = caipora.GetComponent<EncontroCaipora>();
        if (encontro == null) encontro = caipora.AddComponent<EncontroCaipora>();
        encontro.barreiras = new[] { barreira };
    }

    static void GarantirEncontroCurupira()
    {
        GameObject portao = GameObject.Find("BarreiraArenaCurupira");
        if (portao == null)
        {
            portao = CriarMarcador("BarreiraArenaCurupira", LayoutPrototipo.BarreiraArena,
                new Color(0.35f, 0.12f, 0.08f), new Vector3(0.8f, 5.2f, 1f));
            var colP = portao.GetComponent<BoxCollider2D>();
            colP.isTrigger = false;
            colP.size = new Vector2(1f, 1f);
            int camadaChao = LayerMask.NameToLayer("Chao");
            if (camadaChao >= 0) portao.layer = camadaChao;
            portao.AddComponent<BarreiraPuzzle>();
        }

        GameObject curupira = GameObject.Find("EncontroCurupira");
        if (curupira == null)
        {
            curupira = CriarMarcador("EncontroCurupira", LayoutPrototipo.Curupira,
                new Color(0.55f, 0.12f, 0.08f), new Vector3(1.3f, 2.1f, 1f));
            var col = curupira.GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.6f, 1.4f);
        }
        else
        {
            var col = curupira.GetComponent<BoxCollider2D>();
            if (col != null)
            {
                col.isTrigger = true;
                col.size = new Vector2(1.6f, 1.4f);
            }
        }

        var placeholder = curupira.GetComponent<MarcadorEncontro>();
        if (placeholder != null) Object.Destroy(placeholder);

        var encontro = curupira.GetComponent<EncontroCurupira>();
        if (encontro == null) encontro = curupira.AddComponent<EncontroCurupira>();
        encontro.portaoArena = portao;
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

    static void CriarCheckpoint(string nome, Vector3 posicao, bool iniciar, int flechasMinimas = 0, string rotulo = "Checkpoint")
    {
        GameObject go = GameObject.Find(nome);
        if (go == null)
        {
            go = CriarMarcador(nome, posicao, new Color(0.25f, 1f, 0.55f, 0.95f), new Vector3(1.1f, 0.7f, 1f));
            var colNovo = go.GetComponent<BoxCollider2D>();
            colNovo.size = new Vector2(3.2f, 2.4f);
        }

        go.transform.position = posicao;
        var col = go.GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.isTrigger = true;
            col.size = new Vector2(3.2f, 2.4f);
        }

        var sr = go.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(0.25f, 1f, 0.55f, 0.95f);
            sr.sortingOrder = 4;
        }

        var cp = go.GetComponent<Checkpoint>();
        if (cp == null) cp = go.AddComponent<Checkpoint>();
        cp.ativarAoIniciar = iniciar;
        cp.flechasMinimas = flechasMinimas;
        cp.rotulo = rotulo;
    }

    static void GarantirFoco(string nome, Vector3 posicao)
    {
        GameObject go = GameObject.Find(nome);
        if (go == null)
        {
            go = CriarMarcador(nome, posicao, new Color(0.7f, 0.2f, 0.85f, 0.9f), new Vector3(0.55f, 0.9f, 1f));
            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.4f, 1.4f);
            var espectral = go.AddComponent<ObjetoEspectral>();
            espectral.tipo = TipoObjetoEspectral.Pista;
            espectral.textoPista = "Foco da corrupcao. O novelo de cipo se encaixa aqui.";
            espectral.idPaginaCaderno = CatalogoPaginasCap1.IdCurupira;
            espectral.ocultarDica = true;
        }

        if (go.GetComponent<FocoCorrupcao>() == null)
            go.AddComponent<FocoCorrupcao>();
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
            new[] { CatalogoPaginasCap1.IdTacaca, CatalogoPaginasCap1.IdIara },
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
                    emocao = "carinho",
                    texto = "Fiz um Tacaca pra voce. Bebe quando o canto da Iara vier. Guarda outro pra oferenda ao Boto — o rio nao aceita cuia vazia."
                },
                new FalaDialogo
                {
                    personagem = "Dona Nair",
                    emocao = "respeito",
                    texto = "E leva estas fitas coloridas. A Mae d'Agua pede licenca, nao comida. Fitas, pente e espelho: pacto com o rio."
                }
            },
            new[] { CatalogoItensNorte.IdTacaca, CatalogoItensNorte.IdFitasColoridas },
            new[]
            {
                new FalaDialogo
                {
                    personagem = "Dona Nair",
                    emocao = "serio",
                    texto = "Tacaca pro Boto e pro Treme. Fitas pra Iara. Se faltar oferenda, fala com a Lia e o Seu Zeca."
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
            new[] { CatalogoPaginasCap1.IdBoto, CatalogoPaginasCap1.IdIara },
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
                    texto = "Leva este retrato pro Boto. E este espelho pra Iara — ela se mira nas pedras. Pedir licenca ao rio e o que a vila ensina."
                }
            },
            new[] { CatalogoItensNorte.IdFotoDaLia, CatalogoItensNorte.IdEspelho },
            new[]
            {
                new FalaDialogo
                {
                    personagem = "Lia",
                    emocao = "esperanca",
                    texto = "Retrato pro rapaz das noites. Espelho pra Mae d'Agua. Eu continuo na beira, esperando."
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
            new[] { CatalogoPaginasCap1.IdIara, CatalogoPaginasCap1.IdBoto, CatalogoPaginasCap1.IdPatoNoTucupi },
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
                    texto = "Quando ele bateu no cais, a chave do meu caico caiu na lama! Use o amuleto do seu avo — a Visao Espectral [V] — ali na margem pra achar a chave."
                },
                new FalaDialogo
                {
                    personagem = "Seu Zeca",
                    emocao = "aviso",
                    texto = "Na agua turva, use tambem o [V] pra enxergar a aura espiritual do Boto e acertar tres flechas purificadoras. Depois, Tacaca e o retrato da Lia."
                },
                new FalaDialogo
                {
                    personagem = "Seu Zeca",
                    emocao = "respeito",
                    texto = "Antes de remar trecho fundo, a gente pede licenca a Mae d'Agua. Leva meu pente de osso. Come Tacaca se o canto vier — sem Treme, ela puxa pro fundo."
                },
                new FalaDialogo
                {
                    personagem = "Seu Zeca",
                    emocao = "carinho",
                    texto = "E Bento... minha vizinha Taina ficou presa do outro lado do rio, na entrada da mata, cuidando dos bichos. Ela adora Pato no Tucupi! Leva este Pato Assado, cozinha na sua Barca com tucupi e jambu e leva pra ela."
                }
            },
            new[] { CatalogoItensNorte.IdPenteDeOsso, CatalogoItensNorte.IdPatoAssado },
            new[]
            {
                new FalaDialogo
                {
                    personagem = "Seu Zeca",
                    emocao = "serio",
                    texto = "Segura [V] na beira do cais pra achar a chave na lama, e usa [V] na canoa pra ver a aura do Boto. Nao esquece de cozinhar o Pato no Tucupi na Barca pra levar pra Taina na entrada da mata!"
                }
            });
    }

    static void GarantirTainaMata()
    {
        var npc = CriarNpc(
            "TainaMata",
            LayoutPrototipo.Taina,
            new Color(0.3f, 0.85f, 0.45f),
            new Vector3(1.05f, 1.65f, 1f),
            new[] { CatalogoPaginasCap1.IdCaipora, CatalogoPaginasCap1.IdPatoNoTucupi },
            new[]
            {
                new FalaDialogo
                {
                    personagem = "Taina",
                    emocao = "preocupada",
                    texto = "Bento! Ainda bem que voce atravessou o rio. Os capangas do Barao espalharam jaulas de ferro na trilha e prenderam os bichos da mata!"
                },
                new FalaDialogo
                {
                    personagem = "Taina",
                    emocao = "alerta",
                    texto = "O Caipora montou no queixada e nao deixa ninguem passar. Primeiro voce precisa quebrar as 2 jaulas da fauna, derrotar o Capanga cacador e usar a Visao Espectral [V] quando ouvir o assobio dele na folhagem para acertar duas flechas purificadoras."
                },
                new FalaDialogo
                {
                    personagem = "Taina",
                    emocao = "respeito",
                    texto = "Depois que ele acalmar, a licenca do toco pede um Favo de Mel da mata e o meu Colar de Sementes."
                },
                new FalaDialogo
                {
                    personagem = "Taina",
                    emocao = "esperanca",
                    texto = "Faz dois dias que nao consigo voltar pra vila... Se o meu vizinho Seu Zeca te deu o Pato Assado, prepara um Pato no Tucupi la na Barca e traz pra mim que eu faco escambo pelo meu Colar de Sementes!"
                }
            },
            null,
            new[]
            {
                new FalaDialogo
                {
                    personagem = "Taina",
                    emocao = "carinho",
                    texto = "O Colar de Sementes ja ta com voce! Quebre as 2 jaulas da fauna, derrote o Capanga, use [V] no assobio do Caipora e ofereca o colar junto com o Favo de Mel."
                }
            });

        if (npc != null)
        {
            npc.idItemPedidoEscambo = CatalogoItensNorte.IdPatoNoTucupi;
            npc.idItemRecompensaEscambo = CatalogoItensNorte.IdColarDeSementes;
            npc.falasAoEscambar = new[]
            {
                new FalaDialogo
                {
                    personagem = "Taina",
                    emocao = "feliz",
                    texto = "Pato no Tucupi quentinho! Sentir o tempero da vila renova a alma... Agradece ao Seu Zeca por mim!"
                },
                new FalaDialogo
                {
                    personagem = "Taina",
                    emocao = "respeito",
                    texto = "Aqui esta o meu Colar de Sementes. Quebre as jaulas dos bichos, pegue o Favo de Mel perto do gancho da mata e peca licenca ao Caipora — ele vai te entregar a fibra sagrada pro Novelo do Curupira!"
                }
            };
        }
    }

    static NpcDialogo CriarNpc(string nome, Vector3 posicao, Color cor, Vector3 escala, string[] paginasCaderno, FalaDialogo[] falas, string[] idsItens = null, FalaDialogo[] falasApos = null)
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

        if (npc == null) return null;
        npc.falas = falas;
        npc.paginasAoConversar = paginasCaderno;
        npc.idsItensAoConversar = idsItens;
        npc.falasAposEntrega = falasApos;
        return npc;
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
