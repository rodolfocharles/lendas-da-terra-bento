using UnityEngine;

[System.Serializable]
public class FalaDialogo
{
    public string personagem = "NPC";
    [TextArea(2, 5)]
    public string texto;
    public string emocao = "neutro";
    public Sprite retrato;
}
