using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ShaderHandler : MonoBehaviour
{
    public Material material;
    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (!material)
        {
            Graphics.Blit(src, dest);
            return;
        }

        Graphics.Blit(src, dest, material);
    }
}
