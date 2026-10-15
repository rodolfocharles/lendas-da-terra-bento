using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BentoMovimento))]
public class BentoVida : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 5;
    public float tempoInvencivel = 1f;
    public float forcaEmpurrao = 6f;

    public int VidaAtual { get; private set; }
    public bool EstaMorto { get; private set; }
    public bool Invencivel => Time.time < fimInvencivel;

    public event Action<int, int> VidaAlterada;
    public event Action Morreu;
    public event Action Revivesceu;

    private Rigidbody2D rb;
    private BentoMovimento movimento;
    private BentoChicote chicote;
    private SpriteRenderer sprite;
    private Vector2 pontoRespawn;
    private float fimInvencivel;
    private Color corOriginal;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movimento = GetComponent<BentoMovimento>();
        chicote = GetComponent<BentoChicote>();
        sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) corOriginal = sprite.color;

        VidaAtual = vidaMaxima;
        pontoRespawn = transform.position;
    }

    public void DefinirCheckpoint(Vector2 posicao)
    {
        pontoRespawn = posicao;
    }

    public void Curar(int quantidade)
    {
        if (EstaMorto || quantidade <= 0) return;
        VidaAtual = Mathf.Min(vidaMaxima, VidaAtual + quantidade);
        VidaAlterada?.Invoke(VidaAtual, vidaMaxima);
    }

    public void ReceberDano(int quantidade, Vector2 origemEmpurrao)
    {
        if (EstaMorto || Invencivel || quantidade <= 0) return;

        VidaAtual = Mathf.Max(0, VidaAtual - quantidade);
        VidaAlterada?.Invoke(VidaAtual, vidaMaxima);
        fimInvencivel = Time.time + tempoInvencivel;

        Vector2 direcao = ((Vector2)transform.position - origemEmpurrao).normalized;
        if (direcao.sqrMagnitude < 0.01f) direcao = Vector2.up;
        rb.linearVelocity = new Vector2(direcao.x * forcaEmpurrao, forcaEmpurrao * 0.6f);

        if (VidaAtual <= 0)
        {
            Morrer();
        }
        else
        {
            StartCoroutine(PiscarInvencivel());
        }
    }

    public void MatarImediatamente()
    {
        if (EstaMorto) return;
        VidaAtual = 0;
        VidaAlterada?.Invoke(VidaAtual, vidaMaxima);
        Morrer();
    }

    void Morrer()
    {
        EstaMorto = true;
        movimento.ControleAtivo = false;
        if (chicote != null && chicote.Balancando) chicote.Soltar(false);
        rb.linearVelocity = Vector2.zero;
        Morreu?.Invoke();
        Invoke(nameof(Reviver), 0.6f);
    }

    void Reviver()
    {
        EstaMorto = false;
        VidaAtual = vidaMaxima;
        transform.position = pontoRespawn;
        rb.linearVelocity = Vector2.zero;
        movimento.ControleAtivo = true;
        movimento.RestaurarPulosExtras();
        fimInvencivel = Time.time + tempoInvencivel;
        if (sprite != null) sprite.color = corOriginal;
        VidaAlterada?.Invoke(VidaAtual, vidaMaxima);
        Revivesceu?.Invoke();
        StartCoroutine(PiscarInvencivel());
    }

    System.Collections.IEnumerator PiscarInvencivel()
    {
        if (sprite == null) yield break;
        while (Invencivel && !EstaMorto)
        {
            sprite.color = new Color(corOriginal.r, corOriginal.g, corOriginal.b, 0.35f);
            yield return new WaitForSeconds(0.1f);
            sprite.color = corOriginal;
            yield return new WaitForSeconds(0.1f);
        }
        if (sprite != null) sprite.color = corOriginal;
    }
}
