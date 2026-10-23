using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fragmentos espirituais absorvidos pelo amuleto. Nao ocupam a bolsa.
/// </summary>
public class AmuletoAncestral : MonoBehaviour
{
    public const string IdEssenciaDasAguas = "essencia_aguas";
    public const string IdPassoInvertido = "passo_invertido";

    public static AmuletoAncestral Instancia { get; private set; }

    public static event Action<string> FragmentoRecebido;

    readonly HashSet<string> fragmentos = new HashSet<string>();

    public bool TemEssenciaDasAguas => Tem(IdEssenciaDasAguas);
    public bool TemPassoInvertido => Tem(IdPassoInvertido);

    void Awake()
    {
        if (Instancia != null && Instancia != this)
        {
            Destroy(this);
            return;
        }
        Instancia = this;
    }

    void OnDestroy()
    {
        if (Instancia == this) Instancia = null;
    }

    public bool Tem(string id)
    {
        return !string.IsNullOrWhiteSpace(id) && fragmentos.Contains(id);
    }

    public bool Receber(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || fragmentos.Contains(id)) return false;
        fragmentos.Add(id);
        FragmentoRecebido?.Invoke(id);
        Debug.Log($"[Lendas] Amuleto recebeu fragmento: {id}");
        return true;
    }
}
