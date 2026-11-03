using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    public bool ativarAoIniciar;
    [Tooltip("Se > 0, ao pisar no checkpoint o Bento fica com pelo menos essa quantidade de flechas.")]
    public int flechasMinimas;
    public string rotulo = "Checkpoint";

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void Start()
    {
        if (ativarAoIniciar)
            Ativar(FindFirstObjectByType<BentoVida>());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var vida = other.GetComponent<BentoVida>();
        if (vida == null) return;
        Ativar(vida);
    }

    void Ativar(BentoVida vida)
    {
        if (vida == null) return;
        vida.DefinirCheckpoint(transform.position);
        RestaurarFlechas();
    }

    void RestaurarFlechas()
    {
        if (flechasMinimas <= 0) return;
        CatalogoItensNorte.Garantir();
        var inventario = InventarioBento.Instancia ?? FindFirstObjectByType<InventarioBento>();
        if (inventario == null || CatalogoItensNorte.FlechasPurificadoras == null) return;

        int tem = inventario.QuantidadePorId(CatalogoItensNorte.IdFlechasPurificadoras);
        int falta = flechasMinimas - tem;
        if (falta <= 0) return;

        inventario.Adicionar(CatalogoItensNorte.FlechasPurificadoras, falta);
        RepositorColetaveis.GarantirReagentesDaMata();
        Debug.Log($"[Lendas] {rotulo}: reabasteceu {falta} flecha(s). Cipó e resina voltaram na mata.");
    }

    void OnGUI()
    {
        if (flechasMinimas <= 0) return;
        if (GerenciadorDialogo.Instancia != null && GerenciadorDialogo.Instancia.EmDialogo) return;
        if (Camera.main == null) return;

        Vector3 tela = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.1f);
        if (tela.z < 0f) return;
        float y = Screen.height - tela.y;
        GUI.Box(new Rect(tela.x - 130f, y - 22f, 260f, 26f), rotulo + " — flechas");
    }
}
