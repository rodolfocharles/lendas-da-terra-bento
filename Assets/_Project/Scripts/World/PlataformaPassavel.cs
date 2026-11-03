using UnityEngine;

/// <summary>
/// Plataforma one-way: o Bento sobe por baixo e fica em cima,
/// sem ficar preso entre o chao e a plataforma.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PlataformaPassavel : MonoBehaviour
{
    public float arcoSuperficie = 170f;

    void Awake()
    {
        Aplicar();
    }

    void OnEnable()
    {
        Aplicar();
    }

    public void Aplicar()
    {
        var col = GetComponent<Collider2D>();
        if (col == null) return;

        col.isTrigger = false;
        col.usedByEffector = true;

        var effector = GetComponent<PlatformEffector2D>();
        if (effector == null) effector = gameObject.AddComponent<PlatformEffector2D>();
        effector.useOneWay = true;
        effector.useOneWayGrouping = true;
        effector.surfaceArc = arcoSuperficie;
        effector.useSideFriction = false;
        effector.useSideBounce = false;
    }
}
