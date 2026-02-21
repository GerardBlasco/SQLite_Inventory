using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerConfig : MonoBehaviour
{
    [SerializeField] GameObject characterModel;
    [SerializeField] MeshRenderer[] eyeMeshes;

    private MeshRenderer[] renderers;

    private void Start()
    {
        renderers = characterModel.GetComponentsInChildren<MeshRenderer>();
        DataBaseController.LoadDataIntoPlayerConfig(this);
    }

    public void SetColor(string codeHEX)
    {
        ColorUtility.TryParseHtmlString("#" + codeHEX, out Color color);

        foreach (MeshRenderer m in renderers)
        {
            if (!eyeMeshes.Contains(m))
            {
                m.material.color = color;
            }
        }
    }

    public void SetEyes(string eyeID)
    {
        foreach (MeshRenderer m in eyeMeshes)
        {
            m.material.SetTexture("_MainTex", Resources.Load<Texture2D>($"Eyes/{eyeID}"));
        }
    }
}
