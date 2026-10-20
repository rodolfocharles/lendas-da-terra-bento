using UnityEngine;

/// <summary>
/// Definicao estatica de um item (ScriptableObject).
/// </summary>
[CreateAssetMenu(fileName = "Item", menuName = "Lendas/Item", order = 0)]
public class ItemDados : ScriptableObject
{
    [Tooltip("Identificador estavel para save/craft (sem acento).")]
    public string id;

    public string nomeExibicao;
    [TextArea(2, 4)]
    public string descricao;
    public CategoriaItem categoria = CategoriaItem.Item;
    public Sprite icone;
    [Min(1)]
    public int empilhamentoMaximo = 99;

    public bool IdValido => !string.IsNullOrWhiteSpace(id);
}
