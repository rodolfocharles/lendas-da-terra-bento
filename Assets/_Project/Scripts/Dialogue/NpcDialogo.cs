using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NpcDialogo : MonoBehaviour
{
    public KeyCode teclaInteragir = KeyCode.E;
    public FalaDialogo[] falas;

    private bool jogadorProximo;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Update()
    {
        if (!jogadorProximo) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (Input.GetKeyDown(teclaInteragir))
        {
            GerenciadorDialogo.Instancia?.Iniciar(falas);
        }
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
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;

        Vector3 tela = Camera.main != null
            ? Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.2f)
            : Vector3.zero;
        if (tela.z < 0f) return;

        float y = Screen.height - tela.y;
        GUI.Label(new Rect(tela.x - 60f, y - 10f, 120f, 24f), "[E] Conversar");
    }
}
