using UnityEngine;

public enum TipoObjetoEspectral
{
    Plataforma,
    Pista
}

/// <summary>
/// So aparece (e, se plataforma, colide) com a Visao Espectral ativa.
/// </summary>
public class ObjetoEspectral : MonoBehaviour
{
    public TipoObjetoEspectral tipo = TipoObjetoEspectral.Plataforma;
    [TextArea(1, 3)]
    public string textoPista = "Simbolo antigo...";
    public string idPaginaCaderno;
    public bool forcarVisivel;
    public bool ocultarDica;

    private SpriteRenderer sprite;
    private Collider2D colisor;
    private bool jogadorProximo;
    private Color corVisivel;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        colisor = GetComponent<Collider2D>();
        if (sprite != null) corVisivel = sprite.color;
        Aplicar(BentoVisaoEspectral.Ativa);
    }

    void OnEnable()
    {
        BentoVisaoEspectral.VisaoAlterada += Aplicar;
    }

    void OnDisable()
    {
        BentoVisaoEspectral.VisaoAlterada -= Aplicar;
    }

    public void DefinirCorVisivel(Color cor)
    {
        corVisivel = cor;
        Aplicar(BentoVisaoEspectral.Ativa || forcarVisivel);
    }

    public void RevelarPermanente()
    {
        forcarVisivel = true;
        Aplicar(true);
    }

    void Aplicar(bool visaoAtiva)
    {
        bool mostrar = visaoAtiva || forcarVisivel;

        if (sprite != null)
        {
            var c = corVisivel;
            if (!mostrar)
            {
                c.a = 0f;
                sprite.color = c;
                sprite.enabled = false;
            }
            else
            {
                sprite.enabled = true;
                sprite.color = corVisivel;
            }
        }

        if (colisor != null)
        {
            // Pistas nao bloqueiam; plataformas so colidem quando reveladas
            if (tipo == TipoObjetoEspectral.Plataforma)
                colisor.enabled = mostrar;
            else
                colisor.enabled = mostrar;
        }

        if (!mostrar) jogadorProximo = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (tipo != TipoObjetoEspectral.Pista) return;
        if (!BentoVisaoEspectral.Ativa) return;
        if (other.GetComponent<BentoMovimento>() != null)
        {
            jogadorProximo = true;
            if (!string.IsNullOrEmpty(idPaginaCaderno))
                CadernoCampo.Instancia?.Desbloquear(idPaginaCaderno);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<BentoMovimento>() != null) jogadorProximo = false;
    }

    void OnGUI()
    {
        if (tipo != TipoObjetoEspectral.Pista || ocultarDica) return;
        if (!BentoVisaoEspectral.Ativa && !forcarVisivel) return;
        if (!jogadorProximo) return;
        if (Camera.main == null) return;

        Vector3 tela = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 0.9f);
        if (tela.z < 0f) return;

        float y = Screen.height - tela.y;
        float largura = 280f;
        GUI.Box(new Rect(tela.x - largura * 0.5f, y - 40f, largura, 50f), "");
        var estilo = new GUIStyle(GUI.skin.label) { fontSize = 14, wordWrap = true, alignment = TextAnchor.MiddleCenter };
        estilo.normal.textColor = new Color(0.75f, 0.9f, 1f);
        GUI.Label(new Rect(tela.x - largura * 0.5f + 8f, y - 36f, largura - 16f, 42f), textoPista, estilo);
    }
}
