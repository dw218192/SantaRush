using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SnowParticle : MonoBehaviour, IResolutionScaleHandler
{
    [SerializeField]
    float _xOffset = 0;

    ParticleSystem _particles = null;
    float _baseShapeWidthScale = 0;

    // Start is called before the first frame update
    void Start()
    {
        var pos = transform.position;
        pos.x = GameConsts.worldCameraCenter.x + _xOffset;
        transform.position = pos;

        _particles = GetComponent<ParticleSystem>();
        var shape = _particles.shape;
        _baseShapeWidthScale = shape.scale.x;
    }

    public void OnResolutionScale(ResolutionScaleEventData eventData)
    {
        var pos = transform.position;
        pos.x = GameConsts.worldCameraCenter.x + _xOffset;
        transform.position = pos;

        var shape = _particles.shape;
        var scale = shape.scale;
        scale.x = _baseShapeWidthScale * eventData.scaleFactor;
        shape.scale = scale;
    }
}
