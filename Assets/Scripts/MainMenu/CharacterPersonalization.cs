using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterPersonalization : MonoBehaviour
{
    [SerializeField] GameObject characterModel;
    [SerializeField] MeshRenderer[] eyeMeshes;
    [SerializeField] string defaultColorHEX;

    private MeshRenderer[] renderers;
    private Texture2D[] eyes;
    private int currentEyeIndex = 0;

    private Color currentColor;

    private void Start()
    {
        renderers = characterModel.GetComponentsInChildren<MeshRenderer>();
        eyes = Resources.LoadAll<Texture2D>("Eyes");
    }

    public void ChangeColor(string codeHEX)
    {
        ColorUtility.TryParseHtmlString(codeHEX, out Color color);
        currentColor = color;

        foreach (MeshRenderer m in renderers)
        {
            if (!eyeMeshes.Contains(m))
            {
                m.material.color = currentColor;
            }
        }
    }

    public void MoveIndex(bool toNext)
    {
        int finalIndex = eyes.Length - 1;

        if (toNext)
        {
            currentEyeIndex++;

            if (currentEyeIndex > finalIndex)
            {
                currentEyeIndex = 0;
            }
        }
        else
        {
            currentEyeIndex--;

            if (currentEyeIndex < 0)
            {
                currentEyeIndex = finalIndex;
            }
        }

        ChangeEye(currentEyeIndex);
    }

    public void ChangeEye(int index)
    {
        foreach (MeshRenderer m in eyeMeshes)
        {
            m.material.SetTexture("_MainTex", eyes[index]);
        }
    }

    public void ResetParameters()
    {
        currentEyeIndex = 0;

        ChangeEye(currentEyeIndex);
        ChangeColor(defaultColorHEX);
    }

    public string GetEyeID()
    {
        return eyes[currentEyeIndex].name;
    }

    public string GetColor()
    {
        return ColorUtility.ToHtmlStringRGB(currentColor);
    }
}
