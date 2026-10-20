using UnityEngine;

/// <summary>
/// Arbusto / cipó que o facao corta (abre caminho).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class VegetacaoCortavel : MonoBehaviour, IRecebeDano
{
    public int golpesParaCortar = 1;
    public bool destruirAoCortar = true;

    private int golpesRestantes;

    void Awake()
    {
        golpesRestantes = Mathf.Max(1, golpesParaCortar);
    }

    public void ReceberDano(int quantidade, Vector2 origem, TipoDano tipo)
    {
        if (quantidade <= 0) return;

        // Aceita fisico e corte explicito
        if (tipo != TipoDano.Fisico && tipo != TipoDano.CorteVegetacao) return;

        golpesRestantes -= quantidade;
        if (golpesRestantes > 0) return;

        Debug.Log($"[Lendas] Vegetacao cortada: {name}");
        if (destruirAoCortar) Destroy(gameObject);
        else gameObject.SetActive(false);
    }
}
