using UnityEngine;

/// <summary>
/// Barra de energia do amuleto (Visao Espectral).
/// </summary>
public class HudEnergiaEspiritual : MonoBehaviour
{
    private BentoVisaoEspectral visao;

    void Start()
    {
        visao = BentoVisaoEspectral.Instancia ?? FindFirstObjectByType<BentoVisaoEspectral>();
    }

    void OnGUI()
    {
        if (visao == null)
            visao = BentoVisaoEspectral.Instancia ?? FindFirstObjectByType<BentoVisaoEspectral>();
        if (visao == null) return;

        float x = 16f;
        float y = 132f;
        float largura = 160f;
        float altura = 14f;

        GUI.Box(new Rect(x, y, largura, altura + 18f), "");
        var titulo = new GUIStyle(GUI.skin.label) { fontSize = 13, fontStyle = FontStyle.Bold };
        titulo.normal.textColor = BentoVisaoEspectral.Ativa
            ? new Color(0.6f, 0.95f, 1f)
            : new Color(0.8f, 0.85f, 0.95f);
        GUI.Label(new Rect(x + 4f, y + 1f, largura - 8f, 16f),
            BentoVisaoEspectral.Ativa ? "Amuleto [V] ATIVO" : "Amuleto [V]", titulo);

        float fill = visao.energiaMaxima <= 0f ? 0f : visao.EnergiaAtual / visao.energiaMaxima;
        GUI.color = new Color(0.25f, 0.55f, 0.85f, 0.95f);
        GUI.DrawTexture(new Rect(x + 4f, y + 18f, (largura - 8f) * fill, 10f), Texture2D.whiteTexture);
        GUI.color = Color.white;

        // Leve tom azulado na tela enquanto a visao esta ativa
        if (BentoVisaoEspectral.Ativa)
        {
            GUI.color = new Color(0.35f, 0.55f, 0.95f, 0.08f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
    }
}
