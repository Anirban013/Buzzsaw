using UnityEngine;
using System.Collections;
using System;

public class FromWorldPointTextPositioner : IFloatingTextPositioner
{

    private readonly Camera _camera;
    private readonly Vector3 _worldPosition;
    private readonly float _speed;
    private float _timetoLive;
    private float _yoffset; 

    public FromWorldPointTextPositioner(Camera camera, Vector3 worldPosition, float timetoLive, float speed)
    {
        _camera = camera;
        _worldPosition = worldPosition;
        _speed = speed;
        _timetoLive = timetoLive;
    }

    public bool GetPosition(ref Vector2 position, GUIContent content, Vector2 size)
    {
        if ((_timetoLive -= Time.deltaTime) <= 0)
            return false;

        var screePosition = _camera.WorldToScreenPoint(_worldPosition);
        position.x = screePosition.x - (size.x / 2);
        position.y = Screen.height - screePosition.y - _yoffset;

        _yoffset += Time.deltaTime * _speed;
        return true;
    }
}
