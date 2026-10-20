using UnityEngine;

/// <summary>
/// Mostra flechas da Besta no HUD (prototipo).
/// </summary>
public class HudMunicao : MonoBehaviour
{
    private BentoBesta besta;

    void Start()
    {
        besta = FindFirstObjectByType<BentoBesta>();
    }

    void OnGUI()
    {
        if (besta == null) besta = FindFirstObjectByType<BentoBesta>();
        int flechas = besta != null ? besta.QuantidadeFlechas() : 0;

        var estilo = new GUIStyle(GUI.skin.label)
        {
            fontSize = 18,
            fontStyle = FontStyle.Bold
        };
        estilo.normal.textColor = flechas > 0
            ? new Color(0.7f, 0.9f, 1f)
            : new Color(1f, 0.5f, 0.4f);

        string mira = besta != null && besta.Mirando ? "  [mirando]" : "";
        GUI.Label(new Rect(16, 104, 360, 28), $"Flechas: {flechas}{mira}", estilo);
    }
}
