using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BentoMovimento))]
public class BentoAnimacao : MonoBehaviour
{
    const float FpsIdle = 6f;
    const float FpsCorrer = 10f;
    const float FpsAcao = 12f;

    SpriteRenderer sprite;
    BentoMovimento movimento;
    BentoChicote chicote;
    BentoBesta besta;
    Rigidbody2D rb;

    Sprite[] idle;
    Sprite[] correr;
    Sprite[] pular;
    Sprite[] chicoteQuadros;
    Sprite[] bestaQuadros;
    Sprite[] cozinhar;
    Sprite[] sentado;

    enum Estado { Idle, Correr, Pular, Chicote, Besta, Cozinhar, Sentado }

    Estado estadoAtual = Estado.Idle;
    int indice;
    float acumulado;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        movimento = GetComponent<BentoMovimento>();
        chicote = GetComponent<BentoChicote>();
        besta = GetComponent<BentoBesta>();
        rb = GetComponent<Rigidbody2D>();

        idle = CarregadorPixelArt.Folha("Bento/Animacoes/Bento_Idle_east_64x64_4f.png", 64, 64, 4);
        correr = CarregadorPixelArt.Folha("Bento/Animacoes/Bento_Correr_east_64x64_6f.png", 64, 64, 6);
        pular = CarregadorPixelArt.Folha("Bento/Animacoes/Bento_Pular_east_64x64_9f.png", 64, 64, 9);
        chicoteQuadros = CarregadorPixelArt.Folha("Bento/Animacoes/Bento_Chicote_east_84x84_9f.png", 84, 84, 9);
        bestaQuadros = CarregadorPixelArt.Folha("Bento/Animacoes/Bento_Besta_east_84x84_9f.png", 84, 84, 9);
        cozinhar = CarregadorPixelArt.Folha("Bento/Animacoes/Bento_Cozinhar_east_84x84_9f.png", 84, 84, 9);
        sentado = CarregadorPixelArt.Folha("Bento/Animacoes/Bento_Sentado_east_84x84_9f.png", 84, 84, 9);

        sprite.color = Color.white;
        sprite.sortingOrder = 5;
        float escala = CarregadorPixelArt.EscalaMundo;
        transform.localScale = new Vector3(escala, escala, 1f);
        AjustarColisor();
        sprite.flipX = movimento != null && !movimento.OlhandoDireita;
        if (idle.Length > 0) sprite.sprite = idle[0];
    }

    void AjustarColisor()
    {
        var col = GetComponent<BoxCollider2D>();
        if (col == null) return;

        // Em unidades locais: com EscalaMundo ~2.4 fica ~0.77 x 1.73 no mundo
        col.size = new Vector2(0.32f, 0.72f);
        col.offset = new Vector2(0f, -0.05f);
    }

    void LateUpdate()
    {
        var estado = EscolherEstado();
        if (estado != estadoAtual)
        {
            estadoAtual = estado;
            indice = 0;
            acumulado = 0f;
        }

        var quadros = Quadros(estado);
        if (quadros == null || quadros.Length == 0) return;

        if (estado == Estado.Pular)
        {
            indice = QuadroDoPulo(quadros.Length);
        }
        else if (estado == Estado.Sentado)
        {
            indice = quadros.Length - 1;
        }
        else
        {
            acumulado += Time.deltaTime * Fps(estado);
            while (acumulado >= 1f)
            {
                acumulado -= 1f;
                indice = (indice + 1) % quadros.Length;
            }
        }

        indice = Mathf.Clamp(indice, 0, quadros.Length - 1);
        var quadro = quadros[indice];
        if (quadro != null)
            sprite.sprite = quadro;
    }

    Estado EscolherEstado()
    {
        if (CanoaRio.JogadorDentro) return Estado.Sentado;
        if (HudFogao.Aberto) return Estado.Cozinhar;
        if (besta != null && besta.Mirando) return Estado.Besta;
        if (chicote != null && (chicote.Balancando || chicote.EmGolpeChicote)) return Estado.Chicote;
        if (movimento != null && !movimento.NoChao) return Estado.Pular;
        if (movimento != null && Mathf.Abs(movimento.MovimentoHorizontal) > 0.1f) return Estado.Correr;
        return Estado.Idle;
    }

    Sprite[] Quadros(Estado estado)
    {
        switch (estado)
        {
            case Estado.Correr: return correr;
            case Estado.Pular: return pular;
            case Estado.Chicote: return chicoteQuadros;
            case Estado.Besta: return bestaQuadros;
            case Estado.Cozinhar: return cozinhar;
            case Estado.Sentado: return sentado;
            default: return idle;
        }
    }

    static float Fps(Estado estado)
    {
        switch (estado)
        {
            case Estado.Correr: return FpsCorrer;
            case Estado.Chicote:
            case Estado.Besta:
            case Estado.Cozinhar:
                return FpsAcao;
            default:
                return FpsIdle;
        }
    }

    int QuadroDoPulo(int total)
    {
        float vy = rb != null ? rb.linearVelocity.y : 0f;
        if (vy > 4f) return Mathf.Min(3, total - 1);
        if (vy > 1f) return Mathf.Min(4, total - 1);
        if (vy > -1f) return Mathf.Min(5, total - 1);
        if (vy > -4f) return Mathf.Min(6, total - 1);
        return Mathf.Min(7, total - 1);
    }
}
