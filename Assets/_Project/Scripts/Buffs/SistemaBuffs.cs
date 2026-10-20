using System;
using UnityEngine;

/// <summary>
/// Buffs temporarios do Bento (comida). Um buff de comida por vez.
/// </summary>
public class SistemaBuffs : MonoBehaviour
{
    public static SistemaBuffs Instancia { get; private set; }

    public event Action BuffAlterado;

    private BuffAtivo buffAtual;

    public BuffAtivo BuffAtual => buffAtual;
    public bool TemBuffAtivo => buffAtual != null && !buffAtual.Expirado;

    public bool TemTreme => TemBuffAtivo && buffAtual.tipo == TipoBuff.Treme;
    public bool ResistenciaParalisiaVeneno => TemBuffAtivo && buffAtual.resistenciaParalisiaVeneno;
    public float MultiplicadorDanoEspiritual =>
        TemBuffAtivo ? buffAtual.multiplicadorDanoEspiritual : 1f;

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(this);
            return;
        }
        Instancia = this;
        CatalogoEfeitosComida.Garantir();
    }

    void OnDestroy()
    {
        if (Instancia == this) Instancia = null;
    }

    void Update()
    {
        if (buffAtual == null) return;

        buffAtual.restante -= Time.deltaTime;
        if (buffAtual.Expirado)
        {
            Debug.Log($"[Lendas] Buff expirou: {buffAtual.nomeExibicao}");
            buffAtual = null;
            BuffAlterado?.Invoke();
        }
    }

    public bool ConsumirComida(ItemDados item)
    {
        if (item == null) return false;

        var efeito = CatalogoEfeitosComida.BuscarPorItem(item.id);
        if (efeito == null) return false;

        var inventario = InventarioBento.Instancia ?? GetComponent<InventarioBento>();
        if (inventario == null || !inventario.Remover(item, 1)) return false;

        Aplicar(efeito);
        return true;
    }

    public void Aplicar(EfeitoComida efeito)
    {
        if (efeito == null) return;

        buffAtual = new BuffAtivo
        {
            tipo = efeito.tipoBuff,
            nomeExibicao = efeito.nomeBuff,
            descricao = efeito.descricao,
            duracaoTotal = efeito.duracaoSegundos,
            restante = efeito.duracaoSegundos,
            multiplicadorDanoEspiritual = efeito.multiplicadorDanoEspiritual,
            resistenciaParalisiaVeneno = efeito.resistenciaParalisiaVeneno
        };

        Debug.Log($"[Lendas] Buff ativo: {buffAtual.nomeExibicao} ({buffAtual.restante:0}s)");
        BuffAlterado?.Invoke();
    }

    public void Limpar()
    {
        if (buffAtual == null) return;
        buffAtual = null;
        BuffAlterado?.Invoke();
    }
}
