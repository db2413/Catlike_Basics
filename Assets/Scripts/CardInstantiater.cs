using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInstantiater : MonoBehaviour
{
    public Transform card;
    ComputeBuffer sourceVerticeBuffer;
    MaterialPropertyBlock propertyBlock;
    MeshAnalyzer meshAnalyzer;

    private void OnEnable()
    {
        meshAnalyzer = GetComponent<MeshAnalyzer>();
        meshAnalyzer.OnDone.AddListener(OnValidate);
    }

    private void InstantiateCards()
    {
        List<Vertex> sourceVerts = meshAnalyzer?.GetScatteredVerts();
        if (sourceVerts == null)
        {
            return;
        }
        Debug.Log("Have source verts");
        sourceVerticeBuffer = new ComputeBuffer(sourceVerts.Count, 8 * 4);
    }

    private void OnDisable()
    {
        if (sourceVerticeBuffer != null)
        {
            sourceVerticeBuffer.Release();
            sourceVerticeBuffer = null;
        }
        meshAnalyzer?.OnDone.RemoveListener(OnValidate);
    }

    private void OnValidate()
    {
        if (sourceVerticeBuffer != null && enabled)
        {
            OnDisable();
            OnEnable();
        }
    }

    private void Update()
    {
        if (sourceVerticeBuffer == null)
        {
            InstantiateCards();
        }
    }
}
