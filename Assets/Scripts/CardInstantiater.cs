using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInstantiater : MonoBehaviour
{
    [SerializeField] Mesh card;
    [SerializeField] float cardSize;
    [SerializeField] Material material;


    ComputeBuffer sourceVerticeBuffer;
    MaterialPropertyBlock propertyBlock;
    MeshAnalyzer meshAnalyzer;

    static int
        srcVertId = Shader.PropertyToID("_SourceVertices");

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
        Debug.Log("Have source verts:" + sourceVerts.Count);
        sourceVerticeBuffer = new ComputeBuffer(sourceVerts.Count, 21 * 4); // 3 for position,3 for normal,2 for uv. 4 Bytes each
        sourceVerticeBuffer.SetData(sourceVerts.ToArray());
        propertyBlock ??= new MaterialPropertyBlock();
        propertyBlock.SetBuffer(srcVertId, sourceVerticeBuffer);
        Vector3 meshOrigin = meshAnalyzer.transform.position;
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
            return;
        }

        Vector3 meshOrigin = meshAnalyzer.transform.position;
        propertyBlock.SetMatrix(Shader.PropertyToID("_ObjectToWarldRotation"), Matrix4x4.Rotate(transform.rotation));
        propertyBlock.SetMatrix(Shader.PropertyToID("_ObjectToWarldPosition"), Matrix4x4.Translate(transform.position));
        propertyBlock.SetMatrix(Shader.PropertyToID("_ObjectToWarldScale"), Matrix4x4.Scale(transform.localScale));
        propertyBlock.SetFloat("_CardSize", cardSize);
        Graphics.DrawMeshInstancedProcedural(
            card,
            0,
            material,
            meshAnalyzer.GetBounds(),
            sourceVerticeBuffer.count,
            propertyBlock
        );
    }
}
