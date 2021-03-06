﻿using ShaderGen;
using System.Numerics;
using static ShaderGen.ShaderBuiltins;

namespace TestShaders
{
    public class TestVertexShader
    {
        public Matrix4x4 World;
        public Matrix4x4 View;
        public Matrix4x4 Projection;

        [VertexShader]
        public VertexOutput VS(PositionTexture input)
        {
            VertexOutput output;
            Vector4 worldPosition = Mul(World, new Vector4(input.Position, 1));
            Vector4 viewPosition = Mul(View, worldPosition);
            output.Position = Mul(Projection, viewPosition);
            output.TextureCoord = input.TextureCoord;
            return output;
        }

        public struct VertexOutput
        {
            [VertexSemantic(SemanticType.Position)]
            public Vector4 Position;
            [VertexSemantic(SemanticType.TextureCoordinate)]
            public Vector2 TextureCoord;
        }
    }
}

// Should generate this HLSL:

/*

cbuffer WorldBuffer : register(b0)
{
    float4x4 World;
}

cbuffer ViewMatrixBuffer : register(b1)
{
    float4x4 View;
}

cbuffer ProjectionMatrixBuffer : register(b2)
{
    float4x4 Projection;
}

struct PositionTexture
{
    float3 Position : POSITION;
    float2 TextureCoord : TEXCOORD0
};

struct PSInput
{
    float4 Position : SV_POSITION;
    float2 TextureCoord: TEXCOORD0;
};

PSInput VS(VSInput input)
{
    PSInput output;
    float4 worldPosition = mul(view, float4(input.Position, 1));
    float4 viewPosition = mul(view, worldPosition);
    output.position = mul(projection, viewPosition);
    output.worldPosition = worldPosition.xyz;

    return output;
}

*/
