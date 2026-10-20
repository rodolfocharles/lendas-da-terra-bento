using System;

/// <summary>
/// Instancia de um buff em andamento no Bento.
/// </summary>
[Serializable]
public class BuffAtivo
{
    public TipoBuff tipo;
    public string nomeExibicao;
    public string descricao;
    public float duracaoTotal;
    public float restante;
    public float multiplicadorDanoEspiritual = 1f;
    public bool resistenciaParalisiaVeneno;

    public bool Expirado => restante <= 0f;

    public float Progresso => duracaoTotal <= 0f ? 0f : restante / duracaoTotal;
}
