using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Node(float x, float y)
    {
        fx = x;
        fy = y;
    }

    public int Height;
    
    public int x { get; private set; }

    public int y { get; private set; }

    public float fx
    {
        get => _fx;
        set {
            x = Mathf.RoundToInt(value);
            _fx = value;
        }
    }
    public float fy
    {
        get => _fy;
        set {
            y = Mathf.RoundToInt(value);
            _fy = value;
        }
    }
    

    private float _fy;
    private float _fx;


    public List<Node> Children = new();
}


public enum OutputMode
{
    Mesh,
    Terrain,
    Heightmap
}

public class Flattened2DArray<T>
{
    private readonly T[] _array;
    private readonly int _width;
    private readonly int _height;

    public int Width => _width;
    public int Height => _height;

    public Flattened2DArray(int width, int height)
    {
        _array = new T[width * height];
        _width = width;
        _height = height;
    }
    
    public T this[int x, int y]
    {
        get => _array[y * _width + x];
        set => _array[y * _width + x] = value;
    }

    public T[] As1DArray() => _array;

}
