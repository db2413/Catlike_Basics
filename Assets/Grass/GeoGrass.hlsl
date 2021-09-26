struct GeomData
{
	float4 positionCS         : SV_POSITION;
	float3 positionWS         : TEXCOORD0;  
	float3 normalWS         : TEXCOORD1; 
	float4 tangentWS         : TEXCOORD2; 
	float3 viewDirectionWS     : TEXCOORD3; 
	float2 lightmapUV         : TEXCOORD4; 
	float3 sh                 : TEXCOORD5; 
	float4 fogFactorAndVertexLight : TEXCOORD6; 
	float4 shadowCoord         : TEXCOORD7;		
};

GeomData SetupVertex(float3 positionWS, float3 normalWS){
	GeomData output = (GeomData)0;
	output.positionWS = positionWS;
	output.normalWS = normalWS;

	output.positionCS = TransformWorldToHClip(positionWS);
	return output;
}



// Returns the normal of a plane containing the triangle defined by the three arguments
float3 GetNormalFromTriangle(float3 a, float3 b, float3 c) {
    return normalize(cross(b - a, c - a));
}

void SetupAndOutputTriangle(inout TriangleStream<GeomData> outputStream, GeomData a, GeomData b, GeomData c) {
	outputStream.RestartStrip();
	float3 normalWS = GetNormalFromTriangle(a.positionWS,b.positionWS, c.positionWS);
	outputStream.Append(SetupVertex(a.positionWS, normalWS));
	outputStream.Append(SetupVertex(b.positionWS, normalWS));
	outputStream.Append(SetupVertex(c.positionWS, normalWS));
}

void SetupAndOutPutQuad(inout TriangleStream<GeomData> outputStream, GeomData a, GeomData b, GeomData c, GeomData d){
	SetupAndOutputTriangle(outputStream, a, b, c);
	SetupAndOutputTriangle(outputStream, c, d, a);
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
	GeomData vert1 = input[1];
	GeomData vert2 = input[2];

	float3 center = GetTriangleCenter(vert0.positionWS, vert1.positionWS, vert2.positionWS);

	GeomData bottomA = (GeomData)0;
	GeomData bottomD = (GeomData)0;
	bottomA.positionWS = center + 0.5 * (vert0.positionWS - center);
	bottomD.positionWS = center - 0.5 * (vert0.positionWS - center);
	GeomData topB = (GeomData)0;
	GeomData topC = (GeomData)0;
	topB.positionWS = center + 0.5 * (vert0.positionWS - center) + GetNormalFromTriangle(vert0.positionWS, vert1.positionWS, vert2.positionWS) ;
	topC.positionWS = center - 0.5 * (vert0.positionWS - center) + GetNormalFromTriangle(vert0.positionWS, vert1.positionWS, vert2.positionWS);

	SetupAndOutPutQuad(triStream, bottomA, topB, topC, bottomD);
	SetupAndOutputTriangle(triStream, vert0, vert1, vert2);
}