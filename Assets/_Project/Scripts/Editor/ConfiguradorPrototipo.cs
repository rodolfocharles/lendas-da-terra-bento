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

        Debug.Log("[Lendas] Prototipo configurado: camadas, prefab do Bento e cena.");
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

        if (raiz.GetComponent<BentoChicote>() == null)
        {
            raiz.AddComponent<BentoChicote>();
        }

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
        if (material != null)
        {
            linha.sharedMaterial = material;
        }

        PrefabUtility.SaveAsPrefabAsset(raiz, CaminhoPrefabBento);
        PrefabUtility.UnloadPrefabContents(raiz);
    }

    static void ConfigurarCena(int camadaChao, int camadaGancho)
    {
        AplicarCamadaPorTag("Chao", camadaChao);
        AplicarCamadaPorTag("Gancho", camadaGancho);

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
}
