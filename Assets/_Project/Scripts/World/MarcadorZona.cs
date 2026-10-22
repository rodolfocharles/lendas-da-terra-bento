using UnityEngine;

/// <summary>
/// Placa de bloco da fase. Mostra o nome no topo enquanto o Bento esta na area.
/// </summary>
public class MarcadorZona : MonoBehaviour
{
    public string nomeZona = "Vila";

    private static MarcadorZona zonaAtual;
    private bool jogadorDentro;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<BentoMovimento>() == null) return;
        jogadorDentro = true;
        zonaAtual = this;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<BentoMovimento>() == null) return;
        jogadorDentro = false;
        if (zonaAtual == this) zonaAtual = null;
    }

    void OnGUI()
    {
        if (zonaAtual != this || !jogadorDentro) return;
        if (HudBloqueada()) return;

        var estilo = new GUIStyle(GUI.skin.box)
        {
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
        estilo.normal.textColor = Color.white;
        GUI.Box(new Rect(Screen.width * 0.5f - 90f, 10f, 180f, 28f), nomeZona, estilo);
    }

    static bool HudBloqueada()
    {
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return true;
        return HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto
            || HudCaderno.Aberto || HudPesquisa.Aberto;
    }
}
