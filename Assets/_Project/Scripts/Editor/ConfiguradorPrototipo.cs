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

        Debug.Log("[Lendas] Prototipo configurado: camadas, vida, checkpoint, zona de morte, dialogo e HUD.");
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
        GarantirGerenciadorDialogo();
        GarantirCheckpointInicial();
        GarantirZonaMorte();
        GarantirDonaNair();

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
        if (GameObject.Find("DonaNair") != null) return;

        var go = new GameObject("DonaNair");
        Undo.RegisterCreatedObjectUndo(go, "Criar DonaNair");
        go.transform.position = new Vector3(-4f, 0.5f, 0f);
        go.transform.localScale = new Vector3(0.8f, 1.2f, 1f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = CarregarSpriteQuadrado();
        sr.color = new Color(0.85f, 0.65f, 0.45f);

        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(2.5f, 2f);

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
