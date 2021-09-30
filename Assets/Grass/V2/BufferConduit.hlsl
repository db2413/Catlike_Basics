struct sourceVert
{
    float3 position;
    float4x4 rot;
    float2 uv0;
};

#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	StructuredBuffer<sourceVert> _SourceVertices;
	float4x4 _ObjectToWarldRotation;
	float4x4 _ObjectToWarldScale;
	float4x4 _ObjectToWarldPosition;
	float _CardSize;
#endif

void ConfigureProcedural () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		float3 pos = _SourceVertices[unity_InstanceID].position;
		float4x4 rot = _SourceVertices[unity_InstanceID].rot;

		unity_ObjectToWorld = 0;
		unity_ObjectToWorld._m00_m11_m22 = _CardSize;
		unity_ObjectToWorld = mul(rot, unity_ObjectToWorld);
		unity_ObjectToWorld._m03_m13_m23_m33 = float4(pos.x,pos.y,pos.z, 1);
		unity_ObjectToWorld._m03_m13_m23_m33 = mul(_ObjectToWarldScale, unity_ObjectToWorld._m03_m13_m23_m33);
		unity_ObjectToWorld = mul(_ObjectToWarldRotation, unity_ObjectToWorld);
		unity_ObjectToWorld = mul(_ObjectToWarldPosition, unity_ObjectToWorld);
		//float3 s = unity_ObjectToWorld;
	#endif
}

void ShaderGraphFunction_float (float3 In, out float3 Out) {
	Out = In;
}

void ShaderGraphFunction_half (half3 In, out half3 Out) {
	Out = In;
}