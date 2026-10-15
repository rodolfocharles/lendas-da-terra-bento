using UnityEngine;

public class HudVida : MonoBehaviour
{
    private BentoVida vida;
    private string texto = "Vida: -/-";

    void Start()
    {
        vida = FindFirstObjectByType<BentoVida>();
        if (vida == null) return;

        Atualizar(vida.VidaAtual, vida.vidaMaxima);
        vida.VidaAlterada += Atualizar;
    }

    void OnDestroy()
    {
        if (vida != null) vida.VidaAlterada -= Atualizar;
    }

    void Atualizar(int atual, int maximo)
    {
        texto = $"Vida: {atual}/{maximo}";
    }

    void OnGUI()
    {
        var estilo = new GUIStyle(GUI.skin.label)
        {
            fontSize = 22,
            fontStyle = FontStyle.Bold
        };
        estilo.normal.textColor = Color.white;
        GUI.Label(new Rect(16, 12, 280, 40), texto, estilo);
    }
}
