using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode , ImageEffectAllowedInSceneView]
public class BB_PPScript : MonoBehaviour
{
    [SerializeField] private Material postprocessMaterial;
    [SerializeField] private float Blend = 1.0f;
    [SerializeField]private Camera cam;
    [SerializeField] private RenderTexture cam2;

    protected virtual void Start()
    {
        cam2= cam.activeTexture;
 
    }

    protected virtual void Update()
    {
        if(postprocessMaterial && Blend != 0)
        {
            postprocessMaterial.SetFloat("_Blend", Blend);
        }
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (postprocessMaterial && Blend != 0)
        {
           Graphics.Blit(cam2, dst , postprocessMaterial);
        }
        else{
            Graphics.Blit(cam2, dst);

            if (!postprocessMaterial)
            {
                Debug.LogError("Missing post process material");
            }
        }
    }

}
