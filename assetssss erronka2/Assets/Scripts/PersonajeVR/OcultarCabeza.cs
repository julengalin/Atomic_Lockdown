using UnityEngine;

public class OcultarCabeza : MonoBehaviour
{
    [Header("Arrastra aquí el transform del HUESO Head (mixamorig:Head) o un objeto 'HeadMesh' si lo tienes")]
    public Transform headBoneOrHeadObject;

    void Start()
    {
        if (!headBoneOrHeadObject) return;

        // Oculta todos los renderers que cuelguen de ahí (mallas de cabeza, pelo, etc.)
        var renderers = headBoneOrHeadObject.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
            r.enabled = false;
    }
}
