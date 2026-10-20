using UnityEngine;

/// <summary>
/// Mostra o buff de comida ativo (prototipo OnGUI).
/// </summary>
public class HudBuffs : MonoBehaviour
{
    private SistemaBuffs buffs;

    void Start()
    {
        buffs = SistemaBuffs.Instancia ?? FindFirstObjectByType<SistemaBuffs>();
    }

    void OnGUI()
    {
        if (buffs == null)
            buffs = SistemaBuffs.Instancia ?? FindFirstObjectByType<SistemaBuffs>();
        if (buffs == null || !buffs.TemBuffAtivo) return;

        var buff = buffs.BuffAtual;
        var estilo = new GUIStyle(GUI.skin.label)
        {
            fontSize = 16,
            fontStyle = FontStyle.Bold
        };
        estilo.normal.textColor = new Color(1f, 0.85f, 0.35f);

        int segundos = Mathf.CeilToInt(buff.restante);
        GUI.Label(new Rect(16, 76, 420, 28),
            $"Buff: {buff.nomeExibicao} ({segundos}s) — {buff.descricao}", estilo);
    }
}
