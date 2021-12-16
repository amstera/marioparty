using UnityEngine;

public class LightBulb : MonoBehaviour
{
    public Material ActivatedMaterial;
    public MeshRenderer Renderer;

    public void Activate()
    {
        Material[] mats = Renderer.materials;
        mats[0] = ActivatedMaterial;
        Renderer.materials = mats;
    }
}
