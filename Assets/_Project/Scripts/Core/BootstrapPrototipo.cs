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

        GarantirBentoVida();
        GarantirComponente<HudVida>("HudVida");
        GarantirComponente<GerenciadorDialogo>("GerenciadorDialogo");
        GarantirCheckpoint();
        GarantirZonaMorte();
        GarantirDonaNair();
        GarantirNamoradaDoBoto();
        GarantirPescadorDaIara();

        Debug.Log("[Lendas] Bootstrap: Dona Nair (amarelo), Namorada do Boto (rosa) e Pescador (azul). Aproxime e aperte E.");
    }

    static void GarantirBentoVida()
    {
        var bento = Object.FindFirstObjectByType<BentoMovimento>();
        if (bento == null) return;
        if (bento.GetComponent<BentoVida>() == null)
            bento.gameObject.AddComponent<BentoVida>();
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
