using System;
using UnityEngine;

public class FluidContainerSystem : ContainerPouringSystem
{
    // fixed value for current implementation of shader graph slider
    private const float MinSliderVal = 0.48f;
    // fixed value for current implementation of shader graph slider
    private const float MaxSliderVal = 0.567f;
    // maxSliderVal - minSliderVal, to make calculations easier to read
    private float SliderDifference => MaxSliderVal - MinSliderVal;

    [NonSerialized]
    // the material of the applied water shader
    public Material waterShaderMat;

    [NonSerialized]
    // the initial color of the water shader on the side
    public Color initialWaterColorSide;

    [NonSerialized]
    // the initial color of the water shader on the top
    public Color initialWaterColorTop;

    [Tooltip("The GameObject of the water")]
    public GameObject waterObject;

    protected new void Start()
    {
        base.Start();
        // set initial water
        waterShaderMat = waterObject.GetComponent<Renderer>().material;
        waterShaderMat.SetFloat("_WaterAmount", MinSliderVal);

        // get initial colors from water shader
        initialWaterColorSide = waterShaderMat.GetColor("_SideColor");
        initialWaterColorTop = waterShaderMat.GetColor("_TopColor");
    }

    protected void UpdateWaterShader()
    {
        waterShaderMat.SetFloat("_WaterAmount", container.FilledPercantage() * SliderDifference + MinSliderVal);
        var color = container.GetColor();
        waterShaderMat.SetColor("_SideColor", color);
        waterShaderMat.SetColor("_TopColor", color + initialWaterColorTop - initialWaterColorSide);
    }
    protected new void FixedUpdate()
    {
        UpdateWaterShader();
        base.FixedUpdate();
    }
}
