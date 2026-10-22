using UnityEngine;

/// <summary>
/// Alavanca que o chicote puxa para abrir barreiras (puzzle).
/// </summary>
public class AlavancaChicote : MonoBehaviour, IAlvoChicote
{
    public GameObject[] barreiras;
    public bool comecaAberta;
    public float cooldown = 0.4f;

    public Transform Transforme => transform;
    public bool PodeSerPuxado => Time.time >= proximoPuxao;
    public bool Acionada { get; private set; }

    private SpriteRenderer sprite;
    private Color corOff = new Color(0.75f, 0.55f, 0.2f);
    private Color corOn = new Color(0.95f, 0.85f, 0.25f);
    private float proximoPuxao;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        Acionada = comecaAberta;
        Aplicar();
    }

    public void AoSerPuxado(Vector2 origemBento)
    {
        if (!PodeSerPuxado) return;
        proximoPuxao = Time.time + cooldown;
        Acionada = !Acionada;
        Aplicar();
        Debug.Log(Acionada
            ? "[Lendas] Alavanca puxada: caminho aberto."
            : "[Lendas] Alavanca puxada: caminho fechado.");
    }

    void Aplicar()
    {
        if (sprite != null) sprite.color = Acionada ? corOn : corOff;
        transform.localRotation = Quaternion.Euler(0f, 0f, Acionada ? -35f : 35f);

        if (barreiras == null) return;
        for (int i = 0; i < barreiras.Length; i++)
        {
            if (barreiras[i] != null) barreiras[i].SetActive(!Acionada);
        }
    }

    void OnGUI()
    {
        if (Camera.main == null) return;
        var bento = FindFirstObjectByType<BentoMovimento>();
        if (bento == null || Vector2.Distance(bento.transform.position, transform.position) > 4.5f) return;

        Vector3 tela = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.1f);
        if (tela.z < 0f) return;
        float y = Screen.height - tela.y;
        GUI.Label(new Rect(tela.x - 80f, y - 10f, 180f, 24f),
            Acionada ? "Alavanca [aberta]" : "[botao dir.] Puxar alavanca");
    }
}
