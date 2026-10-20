using System;

/// <summary>
/// Liga um item de comida a um buff (por id do item).
/// </summary>
[Serializable]
public class EfeitoComida
{
    public string idItem;
    public TipoBuff tipoBuff;
    public string nomeBuff;
    public string descricao;
    public float duracaoSegundos;
    public float multiplicadorDanoEspiritual;
    public bool resistenciaParalisiaVeneno;

    public EfeitoComida(
        string idItem,
        TipoBuff tipoBuff,
        string nomeBuff,
        string descricao,
        float duracaoSegundos,
        float multiplicadorDanoEspiritual,
        bool resistenciaParalisiaVeneno)
    {
        this.idItem = idItem;
        this.tipoBuff = tipoBuff;
        this.nomeBuff = nomeBuff;
        this.descricao = descricao;
        this.duracaoSegundos = duracaoSegundos;
        this.multiplicadorDanoEspiritual = multiplicadorDanoEspiritual;
        this.resistenciaParalisiaVeneno = resistenciaParalisiaVeneno;
    }
}
