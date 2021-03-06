﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShapeController : MonoBehaviour
{
    #region Constants
    private Vector2[] hexagon = new Vector2[] {new Vector2(0, 1.1f),
                                               new Vector2(1.0f, 0.50f),
                                               new Vector2(1.0f, -0.50f),
                                               new Vector2(0, -1.1f),
                                               new Vector2(-1.0f, -0.50f),
                                               new Vector2(-1.0f, 0.50f)};
    private Vector2[] triangle = new Vector2[] {new Vector2(-1, -1),
                                                new Vector2(0, 1),
                                                new Vector2(1, -1)};
    private Vector2[] square = new Vector2[] {new Vector2(-1, -1),
                                              new Vector2(-1, 1),
                                              new Vector2(1, 1),
                                              new Vector2(1, -1)};
    private Vector2[] pentagon = new Vector2[] {new Vector2(-1, 0.33f),
                                                new Vector2(0, 1),
                                                new Vector2(1, 0.33f),
                                                new Vector2(0.6f, -0.8f),
                                                new Vector2(-0.6f, -0.8f)};

    public enum ShapeType
    {
        Hexagon,
        Triangle,
        Square,
        Pentagon,
    }
    #endregion

    #region Fields
    private Dictionary<ShapeType, Vector2[]> Vertices;
    #endregion

    #region Inspector Variables
    public GameObject ShapeObject;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Create our vertices dictionary.
        Vertices = new Dictionary<ShapeType, Vector2[]>();
        Vertices.Add(ShapeType.Hexagon, hexagon);
        Vertices.Add(ShapeType.Triangle, triangle);
        Vertices.Add(ShapeType.Square, square);
        Vertices.Add(ShapeType.Pentagon, pentagon);

        // Create a hexagon to start.
        CreateShape(ShapeType.Hexagon);

        // Add event handler.
        ShapeObject.GetComponent<Shape>().OnDoubleClick += OnDoubleClick;

        /*EventTrigger trigger = Shape.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { OnPointerClick((PointerEventData)data); });
        trigger.triggers.Add(entry);*/
    }

    public void CreateShape(ShapeType shape)
    {
        // Make sure the shape is being shown.
        ShapeObject.SetActive(true);

        // Create our base shape.
        var vertices = Array.ConvertAll<Vector2, Vector3>(Vertices[shape], v => v);

        // Use the triangulator to get indices for creating triangles.
        var triangulator = new Triangulator(Vertices[shape]);
        var indices = triangulator.Triangulate();

        // Set default color for each vertex.
        var colors = Enumerable.Range(0, vertices.Length).Select(i => Color.blue).ToArray();

        // Create the mesh.
        var mesh = new Mesh
        {
            vertices = vertices,
            triangles = indices,
            colors = colors
        };

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var meshRenderer = ShapeObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = ShapeObject.AddComponent<MeshRenderer>();
        }

        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));

        var filter = ShapeObject.GetComponent<MeshFilter>();
        if (filter == null)
        {
            filter = ShapeObject.AddComponent<MeshFilter>();
        }

        filter.mesh = mesh;

        var collider = ShapeObject.GetComponent<MeshCollider>();
        if (collider == null)
        {
            collider = ShapeObject.AddComponent<MeshCollider>();
        }

        collider.sharedMesh = mesh;
    }

    public void HideShape()
    {
        ShapeObject.SetActive(false);
    }

    private void OnDoubleClick(object sender, EventArgs eventArgs)
    {
        // Unregister while we're updating the color.
        ShapeObject.GetComponent<Shape>().OnDoubleClick -= OnDoubleClick;

        // Assign a random color.
        var filter = ShapeObject.GetComponent<MeshFilter>();
        // Randomly decide to get 1 single color or multiple colors.
        bool multiColor = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
        if (multiColor)
        {
            filter.mesh.colors = Enumerable.Range(0, filter.mesh.vertexCount).Select(i => UnityEngine.Random.ColorHSV()).ToArray();
        }
        else
        {
            var color = UnityEngine.Random.ColorHSV();
            filter.mesh.colors = Enumerable.Range(0, filter.mesh.vertexCount).Select(i => color).ToArray();
        }

        // Register the double click event.
        ShapeObject.GetComponent<Shape>().OnDoubleClick += OnDoubleClick;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
