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
        GarantirComponente<GerenciadorDialogo>("GerenciadorDialogo");
        GarantirCheckpoint();
        GarantirZonaMorte();
        GarantirDonaNair();
        GarantirNamoradaDoBoto();
        GarantirPescadorDaIara();
        GarantirColetaveisNorte();
        GarantirBarcaEEstacoes();
        GarantirCombatePrototipo();

        Debug.Log("[Lendas] Bootstrap: Facao (J / clique esq.), Barca, inventario (I).");
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
    }

    static void GarantirCombatePrototipo()
    {
        if (GameObject.Find("Vegetacao_Cipo") == null)
        {
            var go = CriarMarcador("Vegetacao_Cipo", new Vector3(9f, 1.2f, 0f),
                new Color(0.2f, 0.55f, 0.2f), new Vector3(1.2f, 1.6f, 1f));
            var col = go.GetComponent<BoxCollider2D>();
            col.isTrigger = false;
            col.size = new Vector2(1f, 1f);
            go.AddComponent<VegetacaoCortavel>();
        }

        if (GameObject.Find("CapangaTreino") == null)
        {
            var go = CriarMarcador("CapangaTreino", new Vector3(12f, 1.3f, 0f),
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
    }

    static void GarantirColetaveisNorte()
    {
        CatalogoItensNorte.Garantir();

        if (GameObject.Find("Coletavel_Jambu") == null)
            CriarColetavel("Coletavel_Jambu", new Vector3(-1f, 1.1f, 0f), new Color(0.35f, 0.75f, 0.25f), CatalogoItensNorte.Jambu, 2);
        if (GameObject.Find("Coletavel_Tucupi") == null)
            CriarColetavel("Coletavel_Tucupi", new Vector3(0.5f, 1.1f, 0f), new Color(0.95f, 0.8f, 0.15f), CatalogoItensNorte.Tucupi, 3);
        if (GameObject.Find("Coletavel_Goma") == null)
            CriarColetavel("Coletavel_Goma", new Vector3(4f, 1.1f, 0f), new Color(0.95f, 0.95f, 0.9f), CatalogoItensNorte.GomaDeTapioca, 1);
        // 3 cipo + 2 resina = flechas e novelo
        if (GameObject.Find("Coletavel_Cipo") == null)
            CriarColetavel("Coletavel_Cipo", new Vector3(-2.2f, 1.1f, 0f), new Color(0.55f, 0.7f, 0.35f), CatalogoItensNorte.Cipo, 3);
        if (GameObject.Find("Coletavel_Resina") == null)
            CriarColetavel("Coletavel_Resina", new Vector3(5.2f, 1.1f, 0f), new Color(0.7f, 0.4f, 0.15f), CatalogoItensNorte.Resina, 2);
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
            var barcaGo = CriarMarcador("BarcaRio", new Vector3(-11f, 1.2f, 0f),
                new Color(0.45f, 0.3f, 0.18f), new Vector3(3.2f, 1.4f, 1f));
            var colBarca = barcaGo.GetComponent<BoxCollider2D>();
            colBarca.size = new Vector2(1.4f, 1.6f);

            var interior = new GameObject("PontoInteriorBarca");
            interior.transform.SetParent(barcaGo.transform);
            interior.transform.localPosition = new Vector3(0f, 0.8f, 0f);

            var saida = new GameObject("PontoSaidaBarca");
            saida.transform.SetParent(barcaGo.transform);
            saida.transform.localPosition = new Vector3(2.2f, 0f, 0f);

            barca = barcaGo.AddComponent<BarcaRio>();
            barca.pontoInterior = interior.transform;
            barca.pontoSaida = saida.transform;
        }

        if (Object.FindFirstObjectByType<EstacaoFogao>() == null)
        {
            var fogaoGo = CriarMarcador("FogaoALenha", new Vector3(-11.6f, 2.2f, 0f),
                new Color(0.85f, 0.25f, 0.1f), new Vector3(0.7f, 0.7f, 1f));
            fogaoGo.transform.SetParent(barca.transform, true);
            var colFogao = fogaoGo.GetComponent<BoxCollider2D>();
            colFogao.size = new Vector2(1.4f, 1.4f);
            fogaoGo.AddComponent<EstacaoFogao>();
        }

        if (Object.FindFirstObjectByType<EstacaoAlquimia>() == null)
        {
            var alquimiaGo = CriarMarcador("BancadaAlquimia", new Vector3(-10.2f, 2.2f, 0f),
                new Color(0.55f, 0.35f, 0.85f), new Vector3(0.7f, 0.7f, 1f));
            alquimiaGo.transform.SetParent(barca.transform, true);
            var colAlq = alquimiaGo.GetComponent<BoxCollider2D>();
            colAlq.size = new Vector2(1.4f, 1.4f);
            alquimiaGo.AddComponent<EstacaoAlquimia>();
        }
    }

    static void GarantirComponente<T>(string nome) where T : Component
    {
        if (Object.FindFirstObjectByType<T>() != null) return;
        new GameObject(nome).AddComponent<T>();
    }

    static void GarantirCheckpoint()
    {
        if (Object.FindFirstObjectByType<Checkpoint>() != null) return;

        var go = CriarMarcador("CheckpointInicial", new Vector3(-6f, 1f, 0f),
            new Color(0.2f, 0.9f, 0.3f, 0.85f), new Vector3(0.6f, 0.6f, 1f));
        var col = go.GetComponent<BoxCollider2D>();
        col.size = new Vector2(2f, 2f);
        var cp = go.AddComponent<Checkpoint>();
        cp.ativarAoIniciar = true;
    }

    static void GarantirZonaMorte()
    {
        if (Object.FindFirstObjectByType<ZonaMorte>() != null) return;

        var go = new GameObject("ZonaMorte");
        go.transform.position = new Vector3(0f, -12f, 0f);
        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(80f, 4f);
        go.AddComponent<ZonaMorte>();
    }

    static void GarantirDonaNair()
    {
        CriarNpc(
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
        CriarNpc(
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
        CriarNpc(
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

    static void CriarNpc(string nome, Vector3 posicao, Color cor, Vector3 escala, FalaDialogo[] falas)
    {
        if (GameObject.Find(nome) != null) return;

        var go = CriarMarcador(nome, posicao, cor, escala);
        var col = go.GetComponent<BoxCollider2D>();
        col.size = new Vector2(2.2f, 1.4f);

        var npc = go.AddComponent<NpcDialogo>();
        npc.falas = falas;
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
