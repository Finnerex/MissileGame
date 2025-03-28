using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Windows;
using Random = System.Random;

public class DLATerrainGenerator : MonoBehaviour
{
    [SerializeField] private ComputeShader DLAComputeShader;

    [SerializeField] private int resolutionTarget = 512;
    [SerializeField] private int startSize = 8; // TODO, end size must be power of 2

    [SerializeField] private int blurAmount = 3;
    [SerializeField] private int smoothingIterations = 2;
    [SerializeField] private int smoothAmount = 2;

    [SerializeField] [Range(0, 0.7f)] private float fillThreshold = 0.2f;

    [SerializeField] private int peaks = 4;

    [SerializeField] private AnimationCurve ridgeFalloffCurve;

    // [SerializeField] private float falloffMax = 20; // effectively correlates to range scale
    [SerializeField] private float heightScale = 15;

    [SerializeField] private bool randomSeed;
    [SerializeField] private int seed;

    public OutputMode outputMode = OutputMode.Mesh;

    [HideInInspector] public float meshScale = 10;
    [HideInInspector] public MeshFilter meshFilter; // change to getcomponent

    [HideInInspector] public Terrain terrain;

    [HideInInspector] public string heightmapFileLocation;

    private List<Node> _rootNodes;
    private Node[,] _map;
    private Flattened2DArray<float> /*float[,]*/ _heightmap;
    private int _currentSize;

    private int _pixelsFilled;
    private Random _random; // might use unity static random maybe

    private static readonly int ShaderInputBufID = Shader.PropertyToID("Input");
    private static readonly int ShaderOutputBufID = Shader.PropertyToID("Output");
    private static readonly int BlurSizeID = Shader.PropertyToID("blurSize");
    

    public void GenerateTerrain()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        // initialization
        _map = new Node[startSize, startSize];
        _heightmap = new Flattened2DArray<float>(startSize, startSize);//float[startSize, startSize];

        _currentSize = startSize;

        if (randomSeed)
            seed = new Random().Next();

        _random = new Random(seed);

        // peaks / root nodes
        _rootNodes = new List<Node>();
        _pixelsFilled = 0;
        for (int i = 0; i < peaks; i++)
        {
            int x = _random.Next(0, _currentSize);
            int y = _random.Next(0, _currentSize);

            if (_map[x, y] != null) continue;

            Node start = new Node(x, y);
            _map[x, y] = start;
            _rootNodes.Add(start);
            _pixelsFilled++;
        }

        

        // main generation
        while (_currentSize < resolutionTarget)
        {
            // add new nodes until some amount full
            while (_pixelsFilled <= _currentSize * _currentSize * fillThreshold)
                AddPixel();

            // upscale both maps and blur
            _currentSize *= 2;
            _pixelsFilled *= 2;

            UpscaleDetailed();
            UpdateAndScaleHeightmap();
            
        }
        
        
        // one final pass
        while (_pixelsFilled <= _currentSize * _currentSize * fillThreshold)
            AddPixel();
        
        foreach (Node node in _rootNodes)
            AddDetailedWithFalloff(node);

        for (int i = 0; i < smoothingIterations; i++)
            _heightmap = Blur(_heightmap, smoothAmount);
        

        watch.Stop();
        Debug.Log($"Generated Terrain Heightmap in {watch.Elapsed.TotalMilliseconds}ms");


        switch (outputMode)
        {
            case OutputMode.Mesh:
                GenerateMesh();
                break;
            case OutputMode.Terrain:
                ApplyToTerrain();
                break;
            case OutputMode.Heightmap:
                SaveGrayscaleImage(_heightmap, heightmapFileLocation  + "map.png");
                break;
        }

    }

    private void AddPixel()
    {
        // start in an empty space
        int x, y;
        do
        {
            x = _random.Next(0, _currentSize);
            y = _random.Next(0, _currentSize);
        } while (_map[x, y] != null);

        // move until it hits something
        while (true)
        {
            int axis = _random.Next(0, 2);
            int dx = axis == 0 ? (_random.Next(0, 2) == 0 ? -1 : 1) : 0;
            int dy = axis == 1 ? (_random.Next(0, 2) == 0 ? -1 : 1) : 0;

            if (x + dx > _currentSize - 1 || x + dx < 0)
                dx = -dx;
            if (y + dy > _currentSize - 1 || y + dy < 0)
                dy = -dy;

            Node hitNode = _map[x + dx, y + dy];
            if (hitNode == null)
            {
                x += dx;
                y += dy;
                continue;
            }

            Node newNode = new Node(x, y);
            _map[x, y] = newNode;
            hitNode.Children.Add(newNode);

            _pixelsFilled++;
            return;
        }

    }

    private void UpscaleDetailed()
    {
        // current size is now updated
        _map = new Node[_currentSize, _currentSize];

        foreach (Node node in _rootNodes)
            UpscaleSingleDetailed(node);
    }

    private void UpscaleSingleDetailed(Node node)
    {
        node.fx *= 2;
        // node.Fx += (float)_random.NextDouble() - 0.5f;
        node.fy *= 2;
        // node.Fy += (float)_random.NextDouble() - 0.5f;
        _map[node.x, node.y] = node;

        List<Node> oldChildren = node.Children;
        node.Children = new List<Node>();

        foreach (Node child in oldChildren)
        {
            // make a point in between
            float dx = (child.fx * 2 - node.fx) * 0.5f;
            float dy = (child.fy * 2 - node.fy) * 0.5f;

            // random jiggle
            float x = node.fx + dx + ((float)_random.NextDouble() - 0.5f);
            float y = node.fy + dy + ((float)_random.NextDouble() - 0.5f);

            x = Math.Clamp(x, 0, _currentSize - 1);
            y = Math.Clamp(y, 0, _currentSize - 1);

            Node newChild = new Node(x, y);
            _map[newChild.x, newChild.y] = newChild;

            node.Children.Add(newChild);
            newChild.Children.Add(child);

            // do all this to the children (i sure love recursion)
            UpscaleSingleDetailed(child);
        }

    }

    private void AddUpscaleBlurred()
    {
        // convolution and linear interpolation upscale
        var watch = System.Diagnostics.Stopwatch.StartNew();

        _heightmap = LerpScale(_heightmap);
        _heightmap = Blur(_heightmap, blurAmount);
        
        watch.Stop();
        Debug.Log($"Completetd blur and lerpscale in {watch.Elapsed.TotalMilliseconds}ms");
    }

    private Flattened2DArray<float> LerpScale(Flattened2DArray<float> input)
    {
        return DispatchToShader(input, "Upscale");
    }

    // HDRP Fuckin sucks idk how to make the compute shader work eventhough it did elswise
    private Flattened2DArray<float> DispatchToShader(Flattened2DArray<float> input, string kernelName) // TODO try testing with set in/outs
    {
        int kernelIndex = DLAComputeShader.FindKernel(kernelName); // should probably cache
        
        DLAComputeShader.SetInt("size", input.Width); // cache please
        Debug.Log($"kernel: {kernelName}, cs: {_currentSize}, ins{input.Width}, tg: {((float)_currentSize + 7) * 0.125f}");

        ComputeBuffer inputBuffer = new ComputeBuffer(input.Height * input.Width, sizeof(float), ComputeBufferType.Structured);
        inputBuffer.SetData(input.As1DArray());

        Flattened2DArray<float> output = new Flattened2DArray<float>(_currentSize, _currentSize);
        ComputeBuffer outputBuffer = new ComputeBuffer(_currentSize * _currentSize, sizeof(float), ComputeBufferType.Structured);
        outputBuffer.SetData(output.As1DArray());

        DLAComputeShader.SetBuffer(kernelIndex, ShaderInputBufID, inputBuffer);
        DLAComputeShader.SetBuffer(kernelIndex, ShaderOutputBufID, outputBuffer);

        int threadGroups = Mathf.CeilToInt(((float)_currentSize + 7) * 0.125f);

        DLAComputeShader.Dispatch(kernelIndex, threadGroups, threadGroups, 1);
        outputBuffer.GetData(output.As1DArray());
        
        inputBuffer.Release();
        outputBuffer.Release();
        
        return output;
    }
    

    private Flattened2DArray<float> Blur(Flattened2DArray<float> image, int blurSize)
    {
        
        blurSize = Math.Max(1, blurSize);
        DLAComputeShader.SetInt(BlurSizeID, blurSize);
        return DispatchToShader(image, "Blur");
        
    }

    private void UpdateAndScaleHeightmap()
    {
        AddUpscaleBlurred();
        
        foreach (Node node in _rootNodes)
            AddDetailedWithFalloff(node);
        
    }

    private void AddDetailedWithFalloff(Node node)
    {
        GetNodeHeights(node);
        ApplyNodeHeights(node, node.Height);
    }

    private int GetNodeHeights(Node node)
    {
        // recursion again!!
        int max = 0;
        foreach (Node child in node.Children)
        {
            int height = GetNodeHeights(child);
            if (height > max)
                max = height;
        }

        node.Height = max + 1;
        
        return max + 1;
    }

    private void ApplyNodeHeights(Node node, int maxHeight)
    {
        
        _heightmap[node.x, node.y] += heightScale * ridgeFalloffCurve.Evaluate((float)node.Height / maxHeight);

        foreach (Node child in node.Children)
            ApplyNodeHeights(child, maxHeight);

    }
    
   
    private void GenerateMesh() // maybe this could be done with compute shader
    {
        int width = _currentSize;
        int height = _currentSize;

        Vector3[] vertices = new Vector3[width * height];
        int[] triangles = new int[(width - 1) * (height - 1) * 6];
        Vector2[] uvs = new Vector2[vertices.Length];

        // Generate vertices and UVs
        for (int z = 0; z < height; z++)
        for (int x = 0; x < width; x++)
        {
            int index = z * width + x;
            vertices[index] = new Vector3(x * meshScale, _heightmap[x, z], z * meshScale);
            uvs[index] = new Vector2((float)x / width, (float)z / height);
        }
        

        // Generate triangles
        int triangleIndex = 0;
        for (int z = 0; z < height - 1; z++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                int topLeft = z * width + x;
                int topRight = topLeft + 1;
                int bottomLeft = topLeft + width;
                int bottomRight = bottomLeft + 1;

                // First triangle
                triangles[triangleIndex++] = topLeft;
                triangles[triangleIndex++] = bottomLeft;
                triangles[triangleIndex++] = topRight;

                // Second triangle
                triangles[triangleIndex++] = topRight;
                triangles[triangleIndex++] = bottomLeft;
                triangles[triangleIndex++] = bottomRight;
            }
        }

        // Create mesh
        Mesh mesh = new Mesh();
        if (width * height > 65535)
            mesh.indexFormat = IndexFormat.UInt32; // Use 32-bit indices if needed

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        // Assign to MeshFilter
        meshFilter.mesh = mesh;
    }
    
    private void SaveGrayscaleImage(Flattened2DArray<float> pixelValues, string filePath)
    {
        int width = pixelValues.Width;
        int height = pixelValues.Height;

        // Find min and max values in the float array
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            float value = pixelValues[x, y];
            if (value < minValue) minValue = value;
            if (value > maxValue) maxValue = value;
        }

        // Avoid division by zero if all values are the same
        float range = maxValue - minValue;
        if (range == 0) range = 1;

        Texture2D texture = new Texture2D(width, height, TextureFormat.RFloat, false);

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            // Normalize value between 0 and 1
            float normalizedValue = (pixelValues[x, y] - minValue) / range;
            Color color = new Color(normalizedValue, normalizedValue, normalizedValue); // Grayscale
            texture.SetPixel(x, y, color);
        }
        
        texture.Apply();

        // Encode to PNG and save the file
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        Debug.Log($"Image saved to: {filePath}");

        // Clean up
        DestroyImmediate(texture);
    }
    
    private void ApplyToTerrain()
    {
        int width = _currentSize;
        int height = _currentSize;
        TerrainData terrainData = terrain.terrainData;
        terrainData.heightmapResolution = _currentSize + 1;
        
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            float value = _heightmap[x, y];
            if (value < minValue) minValue = value;
            if (value > maxValue) maxValue = value;
        }
        
        float range = maxValue - minValue;
        if (range == 0) range = 1;

        float[,] heights = new float[width, height];

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
            heights[y, x] = _heightmap[x, y] / range;

        terrainData.SetHeights(0, 0, heights);
    }
    
}
