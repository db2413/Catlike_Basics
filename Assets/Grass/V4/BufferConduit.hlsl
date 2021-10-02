struct sourceVert
{
    float3 position;
    float4x4 rot;
    float2 uv0;
};

#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	StructuredBuffer<sourceVert> _SourceVertices;
	float4x4 _ObjectToWarldPosition;
	float4x4 _ObjectToWarldRotation;
	float4x4 _ObjectToWarldScale;
#endif

void ConfigureProcedural () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		float3 pos = _SourceVertices[unity_InstanceID].position;

		//unity_ObjectToWorld = 0;
		unity_ObjectToWorld = _ObjectToWarldPosition;
	#endif
}

void ShaderGraphFunction_float (float3 In, out float3 Out, out float3 Normal, out float3 Pos) {
	Out = In;
	Normal = -GetViewForwardDir();
	Pos = float3(1,0,0);
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		Normal = mul(_SourceVertices[unity_InstanceID].rot, float3(0,1,0));
		Normal = normalize(mul(_ObjectToWarldRotation, Normal));
		Pos = mul(_ObjectToWarldRotation, mul(_ObjectToWarldScale, _SourceVertices[unity_InstanceID].position)).xyz;
	#endif
}

void ShaderGraphFunction_half (half3 In, out half3 Out, out half3 Normal, out half3 Pos) {
	Out = In;
	Pos = float3(1,0,0);
	Normal = -GetViewForwardDir();
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		Normal = mul(_SourceVertices[unity_InstanceID].rot, float3(0,1,0));
		Normal = normalize(mul(_ObjectToWarldRotation, Normal));
		Pos = mul(_ObjectToWarldRotation, mul(_ObjectToWarldScale, _SourceVertices[unity_InstanceID].position)).xyz;
	#endif
}
