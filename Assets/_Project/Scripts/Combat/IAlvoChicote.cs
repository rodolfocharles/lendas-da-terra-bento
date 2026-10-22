using UnityEngine;

/// <summary>
/// Alvo que o chicote puxa (alavanca, capanga, Sombra).
/// </summary>
public interface IAlvoChicote
{
    Transform Transforme { get; }
    bool PodeSerPuxado { get; }
    void AoSerPuxado(Vector2 origemBento);
}
