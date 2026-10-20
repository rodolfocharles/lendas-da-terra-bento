using UnityEngine;

/// <summary>
/// Qualquer coisa que o facao (ou outros ataques) pode atingir.
/// </summary>
public interface IRecebeDano
{
    void ReceberDano(int quantidade, Vector2 origem, TipoDano tipo);
}

public enum TipoDano
{
    Fisico,
    Espiritual,
    CorteVegetacao
}
