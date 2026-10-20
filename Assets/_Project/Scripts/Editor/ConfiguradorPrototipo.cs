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

        Debug.Log("[Lendas] Prototipo configurado: inventario, Barca, Fogao, dialogo e HUD.");
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
            "Oferenda ao Boto e a Iara. Concede o buff Treme.", CategoriaItem.Comida);
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

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Lendas] Itens Norte criados em " + pasta);
    }

    static void CriarOuAtualizarItem(string caminho, string id, string nome, string descricao, CategoriaItem categoria)
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
        item.empilhamentoMaximo = 99;
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
        GarantirGerenciadorDialogo();
        GarantirCheckpointInicial();
        GarantirZonaMorte();
        GarantirDonaNair();
        GarantirNamoradaDoBoto();
        GarantirPescadorDaIara();
        GarantirColetaveisNorte();
        GarantirBarcaEEstacoes();
        GarantirCombatePrototipo();

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

    static void GarantirColetaveisNorte()
    {
        CriarItensNorteAssets();
        var jambu = AssetDatabase.LoadAssetAtPath<ItemDados>("Assets/_Project/Data/Itens/Jambu.asset");
        var tucupi = AssetDatabase.LoadAssetAtPath<ItemDados>("Assets/_Project/Data/Itens/Tucupi.asset");
        var goma = AssetDatabase.LoadAssetAtPath<ItemDados>("Assets/_Project/Data/Itens/GomaDeTapioca.asset");
        var cipo = AssetDatabase.LoadAssetAtPath<ItemDados>("Assets/_Project/Data/Itens/Cipo.asset");
        var resina = AssetDatabase.LoadAssetAtPath<ItemDados>("Assets/_Project/Data/Itens/Resina.asset");

        if (GameObject.Find("Coletavel_Jambu") == null)
            CriarColetavelEditor("Coletavel_Jambu", new Vector3(-1f, 1.1f, 0f), new Color(0.35f, 0.75f, 0.25f), jambu, 2);
        if (GameObject.Find("Coletavel_Tucupi") == null)
            CriarColetavelEditor("Coletavel_Tucupi", new Vector3(0.5f, 1.1f, 0f), new Color(0.95f, 0.8f, 0.15f), tucupi, 3);
        if (GameObject.Find("Coletavel_Goma") == null)
            CriarColetavelEditor("Coletavel_Goma", new Vector3(4f, 1.1f, 0f), new Color(0.95f, 0.95f, 0.9f), goma, 1);
        if (GameObject.Find("Coletavel_Cipo") == null)
            CriarColetavelEditor("Coletavel_Cipo", new Vector3(-2.2f, 1.1f, 0f), new Color(0.55f, 0.7f, 0.35f), cipo, 3);
        if (GameObject.Find("Coletavel_Resina") == null)
            CriarColetavelEditor("Coletavel_Resina", new Vector3(5.2f, 1.1f, 0f), new Color(0.7f, 0.4f, 0.15f), resina, 2);
    }

    static void GarantirBarcaEEstacoes()
    {
        var barca = Object.FindFirstObjectByType<BarcaRio>();
        if (barca == null)
        {
            var barcaGo = new GameObject("BarcaRio");
            Undo.RegisterCreatedObjectUndo(barcaGo, "Criar BarcaRio");
            barcaGo.transform.position = new Vector3(-11f, 1.2f, 0f);
            barcaGo.transform.localScale = new Vector3(3.2f, 1.4f, 1f);

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
            interior.transform.localPosition = new Vector3(0f, 0.8f, 0f);

            var saida = new GameObject("PontoSaidaBarca");
            Undo.RegisterCreatedObjectUndo(saida, "Criar PontoSaidaBarca");
            saida.transform.SetParent(barcaGo.transform);
            saida.transform.localPosition = new Vector3(2.2f, 0f, 0f);

            barca = barcaGo.AddComponent<BarcaRio>();
            barca.pontoInterior = interior.transform;
            barca.pontoSaida = saida.transform;
        }

        if (Object.FindFirstObjectByType<EstacaoFogao>() == null)
        {
            var fogaoGo = new GameObject("FogaoALenha");
            Undo.RegisterCreatedObjectUndo(fogaoGo, "Criar FogaoALenha");
            fogaoGo.transform.SetParent(barca.transform);
            fogaoGo.transform.position = new Vector3(-11.6f, 2.2f, 0f);
            fogaoGo.transform.localScale = new Vector3(0.7f, 0.7f, 1f);

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
            alquimiaGo.transform.SetParent(barca.transform);
            alquimiaGo.transform.position = new Vector3(-10.2f, 2.2f, 0f);
            alquimiaGo.transform.localScale = new Vector3(0.7f, 0.7f, 1f);

            var srAlq = alquimiaGo.AddComponent<SpriteRenderer>();
            srAlq.sprite = CarregarSpriteQuadrado();
            srAlq.color = new Color(0.55f, 0.35f, 0.85f);
            srAlq.sortingOrder = 2;

            var colAlq = alquimiaGo.AddComponent<BoxCollider2D>();
            colAlq.isTrigger = true;
            colAlq.size = new Vector2(1.4f, 1.4f);
            alquimiaGo.AddComponent<EstacaoAlquimia>();
        }
    }

    static void GarantirCombatePrototipo()
    {
        if (GameObject.Find("Vegetacao_Cipo") == null)
        {
            var go = new GameObject("Vegetacao_Cipo");
            Undo.RegisterCreatedObjectUndo(go, "Criar Vegetacao_Cipo");
            go.transform.position = new Vector3(9f, 1.2f, 0f);
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
            go.transform.position = new Vector3(12f, 1.3f, 0f);
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
        if (Object.FindFirstObjectByType<Checkpoint>() != null) return;

        var go = new GameObject("CheckpointInicial");
        Undo.RegisterCreatedObjectUndo(go, "Criar Checkpoint");
        go.transform.position = new Vector3(-6f, 1f, 0f);
        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(2f, 2f);
        var checkpoint = go.AddComponent<Checkpoint>();
        checkpoint.ativarAoIniciar = true;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = CarregarSpriteQuadrado();
        sr.color = new Color(0.3f, 0.9f, 0.4f, 0.7f);
    }

    static void GarantirZonaMorte()
    {
        if (Object.FindFirstObjectByType<ZonaMorte>() != null) return;

        var go = new GameObject("ZonaMorte");
        Undo.RegisterCreatedObjectUndo(go, "Criar ZonaMorte");
        go.transform.position = new Vector3(0f, -12f, 0f);
        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(80f, 4f);
        go.AddComponent<ZonaMorte>();
    }

    static void GarantirDonaNair()
    {
        CriarNpcEditor(
            "DonaNair",
            new Vector3(-3.5f, 1.2f, 0f),
            new Color(1f, 0.75f, 0.2f),
            new Vector3(1.1f, 1.8f, 1f),
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
        CriarNpcEditor(
            "NamoradaDoBoto",
            new Vector3(2.5f, 1.2f, 0f),
            new Color(1f, 0.45f, 0.7f),
            new Vector3(1f, 1.6f, 1f),
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
                    texto = "Os mais velhos dizem que aquele rapaz e o Boto. Agora o rio esta doente — e ele tambem."
                },
                new FalaDialogo
                {
                    personagem = "Lia",
                    emocao = "esperanca",
                    texto = "Se voce acalmar o Boto, talvez ele volte a aparecer nas noites claras. Eu ainda espero."
                }
            });
    }

    static void GarantirPescadorDaIara()
    {
        CriarNpcEditor(
            "PescadorDaIara",
            new Vector3(6.5f, 1.2f, 0f),
            new Color(0.35f, 0.65f, 1f),
            new Vector3(1.1f, 1.7f, 1f),
            new[]
            {
                new FalaDialogo
                {
                    personagem = "Seu Zeca",
                    emocao = "assustado",
                    texto = "A Iara protegia a gente! Com o canto dela, as piranhas se afastavam e a rede vinha cheia."
                },
                new FalaDialogo
                {
                    personagem = "Seu Zeca",
                    emocao = "raiva",
                    texto = "Agora ela arrasta pescador pro fundo. A agua ta preta da poluicao do Barao — a corrupcao pegou nela tambem."
                },
                new FalaDialogo
                {
                    personagem = "Seu Zeca",
                    emocao = "serio",
                    texto = "Leve Tacaca de oferenda e cuidado com o canto. So a Gota de Quartzo dela abre o caminho ate o Curupira."
                }
            });
    }

    static void CriarNpcEditor(string nome, Vector3 posicao, Color cor, Vector3 escala, FalaDialogo[] falas)
    {
        if (GameObject.Find(nome) != null) return;

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

        var npc = go.AddComponent<NpcDialogo>();
        npc.falas = falas;
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
