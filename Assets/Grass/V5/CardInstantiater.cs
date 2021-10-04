using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V5
{
    public class CardInstantiater : MonoBehaviour
    {
        [SerializeField] Mesh card;
        [SerializeField] float cardSize = 0.1f;
        [SerializeField] Material material;
        [SerializeField] private ComputeShader computeShader = default;
        [SerializeField] private Influence mainInfluence;

        // Instantiate the shaders so data belong to their unique compute buffers
        private ComputeShader m_InfluenceComputeShader;
        private int influenceKernel;
        private int dispatchSize;
        ComputeBuffer sourceVerticeBuffer;
        ComputeBuffer influenceBuffer;
        MaterialPropertyBlock propertyBlock;
        MeshAnalyzer meshAnalyzer;
        List<Vertex> sourceVerts;

        static int
            srcVertId = Shader.PropertyToID("_SourceVertices");

        private void OnEnable()
        {
            meshAnalyzer = GetComponent<MeshAnalyzer>();
            meshAnalyzer.OnDone.AddListener(OnValidate);
        }

        private void SetUpComputeShader()
        {
            // Instantiate the compute shaders so they can point to their own buffers
            m_InfluenceComputeShader = Instantiate(computeShader);
            influenceKernel = m_InfluenceComputeShader.FindKernel("CSMain");
            influenceBuffer = new ComputeBuffer(sourceVerts.Count, 3 * 4); // 3 for influence vector. 4 Bytes each
            //Shader.SetGlobalBuffer("_VertexInfluences", influenceBuffer);
            m_InfluenceComputeShader.SetBuffer(influenceKernel, "_VertexInfluences", influenceBuffer);
            m_InfluenceComputeShader.SetBuffer(influenceKernel,srcVertId,sourceVerticeBuffer);
            m_InfluenceComputeShader.GetKernelThreadGroupSizes(influenceKernel, out uint threadGroupSize, out _, out _);
            dispatchSize = Mathf.CeilToInt((float)sourceVerts.Count / threadGroupSize);
            propertyBlock.SetBuffer("_VertexInfluences", influenceBuffer);
        }

        private void InstantiateCards()
        {
            sourceVerts = meshAnalyzer?.GetScatteredVerts();
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
            if (influenceBuffer != null)
            {
                influenceBuffer.Release();
                influenceBuffer = null;
            }
            meshAnalyzer?.OnDone.RemoveListener(OnValidate);
        }

        private void OnValidate()
        {
            if ((sourceVerticeBuffer != null || influenceBuffer != null) && enabled)
            {
                OnDisable();
                OnEnable();
            }
        }

        private void Update()
        {
            if (sourceVerticeBuffer == null || influenceBuffer == null)
            {
                InstantiateCards();
                SetUpComputeShader();
                return;
            }

            m_InfluenceComputeShader.SetVector("_Influences", mainInfluence.transform.position);
            m_InfluenceComputeShader.Dispatch( influenceKernel, dispatchSize, 1, 1);

            propertyBlock.SetMatrix(Shader.PropertyToID("_ObjectToWarldRotation"), Matrix4x4.Rotate(transform.rotation));
            propertyBlock.SetVector(Shader.PropertyToID("_ObjPos"), transform.position);
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
}