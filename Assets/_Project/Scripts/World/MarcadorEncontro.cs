using UnityEngine;

/// <summary>
/// Placeholder de encontro (Boto, Iara, Curupira) ate H10/H11/H01.
/// </summary>
public class MarcadorEncontro : MonoBehaviour
{
    public string titulo = "Encontro";
    [TextArea(1, 3)]
    public string dica = "";

    private bool jogadorProximo;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<BentoMovimento>() != null) jogadorProximo = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<BentoMovimento>() != null) jogadorProximo = false;
    }

    void OnGUI()
    {
        if (!jogadorProximo) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto
            || HudCaderno.Aberto || HudPesquisa.Aberto) return;
        if (Camera.main == null) return;

        Vector3 tela = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.3f);
        if (tela.z < 0f) return;

        float y = Screen.height - tela.y;
        GUI.Box(new Rect(tela.x - 140f, y - 48f, 280f, 52f), titulo + "\n" + dica);
    }
}
