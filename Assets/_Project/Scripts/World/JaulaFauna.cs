using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Jaula de ferro do Barao prendendo animais da mata (H12).
/// Pode ser quebrada com o Facao (J / clique esq.) ou puxada com o Chicote (clique dir.).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class JaulaFauna : MonoBehaviour, IRecebeDano, IAlvoChicote
{
    static readonly List<JaulaFauna> todas = new List<JaulaFauna>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void AoReiniciarPlay()
    {
        todas.Clear();
        JaulaAberta = null;
    }

    public static event Action<JaulaFauna> JaulaAberta;

    public int golpesParaAbrir = 2;
    public string nomeAnimal = "Filhote de Queixada";

    public bool Aberta { get; private set; }
    public Transform Transforme => transform;
    public bool PodeSerPuxado => !Aberta;

    int golpesRestantes;
    bool jogadorProximo;
    SpriteRenderer sprite;
    Color corPresa = new Color(0.32f, 0.34f, 0.38f, 0.95f);
    Color corLiberta = new Color(0.35f, 0.78f, 0.38f, 0.65f);

    public static int TotalJaulas
    {
        get
        {
            LimparNulos();
            return todas.Count;
        }
    }

    public static int JaulasLibertadas
    {
        get
        {
            LimparNulos();
            int n = 0;
            for (int i = 0; i < todas.Count; i++)
            {
                if (todas[i].Aberta) n++;
            }
            return n;
        }
    }

    public static bool TodasLibertadas => TotalJaulas > 0 && JaulasLibertadas >= TotalJaulas;

    static void LimparNulos()
    {
        for (int i = todas.Count - 1; i >= 0; i--)
        {
            if (todas[i] == null) todas.RemoveAt(i);
        }
    }

    void Awake()
    {
        Aberta = false;
        golpesRestantes = Mathf.Max(1, golpesParaAbrir);
        sprite = GetComponent<SpriteRenderer>();
        if (!todas.Contains(this)) todas.Add(this);
        AplicarVisual();
    }

    void OnDestroy()
    {
        todas.Remove(this);
    }

    public void ReceberDano(int quantidade, Vector2 origem, TipoDano tipo)
    {
        if (Aberta || quantidade <= 0) return;
        if (tipo != TipoDano.Fisico && tipo != TipoDano.CorteVegetacao) return;

        golpesRestantes -= quantidade;
        if (golpesRestantes > 0)
        {
            if (sprite != null) sprite.color = new Color(0.65f, 0.55f, 0.35f, 0.95f);
            return;
        }

        Libertar();
    }

    public void AoSerPuxado(Vector2 origemBento)
    {
        if (Aberta) return;
        Libertar();
    }

    void Libertar()
    {
        if (Aberta) return;
        Aberta = true;
        AplicarVisual();
        CadernoCampo.Instancia?.Desbloquear(CatalogoPaginasCap1.IdCaipora);
        Debug.Log($"[Lendas] Jaula quebrada! {nomeAnimal} voltou para a mata ({JaulasLibertadas}/{TotalJaulas}).");
        JaulaAberta?.Invoke(this);
    }

    void AplicarVisual()
    {
        if (sprite != null)
            sprite.color = Aberta ? corLiberta : corPresa;
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
        if (Aberta || !jogadorProximo) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (HudInventario.Aberto || HudFogao.Aberto || HudAlquimia.Aberto
            || HudCaderno.Aberto || HudPesquisa.Aberto) return;
        if (Camera.main == null) return;

        Vector3 tela = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.15f);
        if (tela.z < 0f) return;
        float y = Screen.height - tela.y;

        GUI.Box(new Rect(tela.x - 155f, y - 28f, 310f, 30f),
            $"Jaula ({nomeAnimal}) — [J] Facao ou Chicote");
    }
}
