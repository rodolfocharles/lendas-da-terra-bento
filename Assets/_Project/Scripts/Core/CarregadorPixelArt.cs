using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Carrega PNGs de pixel art direto do disco, com filtro Point.
/// Assim o Play funciona mesmo se a importacao do Unity ainda estiver no padrao.
/// </summary>
public static class CarregadorPixelArt
{
    public const float PixelsPorUnidade = 64f;
    public const float EscalaMundo = 2.4f;
    public const float TopoDoChao = 0.5f;
    public const float YNpcNoChao = TopoDoChao + EscalaMundo * 0.5f;
    const string PastaPersonagens = "_Project/Art/Personagens";

    static readonly Dictionary<string, Texture2D> texturas = new Dictionary<string, Texture2D>();
    static readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
    static readonly Dictionary<string, Sprite[]> folhas = new Dictionary<string, Sprite[]>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void AoEntrarEmPlay()
    {
        LimparCache();
    }

    public static void LimparCache()
    {
        texturas.Clear();
        sprites.Clear();
        folhas.Clear();
    }

    public static Sprite Personagem(string pasta, string arquivoSemExtensao)
    {
        return CarregarSprite(pasta + "/" + arquivoSemExtensao + ".png");
    }

    public static Sprite CarregarSprite(string relativoAosPersonagens)
    {
        if (sprites.TryGetValue(relativoAosPersonagens, out var sprite) && SpriteVivo(sprite))
            return sprite;

        var tex = CarregarTextura(relativoAosPersonagens);
        if (tex == null) return null;

        sprite = Sprite.Create(
            tex,
            new Rect(0f, 0f, tex.width, tex.height),
            new Vector2(0.5f, 0.5f),
            PixelsPorUnidade,
            0,
            SpriteMeshType.FullRect);
        sprite.name = Path.GetFileNameWithoutExtension(relativoAosPersonagens);
        sprites[relativoAosPersonagens] = sprite;
        return sprite;
    }

    public static Sprite[] Folha(string relativoAosPersonagens, int larguraQuadro, int alturaQuadro, int quadros)
    {
        if (folhas.TryGetValue(relativoAosPersonagens, out var lista) && FolhaViva(lista))
            return lista;

        var tex = CarregarTextura(relativoAosPersonagens);
        if (tex == null) return System.Array.Empty<Sprite>();

        lista = new Sprite[quadros];
        for (int i = 0; i < quadros; i++)
        {
            lista[i] = Sprite.Create(
                tex,
                new Rect(i * larguraQuadro, 0f, larguraQuadro, alturaQuadro),
                new Vector2(0.5f, 0.5f),
                PixelsPorUnidade,
                0,
                SpriteMeshType.FullRect);
            lista[i].name = Path.GetFileNameWithoutExtension(relativoAosPersonagens) + "_" + i;
        }

        folhas[relativoAosPersonagens] = lista;
        return lista;
    }

    public static Texture2D CarregarTextura(string relativoAosPersonagens)
    {
        if (texturas.TryGetValue(relativoAosPersonagens, out var tex) && tex != null)
            return tex;

        string caminho = Path.Combine(Application.dataPath, PastaPersonagens, relativoAosPersonagens);
        if (!File.Exists(caminho))
        {
            Debug.LogWarning("[Lendas] Pixel art nao encontrada: " + caminho);
            return null;
        }

        tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.LoadImage(File.ReadAllBytes(caminho));
        tex.name = Path.GetFileNameWithoutExtension(relativoAosPersonagens);
        texturas[relativoAosPersonagens] = tex;
        return tex;
    }

    static bool SpriteVivo(Sprite sprite)
    {
        return sprite != null;
    }

    static bool FolhaViva(Sprite[] lista)
    {
        if (lista == null || lista.Length == 0) return false;
        for (int i = 0; i < lista.Length; i++)
        {
            if (lista[i] == null) return false;
        }
        return true;
    }
}
