using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SpotLight : MonoBehaviour
{
    public Light2D light2D;

    public void SetColor(Color color)
    {
        light2D.color = color;
    }



}
