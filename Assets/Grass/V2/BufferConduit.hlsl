struct sourceVert
{
    float3 position;
    float3 normal;
    float2 uv0;
};

#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	StructuredBuffer<sourceVert> _SourceVertices;
	float4 _MeshOrigin;
#endif

void ConfigureProcedural () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		float3 pos = _SourceVertices[unity_InstanceID].position;

		unity_ObjectToWorld = 0.0;
		unity_ObjectToWorld._m03_m13_m23_m33 = float4(pos.x,pos.y,pos.z, 1.0) + _MeshOrigin;
		unity_ObjectToWorld._m00_m11_m22 = 0.05;
	#endif
}

void ShaderGraphFunction_float (float3 In, out float3 Out) {
	Out = In;
}

void ShaderGraphFunction_half (half3 In, out half3 Out) {
	Out = In;
}