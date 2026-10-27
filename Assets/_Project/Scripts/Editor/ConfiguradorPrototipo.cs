using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ConfiguradorPrototipo
{
    const string CaminhoPrefabBento = "Assets/Prefabs/Bento.prefab";
    const string CaminhoMaterialLinha = "Packages/com.unity.render-pipelines.universal/Runtime/Materials/Sprite-Unlit-Default.mat";

    [MenuItem("Lendas/Configurar Prototipo")]
    static void Configurar()
    {
        int camadaChao = GarantirCamada("Chao");
        int camadaGancho = GarantirCamada("Gancho");
        int camadaJogador = GarantirCamada("Jogador");

        if (camadaChao < 0 || camadaGancho < 0 || camadaJogador < 0)
        {
            Debug.LogError("[Lendas] Nao ha espaco livre na lista de Layers.");
            return;
        }

        ConfigurarPrefabBento(camadaChao, camadaJogador);
        ConfigurarCena(camadaChao, camadaGancho);

        Debug.Log("[Lendas] Prototipo configurado: layout Barca / Vila / Mata / Ruinas.");
    }

    [MenuItem("Lendas/Criar Itens Norte (ScriptableObjects)")]
    static void CriarItensNorteAssets()
    {
        const string pasta = "Assets/_Project/Data/Itens";
        if (!AssetDatabase.IsValidFolder("Assets/_Project"))
            AssetDatabase.CreateFolder("Assets", "_Project");
        if (!AssetDatabase.IsValidFolder("Assets/_Project/Data"))
            AssetDatabase.CreateFolder("Assets/_Project", "Data");
        if (!AssetDatabase.IsValidFolder(pasta))
            AssetDatabase.CreateFolder("Assets/_Project/Data", "Itens");

        CriarOuAtualizarItem(pasta + "/Jambu.asset", CatalogoItensNorte.IdJambu, "Jambu",
            "Folha anestesiante do Norte. Ingrediente do Tacaca.", CategoriaItem.Ingrediente);
        CriarOuAtualizarItem(pasta + "/Tucupi.asset", CatalogoItensNorte.IdTucupi, "Tucupi",
            "Caldo amarelo da mandioca brava. Base do Tacaca e do Pato no Tucupi.", CategoriaItem.Ingrediente);
        CriarOuAtualizarItem(pasta + "/GomaDeTapioca.asset", CatalogoItensNorte.IdGoma, "Goma de Tapioca",
            "Goma que da o ponto do Tacaca.", CategoriaItem.Ingrediente);
        CriarOuAtualizarItem(pasta + "/Tacaca.asset", CatalogoItensNorte.IdTacaca, "Tacaca",
            "Oferenda ao Boto. Beber concede Treme contra o canto da Iara. Guarde um pra oferenda.", CategoriaItem.Comida);
        CriarOuAtualizarItem(pasta + "/PatoNoTucupi.asset", CatalogoItensNorte.IdPatoNoTucupi, "Pato no Tucupi",
            "Prato tipico do Norte. Variacao do buff Treme.", CategoriaItem.Comida);
        CriarOuAtualizarItem(pasta + "/Cipo.asset", CatalogoItensNorte.IdCipo, "Cipo",
            "Fibra da mata. Base do Novelo Encantado.", CategoriaItem.Reagente);
        CriarOuAtualizarItem(pasta + "/Resina.asset", CatalogoItensNorte.IdResina, "Resina",
            "Seiva endurecida. Usada em flechas e no novelo.", CategoriaItem.Reagente);
        CriarOuAtualizarItem(pasta + "/FlechasPurificadoras.asset", CatalogoItensNorte.IdFlechasPurificadoras, "Flechas Purificadoras",
            "Municao da Besta Espiritual contra Sombras.", CategoriaItem.Item);
        CriarOuAtualizarItem(pasta + "/NoveloEncantado.asset", CatalogoItensNorte.IdNoveloEncantado, "Novelo Encantado",
            "Novelo de cipo ritual. Fraqueza do Curupira.", CategoriaItem.Reliquia);
        CriarOuAtualizarItem(pasta + "/MarcaMarajoara.asset", CatalogoItensNorte.IdMarcaMarajoara, "Marca Marajoara",
            "Placa de ceramica da ruina. Pisos falsos e alavancas: o avo ja passou por aqui.", CategoriaItem.Reliquia);
        CriarOuAtualizarItem(pasta + "/GotaDeQuartzo.asset", CatalogoItensNorte.IdGotaDeQuartzo, "Gota de Quartzo",
            "Lagrima purificada da Iara. Abre o caminho ate a arena do Curupira.", CategoriaItem.Reliquia);
        CriarOuAtualizarItem(pasta + "/ChaveDaCanoa.asset", CatalogoItensNorte.IdChaveDaCanoa, "Chave da Canoa",
            "Chave que Seu Zeca deixou para o caico no cais.", CategoriaItem.Item, 1);
        CriarOuAtualizarItem(pasta + "/FotoDaLia.asset", CatalogoItensNorte.IdFotoDaLia, "Foto da Lia",
            "Retrato que Lia mandou ao Boto: para ele lembrar das noites no igarape.", CategoriaItem.Reliquia, 1);
        CriarOuAtualizarItem(pasta + "/FitasColoridas.asset", CatalogoItensNorte.IdFitasColoridas, "Fitas Coloridas",
            "Oferenda de respeito a Iara. Dona Nair mandou para pedir licenca ao rio.", CategoriaItem.Reliquia, 1);
        CriarOuAtualizarItem(pasta + "/Espelho.asset", CatalogoItensNorte.IdEspelho, "Espelho",
            "Oferenda a Mae d'Agua. Lia mandou para ela admirar o reflexo nas pedras.", CategoriaItem.Reliquia, 1);
        CriarOuAtualizarItem(pasta + "/PenteDeOsso.asset", CatalogoItensNorte.IdPenteDeOsso, "Pente de Osso",
            "Oferenda de pescador. Seu Zeca pede licenca a Iara antes de remar trecho fundo.", CategoriaItem.Reliquia, 1);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Lendas] Itens Norte criados em " + pasta);
    }

    static void CriarOuAtualizarItem(string caminho, string id, string nome, string descricao, CategoriaItem categoria, int empilhamentoMaximo = 99)
    {
        var item = AssetDatabase.LoadAssetAtPath<ItemDados>(caminho);
        if (item == null)
        {
            item = ScriptableObject.CreateInstance<ItemDados>();
            AssetDatabase.CreateAsset(item, caminho);
        }

        item.id = id;
        item.nomeExibicao = nome;
        item.descricao = descricao;
        item.categoria = categoria;
        item.empilhamentoMaximo = Mathf.Max(1, empilhamentoMaximo);
        EditorUtility.SetDirty(item);
    }

    static int GarantirCamada(string nome)
    {
        var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty camadas = tagManager.FindProperty("layers");

        for (int i = 0; i < camadas.arraySize; i++)
        {
            if (camadas.GetArrayElementAtIndex(i).stringValue == nome) return i;
        }

        for (int i = 8; i < camadas.arraySize; i++)
        {
            SerializedProperty elemento = camadas.GetArrayElementAtIndex(i);
            if (string.IsNullOrEmpty(elemento.stringValue))
            {
                elemento.stringValue = nome;
                tagManager.ApplyModifiedProperties();
                return i;
            }
        }

        return -1;
    }

    static void ConfigurarPrefabBento(int camadaChao, int camadaJogador)
    {
        GameObject raiz = PrefabUtility.LoadPrefabContents(CaminhoPrefabBento);

        foreach (Transform t in raiz.GetComponentsInChildren<Transform>(true))
        {
            t.gameObject.layer = camadaJogador;
        }

        var rb = raiz.GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        var movimento = raiz.GetComponent<BentoMovimento>();
        movimento.camadaChao = 1 << camadaChao;

        if (raiz.GetComponent<BentoChicote>() == null) raiz.AddComponent<BentoChicote>();
        if (raiz.GetComponent<BentoVida>() == null) raiz.AddComponent<BentoVida>();
        if (raiz.GetComponent<InventarioBento>() == null) raiz.AddComponent<InventarioBento>();
        if (raiz.GetComponent<SistemaBuffs>() == null) raiz.AddComponent<SistemaBuffs>();
        if (raiz.GetComponent<BentoFacao>() == null) raiz.AddComponent<BentoFacao>();
        if (raiz.GetComponent<BentoBesta>() == null) raiz.AddComponent<BentoBesta>();
        if (raiz.GetComponent<BentoVisaoEspectral>() == null) raiz.AddComponent<BentoVisaoEspectral>();
        if (raiz.GetComponent<AmuletoAncestral>() == null) raiz.AddComponent<AmuletoAncestral>();

        var junta = raiz.GetComponent<DistanceJoint2D>();
        junta.enabled = false;

        var linha = raiz.GetComponent<LineRenderer>();
        linha.startWidth = 0.05f;
        linha.endWidth = 0.05f;
        linha.positionCount = 2;
        linha.useWorldSpace = true;
        linha.startColor = new Color(0.45f, 0.28f, 0.12f);
        linha.endColor = new Color(0.55f, 0.36f, 0.18f);
        linha.sortingOrder = 5;
        linha.enabled = false;

        var material = AssetDatabase.LoadAssetAtPath<Material>(CaminhoMaterialLinha);
        if (material != null) linha.sharedMaterial = material;

        PrefabUtility.SaveAsPrefabAsset(raiz, CaminhoPrefabBento);
        PrefabUtility.UnloadPrefabContents(raiz);
    }

    static void ConfigurarCena(int camadaChao, int camadaGancho)
    {
        AplicarCamadaPorTag("Chao", camadaChao);
        AplicarCamadaPorTag("Gancho", camadaGancho);

        GarantirHud();
        GarantirHudInventario();
        GarantirHudFogao();
        GarantirHudAlquimia();
        GarantirHudBuffs();
        GarantirHudMunicao();
        GarantirHudEnergiaEspiritual();
        GarantirCaderno();
        GarantirGerenciadorDialogo();
        GarantirCheckpointInicial();
        GarantirZonaMorte();
        GarantirDonaNair();
        GarantirNamoradaDoBoto();
        GarantirPescadorDaIara();
        GarantirColetaveisNorte();
        GarantirBarcaEEstacoes();
        GarantirCombatePrototipo();
        GarantirObjetosEspectrais();
        GarantirPuzzlesIndy();
        GarantirBlocosCap1();
        LayoutPrototipo.Aplicar();

        var cena = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(cena);
        EditorSceneManager.SaveScene(cena);
    }

    static void AplicarCamadaPorTag(string tag, int camada)
    {
        foreach (GameObject objeto in GameObject.FindGameObjectsWithTag(tag))
        {
            Undo.RecordObject(objeto, "Configurar camada");
            objeto.layer = camada;
        }
    }

    static void GarantirHud()
    {
        if (Object.FindFirstObjectByType<HudVida>() != null) return;
        var go = new GameObject("HudVida");
        Undo.RegisterCreatedObjectUndo(go, "Criar HudVida");
        go.AddComponent<HudVida>();
    }

    static void GarantirHudInventario()
    {
        if (Object.FindFirstObjectByType<HudInventario>() != null) return;
        var go = new GameObject("HudInventario");
        Undo.RegisterCreatedObjectUndo(go, "Criar HudInventario");
        go.AddComponent<HudInventario>();
    }

    static void GarantirHudFogao()
    {
        if (Object.FindFirstObjectByType<HudFogao>() != null) return;
        var go = new GameObject("HudFogao");
        Undo.RegisterCreatedObjectUndo(go, "Criar HudFogao");
        go.AddComponent<HudFogao>();
    }

    static void GarantirHudAlquimia()
    {
        if (Object.FindFirstObjectByType<HudAlquimia>() != null) return;
        var go = new GameObject("HudAlquimia");
        Undo.RegisterCreatedObjectUndo(go, "Criar HudAlquimia");
        go.AddComponent<HudAlquimia>();
    }

    static void GarantirHudBuffs()
    {
        if (Object.FindFirstObjectByType<HudBuffs>() != null) return;
        var go = new GameObject("HudBuffs");
        Undo.RegisterCreatedObjectUndo(go, "Criar HudBuffs");
        go.AddComponent<HudBuffs>();
    }

    static void GarantirHudMunicao()
    {
        if (Object.FindFirstObjectByType<HudMunicao>() != null) return;
        var go = new GameObject("HudMunicao");
        Undo.RegisterCreatedObjectUndo(go, "Criar HudMunicao");
        go.AddComponent<HudMunicao>();
    }

    static void GarantirHudEnergiaEspiritual()
    {
        if (Object.FindFirstObjectByType<HudEnergiaEspiritual>() != null) return;
        var go = new GameObject("HudEnergiaEspiritual");
        Undo.RegisterCreatedObjectUndo(go, "Criar HudEnergiaEspiritual");
        go.AddComponent<HudEnergiaEspiritual>();
    }

    static void GarantirCaderno()
    {
        if (Object.FindFirstObjectByType<CadernoCampo>() == null)
        {
            var go = new GameObject("CadernoCampo");
            Undo.RegisterCreatedObjectUndo(go, "Criar CadernoCampo");
            go.AddComponent<CadernoCampo>();
        }

        if (Object.FindFirstObjectByType<HudCaderno>() == null)
        {
            var go = new GameObject("HudCaderno");
            Undo.RegisterCreatedObjectUndo(go, "Criar HudCaderno");
            go.AddComponent<HudCaderno>();
        }

        if (Object.FindFirstObjectByType<HudPesquisa>() == null)
        {
            var go = new GameObject("HudPesquisa");
            Undo.RegisterCreatedObjectUndo(go, "Criar HudPesquisa");
            go.AddComponent<HudPesquisa>();
        }
    }

    static void GarantirObjetosEspectrais()
    {
        if (GameObject.Find("PistaEspectral") == null)
        {
            var go = new GameObject("PistaEspectral");
            Undo.RegisterCreatedObjectUndo(go, "Criar PistaEspectral");
            go.transform.position = LayoutPrototipo.PistaEspectral;
            go.transform.localScale = new Vector3(0.6f, 0.6f, 1f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CarregarSpriteQuadrado();
            sr.color = new Color(0.45f, 0.75f, 1f, 0.85f);
            sr.sortingOrder = 3;

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.5f, 1.5f);
            var pista = go.AddComponent<ObjetoEspectral>();
            pista.tipo = TipoObjetoEspectral.Pista;
            pista.textoPista = "Marca do avo: \"O novelo de cipo acalma o Curupira.\"";
            pista.idPaginaCaderno = CatalogoPaginasCap1.IdCurupira;
        }

        if (GameObject.Find("PlataformaEspectral") == null)
        {
            var go = new GameObject("PlataformaEspectral");
            Undo.RegisterCreatedObjectUndo(go, "Criar PlataformaEspectral");
            go.transform.position = LayoutPrototipo.PlataformaEspectral;
            go.transform.localScale = new Vector3(2.2f, 0.35f, 1f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CarregarSpriteQuadrado();
            sr.color = new Color(0.4f, 0.7f, 1f, 0.75f);
            sr.sortingOrder = 1;

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = false;
            col.size = new Vector2(1f, 1f);

            int camadaChao = LayerMask.NameToLayer("Chao");
            if (camadaChao >= 0) go.layer = camadaChao;

            var plat = go.AddComponent<ObjetoEspectral>();
            plat.tipo = TipoObjetoEspectral.Plataforma;
        }
    }

    static void GarantirColetaveisNorte()
    {
        CriarItensNorteAssets();
        var jambu = AssetDatabase.LoadAssetAtPath<ItemDados>("Assets/_Project/Data/Itens/Jambu.asset");
        var tucupi = AssetDatabase.LoadAssetAtPath<ItemDados>("Assets/_Project/Data/Itens/Tucupi.asset");
        var goma = AssetDatabase.LoadAssetAtPath<ItemDados>("Assets/_Project/Data/Itens/GomaDeTapioca.asset");
        var cipo = AssetDatabase.LoadAssetAtPath<ItemDados>("Assets/_Project/Data/Itens/Cipo.asset");
        var resina = AssetDatabase.LoadAssetAtPath<ItemDados>("Assets/_Project/Data/Itens/Resina.asset");

        if (GameObject.Find("Coletavel_Jambu") == null)
            CriarColetavelEditor("Coletavel_Jambu", LayoutPrototipo.Jambu, new Color(0.35f, 0.75f, 0.25f), jambu, 2);
        if (GameObject.Find("Coletavel_Tucupi") == null)
            CriarColetavelEditor("Coletavel_Tucupi", LayoutPrototipo.Tucupi, new Color(0.95f, 0.8f, 0.15f), tucupi, 3);
        if (GameObject.Find("Coletavel_Goma") == null)
            CriarColetavelEditor("Coletavel_Goma", LayoutPrototipo.Goma, new Color(0.95f, 0.95f, 0.9f), goma, 1);
        if (GameObject.Find("Coletavel_Cipo") == null)
            CriarColetavelEditor("Coletavel_Cipo", LayoutPrototipo.Cipo, new Color(0.55f, 0.7f, 0.35f), cipo, 3);
        if (GameObject.Find("Coletavel_Resina") == null)
            CriarColetavelEditor("Coletavel_Resina", LayoutPrototipo.Resina, new Color(0.7f, 0.4f, 0.15f), resina, 2);
        if (GameObject.Find("Coletavel_Jambu_Rio") == null)
            CriarColetavelEditor("Coletavel_Jambu_Rio", LayoutPrototipo.JambuRio, new Color(0.35f, 0.75f, 0.25f), jambu, 1);
        if (GameObject.Find("Coletavel_Tucupi_Rio") == null)
            CriarColetavelEditor("Coletavel_Tucupi_Rio", LayoutPrototipo.TucupiRio, new Color(0.95f, 0.8f, 0.15f), tucupi, 1);
        if (GameObject.Find("Coletavel_Goma_Rio") == null)
            CriarColetavelEditor("Coletavel_Goma_Rio", LayoutPrototipo.GomaRio, new Color(0.95f, 0.95f, 0.9f), goma, 1);
    }

    static void GarantirBarcaEEstacoes()
    {
        var barca = Object.FindFirstObjectByType<BarcaRio>();
        if (barca == null)
        {
            var barcaGo = new GameObject("BarcaRio");
            Undo.RegisterCreatedObjectUndo(barcaGo, "Criar BarcaRio");
            barcaGo.transform.position = LayoutPrototipo.Barca;
            barcaGo.transform.localScale = new Vector3(6.5f, 1.7f, 1f);

            var sr = barcaGo.AddComponent<SpriteRenderer>();
            sr.sprite = CarregarSpriteQuadrado();
            sr.color = new Color(0.45f, 0.3f, 0.18f);
            sr.sortingOrder = 1;

            var colBarca = barcaGo.AddComponent<BoxCollider2D>();
            colBarca.isTrigger = true;
            colBarca.size = new Vector2(1.4f, 1.6f);

            var interior = new GameObject("PontoInteriorBarca");
            Undo.RegisterCreatedObjectUndo(interior, "Criar PontoInteriorBarca");
            interior.transform.SetParent(barcaGo.transform);
            interior.transform.localPosition = new Vector3(0f, 0.55f, 0f);

            var saida = new GameObject("PontoSaidaBarca");
            Undo.RegisterCreatedObjectUndo(saida, "Criar PontoSaidaBarca");
            saida.transform.SetParent(barcaGo.transform);
            saida.transform.localPosition = new Vector3(0.7f, 0.12f, 0f);

            barca = barcaGo.AddComponent<BarcaRio>();
            barca.pontoInterior = interior.transform;
            barca.pontoSaida = saida.transform;
        }

        if (Object.FindFirstObjectByType<EstacaoFogao>() == null)
        {
            var fogaoGo = new GameObject("FogaoALenha");
            Undo.RegisterCreatedObjectUndo(fogaoGo, "Criar FogaoALenha");
            fogaoGo.transform.position = LayoutPrototipo.Fogao;
            fogaoGo.transform.localScale = new Vector3(0.75f, 0.75f, 1f);

            var srFogao = fogaoGo.AddComponent<SpriteRenderer>();
            srFogao.sprite = CarregarSpriteQuadrado();
            srFogao.color = new Color(0.85f, 0.25f, 0.1f);
            srFogao.sortingOrder = 2;

            var colFogao = fogaoGo.AddComponent<BoxCollider2D>();
            colFogao.isTrigger = true;
            colFogao.size = new Vector2(1.4f, 1.4f);
            fogaoGo.AddComponent<EstacaoFogao>();
        }

        if (Object.FindFirstObjectByType<EstacaoAlquimia>() == null)
        {
            var alquimiaGo = new GameObject("BancadaAlquimia");
            Undo.RegisterCreatedObjectUndo(alquimiaGo, "Criar BancadaAlquimia");
            alquimiaGo.transform.position = LayoutPrototipo.Alquimia;
            alquimiaGo.transform.localScale = new Vector3(0.75f, 0.75f, 1f);

            var srAlq = alquimiaGo.AddComponent<SpriteRenderer>();
            srAlq.sprite = CarregarSpriteQuadrado();
            srAlq.color = new Color(0.55f, 0.35f, 0.85f);
            srAlq.sortingOrder = 2;

            var colAlq = alquimiaGo.AddComponent<BoxCollider2D>();
            colAlq.isTrigger = true;
            colAlq.size = new Vector2(1.4f, 1.4f);
            alquimiaGo.AddComponent<EstacaoAlquimia>();
        }

        if (Object.FindFirstObjectByType<EstacaoPesquisa>() == null)
        {
            var pesquisaGo = new GameObject("MesaPesquisa");
            Undo.RegisterCreatedObjectUndo(pesquisaGo, "Criar MesaPesquisa");
            pesquisaGo.transform.position = LayoutPrototipo.Pesquisa;
            pesquisaGo.transform.localScale = new Vector3(0.8f, 0.6f, 1f);

            var srPesq = pesquisaGo.AddComponent<SpriteRenderer>();
            srPesq.sprite = CarregarSpriteQuadrado();
            srPesq.color = new Color(0.9f, 0.82f, 0.55f);
            srPesq.sortingOrder = 2;

            var colPesq = pesquisaGo.AddComponent<BoxCollider2D>();
            colPesq.isTrigger = true;
            colPesq.size = new Vector2(1.4f, 1.4f);
            pesquisaGo.AddComponent<EstacaoPesquisa>();
        }
    }

    static void GarantirCombatePrototipo()
    {
        if (GameObject.Find("Vegetacao_Cipo") == null)
        {
            var go = new GameObject("Vegetacao_Cipo");
            Undo.RegisterCreatedObjectUndo(go, "Criar Vegetacao_Cipo");
            go.transform.position = LayoutPrototipo.Vegetacao;
            go.transform.localScale = new Vector3(1.2f, 1.6f, 1f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CarregarSpriteQuadrado();
            sr.color = new Color(0.2f, 0.55f, 0.2f);
            sr.sortingOrder = 2;

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = false;
            col.size = new Vector2(1f, 1f);
            go.AddComponent<VegetacaoCortavel>();
        }

        if (GameObject.Find("CapangaTreino") == null)
        {
            var go = new GameObject("CapangaTreino");
            Undo.RegisterCreatedObjectUndo(go, "Criar CapangaTreino");
            go.transform.position = LayoutPrototipo.Capanga;
            go.transform.localScale = new Vector3(1f, 1.6f, 1f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CarregarSpriteQuadrado();
            sr.color = new Color(0.55f, 0.2f, 0.2f);
            sr.sortingOrder = 2;

            var col = go.AddComponent<BoxCollider2D>();
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
            var go = new GameObject("SombraTreino");
            Undo.RegisterCreatedObjectUndo(go, "Criar SombraTreino");
            go.transform.position = LayoutPrototipo.Sombra;
            go.transform.localScale = new Vector3(1.1f, 1.4f, 1f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CarregarSpriteQuadrado();
            sr.color = new Color(0.55f, 0.25f, 0.75f, 0.9f);
            sr.sortingOrder = 2;

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1f, 1f);
            go.AddComponent<InimigoSombra>();
        }

        if (GameObject.Find("BarreiraAlavanca") == null)
        {
            var barreira = new GameObject("BarreiraAlavanca");
            Undo.RegisterCreatedObjectUndo(barreira, "Criar BarreiraAlavanca");
            barreira.transform.position = LayoutPrototipo.Barreira;
            barreira.transform.localScale = new Vector3(0.5f, 2.4f, 1f);

            var srB = barreira.AddComponent<SpriteRenderer>();
            srB.sprite = CarregarSpriteQuadrado();
            srB.color = new Color(0.4f, 0.32f, 0.22f);
            srB.sortingOrder = 2;

            var colB = barreira.AddComponent<BoxCollider2D>();
            colB.isTrigger = false;
            colB.size = new Vector2(1f, 1f);
            int camadaChao = LayerMask.NameToLayer("Chao");
            if (camadaChao >= 0) barreira.layer = camadaChao;
            barreira.AddComponent<BarreiraPuzzle>();
        }

        if (GameObject.Find("AlavancaChicote") == null)
        {
            var go = new GameObject("AlavancaChicote");
            Undo.RegisterCreatedObjectUndo(go, "Criar AlavancaChicote");
            go.transform.position = LayoutPrototipo.Alavanca;
            go.transform.localScale = new Vector3(0.4f, 1.2f, 1f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CarregarSpriteQuadrado();
            sr.color = new Color(0.75f, 0.55f, 0.2f);
            sr.sortingOrder = 2;

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.4f, 1.4f);

            var alavanca = go.AddComponent<AlavancaChicote>();
            var barreira = GameObject.Find("BarreiraAlavanca");
            if (barreira != null)
                alavanca.barreiras = new[] { barreira };
        }
    }

    static void GarantirPuzzlesIndy()
    {
        CriarItensNorteAssets();
        int camadaChao = LayerMask.NameToLayer("Chao");
        var sprite = CarregarSpriteQuadrado();

        if (GameObject.Find("PlataformaEspectral_2") == null)
        {
            var go = new GameObject("PlataformaEspectral_2");
            Undo.RegisterCreatedObjectUndo(go, "Criar PlataformaEspectral_2");
            go.transform.position = LayoutPrototipo.PlataformaEspectral2;
            go.transform.localScale = new Vector3(1.6f, 0.32f, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = new Color(0.4f, 0.7f, 1f, 0.75f);
            sr.sortingOrder = 1;
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = false;
            if (camadaChao >= 0) go.layer = camadaChao;
            var plat = go.AddComponent<ObjetoEspectral>();
            plat.tipo = TipoObjetoEspectral.Plataforma;
        }

        CriarPisoPuzzleEditor("PisoFalso_A", LayoutPrototipo.PisoA, false, camadaChao, sprite);
        CriarPisoPuzzleEditor("PisoFalso_B", LayoutPrototipo.PisoB, true, camadaChao, sprite);
        CriarPisoPuzzleEditor("PisoFalso_C", LayoutPrototipo.PisoC, false, camadaChao, sprite);

        if (GameObject.Find("PisoChegadaRuina") == null)
        {
            var go = new GameObject("PisoChegadaRuina");
            Undo.RegisterCreatedObjectUndo(go, "Criar PisoChegadaRuina");
            go.transform.position = LayoutPrototipo.PisoChegada;
            go.transform.localScale = new Vector3(2.2f, 0.35f, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = new Color(0.5f, 0.42f, 0.3f);
            sr.sortingOrder = 1;
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = false;
            if (camadaChao >= 0) go.layer = camadaChao;
        }

        if (GameObject.Find("PistaPisosFalsos") == null)
        {
            var go = new GameObject("PistaPisosFalsos");
            Undo.RegisterCreatedObjectUndo(go, "Criar PistaPisosFalsos");
            go.transform.position = LayoutPrototipo.PistaPisos;
            go.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = new Color(0.45f, 0.75f, 1f, 0.85f);
            sr.sortingOrder = 3;
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.6f, 1.6f);
            var pista = go.AddComponent<ObjetoEspectral>();
            pista.tipo = TipoObjetoEspectral.Pista;
            pista.textoPista = "Pisos mentem. O amuleto pinta o falso de vermelho.";
        }

        if (GameObject.Find("ReliquiaMarajoara") == null)
        {
            var go = new GameObject("ReliquiaMarajoara");
            Undo.RegisterCreatedObjectUndo(go, "Criar ReliquiaMarajoara");
            go.transform.position = LayoutPrototipo.Reliquia;
            go.transform.localScale = new Vector3(0.45f, 0.45f, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = new Color(0.85f, 0.7f, 0.25f);
            sr.sortingOrder = 3;
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.4f, 1.4f);
            var rel = go.AddComponent<ReliquiaPuzzle>();
            rel.item = AssetDatabase.LoadAssetAtPath<ItemDados>("Assets/_Project/Data/Itens/MarcaMarajoara.asset");
            rel.idPaginaCaderno = CatalogoPaginasCap1.IdRuinaMarajoara;
        }
    }

    static void CriarPisoPuzzleEditor(string nome, Vector3 posicao, bool seguro, int camadaChao, Sprite sprite)
    {
        if (GameObject.Find(nome) != null) return;
        var go = new GameObject(nome);
        Undo.RegisterCreatedObjectUndo(go, "Criar " + nome);
        go.transform.position = posicao;
        go.transform.localScale = new Vector3(1.35f, 0.32f, 1f);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = new Color(0.48f, 0.4f, 0.28f);
        sr.sortingOrder = 1;
        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = false;
        if (camadaChao >= 0) go.layer = camadaChao;
        var piso = go.AddComponent<PisoFalso>();
        piso.seguro = seguro;
    }

    static void CriarColetavelEditor(string nome, Vector3 posicao, Color cor, ItemDados item, int quantidade = 1)
    {
        var go = new GameObject(nome);
        Undo.RegisterCreatedObjectUndo(go, "Criar " + nome);
        go.transform.position = posicao;
        go.transform.localScale = new Vector3(0.55f, 0.55f, 1f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = CarregarSpriteQuadrado();
        sr.color = cor;
        sr.sortingOrder = 2;

        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(1.2f, 1.2f);

        var coletavel = go.AddComponent<ColetavelItem>();
        coletavel.item = item;
        coletavel.quantidade = quantidade;
    }

    static void GarantirGerenciadorDialogo()
    {
        if (Object.FindFirstObjectByType<GerenciadorDialogo>() != null) return;
        var go = new GameObject("GerenciadorDialogo");
        Undo.RegisterCreatedObjectUndo(go, "Criar GerenciadorDialogo");
        go.AddComponent<GerenciadorDialogo>();
    }

    static void GarantirCheckpointInicial()
    {
        CriarCheckpointEditor("CheckpointInicial", LayoutPrototipo.Checkpoint, true);
    }

    static void GarantirBlocosCap1()
    {
        var sprite = CarregarSpriteQuadrado();

        CriarFaixaEditor("FaixaVila", LayoutPrototipo.ZonaVila.x, 40f, new Color(0.45f, 0.4f, 0.35f, 0.55f), sprite);
        CriarFaixaEditor("FaixaRio", LayoutPrototipo.ZonaRio.x, 32f, new Color(0.2f, 0.45f, 0.65f, 0.55f), sprite);
        CriarFaixaEditor("FaixaMata", LayoutPrototipo.ZonaMata.x, 36f, new Color(0.25f, 0.45f, 0.22f, 0.5f), sprite);
        CriarFaixaEditor("FaixaRuinas", LayoutPrototipo.ZonaRuinas.x, 32f, new Color(0.55f, 0.45f, 0.28f, 0.5f), sprite);
        CriarFaixaEditor("FaixaArena", LayoutPrototipo.ZonaArena.x, 28f, new Color(0.5f, 0.22f, 0.18f, 0.55f), sprite);

        CriarZonaEditor("Zona_Vila", LayoutPrototipo.ZonaVila, "Vila", 38f);
        CriarZonaEditor("Zona_Rio", LayoutPrototipo.ZonaRio, "Rio", 30f);
        CriarZonaEditor("Zona_Mata", LayoutPrototipo.ZonaMata, "Mata", 34f);
        CriarZonaEditor("Zona_Ruinas", LayoutPrototipo.ZonaRuinas, "Ruinas", 30f);
        CriarZonaEditor("Zona_Arena", LayoutPrototipo.ZonaArena, "Arena", 26f);

        if (GameObject.Find("AguaRio") == null)
        {
            var agua = new GameObject("AguaRio");
            Undo.RegisterCreatedObjectUndo(agua, "Criar AguaRio");
            agua.transform.position = LayoutPrototipo.AguaRio;
            agua.transform.localScale = new Vector3(28f, 0.7f, 1f);
            var srAgua = agua.AddComponent<SpriteRenderer>();
            srAgua.sprite = sprite;
            srAgua.color = new Color(0.15f, 0.4f, 0.7f, 0.85f);
            srAgua.sortingOrder = 0;
        }

        GarantirCanoaECaisEditor(sprite);
        GarantirEncontroBotoEditor(sprite);
        GarantirEncontroIaraEditor(sprite);
        GarantirEncontroCurupiraEditor(sprite);

        CriarCheckpointEditor("CheckpointRio", LayoutPrototipo.CheckpointRio, false);
        CriarCheckpointEditor("CheckpointArena", LayoutPrototipo.CheckpointArena, false);

        GarantirFocoEditor("FocoCorrupcao_A", LayoutPrototipo.FocoA, sprite);
        GarantirFocoEditor("FocoCorrupcao_B", LayoutPrototipo.FocoB, sprite);
        GarantirFocoEditor("FocoCorrupcao_C", LayoutPrototipo.FocoC, sprite);

        if (GameObject.Find("SombraArena") == null)
        {
            var go = new GameObject("SombraArena");
            Undo.RegisterCreatedObjectUndo(go, "Criar SombraArena");
            go.transform.position = LayoutPrototipo.SombraArena;
            go.transform.localScale = new Vector3(1.1f, 1.4f, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = new Color(0.55f, 0.25f, 0.75f, 0.9f);
            sr.sortingOrder = 2;
            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            go.AddComponent<InimigoSombra>();
        }
    }

    static void CriarZonaEditor(string nome, Vector3 posicao, string rotulo, float largura)
    {
        if (GameObject.Find(nome) != null) return;
        var go = new GameObject(nome);
        Undo.RegisterCreatedObjectUndo(go, "Criar " + nome);
        go.transform.position = posicao;
        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(largura, 8f);
        var zona = go.AddComponent<MarcadorZona>();
        zona.nomeZona = rotulo;
    }

    static void CriarFaixaEditor(string nome, float x, float largura, Color cor, Sprite sprite)
    {
        if (GameObject.Find(nome) != null) return;
        var go = new GameObject(nome);
        Undo.RegisterCreatedObjectUndo(go, "Criar " + nome);
        go.transform.position = new Vector3(x, 0.12f, 0f);
        go.transform.localScale = new Vector3(largura, 0.22f, 1f);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = cor;
        sr.sortingOrder = 1;
    }

    static void GarantirCanoaECaisEditor(Sprite sprite)
    {
        GameObject barreira = GameObject.Find("BarreiraCais");
        if (barreira == null)
        {
            barreira = new GameObject("BarreiraCais");
            Undo.RegisterCreatedObjectUndo(barreira, "Criar BarreiraCais");
            barreira.transform.position = LayoutPrototipo.BarreiraCais;
            barreira.transform.localScale = new Vector3(0.8f, 5.2f, 1f);
            var srB = barreira.AddComponent<SpriteRenderer>();
            srB.sprite = sprite;
            srB.color = new Color(0.4f, 0.28f, 0.16f);
            srB.sortingOrder = 2;
            var colB = barreira.AddComponent<BoxCollider2D>();
            colB.isTrigger = false;
            int camadaChao = LayerMask.NameToLayer("Chao");
            if (camadaChao >= 0) barreira.layer = camadaChao;
            barreira.AddComponent<BarreiraPuzzle>();
        }

        GameObject canoa = GameObject.Find("CanoaRio");
        if (canoa == null)
        {
            canoa = new GameObject("CanoaRio");
            Undo.RegisterCreatedObjectUndo(canoa, "Criar CanoaRio");
            canoa.transform.position = LayoutPrototipo.CanoaCais;
            canoa.transform.localScale = new Vector3(2.6f, 0.45f, 1f);
            var sr = canoa.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = new Color(0.55f, 0.32f, 0.16f);
            sr.sortingOrder = 3;
            var col = canoa.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.1f, 1.8f);
        }

        if (canoa.GetComponent<CanoaRio>() == null)
            canoa.AddComponent<CanoaRio>();

        if (GameObject.Find("Coletavel_ChaveCanoa") == null)
        {
            var chaveAsset = AssetDatabase.LoadAssetAtPath<ItemDados>("Assets/_Project/Data/Itens/ChaveDaCanoa.asset");
            var chaveGo = new GameObject("Coletavel_ChaveCanoa");
            Undo.RegisterCreatedObjectUndo(chaveGo, "Criar Coletavel_ChaveCanoa");
            chaveGo.transform.position = LayoutPrototipo.ChaveCanoaCais;
            chaveGo.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            var srChave = chaveGo.AddComponent<SpriteRenderer>();
            srChave.sprite = sprite;
            srChave.color = new Color(0.55f, 0.9f, 1f, 0.95f);
            srChave.sortingOrder = 4;
            var colChave = chaveGo.AddComponent<BoxCollider2D>();
            colChave.isTrigger = true;
            colChave.size = new Vector2(1.4f, 1.4f);

            var coletavel = chaveGo.AddComponent<ColetavelItem>();
            coletavel.item = chaveAsset;
            coletavel.quantidade = 1;

            var espectral = chaveGo.AddComponent<ObjetoEspectral>();
            espectral.tipo = TipoObjetoEspectral.Pista;
            espectral.textoPista = "Brilho na lama: Chave da Canoa";
        }
    }

    static void GarantirEncontroBotoEditor(Sprite sprite)
    {
        GameObject barreira = GameObject.Find("BarreiraRioBoto");
        if (barreira == null)
        {
            barreira = new GameObject("BarreiraRioBoto");
            Undo.RegisterCreatedObjectUndo(barreira, "Criar BarreiraRioBoto");
            barreira.transform.position = LayoutPrototipo.BarreiraBoto;
            barreira.transform.localScale = new Vector3(0.8f, 5.2f, 1f);
            var srB = barreira.AddComponent<SpriteRenderer>();
            srB.sprite = sprite;
            srB.color = new Color(0.35f, 0.12f, 0.32f);
            srB.sortingOrder = 2;
            var colB = barreira.AddComponent<BoxCollider2D>();
            colB.isTrigger = false;
            int camadaChao = LayerMask.NameToLayer("Chao");
            if (camadaChao >= 0) barreira.layer = camadaChao;
            barreira.AddComponent<BarreiraPuzzle>();
        }

        GameObject boto = GameObject.Find("BloqueioBoto");
        if (boto == null)
        {
            boto = new GameObject("BloqueioBoto");
            Undo.RegisterCreatedObjectUndo(boto, "Criar BloqueioBoto");
            boto.transform.position = LayoutPrototipo.BloqueioBoto;
            boto.transform.localScale = new Vector3(1.4f, 1.6f, 1f);
            var sr = boto.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = new Color(0.55f, 0.2f, 0.45f);
            sr.sortingOrder = 2;
            var col = boto.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(2.2f, 1.8f);
        }

        var placeholder = boto.GetComponent<MarcadorEncontro>();
        if (placeholder != null) Object.DestroyImmediate(placeholder);

        var encontro = boto.GetComponent<EncontroBoto>();
        if (encontro == null) encontro = boto.AddComponent<EncontroBoto>();
        encontro.barreiras = new[] { barreira };
    }

    static void GarantirEncontroIaraEditor(Sprite sprite)
    {
        GameObject barreira = GameObject.Find("BarreiraRioIara");
        if (barreira == null)
        {
            barreira = new GameObject("BarreiraRioIara");
            Undo.RegisterCreatedObjectUndo(barreira, "Criar BarreiraRioIara");
            barreira.transform.position = LayoutPrototipo.BarreiraIara;
            barreira.transform.localScale = new Vector3(0.8f, 5.2f, 1f);
            var srB = barreira.AddComponent<SpriteRenderer>();
            srB.sprite = sprite;
            srB.color = new Color(0.08f, 0.28f, 0.38f);
            srB.sortingOrder = 2;
            var colB = barreira.AddComponent<BoxCollider2D>();
            colB.isTrigger = false;
            int camadaChao = LayerMask.NameToLayer("Chao");
            if (camadaChao >= 0) barreira.layer = camadaChao;
            barreira.AddComponent<BarreiraPuzzle>();
        }

        GameObject iara = GameObject.Find("EncontroIara");
        if (iara == null)
        {
            iara = new GameObject("EncontroIara");
            Undo.RegisterCreatedObjectUndo(iara, "Criar EncontroIara");
            iara.transform.position = LayoutPrototipo.Iara;
            iara.transform.localScale = new Vector3(1.2f, 1.7f, 1f);
            var sr = iara.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = new Color(0.12f, 0.35f, 0.42f);
            sr.sortingOrder = 2;
            var col = iara.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(2.4f, 2.2f);
        }

        var placeholder = iara.GetComponent<MarcadorEncontro>();
        if (placeholder != null) Object.DestroyImmediate(placeholder);

        var encontro = iara.GetComponent<EncontroIara>();
        if (encontro == null) encontro = iara.AddComponent<EncontroIara>();
        encontro.barreiras = new[] { barreira };
    }

    static void GarantirEncontroCurupiraEditor(Sprite sprite)
    {
        GameObject portao = GameObject.Find("BarreiraArenaCurupira");
        if (portao == null)
        {
            portao = new GameObject("BarreiraArenaCurupira");
            Undo.RegisterCreatedObjectUndo(portao, "Criar BarreiraArenaCurupira");
            portao.transform.position = LayoutPrototipo.BarreiraArena;
            portao.transform.localScale = new Vector3(0.8f, 5.2f, 1f);
            var srP = portao.AddComponent<SpriteRenderer>();
            srP.sprite = sprite;
            srP.color = new Color(0.35f, 0.12f, 0.08f);
            srP.sortingOrder = 2;
            var colP = portao.AddComponent<BoxCollider2D>();
            colP.isTrigger = false;
            int camadaChao = LayerMask.NameToLayer("Chao");
            if (camadaChao >= 0) portao.layer = camadaChao;
            portao.AddComponent<BarreiraPuzzle>();
        }

        GameObject curupira = GameObject.Find("EncontroCurupira");
        if (curupira == null)
        {
            curupira = new GameObject("EncontroCurupira");
            Undo.RegisterCreatedObjectUndo(curupira, "Criar EncontroCurupira");
            curupira.transform.position = LayoutPrototipo.Curupira;
            curupira.transform.localScale = new Vector3(1.3f, 2.1f, 1f);
            var sr = curupira.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = new Color(0.55f, 0.12f, 0.08f);
            sr.sortingOrder = 2;
            var col = curupira.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(1.6f, 1.4f);
        }

        var placeholder = curupira.GetComponent<MarcadorEncontro>();
        if (placeholder != null) Object.DestroyImmediate(placeholder);

        var encontro = curupira.GetComponent<EncontroCurupira>();
        if (encontro == null) encontro = curupira.AddComponent<EncontroCurupira>();
        encontro.portaoArena = portao;
    }

    static void CriarEncontroEditor(string nome, Vector3 posicao, Color cor, Vector3 escala, string titulo, string dica, Sprite sprite)
    {
        if (GameObject.Find(nome) != null) return;
        var go = new GameObject(nome);
        Undo.RegisterCreatedObjectUndo(go, "Criar " + nome);
        go.transform.position = posicao;
        go.transform.localScale = escala;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = cor;
        sr.sortingOrder = 2;
        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(1.6f, 1.4f);
        var enc = go.AddComponent<MarcadorEncontro>();
        enc.titulo = titulo;
        enc.dica = dica;
    }

    static void CriarCheckpointEditor(string nome, Vector3 posicao, bool iniciar)
    {
        if (GameObject.Find(nome) != null) return;
        var go = new GameObject(nome);
        Undo.RegisterCreatedObjectUndo(go, "Criar " + nome);
        go.transform.position = posicao;
        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(2f, 2f);
        var checkpoint = go.AddComponent<Checkpoint>();
        checkpoint.ativarAoIniciar = iniciar;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = CarregarSpriteQuadrado();
        sr.color = new Color(0.3f, 0.9f, 0.4f, 0.7f);
    }

    static void GarantirFocoEditor(string nome, Vector3 posicao, Sprite sprite)
    {
        GameObject go = GameObject.Find(nome);
        if (go == null)
        {
            go = new GameObject(nome);
            Undo.RegisterCreatedObjectUndo(go, "Criar " + nome);
            go.transform.position = posicao;
            go.transform.localScale = new Vector3(0.55f, 0.9f, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = new Color(0.7f, 0.2f, 0.85f, 0.9f);
            sr.sortingOrder = 3;
            var col = go.AddComponent<BoxCollider2D>();
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
        Undo.RegisterCreatedObjectUndo(go, "Criar ZonaMorte");
        go.transform.position = new Vector3(27f, -12f, 0f);
        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(160f, 4f);
        go.AddComponent<ZonaMorte>();
    }

    static void GarantirDonaNair()
    {
        CriarNpcEditor(
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
        CriarNpcEditor(
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
        CriarNpcEditor(
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
                }
            },
            new[] { CatalogoItensNorte.IdPenteDeOsso },
            new[]
            {
                new FalaDialogo
                {
                    personagem = "Seu Zeca",
                    emocao = "serio",
                    texto = "Segura [V] na beira do cais pra achar a chave na lama, e usa [V] na canoa pra ver a aura do Boto na agua turva. O pente da Iara ja ta com voce."
                }
            });
    }

    static void CriarNpcEditor(string nome, Vector3 posicao, Color cor, Vector3 escala, string[] paginasCaderno, FalaDialogo[] falas, string[] idsItens = null, FalaDialogo[] falasApos = null)
    {
        var existente = GameObject.Find(nome);
        NpcDialogo npc;
        if (existente != null)
        {
            npc = existente.GetComponent<NpcDialogo>();
        }
        else
        {
            var go = new GameObject(nome);
            Undo.RegisterCreatedObjectUndo(go, "Criar " + nome);
            go.transform.position = posicao;
            go.transform.localScale = escala;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CarregarSpriteQuadrado();
            sr.color = cor;
            sr.sortingOrder = 2;

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(2.2f, 1.4f);

            npc = go.AddComponent<NpcDialogo>();
        }

        if (npc == null) return;
        npc.falas = falas;
        npc.paginasAoConversar = paginasCaderno;
        npc.idsItensAoConversar = idsItens;
        npc.falasAposEntrega = falasApos;
        EditorUtility.SetDirty(npc);
    }

    static Sprite CarregarSpriteQuadrado()
    {
        foreach (string guid in AssetDatabase.FindAssets("t:Sprite"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.Contains("Square") && !path.EndsWith("Square.png")) continue;
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite != null) return sprite;
        }

        // Fallback: sprite embutido do projeto Universal 2D (guid comum do Square)
        return AssetDatabase.LoadAssetAtPath<Sprite>("Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/v2/Square.png");
    }
}
