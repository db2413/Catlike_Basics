struct GeomData
{
	float4 positionCS         : SV_POSITION;
	float3 positionWS         : TEXCOORD0;  
	float3 normalWS         : TEXCOORD1; 
	float4 tangentWS         : TEXCOORD2; 
	float4 uv0				: TEXCOORD3;		
	float3 viewDirectionWS     : TEXCOORD4; 
	float2 lightmapUV         : TEXCOORD5; 
	float3 sh                 : TEXCOORD6; 
	float4 fogFactorAndVertexLight : TEXCOORD7; 
	float4 shadowCoord         : TEXCOORD8;		
};

GeomData SetupVertex(float3 positionWS, float3 normalWS, float4 uv0, float3 center){
	GeomData output = (GeomData)0;
	output.positionWS = mul(unity_CameraToWorld,positionWS-center) + center;
	//output.positionWS = (positionWS-center + output.positionWS-center)/2 + center;
	output.normalWS = mul(unity_WorldToCamera, normalWS);
	output.uv0 = uv0;
	output.positionCS = TransformWorldToHClip(output.positionWS);
	return output;
}

// Returns the normal of a plane containing the triangle defined by the three arguments
float3 GetNormalFromTriangle(float3 a, float3 b, float3 c) {
    return normalize(cross(b - a, c - a));
}

void SetupAndOutputTriangle(inout TriangleStream<GeomData> outputStream, GeomData a, GeomData b, GeomData c, float3 center) {
	outputStream.RestartStrip();
	float3 normalWS = GetNormalFromTriangle(a.positionWS,b.positionWS, c.positionWS);
	outputStream.Append(SetupVertex(a.positionWS, normalWS, a.uv0, center));
	outputStream.Append(SetupVertex(b.positionWS, normalWS, b.uv0, center));
	outputStream.Append(SetupVertex(c.positionWS, normalWS, c.uv0, center));
}

void SetupAndOutPutQuad(inout TriangleStream<GeomData> outputStream, GeomData a, GeomData b, GeomData c, GeomData d, float3 center){
	SetupAndOutputTriangle(outputStream, a, b, c, center);
	SetupAndOutputTriangle(outputStream, c, d, a, center);
}

// Returns the center point of a triangle defined by the three arguments
float3 GetTriangleCenter(float3 a, float3 b, float3 c) {
    return (a + b + c) / 3.0;
}
float2 GetTriangleCenter(float2 a, float2 b, float2 c) {
    return (a + b + c) / 3.0;
}

// Returns the view direction in world space
float3 GetViewDirectionFromPosition(float3 positionWS) {
    return normalize(GetCameraPositionWS() - positionWS);
}

[maxvertexcount(9)]
void geom(triangle GeomData input[3], inout TriangleStream<GeomData> triStream)
{
	GeomData vert0 = input[0];
	vert0.uv0 = float4(0,0,0,0);
	GeomData vert1 = input[1];
	vert1.uv0 = float4(0,0,0,0);
	GeomData vert2 = input[2];
	vert2.uv0 = float4(0,0,0,0);

	float3 center = GetTriangleCenter(vert0.positionWS, vert1.positionWS, vert2.positionWS);
	float randomX = frac(sin(center.x+center.y+center.z)*10000) - 0.5;
	float randomZ = frac(sin(randomX)*10000) -0.5;
	float randomSize = frac(sin(randomZ)*10000)/4 + 0.25;
	randomSize = randomSize/1.4;
	center = float3(center.x+randomX,center.y, center.z + randomZ);

	GeomData bottomA = (GeomData)0;
	bottomA.positionWS = center + randomSize * float3(1,0,0);
	bottomA.uv0 = float4(0,0,0,0);
	GeomData bottomD = (GeomData)0;
	bottomD.positionWS = center - randomSize * float3(1,0,0);
	bottomD.uv0 = float4(1,0,0,0);
	GeomData topB = (GeomData)0;
	topB.positionWS = center + randomSize * float3(1,0,0) + 2 * randomSize * GetNormalFromTriangle(vert0.positionWS, vert1.positionWS, vert2.positionWS);
	topB.uv0 = float4(0,1,0,0);
	GeomData topC = (GeomData)0;
	topC.positionWS = center - randomSize * float3(1,0,0) + 2 * randomSize * GetNormalFromTriangle(vert0.positionWS, vert1.positionWS, vert2.positionWS);
	topC.uv0 = float4(1,1,0,0);

	SetupAndOutPutQuad(triStream, bottomA, topB, topC, bottomD, center);
	//SetupAndOutputTriangle(triStream, vert0, vert1, vert2);
}