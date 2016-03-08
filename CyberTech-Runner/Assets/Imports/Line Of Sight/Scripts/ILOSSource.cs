using System.Collections;
using UnityEngine;

/// <summary>
/// Interface used for LOSSource and LOSSourceCube scripts
/// </summary>
public interface ILOSSource
{
    Color MaskColor
    {
        get;
        set;
    }

    float MaskIntensity
    {
        get;
        set;
    }

    bool MaskInvert
    {
        get;
        set;
    }

    float DistanceFade
    {
        get;
        set;
    }

    float MinVariance
    {
        get;
        set;
    }

    bool IsVisible
    {
        get;
    }

    Camera SourceCamera
    {
        get;
    }

    Bounds CameraBounds
    {
        get;
    }

    Vector4 SourceInfo
    {
        get;
    }

    LayerMask RevealMask
    {
        get;
        set;
    }
}