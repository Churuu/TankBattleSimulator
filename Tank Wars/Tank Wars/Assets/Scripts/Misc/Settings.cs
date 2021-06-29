using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings settings;

    [Range(0.0f, 1.0f)]
    public float volume;

    private void Awake()
    {
        if (settings == null)
            settings = this;
        else
            Destroy(this);
    }
}
