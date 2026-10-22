using UnityEngine;

/// <summary>
/// Bancada de Alquimia na Barca. Abre o painel com E.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class EstacaoAlquimia : MonoBehaviour
{
    public KeyCode teclaUsar = KeyCode.E;
    public bool exigirDentroDaBarca = true;

    private bool jogadorProximo;

    public bool JogadorProximo => jogadorProximo;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Update()
    {
        if (!jogadorProximo) return;
        if (exigirDentroDaBarca && (BarcaRio.Instancia == null || !BarcaRio.Instancia.JogadorDentro)) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto || HudCaderno.Aberto || HudPesquisa.Aberto) return;
        if (!Input.GetKeyDown(teclaUsar)) return;

        HudAlquimia.Abrir();
    }

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
        if (exigirDentroDaBarca && (BarcaRio.Instancia == null || !BarcaRio.Instancia.JogadorDentro)) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto || HudCaderno.Aberto || HudPesquisa.Aberto) return;

        Vector3 tela = Camera.main != null
            ? Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.1f)
            : Vector3.zero;
        if (tela.z < 0f) return;

        float y = Screen.height - tela.y;
        GUI.Label(new Rect(tela.x - 90f, y - 10f, 200f, 24f), "[E] Bancada de Alquimia");
    }
}
