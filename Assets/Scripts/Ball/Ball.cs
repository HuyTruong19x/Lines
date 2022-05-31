using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ball : MonoBehaviour
{
    private MeshRenderer _mesh;
    private bool _isFinished;
    private Vector2Int _location;
    private void Awake()
    {
        _mesh = GetComponent<MeshRenderer>();
    }
    public void FinishBall()
    {
        if(!_isFinished)
        {
            _isFinished = true;
            transform.localScale = Vector3.one * 0.9f;
            GridManager.Instance.GetTile(_location).SetShowed(true);
        }    
    }

    public void SetColor(Color i_color)
    {
        _mesh.material.color = i_color;
    }
    public void SetFinished(bool i_isFinished)
    {
        _isFinished = i_isFinished;
    }    
    public void SetLocation(Vector2Int i_location)
    {
        _location = i_location;
    }    
    public Vector2Int GetLocation()
    {
        return _location;
    }    
}
