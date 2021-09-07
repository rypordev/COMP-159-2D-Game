using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public float fps = 30.0f;
    public Texture2D[] frames;

    private MeshRenderer rendererMy;
    
    void Start()
    {
        rendererMy = GetComponent<MeshRenderer>();
        InvokeRepeating("NextFrame", 1 / fps, 1 / fps);
        rendererMy.enabled = false;
    }

    public void Play()
    {
        rendererMy.enabled = true;
        StopCoroutine(nameof(NextFrame));
        StartCoroutine(NextFrame());
    }

    IEnumerator NextFrame()
    {
        for (int i = 0; i < frames.Length; i++)
        {
            rendererMy.sharedMaterial.SetTexture("_MainTex", frames[i]);
            yield return new WaitForSeconds(1/fps);
        }
        rendererMy.enabled = false;
    }
}
