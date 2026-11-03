using UnityEngine;

/// <summary>
/// Recria cipó, resina e flechas da Mata quando o jogador fica sem munição
/// e já coletou tudo — evita soft-lock no prototipo.
/// </summary>
public static class RepositorColetaveis
{
    public static void GarantirReagentesDaMata()
    {
        CatalogoItensNorte.Garantir();
        Garantir("Coletavel_Cipo", LayoutPrototipo.Cipo, new Color(0.55f, 0.7f, 0.35f), CatalogoItensNorte.Cipo, 3);
        Garantir("Coletavel_Resina", LayoutPrototipo.Resina, new Color(0.7f, 0.4f, 0.15f), CatalogoItensNorte.Resina, 2);
        Garantir("Coletavel_Flechas_Clareira", LayoutPrototipo.FlechasClareira,
            new Color(0.55f, 0.85f, 1f), CatalogoItensNorte.FlechasPurificadoras, 2);
    }

    public static void Garantir(string nome, Vector3 posicao, Color cor, ItemDados item, int quantidade)
    {
        if (item == null) return;

        var go = GameObject.Find(nome);
        if (go != null)
        {
            var existente = go.GetComponent<ColetavelItem>();
            if (existente != null)
            {
                existente.item = item;
                existente.quantidade = quantidade;
                existente.reaparece = true;
                existente.MostrarDeNovo();
            }

            go.transform.position = posicao;
            go.SetActive(true);
            return;
        }

        go = new GameObject(nome);
        go.transform.position = posicao;
        go.transform.localScale = new Vector3(0.55f, 0.55f, 1f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
        sr.color = cor;
        sr.sortingOrder = 2;

        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(1.2f, 1.2f);

        var coletavel = go.AddComponent<ColetavelItem>();
        coletavel.item = item;
        coletavel.quantidade = quantidade;
        coletavel.reaparece = true;
        coletavel.destruirAoColetar = false;
        coletavel.segundosParaReaparecer = 8f;
    }
}
