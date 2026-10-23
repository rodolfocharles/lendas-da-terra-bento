using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Foco da arena do Curupira: so aparece com Visao Espectral.
/// [E] enrola o Novelo Encantado.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class FocoCorrupcao : MonoBehaviour
{
    static readonly List<FocoCorrupcao> focos = new List<FocoCorrupcao>();

    public static bool AlgumProntoParaEnrolar
    {
        get
        {
            for (int i = 0; i < focos.Count; i++)
            {
                if (focos[i] != null && focos[i].PodeEnrolar) return true;
            }
            return false;
        }
    }

    public static int ContarEnrolados()
    {
        int n = 0;
        for (int i = 0; i < focos.Count; i++)
        {
            if (focos[i] != null && focos[i].Enrolado) n++;
        }
        return n;
    }

    public bool Enrolado { get; private set; }
    public bool PodeEnrolar => !Enrolado && jogadorProximo && BentoVisaoEspectral.Ativa;

    Color corEnrolado = new Color(0.45f, 0.85f, 0.4f, 0.95f);
    ObjetoEspectral espectral;
    bool jogadorProximo;

    void Awake()
    {
        espectral = GetComponent<ObjetoEspectral>();
        if (espectral != null)
        {
            espectral.ocultarDica = true;
            espectral.tipo = TipoObjetoEspectral.Pista;
            if (string.IsNullOrEmpty(espectral.idPaginaCaderno))
                espectral.idPaginaCaderno = CatalogoPaginasCap1.IdCurupira;
        }
    }

    void OnEnable()
    {
        if (!focos.Contains(this)) focos.Add(this);
    }

    void OnDisable()
    {
        focos.Remove(this);
    }

    void Update()
    {
        if (Enrolado) return;
        if (!PodeEnrolar) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto
            || HudCaderno.Aberto || HudPesquisa.Aberto) return;
        if (!Input.GetKeyDown(KeyCode.E)) return;

        EncontroCurupira.Instancia?.TentarEnrolar(this);
    }

    public void Enrolar()
    {
        if (Enrolado) return;
        Enrolado = true;
        if (espectral != null)
        {
            espectral.DefinirCorVisivel(corEnrolado);
            espectral.RevelarPermanente();
        }
        else
        {
            var sprite = GetComponent<SpriteRenderer>();
            if (sprite != null) sprite.color = corEnrolado;
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
        if (Enrolado || !jogadorProximo) return;
        if (!BentoVisaoEspectral.Ativa) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto
            || HudCaderno.Aberto || HudPesquisa.Aberto) return;
        if (Camera.main == null) return;

        Vector3 tela = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.1f);
        if (tela.z < 0f) return;
        float y = Screen.height - tela.y;

        var inventario = InventarioBento.Instancia;
        bool temNovelo = inventario != null && inventario.TemPorId(CatalogoItensNorte.IdNoveloEncantado);
        bool ritualJaUsaNovelo = EncontroCurupira.Instancia != null && EncontroCurupira.Instancia.NoveloJaUsado;
        string dica = (temNovelo || ritualJaUsaNovelo)
            ? "[E] Enrolar novelo no foco"
            : "[E] Foco da corrupcao (falta o Novelo Encantado)";

        GUI.Box(new Rect(tela.x - 160f, y - 28f, 320f, 32f), dica);
    }
}
