using System;
using UnityEngine;

/// <summary>
/// Receita de estacao (Fogao / Alquimia): ingredientes (por id) e resultado.
/// </summary>
[Serializable]
public class ReceitaDados
{
    public string id;
    public string nomeExibicao;
    public string descricao;
    public ItemDados resultado;
    public int quantidadeResultado = 1;
    public RequisitoIngrediente[] ingredientes;

    public ReceitaDados(string id, string nome, string descricao, ItemDados resultado, params RequisitoIngrediente[] ingredientes)
        : this(id, nome, descricao, resultado, 1, ingredientes)
    {
    }

    public ReceitaDados(string id, string nome, string descricao, ItemDados resultado, int quantidadeResultado, params RequisitoIngrediente[] ingredientes)
    {
        this.id = id;
        nomeExibicao = nome;
        this.descricao = descricao;
        this.resultado = resultado;
        this.quantidadeResultado = Mathf.Max(1, quantidadeResultado);
        this.ingredientes = ingredientes;
    }
}

[Serializable]
public class RequisitoIngrediente
{
    public string idItem;
    public int quantidade;

    public RequisitoIngrediente(string idItem, int quantidade = 1)
    {
        this.idItem = idItem;
        this.quantidade = quantidade;
    }
}
