using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NpcAnimacaoIdle : MonoBehaviour
{
    public Sprite[] quadros;
    public float fps = 6f;

    SpriteRenderer sprite;
    float acumulado;
    int indice;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (quadros == null || quadros.Length == 0 || sprite == null) return;

        acumulado += Time.deltaTime * fps;
        while (acumulado >= 1f)
        {
            acumulado -= 1f;
            indice = (indice + 1) % quadros.Length;
        }

        var quadro = quadros[indice];
        if (quadro != null)
            sprite.sprite = quadro;
    }
}
