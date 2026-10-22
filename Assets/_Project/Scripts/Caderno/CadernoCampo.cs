using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Diario ilustrado do Bento: pistas, bestiario, receitas. Abre no puzzle/boss.
/// </summary>
public class CadernoCampo : MonoBehaviour
{
    public static CadernoCampo Instancia { get; private set; }

    public event Action PaginasAlteradas;

    [SerializeField] private List<PaginaCaderno> paginas = new List<PaginaCaderno>();

    public IReadOnlyList<PaginaCaderno> Paginas => paginas;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(this);
            return;
        }
        Instancia = this;
        if (paginas.Count == 0)
            paginas = CatalogoPaginasCap1.Criar();
        else
            MesclarCatalogo();
    }

    void MesclarCatalogo()
    {
        var catalogo = CatalogoPaginasCap1.Criar();
        for (int i = 0; i < catalogo.Count; i++)
        {
            if (Buscar(catalogo[i].id) == null)
                paginas.Add(catalogo[i]);
        }
    }

    void OnDestroy()
    {
        if (Instancia == this) Instancia = null;
    }

    public bool EstaDesbloqueada(string id)
    {
        var pagina = Buscar(id);
        return pagina != null && pagina.desbloqueada;
    }

    public bool Desbloquear(string id)
    {
        var pagina = Buscar(id);
        if (pagina == null || pagina.desbloqueada) return false;

        pagina.desbloqueada = true;
        Debug.Log($"[Lendas] Caderno: anotou '{pagina.titulo}'");
        PaginasAlteradas?.Invoke();
        return true;
    }

    public PaginaCaderno Buscar(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        for (int i = 0; i < paginas.Count; i++)
        {
            if (paginas[i].id == id) return paginas[i];
        }
        return null;
    }

    public int ContarDesbloqueadas()
    {
        int n = 0;
        for (int i = 0; i < paginas.Count; i++)
        {
            if (paginas[i].desbloqueada) n++;
        }
        return n;
    }
}
