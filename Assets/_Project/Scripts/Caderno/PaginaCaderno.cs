using System;

/// <summary>
/// Uma pagina (ou rascunho) do Caderno de Campo.
/// </summary>
[Serializable]
public class PaginaCaderno
{
    public string id;
    public string titulo;
    public CategoriaPagina categoria;
    [UnityEngine.TextArea(3, 8)]
    public string texto;
    public string fonte;
    public bool desbloqueada;

    public PaginaCaderno(string id, string titulo, CategoriaPagina categoria, string texto, string fonte, bool desbloqueada = false)
    {
        this.id = id;
        this.titulo = titulo;
        this.categoria = categoria;
        this.texto = texto;
        this.fonte = fonte;
        this.desbloqueada = desbloqueada;
    }
}
