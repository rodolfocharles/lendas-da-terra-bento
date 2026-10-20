using System;
using UnityEngine;

/// <summary>
/// Visao Espectral do Amuleto: segurar V revela pistas e plataformas ocultas.
/// Consome energia espiritual enquanto ativa.
/// </summary>
[RequireComponent(typeof(BentoMovimento))]
public class BentoVisaoEspectral : MonoBehaviour
{
    public static BentoVisaoEspectral Instancia { get; private set; }
    public static bool Ativa { get; private set; }
    public static event Action<bool> VisaoAlterada;

    [Header("Energia")]
    public float energiaMaxima = 100f;
    public float consumoPorSegundo = 22f;
    public float recargaPorSegundo = 14f;
    public float minimoParaAtivar = 8f;

    [Header("Input")]
    public KeyCode teclaVisao = KeyCode.V;

    public float EnergiaAtual { get; private set; }

    private BentoMovimento movimento;
    private BentoVida vida;
    private BentoBesta besta;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(this);
            return;
        }
        Instancia = this;
        movimento = GetComponent<BentoMovimento>();
        vida = GetComponent<BentoVida>();
        besta = GetComponent<BentoBesta>();
        EnergiaAtual = energiaMaxima;
    }

    void OnDestroy()
    {
        if (Instancia == this)
        {
            if (Ativa) DefinirAtiva(false);
            Instancia = null;
        }
    }

    void Update()
    {
        bool querAtivar = Input.GetKey(teclaVisao) && PodeUsar();

        if (querAtivar && !Ativa && EnergiaAtual >= minimoParaAtivar)
            DefinirAtiva(true);
        else if (Ativa && (!querAtivar || EnergiaAtual <= 0f))
            DefinirAtiva(false);

        if (Ativa)
        {
            EnergiaAtual = Mathf.Max(0f, EnergiaAtual - consumoPorSegundo * Time.deltaTime);
            if (EnergiaAtual <= 0f) DefinirAtiva(false);
        }
        else if (EnergiaAtual < energiaMaxima)
        {
            EnergiaAtual = Mathf.Min(energiaMaxima, EnergiaAtual + recargaPorSegundo * Time.deltaTime);
        }
    }

    bool PodeUsar()
    {
        if (!movimento.ControleAtivo) return false;
        if (vida != null && vida.EstaMorto) return false;
        if (besta != null && besta.Mirando) return false;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return false;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto || HudCaderno.Aberto) return false;
        return true;
    }

    void DefinirAtiva(bool valor)
    {
        if (Ativa == valor) return;
        Ativa = valor;
        VisaoAlterada?.Invoke(Ativa);
        Debug.Log(Ativa ? "[Lendas] Visao Espectral ON" : "[Lendas] Visao Espectral OFF");
    }
}
