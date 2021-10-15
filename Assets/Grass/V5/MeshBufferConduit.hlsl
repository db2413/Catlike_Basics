struct sourceVert
{
    float3 position;
    float4x4 rot;
    float2 uv0;
};

#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	StructuredBuffer<sourceVert> _SourceVertices;
	StructuredBuffer<float3> _VertexInfluences;
	float4x4 _ObjectToWarldRotation;
	float4x4 _ObjectToWarldScale;
	float4x4 _ObjectToWarldPosition;
	float _CardSize;
#endif

float RandNOneAndOne(float seed){
	return 2*frac(sin(seed*321.54))-1; // random between -1 and 1
}

// Rotation with angle (in radians) and axis
float4x4 AngleAxis4x4(float angle, float3 axis)
{
    float c, s;
    sincos(angle, s, c);

    float t = 1 - c;
    float x = axis.x;
    float y = axis.y;
    float z = axis.z;

    return float4x4(
        t * x * x + c,      t * x * y - s * z,  t * x * z + s * y, 0,
        t * x * y + s * z,  t * y * y + c,      t * y * z - s * x, 0,
        t * x * z - s * y,  t * y * z + s * x,  t * z * z + c, 0,
		0,0,0,1
    );
}

void ConfigureProcedural () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
            unity_ObjectToWorld._11_21_31_41 = float4(1, 0, 0, 0);
            unity_ObjectToWorld._12_22_32_42 = float4(0, 1, 0, 0);
            unity_ObjectToWorld._13_23_33_43 = float4(0, 0, 1, 0);
            unity_ObjectToWorld._14_24_34_44 = float4(0, 0, 0, 1);

			unity_WorldToObject = unity_ObjectToWorld;
            unity_WorldToObject._14_24_34 *= -1;
            unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
	#endif
}


void ShaderGraphFunction_float (float3 In, out float3 Out, out float3 InfluenceDisplacement) {
	Out = In;
	InfluenceDisplacement=0;
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		InfluenceDisplacement = _VertexInfluences[unity_InstanceID];

		float3 pos = _SourceVertices[unity_InstanceID].position;
		float4x4 rot = _SourceVertices[unity_InstanceID].rot;
		float3 up = mul(rot,float3(0,1,0));
		float4x4 spin = AngleAxis4x4(frac(sin(unity_InstanceID))*3, up);

		float4x4 uObjWrld = 0;
		uObjWrld._m00_m11_m22 = _CardSize * (1 + _SizeVariance * RandNOneAndOne(unity_InstanceID));
		uObjWrld = mul(rot, uObjWrld);
		uObjWrld = mul(spin, uObjWrld); //TODO Move to Vertex Displacement instead of the objToWorld
		uObjWrld._m03_m13_m23_m33 = float4(pos.x,pos.y,pos.z, 1);
		uObjWrld._m03_m13_m23_m33 = mul(_ObjectToWarldScale, uObjWrld._m03_m13_m23_m33);
		uObjWrld = mul(_ObjectToWarldRotation, uObjWrld);
		uObjWrld = mul(_ObjectToWarldPosition, uObjWrld);
		uObjWrld._m33 = 1;
		Out = mul(uObjWrld, float4(In,1));
	#endif
}

void ShaderGraphFunction_half (half3 In, out half3 Out, out float3 InfluenceDisplacement) {
	Out = In;
	InfluenceDisplacement=0;
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
		InfluenceDisplacement = _VertexInfluences[unity_InstanceID];
	#endif
}