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

        Debug.Log("[Lendas] Bootstrap do prototipo: HUD, dialogo e Dona Nair prontos. Aproxime-se dela e aperte E.");
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

        var go = new GameObject("CheckpointInicial");
        go.transform.position = new Vector3(-6f, 1f, 0f);
        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(2f, 2f);
        var cp = go.AddComponent<Checkpoint>();
        cp.ativarAoIniciar = true;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = CriarSpriteSimples();
        sr.color = new Color(0.2f, 0.9f, 0.3f, 0.85f);
        go.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
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
        if (GameObject.Find("DonaNair") != null) return;

        var go = new GameObject("DonaNair");
        // Perto do spawn do Bento (y~2), a esquerda
        go.transform.position = new Vector3(-3.5f, 1.2f, 0f);
        go.transform.localScale = new Vector3(1.1f, 1.8f, 1f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = CriarSpriteSimples();
        sr.color = new Color(1f, 0.75f, 0.2f); // amarelo bem visivel
        sr.sortingOrder = 2;

        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(2.2f, 1.4f);

        var npc = go.AddComponent<NpcDialogo>();
        npc.falas = new[]
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
        };
    }

    static Sprite CriarSpriteSimples()
    {
        var tex = Texture2D.whiteTexture;
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 4f);
    }
}
