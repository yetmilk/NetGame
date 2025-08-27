Shader "NatureManufacture/URP/Lit/Lava/Volcano Heated Smoke"
{
    Properties
    {
        _Cutoff("Alpha Treshold", Range(0, 1)) = 0.5
        _Color("Particle Color (RGB) Alpha(A)", Color) = (1, 1, 1, 1)
        [NoScaleOffset]_ParticleTexture("Particle Mask (A)", 2D) = "white" {}
        _ParticleMaskTilingOffset("Particle Mask Tiling and Offset", Vector) = (1, 1, 0, 0)
        [Normal][NoScaleOffset]_BumpMap("Particle Normal", 2D) = "bump" {}
        _BumpScale("Particle Normal Scale", Float) = 0.3
        _NormalTilingOffset("Normal Tiling and Offset", Vector) = (1, 1, 0, 0)
        _Distortion("Distortion", Range(0, 1)) = 0.1
        _Metallic("Metallic", Range(0, 1)) = 0
        _AO("AO", Range(0, 1)) = 0
        _Smoothness("Smoothness", Range(0, 1)) = 0
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Lit"
            "Queue"="Transparent"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalLitSubTarget"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
        // Render State
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DYNAMICLIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
        #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
        #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
        #pragma multi_compile_fragment _ _SHADOWS_SOFT
        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ SHADOWS_SHADOWMASK
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ _LIGHT_LAYERS
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        #pragma multi_compile_fragment _ _LIGHT_COOKIES
        #pragma multi_compile _ _CLUSTERED_RENDERING
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
        #define VARYINGS_NEED_SHADOW_COORD
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_FORWARD
        #define _FOG_FRAGMENT 1
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _ALPHATEST_ON 1
        #define REQUIRE_OPAQUE_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
             float4 color;
             float3 viewDirectionWS;
            #if defined(LIGHTMAP_ON)
             float2 staticLightmapUV;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
             float2 dynamicLightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
             float3 sh;
            #endif
             float4 fogFactorAndVertexLight;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
             float4 shadowCoord;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 TangentSpaceNormal;
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float4 interp3 : INTERP3;
             float4 interp4 : INTERP4;
             float3 interp5 : INTERP5;
             float2 interp6 : INTERP6;
             float2 interp7 : INTERP7;
             float3 interp8 : INTERP8;
             float4 interp9 : INTERP9;
             float4 interp10 : INTERP10;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.tangentWS;
            output.interp3.xyzw =  input.texCoord0;
            output.interp4.xyzw =  input.color;
            output.interp5.xyz =  input.viewDirectionWS;
            #if defined(LIGHTMAP_ON)
            output.interp6.xy =  input.staticLightmapUV;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
            output.interp7.xy =  input.dynamicLightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.interp8.xyz =  input.sh;
            #endif
            output.interp9.xyzw =  input.fogFactorAndVertexLight;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.interp10.xyzw =  input.shadowCoord;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.tangentWS = input.interp2.xyzw;
            output.texCoord0 = input.interp3.xyzw;
            output.color = input.interp4.xyzw;
            output.viewDirectionWS = input.interp5.xyz;
            #if defined(LIGHTMAP_ON)
            output.staticLightmapUV = input.interp6.xy;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
            output.dynamicLightmapUV = input.interp7.xy;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.sh = input.interp8.xyz;
            #endif
            output.fogFactorAndVertexLight = input.interp9.xyzw;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.shadowCoord = input.interp10.xyzw;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Cutoff;
        float4 _Color;
        float4 _ParticleTexture_TexelSize;
        float4 _ParticleMaskTilingOffset;
        float4 _BumpMap_TexelSize;
        float _BumpScale;
        float4 _NormalTilingOffset;
        float _Distortion;
        float _Metallic;
        float _AO;
        float _Smoothness;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_ParticleTexture);
        SAMPLER(sampler_ParticleTexture);
        TEXTURE2D(_BumpMap);
        SAMPLER(sampler_BumpMap);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        void Unity_SceneColor_float(float4 UV, out float3 Out)
        {
            Out = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV.xy);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float3 NormalTS;
            float3 Emission;
            float Metallic;
            float Smoothness;
            float Occlusion;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0 = _Color;
            float4 _ScreenPosition_937829c9b1a6d685baa02d92309ac38b_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float _Property_d1852c92a8c24381991d0dcd5b0251b8_Out_0 = _Distortion;
            UnityTexture2D _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0 = UnityBuildTexture2DStructNoScale(_BumpMap);
            float4 _Property_269571a8a5154482b825042058b5c3b3_Out_0 = _NormalTilingOffset;
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_R_1 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[0];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_G_2 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[1];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_B_3 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[2];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_A_4 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[3];
            float4 _Combine_845cb1e758f7a887bcd17694c573d134_RGBA_4;
            float3 _Combine_845cb1e758f7a887bcd17694c573d134_RGB_5;
            float2 _Combine_845cb1e758f7a887bcd17694c573d134_RG_6;
            Unity_Combine_float(_Split_8cbf1573be6f5782bb185dfcee24d55b_B_3, _Split_8cbf1573be6f5782bb185dfcee24d55b_A_4, 0, 0, _Combine_845cb1e758f7a887bcd17694c573d134_RGBA_4, _Combine_845cb1e758f7a887bcd17694c573d134_RGB_5, _Combine_845cb1e758f7a887bcd17694c573d134_RG_6);
            float4 _Combine_ccad6919c709958e92daa536f3084a22_RGBA_4;
            float3 _Combine_ccad6919c709958e92daa536f3084a22_RGB_5;
            float2 _Combine_ccad6919c709958e92daa536f3084a22_RG_6;
            Unity_Combine_float(_Split_8cbf1573be6f5782bb185dfcee24d55b_R_1, _Split_8cbf1573be6f5782bb185dfcee24d55b_G_2, 0, 0, _Combine_ccad6919c709958e92daa536f3084a22_RGBA_4, _Combine_ccad6919c709958e92daa536f3084a22_RGB_5, _Combine_ccad6919c709958e92daa536f3084a22_RG_6);
            float4 _UV_1ef8497bd7d05986b5a077de31e42520_Out_0 = IN.uv0;
            float2 _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2;
            Unity_Multiply_float2_float2(_Combine_ccad6919c709958e92daa536f3084a22_RG_6, (_UV_1ef8497bd7d05986b5a077de31e42520_Out_0.xy), _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2);
            float2 _Add_7ad38bda511b1d848b8cdff1293db07a_Out_2;
            Unity_Add_float2(_Combine_845cb1e758f7a887bcd17694c573d134_RG_6, _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2, _Add_7ad38bda511b1d848b8cdff1293db07a_Out_2);
            float4 _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0 = SAMPLE_TEXTURE2D(_Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.tex, _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.samplerstate, _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.GetTransformedUV(_Add_7ad38bda511b1d848b8cdff1293db07a_Out_2));
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_R_4 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.r;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_G_5 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.g;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_B_6 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.b;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_A_7 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.a;
            float _Property_e9478b395fb69180a8f07c76b1fc22fe_Out_0 = _BumpScale;
            float3 _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2;
            Unity_NormalStrength_float((_SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.xyz), _Property_e9478b395fb69180a8f07c76b1fc22fe_Out_0, _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2);
            float3 _Multiply_322ba3610e564883ae2580757b20ec1d_Out_2;
            Unity_Multiply_float3_float3((_Property_d1852c92a8c24381991d0dcd5b0251b8_Out_0.xxx), _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2, _Multiply_322ba3610e564883ae2580757b20ec1d_Out_2);
            float3 _Multiply_0152dc430ebb2f82af99ea127ec0d961_Out_2;
            Unity_Multiply_float3_float3((_ScreenPosition_937829c9b1a6d685baa02d92309ac38b_Out_0.xyz), _Multiply_322ba3610e564883ae2580757b20ec1d_Out_2, _Multiply_0152dc430ebb2f82af99ea127ec0d961_Out_2);
            float3 _Add_a4a1d3e0c22bfe89afcd8394f4b37438_Out_2;
            Unity_Add_float3((_ScreenPosition_937829c9b1a6d685baa02d92309ac38b_Out_0.xyz), _Multiply_0152dc430ebb2f82af99ea127ec0d961_Out_2, _Add_a4a1d3e0c22bfe89afcd8394f4b37438_Out_2);
            float3 _SceneColor_de8ab00fb48a9482a2d559a42aaa7b99_Out_1;
            Unity_SceneColor_float((float4(_Add_a4a1d3e0c22bfe89afcd8394f4b37438_Out_2, 1.0)), _SceneColor_de8ab00fb48a9482a2d559a42aaa7b99_Out_1);
            float3 _Multiply_7e33b9e9f3462a89b728fca60c3d7230_Out_2;
            Unity_Multiply_float3_float3((_Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0.xyz), _SceneColor_de8ab00fb48a9482a2d559a42aaa7b99_Out_1, _Multiply_7e33b9e9f3462a89b728fca60c3d7230_Out_2);
            float3 _Multiply_ae385e35933640d38dbea9a7e4323c73_Out_2;
            Unity_Multiply_float3_float3(_Multiply_7e33b9e9f3462a89b728fca60c3d7230_Out_2, (IN.VertexColor.xyz), _Multiply_ae385e35933640d38dbea9a7e4323c73_Out_2);
            float _Property_1bc7e44055ca9e8b925f554ba676938c_Out_0 = _Metallic;
            float _Property_5d73071a393be2899f208ef0116c03c1_Out_0 = _Smoothness;
            float _Property_80e22c9adc7d2a8a8cfeb4f37b4ab8ad_Out_0 = _AO;
            float _Split_8b14302553ddc3879748aad6158293a9_R_1 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[0];
            float _Split_8b14302553ddc3879748aad6158293a9_G_2 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[1];
            float _Split_8b14302553ddc3879748aad6158293a9_B_3 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[2];
            float _Split_8b14302553ddc3879748aad6158293a9_A_4 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[3];
            UnityTexture2D _Property_e96052cb5280048fb7045df6517dadfe_Out_0 = UnityBuildTexture2DStructNoScale(_ParticleTexture);
            float4 _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0 = _ParticleMaskTilingOffset;
            float _Split_09316789d5be448aba41ce9a8a79e989_R_1 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[0];
            float _Split_09316789d5be448aba41ce9a8a79e989_G_2 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[1];
            float _Split_09316789d5be448aba41ce9a8a79e989_B_3 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[2];
            float _Split_09316789d5be448aba41ce9a8a79e989_A_4 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[3];
            float4 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4;
            float3 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5;
            float2 _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_B_3, _Split_09316789d5be448aba41ce9a8a79e989_A_4, 0, 0, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5, _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6);
            float4 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4;
            float3 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5;
            float2 _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_R_1, _Split_09316789d5be448aba41ce9a8a79e989_G_2, 0, 0, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5, _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6);
            float4 _UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0 = IN.uv0;
            float2 _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2;
            Unity_Multiply_float2_float2(_Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6, (_UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0.xy), _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2);
            float2 _Add_9ac539502ae37687b2e506d5849f543a_Out_2;
            Unity_Add_float2(_Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6, _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2, _Add_9ac539502ae37687b2e506d5849f543a_Out_2);
            float4 _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e96052cb5280048fb7045df6517dadfe_Out_0.tex, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.samplerstate, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.GetTransformedUV(_Add_9ac539502ae37687b2e506d5849f543a_Out_2));
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_R_4 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.r;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_G_5 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.g;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_B_6 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.b;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.a;
            float _Multiply_da551cc95d964f8086754940028076ab_Out_2;
            Unity_Multiply_float_float(_Split_8b14302553ddc3879748aad6158293a9_A_4, _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7, _Multiply_da551cc95d964f8086754940028076ab_Out_2);
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_R_1 = IN.VertexColor[0];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_G_2 = IN.VertexColor[1];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_B_3 = IN.VertexColor[2];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4 = IN.VertexColor[3];
            float _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            Unity_Multiply_float_float(_Multiply_da551cc95d964f8086754940028076ab_Out_2, _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4, _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2);
            float _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0 = _Cutoff;
            surface.BaseColor = _Multiply_ae385e35933640d38dbea9a7e4323c73_Out_2;
            surface.NormalTS = _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2;
            surface.Emission = float3(0, 0, 0);
            surface.Metallic = _Property_1bc7e44055ca9e8b925f554ba676938c_Out_0;
            surface.Smoothness = _Property_5d73071a393be2899f208ef0116c03c1_Out_0;
            surface.Occlusion = _Property_80e22c9adc7d2a8a8cfeb4f37b4ab8ad_Out_0;
            surface.Alpha = _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            surface.AlphaClipThreshold = _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.VertexColor = input.color;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "GBuffer"
            Tags
            {
                "LightMode" = "UniversalGBuffer"
            }
        
        // Render State
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DYNAMICLIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
        #pragma multi_compile_fragment _ _SHADOWS_SOFT
        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
        #pragma multi_compile _ SHADOWS_SHADOWMASK
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
        #pragma multi_compile_fragment _ _LIGHT_LAYERS
        #pragma multi_compile_fragment _ _RENDER_PASS_ENABLED
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
        #define VARYINGS_NEED_SHADOW_COORD
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_GBUFFER
        #define _FOG_FRAGMENT 1
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _ALPHATEST_ON 1
        #define REQUIRE_OPAQUE_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
             float4 color;
             float3 viewDirectionWS;
            #if defined(LIGHTMAP_ON)
             float2 staticLightmapUV;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
             float2 dynamicLightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
             float3 sh;
            #endif
             float4 fogFactorAndVertexLight;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
             float4 shadowCoord;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 TangentSpaceNormal;
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float4 interp3 : INTERP3;
             float4 interp4 : INTERP4;
             float3 interp5 : INTERP5;
             float2 interp6 : INTERP6;
             float2 interp7 : INTERP7;
             float3 interp8 : INTERP8;
             float4 interp9 : INTERP9;
             float4 interp10 : INTERP10;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.tangentWS;
            output.interp3.xyzw =  input.texCoord0;
            output.interp4.xyzw =  input.color;
            output.interp5.xyz =  input.viewDirectionWS;
            #if defined(LIGHTMAP_ON)
            output.interp6.xy =  input.staticLightmapUV;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
            output.interp7.xy =  input.dynamicLightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.interp8.xyz =  input.sh;
            #endif
            output.interp9.xyzw =  input.fogFactorAndVertexLight;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.interp10.xyzw =  input.shadowCoord;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.tangentWS = input.interp2.xyzw;
            output.texCoord0 = input.interp3.xyzw;
            output.color = input.interp4.xyzw;
            output.viewDirectionWS = input.interp5.xyz;
            #if defined(LIGHTMAP_ON)
            output.staticLightmapUV = input.interp6.xy;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
            output.dynamicLightmapUV = input.interp7.xy;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.sh = input.interp8.xyz;
            #endif
            output.fogFactorAndVertexLight = input.interp9.xyzw;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.shadowCoord = input.interp10.xyzw;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Cutoff;
        float4 _Color;
        float4 _ParticleTexture_TexelSize;
        float4 _ParticleMaskTilingOffset;
        float4 _BumpMap_TexelSize;
        float _BumpScale;
        float4 _NormalTilingOffset;
        float _Distortion;
        float _Metallic;
        float _AO;
        float _Smoothness;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_ParticleTexture);
        SAMPLER(sampler_ParticleTexture);
        TEXTURE2D(_BumpMap);
        SAMPLER(sampler_BumpMap);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        void Unity_SceneColor_float(float4 UV, out float3 Out)
        {
            Out = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV.xy);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float3 NormalTS;
            float3 Emission;
            float Metallic;
            float Smoothness;
            float Occlusion;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0 = _Color;
            float4 _ScreenPosition_937829c9b1a6d685baa02d92309ac38b_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float _Property_d1852c92a8c24381991d0dcd5b0251b8_Out_0 = _Distortion;
            UnityTexture2D _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0 = UnityBuildTexture2DStructNoScale(_BumpMap);
            float4 _Property_269571a8a5154482b825042058b5c3b3_Out_0 = _NormalTilingOffset;
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_R_1 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[0];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_G_2 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[1];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_B_3 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[2];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_A_4 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[3];
            float4 _Combine_845cb1e758f7a887bcd17694c573d134_RGBA_4;
            float3 _Combine_845cb1e758f7a887bcd17694c573d134_RGB_5;
            float2 _Combine_845cb1e758f7a887bcd17694c573d134_RG_6;
            Unity_Combine_float(_Split_8cbf1573be6f5782bb185dfcee24d55b_B_3, _Split_8cbf1573be6f5782bb185dfcee24d55b_A_4, 0, 0, _Combine_845cb1e758f7a887bcd17694c573d134_RGBA_4, _Combine_845cb1e758f7a887bcd17694c573d134_RGB_5, _Combine_845cb1e758f7a887bcd17694c573d134_RG_6);
            float4 _Combine_ccad6919c709958e92daa536f3084a22_RGBA_4;
            float3 _Combine_ccad6919c709958e92daa536f3084a22_RGB_5;
            float2 _Combine_ccad6919c709958e92daa536f3084a22_RG_6;
            Unity_Combine_float(_Split_8cbf1573be6f5782bb185dfcee24d55b_R_1, _Split_8cbf1573be6f5782bb185dfcee24d55b_G_2, 0, 0, _Combine_ccad6919c709958e92daa536f3084a22_RGBA_4, _Combine_ccad6919c709958e92daa536f3084a22_RGB_5, _Combine_ccad6919c709958e92daa536f3084a22_RG_6);
            float4 _UV_1ef8497bd7d05986b5a077de31e42520_Out_0 = IN.uv0;
            float2 _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2;
            Unity_Multiply_float2_float2(_Combine_ccad6919c709958e92daa536f3084a22_RG_6, (_UV_1ef8497bd7d05986b5a077de31e42520_Out_0.xy), _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2);
            float2 _Add_7ad38bda511b1d848b8cdff1293db07a_Out_2;
            Unity_Add_float2(_Combine_845cb1e758f7a887bcd17694c573d134_RG_6, _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2, _Add_7ad38bda511b1d848b8cdff1293db07a_Out_2);
            float4 _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0 = SAMPLE_TEXTURE2D(_Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.tex, _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.samplerstate, _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.GetTransformedUV(_Add_7ad38bda511b1d848b8cdff1293db07a_Out_2));
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_R_4 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.r;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_G_5 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.g;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_B_6 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.b;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_A_7 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.a;
            float _Property_e9478b395fb69180a8f07c76b1fc22fe_Out_0 = _BumpScale;
            float3 _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2;
            Unity_NormalStrength_float((_SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.xyz), _Property_e9478b395fb69180a8f07c76b1fc22fe_Out_0, _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2);
            float3 _Multiply_322ba3610e564883ae2580757b20ec1d_Out_2;
            Unity_Multiply_float3_float3((_Property_d1852c92a8c24381991d0dcd5b0251b8_Out_0.xxx), _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2, _Multiply_322ba3610e564883ae2580757b20ec1d_Out_2);
            float3 _Multiply_0152dc430ebb2f82af99ea127ec0d961_Out_2;
            Unity_Multiply_float3_float3((_ScreenPosition_937829c9b1a6d685baa02d92309ac38b_Out_0.xyz), _Multiply_322ba3610e564883ae2580757b20ec1d_Out_2, _Multiply_0152dc430ebb2f82af99ea127ec0d961_Out_2);
            float3 _Add_a4a1d3e0c22bfe89afcd8394f4b37438_Out_2;
            Unity_Add_float3((_ScreenPosition_937829c9b1a6d685baa02d92309ac38b_Out_0.xyz), _Multiply_0152dc430ebb2f82af99ea127ec0d961_Out_2, _Add_a4a1d3e0c22bfe89afcd8394f4b37438_Out_2);
            float3 _SceneColor_de8ab00fb48a9482a2d559a42aaa7b99_Out_1;
            Unity_SceneColor_float((float4(_Add_a4a1d3e0c22bfe89afcd8394f4b37438_Out_2, 1.0)), _SceneColor_de8ab00fb48a9482a2d559a42aaa7b99_Out_1);
            float3 _Multiply_7e33b9e9f3462a89b728fca60c3d7230_Out_2;
            Unity_Multiply_float3_float3((_Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0.xyz), _SceneColor_de8ab00fb48a9482a2d559a42aaa7b99_Out_1, _Multiply_7e33b9e9f3462a89b728fca60c3d7230_Out_2);
            float3 _Multiply_ae385e35933640d38dbea9a7e4323c73_Out_2;
            Unity_Multiply_float3_float3(_Multiply_7e33b9e9f3462a89b728fca60c3d7230_Out_2, (IN.VertexColor.xyz), _Multiply_ae385e35933640d38dbea9a7e4323c73_Out_2);
            float _Property_1bc7e44055ca9e8b925f554ba676938c_Out_0 = _Metallic;
            float _Property_5d73071a393be2899f208ef0116c03c1_Out_0 = _Smoothness;
            float _Property_80e22c9adc7d2a8a8cfeb4f37b4ab8ad_Out_0 = _AO;
            float _Split_8b14302553ddc3879748aad6158293a9_R_1 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[0];
            float _Split_8b14302553ddc3879748aad6158293a9_G_2 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[1];
            float _Split_8b14302553ddc3879748aad6158293a9_B_3 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[2];
            float _Split_8b14302553ddc3879748aad6158293a9_A_4 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[3];
            UnityTexture2D _Property_e96052cb5280048fb7045df6517dadfe_Out_0 = UnityBuildTexture2DStructNoScale(_ParticleTexture);
            float4 _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0 = _ParticleMaskTilingOffset;
            float _Split_09316789d5be448aba41ce9a8a79e989_R_1 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[0];
            float _Split_09316789d5be448aba41ce9a8a79e989_G_2 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[1];
            float _Split_09316789d5be448aba41ce9a8a79e989_B_3 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[2];
            float _Split_09316789d5be448aba41ce9a8a79e989_A_4 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[3];
            float4 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4;
            float3 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5;
            float2 _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_B_3, _Split_09316789d5be448aba41ce9a8a79e989_A_4, 0, 0, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5, _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6);
            float4 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4;
            float3 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5;
            float2 _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_R_1, _Split_09316789d5be448aba41ce9a8a79e989_G_2, 0, 0, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5, _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6);
            float4 _UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0 = IN.uv0;
            float2 _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2;
            Unity_Multiply_float2_float2(_Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6, (_UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0.xy), _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2);
            float2 _Add_9ac539502ae37687b2e506d5849f543a_Out_2;
            Unity_Add_float2(_Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6, _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2, _Add_9ac539502ae37687b2e506d5849f543a_Out_2);
            float4 _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e96052cb5280048fb7045df6517dadfe_Out_0.tex, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.samplerstate, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.GetTransformedUV(_Add_9ac539502ae37687b2e506d5849f543a_Out_2));
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_R_4 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.r;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_G_5 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.g;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_B_6 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.b;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.a;
            float _Multiply_da551cc95d964f8086754940028076ab_Out_2;
            Unity_Multiply_float_float(_Split_8b14302553ddc3879748aad6158293a9_A_4, _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7, _Multiply_da551cc95d964f8086754940028076ab_Out_2);
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_R_1 = IN.VertexColor[0];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_G_2 = IN.VertexColor[1];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_B_3 = IN.VertexColor[2];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4 = IN.VertexColor[3];
            float _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            Unity_Multiply_float_float(_Multiply_da551cc95d964f8086754940028076ab_Out_2, _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4, _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2);
            float _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0 = _Cutoff;
            surface.BaseColor = _Multiply_ae385e35933640d38dbea9a7e4323c73_Out_2;
            surface.NormalTS = _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2;
            surface.Emission = float3(0, 0, 0);
            surface.Metallic = _Property_1bc7e44055ca9e8b925f554ba676938c_Out_0;
            surface.Smoothness = _Property_5d73071a393be2899f208ef0116c03c1_Out_0;
            surface.Occlusion = _Property_80e22c9adc7d2a8a8cfeb4f37b4ab8ad_Out_0;
            surface.Alpha = _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            surface.AlphaClipThreshold = _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.VertexColor = input.color;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRGBufferPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        ColorMask 0
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SHADOWCASTER
        #define _ALPHATEST_ON 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.normalWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.normalWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.color = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Cutoff;
        float4 _Color;
        float4 _ParticleTexture_TexelSize;
        float4 _ParticleMaskTilingOffset;
        float4 _BumpMap_TexelSize;
        float _BumpScale;
        float4 _NormalTilingOffset;
        float _Distortion;
        float _Metallic;
        float _AO;
        float _Smoothness;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_ParticleTexture);
        SAMPLER(sampler_ParticleTexture);
        TEXTURE2D(_BumpMap);
        SAMPLER(sampler_BumpMap);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0 = _Color;
            float _Split_8b14302553ddc3879748aad6158293a9_R_1 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[0];
            float _Split_8b14302553ddc3879748aad6158293a9_G_2 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[1];
            float _Split_8b14302553ddc3879748aad6158293a9_B_3 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[2];
            float _Split_8b14302553ddc3879748aad6158293a9_A_4 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[3];
            UnityTexture2D _Property_e96052cb5280048fb7045df6517dadfe_Out_0 = UnityBuildTexture2DStructNoScale(_ParticleTexture);
            float4 _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0 = _ParticleMaskTilingOffset;
            float _Split_09316789d5be448aba41ce9a8a79e989_R_1 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[0];
            float _Split_09316789d5be448aba41ce9a8a79e989_G_2 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[1];
            float _Split_09316789d5be448aba41ce9a8a79e989_B_3 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[2];
            float _Split_09316789d5be448aba41ce9a8a79e989_A_4 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[3];
            float4 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4;
            float3 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5;
            float2 _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_B_3, _Split_09316789d5be448aba41ce9a8a79e989_A_4, 0, 0, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5, _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6);
            float4 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4;
            float3 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5;
            float2 _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_R_1, _Split_09316789d5be448aba41ce9a8a79e989_G_2, 0, 0, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5, _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6);
            float4 _UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0 = IN.uv0;
            float2 _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2;
            Unity_Multiply_float2_float2(_Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6, (_UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0.xy), _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2);
            float2 _Add_9ac539502ae37687b2e506d5849f543a_Out_2;
            Unity_Add_float2(_Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6, _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2, _Add_9ac539502ae37687b2e506d5849f543a_Out_2);
            float4 _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e96052cb5280048fb7045df6517dadfe_Out_0.tex, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.samplerstate, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.GetTransformedUV(_Add_9ac539502ae37687b2e506d5849f543a_Out_2));
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_R_4 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.r;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_G_5 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.g;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_B_6 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.b;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.a;
            float _Multiply_da551cc95d964f8086754940028076ab_Out_2;
            Unity_Multiply_float_float(_Split_8b14302553ddc3879748aad6158293a9_A_4, _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7, _Multiply_da551cc95d964f8086754940028076ab_Out_2);
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_R_1 = IN.VertexColor[0];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_G_2 = IN.VertexColor[1];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_B_3 = IN.VertexColor[2];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4 = IN.VertexColor[3];
            float _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            Unity_Multiply_float_float(_Multiply_da551cc95d964f8086754940028076ab_Out_2, _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4, _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2);
            float _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0 = _Cutoff;
            surface.Alpha = _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            surface.AlphaClipThreshold = _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.uv0 = input.texCoord0;
            output.VertexColor = input.color;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormals"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHNORMALS
        #define _ALPHATEST_ON 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 TangentSpaceNormal;
             float4 uv0;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float4 interp3 : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.normalWS;
            output.interp1.xyzw =  input.tangentWS;
            output.interp2.xyzw =  input.texCoord0;
            output.interp3.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.normalWS = input.interp0.xyz;
            output.tangentWS = input.interp1.xyzw;
            output.texCoord0 = input.interp2.xyzw;
            output.color = input.interp3.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Cutoff;
        float4 _Color;
        float4 _ParticleTexture_TexelSize;
        float4 _ParticleMaskTilingOffset;
        float4 _BumpMap_TexelSize;
        float _BumpScale;
        float4 _NormalTilingOffset;
        float _Distortion;
        float _Metallic;
        float _AO;
        float _Smoothness;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_ParticleTexture);
        SAMPLER(sampler_ParticleTexture);
        TEXTURE2D(_BumpMap);
        SAMPLER(sampler_BumpMap);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 NormalTS;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0 = UnityBuildTexture2DStructNoScale(_BumpMap);
            float4 _Property_269571a8a5154482b825042058b5c3b3_Out_0 = _NormalTilingOffset;
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_R_1 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[0];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_G_2 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[1];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_B_3 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[2];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_A_4 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[3];
            float4 _Combine_845cb1e758f7a887bcd17694c573d134_RGBA_4;
            float3 _Combine_845cb1e758f7a887bcd17694c573d134_RGB_5;
            float2 _Combine_845cb1e758f7a887bcd17694c573d134_RG_6;
            Unity_Combine_float(_Split_8cbf1573be6f5782bb185dfcee24d55b_B_3, _Split_8cbf1573be6f5782bb185dfcee24d55b_A_4, 0, 0, _Combine_845cb1e758f7a887bcd17694c573d134_RGBA_4, _Combine_845cb1e758f7a887bcd17694c573d134_RGB_5, _Combine_845cb1e758f7a887bcd17694c573d134_RG_6);
            float4 _Combine_ccad6919c709958e92daa536f3084a22_RGBA_4;
            float3 _Combine_ccad6919c709958e92daa536f3084a22_RGB_5;
            float2 _Combine_ccad6919c709958e92daa536f3084a22_RG_6;
            Unity_Combine_float(_Split_8cbf1573be6f5782bb185dfcee24d55b_R_1, _Split_8cbf1573be6f5782bb185dfcee24d55b_G_2, 0, 0, _Combine_ccad6919c709958e92daa536f3084a22_RGBA_4, _Combine_ccad6919c709958e92daa536f3084a22_RGB_5, _Combine_ccad6919c709958e92daa536f3084a22_RG_6);
            float4 _UV_1ef8497bd7d05986b5a077de31e42520_Out_0 = IN.uv0;
            float2 _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2;
            Unity_Multiply_float2_float2(_Combine_ccad6919c709958e92daa536f3084a22_RG_6, (_UV_1ef8497bd7d05986b5a077de31e42520_Out_0.xy), _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2);
            float2 _Add_7ad38bda511b1d848b8cdff1293db07a_Out_2;
            Unity_Add_float2(_Combine_845cb1e758f7a887bcd17694c573d134_RG_6, _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2, _Add_7ad38bda511b1d848b8cdff1293db07a_Out_2);
            float4 _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0 = SAMPLE_TEXTURE2D(_Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.tex, _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.samplerstate, _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.GetTransformedUV(_Add_7ad38bda511b1d848b8cdff1293db07a_Out_2));
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_R_4 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.r;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_G_5 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.g;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_B_6 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.b;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_A_7 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.a;
            float _Property_e9478b395fb69180a8f07c76b1fc22fe_Out_0 = _BumpScale;
            float3 _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2;
            Unity_NormalStrength_float((_SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.xyz), _Property_e9478b395fb69180a8f07c76b1fc22fe_Out_0, _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2);
            float4 _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0 = _Color;
            float _Split_8b14302553ddc3879748aad6158293a9_R_1 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[0];
            float _Split_8b14302553ddc3879748aad6158293a9_G_2 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[1];
            float _Split_8b14302553ddc3879748aad6158293a9_B_3 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[2];
            float _Split_8b14302553ddc3879748aad6158293a9_A_4 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[3];
            UnityTexture2D _Property_e96052cb5280048fb7045df6517dadfe_Out_0 = UnityBuildTexture2DStructNoScale(_ParticleTexture);
            float4 _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0 = _ParticleMaskTilingOffset;
            float _Split_09316789d5be448aba41ce9a8a79e989_R_1 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[0];
            float _Split_09316789d5be448aba41ce9a8a79e989_G_2 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[1];
            float _Split_09316789d5be448aba41ce9a8a79e989_B_3 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[2];
            float _Split_09316789d5be448aba41ce9a8a79e989_A_4 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[3];
            float4 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4;
            float3 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5;
            float2 _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_B_3, _Split_09316789d5be448aba41ce9a8a79e989_A_4, 0, 0, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5, _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6);
            float4 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4;
            float3 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5;
            float2 _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_R_1, _Split_09316789d5be448aba41ce9a8a79e989_G_2, 0, 0, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5, _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6);
            float4 _UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0 = IN.uv0;
            float2 _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2;
            Unity_Multiply_float2_float2(_Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6, (_UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0.xy), _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2);
            float2 _Add_9ac539502ae37687b2e506d5849f543a_Out_2;
            Unity_Add_float2(_Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6, _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2, _Add_9ac539502ae37687b2e506d5849f543a_Out_2);
            float4 _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e96052cb5280048fb7045df6517dadfe_Out_0.tex, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.samplerstate, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.GetTransformedUV(_Add_9ac539502ae37687b2e506d5849f543a_Out_2));
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_R_4 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.r;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_G_5 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.g;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_B_6 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.b;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.a;
            float _Multiply_da551cc95d964f8086754940028076ab_Out_2;
            Unity_Multiply_float_float(_Split_8b14302553ddc3879748aad6158293a9_A_4, _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7, _Multiply_da551cc95d964f8086754940028076ab_Out_2);
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_R_1 = IN.VertexColor[0];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_G_2 = IN.VertexColor[1];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_B_3 = IN.VertexColor[2];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4 = IN.VertexColor[3];
            float _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            Unity_Multiply_float_float(_Multiply_da551cc95d964f8086754940028076ab_Out_2, _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4, _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2);
            float _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0 = _Cutoff;
            surface.NormalTS = _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2;
            surface.Alpha = _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            surface.AlphaClipThreshold = _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
            output.uv0 = input.texCoord0;
            output.VertexColor = input.color;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "Meta"
            Tags
            {
                "LightMode" = "Meta"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        #pragma shader_feature _ EDITOR_VISUALIZATION
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD1
        #define VARYINGS_NEED_TEXCOORD2
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_META
        #define _FOG_FRAGMENT 1
        #define _ALPHATEST_ON 1
        #define REQUIRE_OPAQUE_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 texCoord1;
             float4 texCoord2;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float4 interp3 : INTERP3;
             float4 interp4 : INTERP4;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.texCoord1;
            output.interp3.xyzw =  input.texCoord2;
            output.interp4.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.texCoord1 = input.interp2.xyzw;
            output.texCoord2 = input.interp3.xyzw;
            output.color = input.interp4.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Cutoff;
        float4 _Color;
        float4 _ParticleTexture_TexelSize;
        float4 _ParticleMaskTilingOffset;
        float4 _BumpMap_TexelSize;
        float _BumpScale;
        float4 _NormalTilingOffset;
        float _Distortion;
        float _Metallic;
        float _AO;
        float _Smoothness;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_ParticleTexture);
        SAMPLER(sampler_ParticleTexture);
        TEXTURE2D(_BumpMap);
        SAMPLER(sampler_BumpMap);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        void Unity_SceneColor_float(float4 UV, out float3 Out)
        {
            Out = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV.xy);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0 = _Color;
            float4 _ScreenPosition_937829c9b1a6d685baa02d92309ac38b_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float _Property_d1852c92a8c24381991d0dcd5b0251b8_Out_0 = _Distortion;
            UnityTexture2D _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0 = UnityBuildTexture2DStructNoScale(_BumpMap);
            float4 _Property_269571a8a5154482b825042058b5c3b3_Out_0 = _NormalTilingOffset;
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_R_1 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[0];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_G_2 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[1];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_B_3 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[2];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_A_4 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[3];
            float4 _Combine_845cb1e758f7a887bcd17694c573d134_RGBA_4;
            float3 _Combine_845cb1e758f7a887bcd17694c573d134_RGB_5;
            float2 _Combine_845cb1e758f7a887bcd17694c573d134_RG_6;
            Unity_Combine_float(_Split_8cbf1573be6f5782bb185dfcee24d55b_B_3, _Split_8cbf1573be6f5782bb185dfcee24d55b_A_4, 0, 0, _Combine_845cb1e758f7a887bcd17694c573d134_RGBA_4, _Combine_845cb1e758f7a887bcd17694c573d134_RGB_5, _Combine_845cb1e758f7a887bcd17694c573d134_RG_6);
            float4 _Combine_ccad6919c709958e92daa536f3084a22_RGBA_4;
            float3 _Combine_ccad6919c709958e92daa536f3084a22_RGB_5;
            float2 _Combine_ccad6919c709958e92daa536f3084a22_RG_6;
            Unity_Combine_float(_Split_8cbf1573be6f5782bb185dfcee24d55b_R_1, _Split_8cbf1573be6f5782bb185dfcee24d55b_G_2, 0, 0, _Combine_ccad6919c709958e92daa536f3084a22_RGBA_4, _Combine_ccad6919c709958e92daa536f3084a22_RGB_5, _Combine_ccad6919c709958e92daa536f3084a22_RG_6);
            float4 _UV_1ef8497bd7d05986b5a077de31e42520_Out_0 = IN.uv0;
            float2 _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2;
            Unity_Multiply_float2_float2(_Combine_ccad6919c709958e92daa536f3084a22_RG_6, (_UV_1ef8497bd7d05986b5a077de31e42520_Out_0.xy), _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2);
            float2 _Add_7ad38bda511b1d848b8cdff1293db07a_Out_2;
            Unity_Add_float2(_Combine_845cb1e758f7a887bcd17694c573d134_RG_6, _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2, _Add_7ad38bda511b1d848b8cdff1293db07a_Out_2);
            float4 _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0 = SAMPLE_TEXTURE2D(_Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.tex, _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.samplerstate, _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.GetTransformedUV(_Add_7ad38bda511b1d848b8cdff1293db07a_Out_2));
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_R_4 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.r;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_G_5 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.g;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_B_6 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.b;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_A_7 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.a;
            float _Property_e9478b395fb69180a8f07c76b1fc22fe_Out_0 = _BumpScale;
            float3 _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2;
            Unity_NormalStrength_float((_SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.xyz), _Property_e9478b395fb69180a8f07c76b1fc22fe_Out_0, _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2);
            float3 _Multiply_322ba3610e564883ae2580757b20ec1d_Out_2;
            Unity_Multiply_float3_float3((_Property_d1852c92a8c24381991d0dcd5b0251b8_Out_0.xxx), _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2, _Multiply_322ba3610e564883ae2580757b20ec1d_Out_2);
            float3 _Multiply_0152dc430ebb2f82af99ea127ec0d961_Out_2;
            Unity_Multiply_float3_float3((_ScreenPosition_937829c9b1a6d685baa02d92309ac38b_Out_0.xyz), _Multiply_322ba3610e564883ae2580757b20ec1d_Out_2, _Multiply_0152dc430ebb2f82af99ea127ec0d961_Out_2);
            float3 _Add_a4a1d3e0c22bfe89afcd8394f4b37438_Out_2;
            Unity_Add_float3((_ScreenPosition_937829c9b1a6d685baa02d92309ac38b_Out_0.xyz), _Multiply_0152dc430ebb2f82af99ea127ec0d961_Out_2, _Add_a4a1d3e0c22bfe89afcd8394f4b37438_Out_2);
            float3 _SceneColor_de8ab00fb48a9482a2d559a42aaa7b99_Out_1;
            Unity_SceneColor_float((float4(_Add_a4a1d3e0c22bfe89afcd8394f4b37438_Out_2, 1.0)), _SceneColor_de8ab00fb48a9482a2d559a42aaa7b99_Out_1);
            float3 _Multiply_7e33b9e9f3462a89b728fca60c3d7230_Out_2;
            Unity_Multiply_float3_float3((_Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0.xyz), _SceneColor_de8ab00fb48a9482a2d559a42aaa7b99_Out_1, _Multiply_7e33b9e9f3462a89b728fca60c3d7230_Out_2);
            float3 _Multiply_ae385e35933640d38dbea9a7e4323c73_Out_2;
            Unity_Multiply_float3_float3(_Multiply_7e33b9e9f3462a89b728fca60c3d7230_Out_2, (IN.VertexColor.xyz), _Multiply_ae385e35933640d38dbea9a7e4323c73_Out_2);
            float _Split_8b14302553ddc3879748aad6158293a9_R_1 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[0];
            float _Split_8b14302553ddc3879748aad6158293a9_G_2 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[1];
            float _Split_8b14302553ddc3879748aad6158293a9_B_3 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[2];
            float _Split_8b14302553ddc3879748aad6158293a9_A_4 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[3];
            UnityTexture2D _Property_e96052cb5280048fb7045df6517dadfe_Out_0 = UnityBuildTexture2DStructNoScale(_ParticleTexture);
            float4 _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0 = _ParticleMaskTilingOffset;
            float _Split_09316789d5be448aba41ce9a8a79e989_R_1 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[0];
            float _Split_09316789d5be448aba41ce9a8a79e989_G_2 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[1];
            float _Split_09316789d5be448aba41ce9a8a79e989_B_3 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[2];
            float _Split_09316789d5be448aba41ce9a8a79e989_A_4 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[3];
            float4 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4;
            float3 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5;
            float2 _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_B_3, _Split_09316789d5be448aba41ce9a8a79e989_A_4, 0, 0, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5, _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6);
            float4 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4;
            float3 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5;
            float2 _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_R_1, _Split_09316789d5be448aba41ce9a8a79e989_G_2, 0, 0, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5, _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6);
            float4 _UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0 = IN.uv0;
            float2 _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2;
            Unity_Multiply_float2_float2(_Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6, (_UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0.xy), _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2);
            float2 _Add_9ac539502ae37687b2e506d5849f543a_Out_2;
            Unity_Add_float2(_Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6, _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2, _Add_9ac539502ae37687b2e506d5849f543a_Out_2);
            float4 _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e96052cb5280048fb7045df6517dadfe_Out_0.tex, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.samplerstate, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.GetTransformedUV(_Add_9ac539502ae37687b2e506d5849f543a_Out_2));
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_R_4 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.r;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_G_5 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.g;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_B_6 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.b;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.a;
            float _Multiply_da551cc95d964f8086754940028076ab_Out_2;
            Unity_Multiply_float_float(_Split_8b14302553ddc3879748aad6158293a9_A_4, _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7, _Multiply_da551cc95d964f8086754940028076ab_Out_2);
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_R_1 = IN.VertexColor[0];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_G_2 = IN.VertexColor[1];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_B_3 = IN.VertexColor[2];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4 = IN.VertexColor[3];
            float _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            Unity_Multiply_float_float(_Multiply_da551cc95d964f8086754940028076ab_Out_2, _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4, _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2);
            float _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0 = _Cutoff;
            surface.BaseColor = _Multiply_ae385e35933640d38dbea9a7e4323c73_Out_2;
            surface.Emission = float3(0, 0, 0);
            surface.Alpha = _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            surface.AlphaClipThreshold = _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.VertexColor = input.color;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        #define _ALPHATEST_ON 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 interp0 : INTERP0;
             float4 interp1 : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            output.interp1.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            output.color = input.interp1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Cutoff;
        float4 _Color;
        float4 _ParticleTexture_TexelSize;
        float4 _ParticleMaskTilingOffset;
        float4 _BumpMap_TexelSize;
        float _BumpScale;
        float4 _NormalTilingOffset;
        float _Distortion;
        float _Metallic;
        float _AO;
        float _Smoothness;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_ParticleTexture);
        SAMPLER(sampler_ParticleTexture);
        TEXTURE2D(_BumpMap);
        SAMPLER(sampler_BumpMap);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0 = _Color;
            float _Split_8b14302553ddc3879748aad6158293a9_R_1 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[0];
            float _Split_8b14302553ddc3879748aad6158293a9_G_2 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[1];
            float _Split_8b14302553ddc3879748aad6158293a9_B_3 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[2];
            float _Split_8b14302553ddc3879748aad6158293a9_A_4 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[3];
            UnityTexture2D _Property_e96052cb5280048fb7045df6517dadfe_Out_0 = UnityBuildTexture2DStructNoScale(_ParticleTexture);
            float4 _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0 = _ParticleMaskTilingOffset;
            float _Split_09316789d5be448aba41ce9a8a79e989_R_1 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[0];
            float _Split_09316789d5be448aba41ce9a8a79e989_G_2 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[1];
            float _Split_09316789d5be448aba41ce9a8a79e989_B_3 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[2];
            float _Split_09316789d5be448aba41ce9a8a79e989_A_4 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[3];
            float4 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4;
            float3 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5;
            float2 _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_B_3, _Split_09316789d5be448aba41ce9a8a79e989_A_4, 0, 0, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5, _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6);
            float4 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4;
            float3 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5;
            float2 _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_R_1, _Split_09316789d5be448aba41ce9a8a79e989_G_2, 0, 0, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5, _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6);
            float4 _UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0 = IN.uv0;
            float2 _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2;
            Unity_Multiply_float2_float2(_Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6, (_UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0.xy), _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2);
            float2 _Add_9ac539502ae37687b2e506d5849f543a_Out_2;
            Unity_Add_float2(_Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6, _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2, _Add_9ac539502ae37687b2e506d5849f543a_Out_2);
            float4 _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e96052cb5280048fb7045df6517dadfe_Out_0.tex, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.samplerstate, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.GetTransformedUV(_Add_9ac539502ae37687b2e506d5849f543a_Out_2));
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_R_4 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.r;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_G_5 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.g;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_B_6 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.b;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.a;
            float _Multiply_da551cc95d964f8086754940028076ab_Out_2;
            Unity_Multiply_float_float(_Split_8b14302553ddc3879748aad6158293a9_A_4, _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7, _Multiply_da551cc95d964f8086754940028076ab_Out_2);
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_R_1 = IN.VertexColor[0];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_G_2 = IN.VertexColor[1];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_B_3 = IN.VertexColor[2];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4 = IN.VertexColor[3];
            float _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            Unity_Multiply_float_float(_Multiply_da551cc95d964f8086754940028076ab_Out_2, _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4, _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2);
            float _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0 = _Cutoff;
            surface.Alpha = _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            surface.AlphaClipThreshold = _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.uv0 = input.texCoord0;
            output.VertexColor = input.color;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
        // Render State
        Cull Back
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        #define _ALPHATEST_ON 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 interp0 : INTERP0;
             float4 interp1 : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            output.interp1.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            output.color = input.interp1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Cutoff;
        float4 _Color;
        float4 _ParticleTexture_TexelSize;
        float4 _ParticleMaskTilingOffset;
        float4 _BumpMap_TexelSize;
        float _BumpScale;
        float4 _NormalTilingOffset;
        float _Distortion;
        float _Metallic;
        float _AO;
        float _Smoothness;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_ParticleTexture);
        SAMPLER(sampler_ParticleTexture);
        TEXTURE2D(_BumpMap);
        SAMPLER(sampler_BumpMap);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0 = _Color;
            float _Split_8b14302553ddc3879748aad6158293a9_R_1 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[0];
            float _Split_8b14302553ddc3879748aad6158293a9_G_2 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[1];
            float _Split_8b14302553ddc3879748aad6158293a9_B_3 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[2];
            float _Split_8b14302553ddc3879748aad6158293a9_A_4 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[3];
            UnityTexture2D _Property_e96052cb5280048fb7045df6517dadfe_Out_0 = UnityBuildTexture2DStructNoScale(_ParticleTexture);
            float4 _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0 = _ParticleMaskTilingOffset;
            float _Split_09316789d5be448aba41ce9a8a79e989_R_1 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[0];
            float _Split_09316789d5be448aba41ce9a8a79e989_G_2 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[1];
            float _Split_09316789d5be448aba41ce9a8a79e989_B_3 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[2];
            float _Split_09316789d5be448aba41ce9a8a79e989_A_4 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[3];
            float4 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4;
            float3 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5;
            float2 _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_B_3, _Split_09316789d5be448aba41ce9a8a79e989_A_4, 0, 0, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5, _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6);
            float4 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4;
            float3 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5;
            float2 _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_R_1, _Split_09316789d5be448aba41ce9a8a79e989_G_2, 0, 0, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5, _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6);
            float4 _UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0 = IN.uv0;
            float2 _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2;
            Unity_Multiply_float2_float2(_Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6, (_UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0.xy), _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2);
            float2 _Add_9ac539502ae37687b2e506d5849f543a_Out_2;
            Unity_Add_float2(_Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6, _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2, _Add_9ac539502ae37687b2e506d5849f543a_Out_2);
            float4 _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e96052cb5280048fb7045df6517dadfe_Out_0.tex, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.samplerstate, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.GetTransformedUV(_Add_9ac539502ae37687b2e506d5849f543a_Out_2));
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_R_4 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.r;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_G_5 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.g;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_B_6 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.b;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.a;
            float _Multiply_da551cc95d964f8086754940028076ab_Out_2;
            Unity_Multiply_float_float(_Split_8b14302553ddc3879748aad6158293a9_A_4, _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7, _Multiply_da551cc95d964f8086754940028076ab_Out_2);
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_R_1 = IN.VertexColor[0];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_G_2 = IN.VertexColor[1];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_B_3 = IN.VertexColor[2];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4 = IN.VertexColor[3];
            float _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            Unity_Multiply_float_float(_Multiply_da551cc95d964f8086754940028076ab_Out_2, _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4, _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2);
            float _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0 = _Cutoff;
            surface.Alpha = _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            surface.AlphaClipThreshold = _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.uv0 = input.texCoord0;
            output.VertexColor = input.color;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            // Name: <None>
            Tags
            {
                "LightMode" = "Universal2D"
            }
        
        // Render State
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_2D
        #define _ALPHATEST_ON 1
        #define REQUIRE_OPAQUE_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.color = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Cutoff;
        float4 _Color;
        float4 _ParticleTexture_TexelSize;
        float4 _ParticleMaskTilingOffset;
        float4 _BumpMap_TexelSize;
        float _BumpScale;
        float4 _NormalTilingOffset;
        float _Distortion;
        float _Metallic;
        float _AO;
        float _Smoothness;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_ParticleTexture);
        SAMPLER(sampler_ParticleTexture);
        TEXTURE2D(_BumpMap);
        SAMPLER(sampler_BumpMap);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        void Unity_SceneColor_float(float4 UV, out float3 Out)
        {
            Out = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV.xy);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0 = _Color;
            float4 _ScreenPosition_937829c9b1a6d685baa02d92309ac38b_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float _Property_d1852c92a8c24381991d0dcd5b0251b8_Out_0 = _Distortion;
            UnityTexture2D _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0 = UnityBuildTexture2DStructNoScale(_BumpMap);
            float4 _Property_269571a8a5154482b825042058b5c3b3_Out_0 = _NormalTilingOffset;
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_R_1 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[0];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_G_2 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[1];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_B_3 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[2];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_A_4 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[3];
            float4 _Combine_845cb1e758f7a887bcd17694c573d134_RGBA_4;
            float3 _Combine_845cb1e758f7a887bcd17694c573d134_RGB_5;
            float2 _Combine_845cb1e758f7a887bcd17694c573d134_RG_6;
            Unity_Combine_float(_Split_8cbf1573be6f5782bb185dfcee24d55b_B_3, _Split_8cbf1573be6f5782bb185dfcee24d55b_A_4, 0, 0, _Combine_845cb1e758f7a887bcd17694c573d134_RGBA_4, _Combine_845cb1e758f7a887bcd17694c573d134_RGB_5, _Combine_845cb1e758f7a887bcd17694c573d134_RG_6);
            float4 _Combine_ccad6919c709958e92daa536f3084a22_RGBA_4;
            float3 _Combine_ccad6919c709958e92daa536f3084a22_RGB_5;
            float2 _Combine_ccad6919c709958e92daa536f3084a22_RG_6;
            Unity_Combine_float(_Split_8cbf1573be6f5782bb185dfcee24d55b_R_1, _Split_8cbf1573be6f5782bb185dfcee24d55b_G_2, 0, 0, _Combine_ccad6919c709958e92daa536f3084a22_RGBA_4, _Combine_ccad6919c709958e92daa536f3084a22_RGB_5, _Combine_ccad6919c709958e92daa536f3084a22_RG_6);
            float4 _UV_1ef8497bd7d05986b5a077de31e42520_Out_0 = IN.uv0;
            float2 _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2;
            Unity_Multiply_float2_float2(_Combine_ccad6919c709958e92daa536f3084a22_RG_6, (_UV_1ef8497bd7d05986b5a077de31e42520_Out_0.xy), _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2);
            float2 _Add_7ad38bda511b1d848b8cdff1293db07a_Out_2;
            Unity_Add_float2(_Combine_845cb1e758f7a887bcd17694c573d134_RG_6, _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2, _Add_7ad38bda511b1d848b8cdff1293db07a_Out_2);
            float4 _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0 = SAMPLE_TEXTURE2D(_Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.tex, _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.samplerstate, _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.GetTransformedUV(_Add_7ad38bda511b1d848b8cdff1293db07a_Out_2));
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_R_4 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.r;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_G_5 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.g;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_B_6 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.b;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_A_7 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.a;
            float _Property_e9478b395fb69180a8f07c76b1fc22fe_Out_0 = _BumpScale;
            float3 _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2;
            Unity_NormalStrength_float((_SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.xyz), _Property_e9478b395fb69180a8f07c76b1fc22fe_Out_0, _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2);
            float3 _Multiply_322ba3610e564883ae2580757b20ec1d_Out_2;
            Unity_Multiply_float3_float3((_Property_d1852c92a8c24381991d0dcd5b0251b8_Out_0.xxx), _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2, _Multiply_322ba3610e564883ae2580757b20ec1d_Out_2);
            float3 _Multiply_0152dc430ebb2f82af99ea127ec0d961_Out_2;
            Unity_Multiply_float3_float3((_ScreenPosition_937829c9b1a6d685baa02d92309ac38b_Out_0.xyz), _Multiply_322ba3610e564883ae2580757b20ec1d_Out_2, _Multiply_0152dc430ebb2f82af99ea127ec0d961_Out_2);
            float3 _Add_a4a1d3e0c22bfe89afcd8394f4b37438_Out_2;
            Unity_Add_float3((_ScreenPosition_937829c9b1a6d685baa02d92309ac38b_Out_0.xyz), _Multiply_0152dc430ebb2f82af99ea127ec0d961_Out_2, _Add_a4a1d3e0c22bfe89afcd8394f4b37438_Out_2);
            float3 _SceneColor_de8ab00fb48a9482a2d559a42aaa7b99_Out_1;
            Unity_SceneColor_float((float4(_Add_a4a1d3e0c22bfe89afcd8394f4b37438_Out_2, 1.0)), _SceneColor_de8ab00fb48a9482a2d559a42aaa7b99_Out_1);
            float3 _Multiply_7e33b9e9f3462a89b728fca60c3d7230_Out_2;
            Unity_Multiply_float3_float3((_Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0.xyz), _SceneColor_de8ab00fb48a9482a2d559a42aaa7b99_Out_1, _Multiply_7e33b9e9f3462a89b728fca60c3d7230_Out_2);
            float3 _Multiply_ae385e35933640d38dbea9a7e4323c73_Out_2;
            Unity_Multiply_float3_float3(_Multiply_7e33b9e9f3462a89b728fca60c3d7230_Out_2, (IN.VertexColor.xyz), _Multiply_ae385e35933640d38dbea9a7e4323c73_Out_2);
            float _Split_8b14302553ddc3879748aad6158293a9_R_1 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[0];
            float _Split_8b14302553ddc3879748aad6158293a9_G_2 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[1];
            float _Split_8b14302553ddc3879748aad6158293a9_B_3 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[2];
            float _Split_8b14302553ddc3879748aad6158293a9_A_4 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[3];
            UnityTexture2D _Property_e96052cb5280048fb7045df6517dadfe_Out_0 = UnityBuildTexture2DStructNoScale(_ParticleTexture);
            float4 _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0 = _ParticleMaskTilingOffset;
            float _Split_09316789d5be448aba41ce9a8a79e989_R_1 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[0];
            float _Split_09316789d5be448aba41ce9a8a79e989_G_2 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[1];
            float _Split_09316789d5be448aba41ce9a8a79e989_B_3 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[2];
            float _Split_09316789d5be448aba41ce9a8a79e989_A_4 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[3];
            float4 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4;
            float3 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5;
            float2 _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_B_3, _Split_09316789d5be448aba41ce9a8a79e989_A_4, 0, 0, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5, _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6);
            float4 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4;
            float3 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5;
            float2 _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_R_1, _Split_09316789d5be448aba41ce9a8a79e989_G_2, 0, 0, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5, _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6);
            float4 _UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0 = IN.uv0;
            float2 _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2;
            Unity_Multiply_float2_float2(_Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6, (_UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0.xy), _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2);
            float2 _Add_9ac539502ae37687b2e506d5849f543a_Out_2;
            Unity_Add_float2(_Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6, _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2, _Add_9ac539502ae37687b2e506d5849f543a_Out_2);
            float4 _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e96052cb5280048fb7045df6517dadfe_Out_0.tex, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.samplerstate, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.GetTransformedUV(_Add_9ac539502ae37687b2e506d5849f543a_Out_2));
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_R_4 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.r;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_G_5 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.g;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_B_6 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.b;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.a;
            float _Multiply_da551cc95d964f8086754940028076ab_Out_2;
            Unity_Multiply_float_float(_Split_8b14302553ddc3879748aad6158293a9_A_4, _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7, _Multiply_da551cc95d964f8086754940028076ab_Out_2);
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_R_1 = IN.VertexColor[0];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_G_2 = IN.VertexColor[1];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_B_3 = IN.VertexColor[2];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4 = IN.VertexColor[3];
            float _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            Unity_Multiply_float_float(_Multiply_da551cc95d964f8086754940028076ab_Out_2, _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4, _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2);
            float _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0 = _Cutoff;
            surface.BaseColor = _Multiply_ae385e35933640d38dbea9a7e4323c73_Out_2;
            surface.Alpha = _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            surface.AlphaClipThreshold = _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.VertexColor = input.color;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Lit"
            "Queue"="Transparent"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalLitSubTarget"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
        // Render State
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DYNAMICLIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
        #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
        #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
        #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
        #pragma multi_compile_fragment _ _SHADOWS_SOFT
        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ SHADOWS_SHADOWMASK
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ _LIGHT_LAYERS
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        #pragma multi_compile_fragment _ _LIGHT_COOKIES
        #pragma multi_compile _ _CLUSTERED_RENDERING
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
        #define VARYINGS_NEED_SHADOW_COORD
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_FORWARD
        #define _FOG_FRAGMENT 1
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _ALPHATEST_ON 1
        #define REQUIRE_OPAQUE_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
             float4 color;
             float3 viewDirectionWS;
            #if defined(LIGHTMAP_ON)
             float2 staticLightmapUV;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
             float2 dynamicLightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
             float3 sh;
            #endif
             float4 fogFactorAndVertexLight;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
             float4 shadowCoord;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 TangentSpaceNormal;
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float4 interp3 : INTERP3;
             float4 interp4 : INTERP4;
             float3 interp5 : INTERP5;
             float2 interp6 : INTERP6;
             float2 interp7 : INTERP7;
             float3 interp8 : INTERP8;
             float4 interp9 : INTERP9;
             float4 interp10 : INTERP10;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyzw =  input.tangentWS;
            output.interp3.xyzw =  input.texCoord0;
            output.interp4.xyzw =  input.color;
            output.interp5.xyz =  input.viewDirectionWS;
            #if defined(LIGHTMAP_ON)
            output.interp6.xy =  input.staticLightmapUV;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
            output.interp7.xy =  input.dynamicLightmapUV;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.interp8.xyz =  input.sh;
            #endif
            output.interp9.xyzw =  input.fogFactorAndVertexLight;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.interp10.xyzw =  input.shadowCoord;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.tangentWS = input.interp2.xyzw;
            output.texCoord0 = input.interp3.xyzw;
            output.color = input.interp4.xyzw;
            output.viewDirectionWS = input.interp5.xyz;
            #if defined(LIGHTMAP_ON)
            output.staticLightmapUV = input.interp6.xy;
            #endif
            #if defined(DYNAMICLIGHTMAP_ON)
            output.dynamicLightmapUV = input.interp7.xy;
            #endif
            #if !defined(LIGHTMAP_ON)
            output.sh = input.interp8.xyz;
            #endif
            output.fogFactorAndVertexLight = input.interp9.xyzw;
            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
            output.shadowCoord = input.interp10.xyzw;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Cutoff;
        float4 _Color;
        float4 _ParticleTexture_TexelSize;
        float4 _ParticleMaskTilingOffset;
        float4 _BumpMap_TexelSize;
        float _BumpScale;
        float4 _NormalTilingOffset;
        float _Distortion;
        float _Metallic;
        float _AO;
        float _Smoothness;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_ParticleTexture);
        SAMPLER(sampler_ParticleTexture);
        TEXTURE2D(_BumpMap);
        SAMPLER(sampler_BumpMap);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        void Unity_SceneColor_float(float4 UV, out float3 Out)
        {
            Out = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV.xy);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float3 NormalTS;
            float3 Emission;
            float Metallic;
            float Smoothness;
            float Occlusion;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0 = _Color;
            float4 _ScreenPosition_937829c9b1a6d685baa02d92309ac38b_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float _Property_d1852c92a8c24381991d0dcd5b0251b8_Out_0 = _Distortion;
            UnityTexture2D _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0 = UnityBuildTexture2DStructNoScale(_BumpMap);
            float4 _Property_269571a8a5154482b825042058b5c3b3_Out_0 = _NormalTilingOffset;
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_R_1 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[0];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_G_2 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[1];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_B_3 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[2];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_A_4 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[3];
            float4 _Combine_845cb1e758f7a887bcd17694c573d134_RGBA_4;
            float3 _Combine_845cb1e758f7a887bcd17694c573d134_RGB_5;
            float2 _Combine_845cb1e758f7a887bcd17694c573d134_RG_6;
            Unity_Combine_float(_Split_8cbf1573be6f5782bb185dfcee24d55b_B_3, _Split_8cbf1573be6f5782bb185dfcee24d55b_A_4, 0, 0, _Combine_845cb1e758f7a887bcd17694c573d134_RGBA_4, _Combine_845cb1e758f7a887bcd17694c573d134_RGB_5, _Combine_845cb1e758f7a887bcd17694c573d134_RG_6);
            float4 _Combine_ccad6919c709958e92daa536f3084a22_RGBA_4;
            float3 _Combine_ccad6919c709958e92daa536f3084a22_RGB_5;
            float2 _Combine_ccad6919c709958e92daa536f3084a22_RG_6;
            Unity_Combine_float(_Split_8cbf1573be6f5782bb185dfcee24d55b_R_1, _Split_8cbf1573be6f5782bb185dfcee24d55b_G_2, 0, 0, _Combine_ccad6919c709958e92daa536f3084a22_RGBA_4, _Combine_ccad6919c709958e92daa536f3084a22_RGB_5, _Combine_ccad6919c709958e92daa536f3084a22_RG_6);
            float4 _UV_1ef8497bd7d05986b5a077de31e42520_Out_0 = IN.uv0;
            float2 _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2;
            Unity_Multiply_float2_float2(_Combine_ccad6919c709958e92daa536f3084a22_RG_6, (_UV_1ef8497bd7d05986b5a077de31e42520_Out_0.xy), _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2);
            float2 _Add_7ad38bda511b1d848b8cdff1293db07a_Out_2;
            Unity_Add_float2(_Combine_845cb1e758f7a887bcd17694c573d134_RG_6, _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2, _Add_7ad38bda511b1d848b8cdff1293db07a_Out_2);
            float4 _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0 = SAMPLE_TEXTURE2D(_Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.tex, _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.samplerstate, _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.GetTransformedUV(_Add_7ad38bda511b1d848b8cdff1293db07a_Out_2));
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_R_4 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.r;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_G_5 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.g;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_B_6 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.b;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_A_7 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.a;
            float _Property_e9478b395fb69180a8f07c76b1fc22fe_Out_0 = _BumpScale;
            float3 _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2;
            Unity_NormalStrength_float((_SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.xyz), _Property_e9478b395fb69180a8f07c76b1fc22fe_Out_0, _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2);
            float3 _Multiply_322ba3610e564883ae2580757b20ec1d_Out_2;
            Unity_Multiply_float3_float3((_Property_d1852c92a8c24381991d0dcd5b0251b8_Out_0.xxx), _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2, _Multiply_322ba3610e564883ae2580757b20ec1d_Out_2);
            float3 _Multiply_0152dc430ebb2f82af99ea127ec0d961_Out_2;
            Unity_Multiply_float3_float3((_ScreenPosition_937829c9b1a6d685baa02d92309ac38b_Out_0.xyz), _Multiply_322ba3610e564883ae2580757b20ec1d_Out_2, _Multiply_0152dc430ebb2f82af99ea127ec0d961_Out_2);
            float3 _Add_a4a1d3e0c22bfe89afcd8394f4b37438_Out_2;
            Unity_Add_float3((_ScreenPosition_937829c9b1a6d685baa02d92309ac38b_Out_0.xyz), _Multiply_0152dc430ebb2f82af99ea127ec0d961_Out_2, _Add_a4a1d3e0c22bfe89afcd8394f4b37438_Out_2);
            float3 _SceneColor_de8ab00fb48a9482a2d559a42aaa7b99_Out_1;
            Unity_SceneColor_float((float4(_Add_a4a1d3e0c22bfe89afcd8394f4b37438_Out_2, 1.0)), _SceneColor_de8ab00fb48a9482a2d559a42aaa7b99_Out_1);
            float3 _Multiply_7e33b9e9f3462a89b728fca60c3d7230_Out_2;
            Unity_Multiply_float3_float3((_Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0.xyz), _SceneColor_de8ab00fb48a9482a2d559a42aaa7b99_Out_1, _Multiply_7e33b9e9f3462a89b728fca60c3d7230_Out_2);
            float3 _Multiply_ae385e35933640d38dbea9a7e4323c73_Out_2;
            Unity_Multiply_float3_float3(_Multiply_7e33b9e9f3462a89b728fca60c3d7230_Out_2, (IN.VertexColor.xyz), _Multiply_ae385e35933640d38dbea9a7e4323c73_Out_2);
            float _Property_1bc7e44055ca9e8b925f554ba676938c_Out_0 = _Metallic;
            float _Property_5d73071a393be2899f208ef0116c03c1_Out_0 = _Smoothness;
            float _Property_80e22c9adc7d2a8a8cfeb4f37b4ab8ad_Out_0 = _AO;
            float _Split_8b14302553ddc3879748aad6158293a9_R_1 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[0];
            float _Split_8b14302553ddc3879748aad6158293a9_G_2 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[1];
            float _Split_8b14302553ddc3879748aad6158293a9_B_3 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[2];
            float _Split_8b14302553ddc3879748aad6158293a9_A_4 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[3];
            UnityTexture2D _Property_e96052cb5280048fb7045df6517dadfe_Out_0 = UnityBuildTexture2DStructNoScale(_ParticleTexture);
            float4 _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0 = _ParticleMaskTilingOffset;
            float _Split_09316789d5be448aba41ce9a8a79e989_R_1 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[0];
            float _Split_09316789d5be448aba41ce9a8a79e989_G_2 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[1];
            float _Split_09316789d5be448aba41ce9a8a79e989_B_3 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[2];
            float _Split_09316789d5be448aba41ce9a8a79e989_A_4 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[3];
            float4 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4;
            float3 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5;
            float2 _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_B_3, _Split_09316789d5be448aba41ce9a8a79e989_A_4, 0, 0, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5, _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6);
            float4 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4;
            float3 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5;
            float2 _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_R_1, _Split_09316789d5be448aba41ce9a8a79e989_G_2, 0, 0, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5, _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6);
            float4 _UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0 = IN.uv0;
            float2 _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2;
            Unity_Multiply_float2_float2(_Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6, (_UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0.xy), _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2);
            float2 _Add_9ac539502ae37687b2e506d5849f543a_Out_2;
            Unity_Add_float2(_Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6, _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2, _Add_9ac539502ae37687b2e506d5849f543a_Out_2);
            float4 _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e96052cb5280048fb7045df6517dadfe_Out_0.tex, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.samplerstate, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.GetTransformedUV(_Add_9ac539502ae37687b2e506d5849f543a_Out_2));
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_R_4 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.r;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_G_5 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.g;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_B_6 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.b;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.a;
            float _Multiply_da551cc95d964f8086754940028076ab_Out_2;
            Unity_Multiply_float_float(_Split_8b14302553ddc3879748aad6158293a9_A_4, _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7, _Multiply_da551cc95d964f8086754940028076ab_Out_2);
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_R_1 = IN.VertexColor[0];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_G_2 = IN.VertexColor[1];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_B_3 = IN.VertexColor[2];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4 = IN.VertexColor[3];
            float _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            Unity_Multiply_float_float(_Multiply_da551cc95d964f8086754940028076ab_Out_2, _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4, _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2);
            float _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0 = _Cutoff;
            surface.BaseColor = _Multiply_ae385e35933640d38dbea9a7e4323c73_Out_2;
            surface.NormalTS = _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2;
            surface.Emission = float3(0, 0, 0);
            surface.Metallic = _Property_1bc7e44055ca9e8b925f554ba676938c_Out_0;
            surface.Smoothness = _Property_5d73071a393be2899f208ef0116c03c1_Out_0;
            surface.Occlusion = _Property_80e22c9adc7d2a8a8cfeb4f37b4ab8ad_Out_0;
            surface.Alpha = _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            surface.AlphaClipThreshold = _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.VertexColor = input.color;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        ColorMask 0
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SHADOWCASTER
        #define _ALPHATEST_ON 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.normalWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.normalWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.color = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Cutoff;
        float4 _Color;
        float4 _ParticleTexture_TexelSize;
        float4 _ParticleMaskTilingOffset;
        float4 _BumpMap_TexelSize;
        float _BumpScale;
        float4 _NormalTilingOffset;
        float _Distortion;
        float _Metallic;
        float _AO;
        float _Smoothness;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_ParticleTexture);
        SAMPLER(sampler_ParticleTexture);
        TEXTURE2D(_BumpMap);
        SAMPLER(sampler_BumpMap);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0 = _Color;
            float _Split_8b14302553ddc3879748aad6158293a9_R_1 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[0];
            float _Split_8b14302553ddc3879748aad6158293a9_G_2 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[1];
            float _Split_8b14302553ddc3879748aad6158293a9_B_3 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[2];
            float _Split_8b14302553ddc3879748aad6158293a9_A_4 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[3];
            UnityTexture2D _Property_e96052cb5280048fb7045df6517dadfe_Out_0 = UnityBuildTexture2DStructNoScale(_ParticleTexture);
            float4 _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0 = _ParticleMaskTilingOffset;
            float _Split_09316789d5be448aba41ce9a8a79e989_R_1 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[0];
            float _Split_09316789d5be448aba41ce9a8a79e989_G_2 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[1];
            float _Split_09316789d5be448aba41ce9a8a79e989_B_3 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[2];
            float _Split_09316789d5be448aba41ce9a8a79e989_A_4 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[3];
            float4 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4;
            float3 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5;
            float2 _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_B_3, _Split_09316789d5be448aba41ce9a8a79e989_A_4, 0, 0, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5, _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6);
            float4 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4;
            float3 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5;
            float2 _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_R_1, _Split_09316789d5be448aba41ce9a8a79e989_G_2, 0, 0, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5, _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6);
            float4 _UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0 = IN.uv0;
            float2 _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2;
            Unity_Multiply_float2_float2(_Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6, (_UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0.xy), _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2);
            float2 _Add_9ac539502ae37687b2e506d5849f543a_Out_2;
            Unity_Add_float2(_Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6, _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2, _Add_9ac539502ae37687b2e506d5849f543a_Out_2);
            float4 _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e96052cb5280048fb7045df6517dadfe_Out_0.tex, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.samplerstate, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.GetTransformedUV(_Add_9ac539502ae37687b2e506d5849f543a_Out_2));
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_R_4 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.r;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_G_5 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.g;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_B_6 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.b;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.a;
            float _Multiply_da551cc95d964f8086754940028076ab_Out_2;
            Unity_Multiply_float_float(_Split_8b14302553ddc3879748aad6158293a9_A_4, _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7, _Multiply_da551cc95d964f8086754940028076ab_Out_2);
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_R_1 = IN.VertexColor[0];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_G_2 = IN.VertexColor[1];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_B_3 = IN.VertexColor[2];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4 = IN.VertexColor[3];
            float _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            Unity_Multiply_float_float(_Multiply_da551cc95d964f8086754940028076ab_Out_2, _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4, _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2);
            float _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0 = _Cutoff;
            surface.Alpha = _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            surface.AlphaClipThreshold = _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.uv0 = input.texCoord0;
            output.VertexColor = input.color;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormals"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHNORMALS
        #define _ALPHATEST_ON 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
             float4 tangentWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 TangentSpaceNormal;
             float4 uv0;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float4 interp3 : INTERP3;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.normalWS;
            output.interp1.xyzw =  input.tangentWS;
            output.interp2.xyzw =  input.texCoord0;
            output.interp3.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.normalWS = input.interp0.xyz;
            output.tangentWS = input.interp1.xyzw;
            output.texCoord0 = input.interp2.xyzw;
            output.color = input.interp3.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Cutoff;
        float4 _Color;
        float4 _ParticleTexture_TexelSize;
        float4 _ParticleMaskTilingOffset;
        float4 _BumpMap_TexelSize;
        float _BumpScale;
        float4 _NormalTilingOffset;
        float _Distortion;
        float _Metallic;
        float _AO;
        float _Smoothness;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_ParticleTexture);
        SAMPLER(sampler_ParticleTexture);
        TEXTURE2D(_BumpMap);
        SAMPLER(sampler_BumpMap);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 NormalTS;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0 = UnityBuildTexture2DStructNoScale(_BumpMap);
            float4 _Property_269571a8a5154482b825042058b5c3b3_Out_0 = _NormalTilingOffset;
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_R_1 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[0];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_G_2 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[1];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_B_3 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[2];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_A_4 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[3];
            float4 _Combine_845cb1e758f7a887bcd17694c573d134_RGBA_4;
            float3 _Combine_845cb1e758f7a887bcd17694c573d134_RGB_5;
            float2 _Combine_845cb1e758f7a887bcd17694c573d134_RG_6;
            Unity_Combine_float(_Split_8cbf1573be6f5782bb185dfcee24d55b_B_3, _Split_8cbf1573be6f5782bb185dfcee24d55b_A_4, 0, 0, _Combine_845cb1e758f7a887bcd17694c573d134_RGBA_4, _Combine_845cb1e758f7a887bcd17694c573d134_RGB_5, _Combine_845cb1e758f7a887bcd17694c573d134_RG_6);
            float4 _Combine_ccad6919c709958e92daa536f3084a22_RGBA_4;
            float3 _Combine_ccad6919c709958e92daa536f3084a22_RGB_5;
            float2 _Combine_ccad6919c709958e92daa536f3084a22_RG_6;
            Unity_Combine_float(_Split_8cbf1573be6f5782bb185dfcee24d55b_R_1, _Split_8cbf1573be6f5782bb185dfcee24d55b_G_2, 0, 0, _Combine_ccad6919c709958e92daa536f3084a22_RGBA_4, _Combine_ccad6919c709958e92daa536f3084a22_RGB_5, _Combine_ccad6919c709958e92daa536f3084a22_RG_6);
            float4 _UV_1ef8497bd7d05986b5a077de31e42520_Out_0 = IN.uv0;
            float2 _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2;
            Unity_Multiply_float2_float2(_Combine_ccad6919c709958e92daa536f3084a22_RG_6, (_UV_1ef8497bd7d05986b5a077de31e42520_Out_0.xy), _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2);
            float2 _Add_7ad38bda511b1d848b8cdff1293db07a_Out_2;
            Unity_Add_float2(_Combine_845cb1e758f7a887bcd17694c573d134_RG_6, _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2, _Add_7ad38bda511b1d848b8cdff1293db07a_Out_2);
            float4 _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0 = SAMPLE_TEXTURE2D(_Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.tex, _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.samplerstate, _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.GetTransformedUV(_Add_7ad38bda511b1d848b8cdff1293db07a_Out_2));
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_R_4 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.r;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_G_5 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.g;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_B_6 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.b;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_A_7 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.a;
            float _Property_e9478b395fb69180a8f07c76b1fc22fe_Out_0 = _BumpScale;
            float3 _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2;
            Unity_NormalStrength_float((_SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.xyz), _Property_e9478b395fb69180a8f07c76b1fc22fe_Out_0, _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2);
            float4 _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0 = _Color;
            float _Split_8b14302553ddc3879748aad6158293a9_R_1 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[0];
            float _Split_8b14302553ddc3879748aad6158293a9_G_2 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[1];
            float _Split_8b14302553ddc3879748aad6158293a9_B_3 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[2];
            float _Split_8b14302553ddc3879748aad6158293a9_A_4 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[3];
            UnityTexture2D _Property_e96052cb5280048fb7045df6517dadfe_Out_0 = UnityBuildTexture2DStructNoScale(_ParticleTexture);
            float4 _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0 = _ParticleMaskTilingOffset;
            float _Split_09316789d5be448aba41ce9a8a79e989_R_1 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[0];
            float _Split_09316789d5be448aba41ce9a8a79e989_G_2 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[1];
            float _Split_09316789d5be448aba41ce9a8a79e989_B_3 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[2];
            float _Split_09316789d5be448aba41ce9a8a79e989_A_4 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[3];
            float4 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4;
            float3 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5;
            float2 _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_B_3, _Split_09316789d5be448aba41ce9a8a79e989_A_4, 0, 0, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5, _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6);
            float4 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4;
            float3 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5;
            float2 _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_R_1, _Split_09316789d5be448aba41ce9a8a79e989_G_2, 0, 0, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5, _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6);
            float4 _UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0 = IN.uv0;
            float2 _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2;
            Unity_Multiply_float2_float2(_Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6, (_UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0.xy), _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2);
            float2 _Add_9ac539502ae37687b2e506d5849f543a_Out_2;
            Unity_Add_float2(_Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6, _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2, _Add_9ac539502ae37687b2e506d5849f543a_Out_2);
            float4 _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e96052cb5280048fb7045df6517dadfe_Out_0.tex, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.samplerstate, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.GetTransformedUV(_Add_9ac539502ae37687b2e506d5849f543a_Out_2));
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_R_4 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.r;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_G_5 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.g;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_B_6 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.b;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.a;
            float _Multiply_da551cc95d964f8086754940028076ab_Out_2;
            Unity_Multiply_float_float(_Split_8b14302553ddc3879748aad6158293a9_A_4, _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7, _Multiply_da551cc95d964f8086754940028076ab_Out_2);
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_R_1 = IN.VertexColor[0];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_G_2 = IN.VertexColor[1];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_B_3 = IN.VertexColor[2];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4 = IN.VertexColor[3];
            float _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            Unity_Multiply_float_float(_Multiply_da551cc95d964f8086754940028076ab_Out_2, _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4, _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2);
            float _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0 = _Cutoff;
            surface.NormalTS = _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2;
            surface.Alpha = _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            surface.AlphaClipThreshold = _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
            output.TangentSpaceNormal = float3(0.0f, 0.0f, 1.0f);
        
        
            output.uv0 = input.texCoord0;
            output.VertexColor = input.color;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "Meta"
            Tags
            {
                "LightMode" = "Meta"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        #pragma shader_feature _ EDITOR_VISUALIZATION
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define ATTRIBUTES_NEED_TEXCOORD2
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD1
        #define VARYINGS_NEED_TEXCOORD2
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_META
        #define _FOG_FRAGMENT 1
        #define _ALPHATEST_ON 1
        #define REQUIRE_OPAQUE_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
             float4 uv2 : TEXCOORD2;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 texCoord1;
             float4 texCoord2;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
             float4 interp3 : INTERP3;
             float4 interp4 : INTERP4;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.texCoord1;
            output.interp3.xyzw =  input.texCoord2;
            output.interp4.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.texCoord1 = input.interp2.xyzw;
            output.texCoord2 = input.interp3.xyzw;
            output.color = input.interp4.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Cutoff;
        float4 _Color;
        float4 _ParticleTexture_TexelSize;
        float4 _ParticleMaskTilingOffset;
        float4 _BumpMap_TexelSize;
        float _BumpScale;
        float4 _NormalTilingOffset;
        float _Distortion;
        float _Metallic;
        float _AO;
        float _Smoothness;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_ParticleTexture);
        SAMPLER(sampler_ParticleTexture);
        TEXTURE2D(_BumpMap);
        SAMPLER(sampler_BumpMap);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        void Unity_SceneColor_float(float4 UV, out float3 Out)
        {
            Out = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV.xy);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float3 Emission;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0 = _Color;
            float4 _ScreenPosition_937829c9b1a6d685baa02d92309ac38b_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float _Property_d1852c92a8c24381991d0dcd5b0251b8_Out_0 = _Distortion;
            UnityTexture2D _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0 = UnityBuildTexture2DStructNoScale(_BumpMap);
            float4 _Property_269571a8a5154482b825042058b5c3b3_Out_0 = _NormalTilingOffset;
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_R_1 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[0];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_G_2 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[1];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_B_3 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[2];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_A_4 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[3];
            float4 _Combine_845cb1e758f7a887bcd17694c573d134_RGBA_4;
            float3 _Combine_845cb1e758f7a887bcd17694c573d134_RGB_5;
            float2 _Combine_845cb1e758f7a887bcd17694c573d134_RG_6;
            Unity_Combine_float(_Split_8cbf1573be6f5782bb185dfcee24d55b_B_3, _Split_8cbf1573be6f5782bb185dfcee24d55b_A_4, 0, 0, _Combine_845cb1e758f7a887bcd17694c573d134_RGBA_4, _Combine_845cb1e758f7a887bcd17694c573d134_RGB_5, _Combine_845cb1e758f7a887bcd17694c573d134_RG_6);
            float4 _Combine_ccad6919c709958e92daa536f3084a22_RGBA_4;
            float3 _Combine_ccad6919c709958e92daa536f3084a22_RGB_5;
            float2 _Combine_ccad6919c709958e92daa536f3084a22_RG_6;
            Unity_Combine_float(_Split_8cbf1573be6f5782bb185dfcee24d55b_R_1, _Split_8cbf1573be6f5782bb185dfcee24d55b_G_2, 0, 0, _Combine_ccad6919c709958e92daa536f3084a22_RGBA_4, _Combine_ccad6919c709958e92daa536f3084a22_RGB_5, _Combine_ccad6919c709958e92daa536f3084a22_RG_6);
            float4 _UV_1ef8497bd7d05986b5a077de31e42520_Out_0 = IN.uv0;
            float2 _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2;
            Unity_Multiply_float2_float2(_Combine_ccad6919c709958e92daa536f3084a22_RG_6, (_UV_1ef8497bd7d05986b5a077de31e42520_Out_0.xy), _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2);
            float2 _Add_7ad38bda511b1d848b8cdff1293db07a_Out_2;
            Unity_Add_float2(_Combine_845cb1e758f7a887bcd17694c573d134_RG_6, _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2, _Add_7ad38bda511b1d848b8cdff1293db07a_Out_2);
            float4 _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0 = SAMPLE_TEXTURE2D(_Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.tex, _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.samplerstate, _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.GetTransformedUV(_Add_7ad38bda511b1d848b8cdff1293db07a_Out_2));
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_R_4 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.r;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_G_5 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.g;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_B_6 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.b;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_A_7 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.a;
            float _Property_e9478b395fb69180a8f07c76b1fc22fe_Out_0 = _BumpScale;
            float3 _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2;
            Unity_NormalStrength_float((_SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.xyz), _Property_e9478b395fb69180a8f07c76b1fc22fe_Out_0, _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2);
            float3 _Multiply_322ba3610e564883ae2580757b20ec1d_Out_2;
            Unity_Multiply_float3_float3((_Property_d1852c92a8c24381991d0dcd5b0251b8_Out_0.xxx), _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2, _Multiply_322ba3610e564883ae2580757b20ec1d_Out_2);
            float3 _Multiply_0152dc430ebb2f82af99ea127ec0d961_Out_2;
            Unity_Multiply_float3_float3((_ScreenPosition_937829c9b1a6d685baa02d92309ac38b_Out_0.xyz), _Multiply_322ba3610e564883ae2580757b20ec1d_Out_2, _Multiply_0152dc430ebb2f82af99ea127ec0d961_Out_2);
            float3 _Add_a4a1d3e0c22bfe89afcd8394f4b37438_Out_2;
            Unity_Add_float3((_ScreenPosition_937829c9b1a6d685baa02d92309ac38b_Out_0.xyz), _Multiply_0152dc430ebb2f82af99ea127ec0d961_Out_2, _Add_a4a1d3e0c22bfe89afcd8394f4b37438_Out_2);
            float3 _SceneColor_de8ab00fb48a9482a2d559a42aaa7b99_Out_1;
            Unity_SceneColor_float((float4(_Add_a4a1d3e0c22bfe89afcd8394f4b37438_Out_2, 1.0)), _SceneColor_de8ab00fb48a9482a2d559a42aaa7b99_Out_1);
            float3 _Multiply_7e33b9e9f3462a89b728fca60c3d7230_Out_2;
            Unity_Multiply_float3_float3((_Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0.xyz), _SceneColor_de8ab00fb48a9482a2d559a42aaa7b99_Out_1, _Multiply_7e33b9e9f3462a89b728fca60c3d7230_Out_2);
            float3 _Multiply_ae385e35933640d38dbea9a7e4323c73_Out_2;
            Unity_Multiply_float3_float3(_Multiply_7e33b9e9f3462a89b728fca60c3d7230_Out_2, (IN.VertexColor.xyz), _Multiply_ae385e35933640d38dbea9a7e4323c73_Out_2);
            float _Split_8b14302553ddc3879748aad6158293a9_R_1 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[0];
            float _Split_8b14302553ddc3879748aad6158293a9_G_2 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[1];
            float _Split_8b14302553ddc3879748aad6158293a9_B_3 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[2];
            float _Split_8b14302553ddc3879748aad6158293a9_A_4 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[3];
            UnityTexture2D _Property_e96052cb5280048fb7045df6517dadfe_Out_0 = UnityBuildTexture2DStructNoScale(_ParticleTexture);
            float4 _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0 = _ParticleMaskTilingOffset;
            float _Split_09316789d5be448aba41ce9a8a79e989_R_1 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[0];
            float _Split_09316789d5be448aba41ce9a8a79e989_G_2 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[1];
            float _Split_09316789d5be448aba41ce9a8a79e989_B_3 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[2];
            float _Split_09316789d5be448aba41ce9a8a79e989_A_4 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[3];
            float4 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4;
            float3 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5;
            float2 _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_B_3, _Split_09316789d5be448aba41ce9a8a79e989_A_4, 0, 0, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5, _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6);
            float4 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4;
            float3 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5;
            float2 _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_R_1, _Split_09316789d5be448aba41ce9a8a79e989_G_2, 0, 0, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5, _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6);
            float4 _UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0 = IN.uv0;
            float2 _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2;
            Unity_Multiply_float2_float2(_Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6, (_UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0.xy), _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2);
            float2 _Add_9ac539502ae37687b2e506d5849f543a_Out_2;
            Unity_Add_float2(_Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6, _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2, _Add_9ac539502ae37687b2e506d5849f543a_Out_2);
            float4 _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e96052cb5280048fb7045df6517dadfe_Out_0.tex, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.samplerstate, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.GetTransformedUV(_Add_9ac539502ae37687b2e506d5849f543a_Out_2));
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_R_4 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.r;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_G_5 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.g;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_B_6 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.b;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.a;
            float _Multiply_da551cc95d964f8086754940028076ab_Out_2;
            Unity_Multiply_float_float(_Split_8b14302553ddc3879748aad6158293a9_A_4, _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7, _Multiply_da551cc95d964f8086754940028076ab_Out_2);
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_R_1 = IN.VertexColor[0];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_G_2 = IN.VertexColor[1];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_B_3 = IN.VertexColor[2];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4 = IN.VertexColor[3];
            float _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            Unity_Multiply_float_float(_Multiply_da551cc95d964f8086754940028076ab_Out_2, _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4, _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2);
            float _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0 = _Cutoff;
            surface.BaseColor = _Multiply_ae385e35933640d38dbea9a7e4323c73_Out_2;
            surface.Emission = float3(0, 0, 0);
            surface.Alpha = _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            surface.AlphaClipThreshold = _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.VertexColor = input.color;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/LightingMetaPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        #define _ALPHATEST_ON 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 interp0 : INTERP0;
             float4 interp1 : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            output.interp1.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            output.color = input.interp1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Cutoff;
        float4 _Color;
        float4 _ParticleTexture_TexelSize;
        float4 _ParticleMaskTilingOffset;
        float4 _BumpMap_TexelSize;
        float _BumpScale;
        float4 _NormalTilingOffset;
        float _Distortion;
        float _Metallic;
        float _AO;
        float _Smoothness;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_ParticleTexture);
        SAMPLER(sampler_ParticleTexture);
        TEXTURE2D(_BumpMap);
        SAMPLER(sampler_BumpMap);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0 = _Color;
            float _Split_8b14302553ddc3879748aad6158293a9_R_1 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[0];
            float _Split_8b14302553ddc3879748aad6158293a9_G_2 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[1];
            float _Split_8b14302553ddc3879748aad6158293a9_B_3 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[2];
            float _Split_8b14302553ddc3879748aad6158293a9_A_4 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[3];
            UnityTexture2D _Property_e96052cb5280048fb7045df6517dadfe_Out_0 = UnityBuildTexture2DStructNoScale(_ParticleTexture);
            float4 _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0 = _ParticleMaskTilingOffset;
            float _Split_09316789d5be448aba41ce9a8a79e989_R_1 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[0];
            float _Split_09316789d5be448aba41ce9a8a79e989_G_2 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[1];
            float _Split_09316789d5be448aba41ce9a8a79e989_B_3 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[2];
            float _Split_09316789d5be448aba41ce9a8a79e989_A_4 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[3];
            float4 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4;
            float3 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5;
            float2 _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_B_3, _Split_09316789d5be448aba41ce9a8a79e989_A_4, 0, 0, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5, _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6);
            float4 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4;
            float3 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5;
            float2 _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_R_1, _Split_09316789d5be448aba41ce9a8a79e989_G_2, 0, 0, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5, _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6);
            float4 _UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0 = IN.uv0;
            float2 _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2;
            Unity_Multiply_float2_float2(_Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6, (_UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0.xy), _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2);
            float2 _Add_9ac539502ae37687b2e506d5849f543a_Out_2;
            Unity_Add_float2(_Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6, _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2, _Add_9ac539502ae37687b2e506d5849f543a_Out_2);
            float4 _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e96052cb5280048fb7045df6517dadfe_Out_0.tex, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.samplerstate, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.GetTransformedUV(_Add_9ac539502ae37687b2e506d5849f543a_Out_2));
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_R_4 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.r;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_G_5 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.g;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_B_6 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.b;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.a;
            float _Multiply_da551cc95d964f8086754940028076ab_Out_2;
            Unity_Multiply_float_float(_Split_8b14302553ddc3879748aad6158293a9_A_4, _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7, _Multiply_da551cc95d964f8086754940028076ab_Out_2);
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_R_1 = IN.VertexColor[0];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_G_2 = IN.VertexColor[1];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_B_3 = IN.VertexColor[2];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4 = IN.VertexColor[3];
            float _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            Unity_Multiply_float_float(_Multiply_da551cc95d964f8086754940028076ab_Out_2, _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4, _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2);
            float _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0 = _Cutoff;
            surface.Alpha = _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            surface.AlphaClipThreshold = _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.uv0 = input.texCoord0;
            output.VertexColor = input.color;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
        // Render State
        Cull Back
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        #define _ALPHATEST_ON 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 interp0 : INTERP0;
             float4 interp1 : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyzw =  input.texCoord0;
            output.interp1.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.interp0.xyzw;
            output.color = input.interp1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Cutoff;
        float4 _Color;
        float4 _ParticleTexture_TexelSize;
        float4 _ParticleMaskTilingOffset;
        float4 _BumpMap_TexelSize;
        float _BumpScale;
        float4 _NormalTilingOffset;
        float _Distortion;
        float _Metallic;
        float _AO;
        float _Smoothness;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_ParticleTexture);
        SAMPLER(sampler_ParticleTexture);
        TEXTURE2D(_BumpMap);
        SAMPLER(sampler_BumpMap);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0 = _Color;
            float _Split_8b14302553ddc3879748aad6158293a9_R_1 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[0];
            float _Split_8b14302553ddc3879748aad6158293a9_G_2 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[1];
            float _Split_8b14302553ddc3879748aad6158293a9_B_3 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[2];
            float _Split_8b14302553ddc3879748aad6158293a9_A_4 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[3];
            UnityTexture2D _Property_e96052cb5280048fb7045df6517dadfe_Out_0 = UnityBuildTexture2DStructNoScale(_ParticleTexture);
            float4 _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0 = _ParticleMaskTilingOffset;
            float _Split_09316789d5be448aba41ce9a8a79e989_R_1 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[0];
            float _Split_09316789d5be448aba41ce9a8a79e989_G_2 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[1];
            float _Split_09316789d5be448aba41ce9a8a79e989_B_3 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[2];
            float _Split_09316789d5be448aba41ce9a8a79e989_A_4 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[3];
            float4 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4;
            float3 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5;
            float2 _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_B_3, _Split_09316789d5be448aba41ce9a8a79e989_A_4, 0, 0, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5, _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6);
            float4 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4;
            float3 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5;
            float2 _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_R_1, _Split_09316789d5be448aba41ce9a8a79e989_G_2, 0, 0, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5, _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6);
            float4 _UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0 = IN.uv0;
            float2 _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2;
            Unity_Multiply_float2_float2(_Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6, (_UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0.xy), _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2);
            float2 _Add_9ac539502ae37687b2e506d5849f543a_Out_2;
            Unity_Add_float2(_Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6, _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2, _Add_9ac539502ae37687b2e506d5849f543a_Out_2);
            float4 _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e96052cb5280048fb7045df6517dadfe_Out_0.tex, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.samplerstate, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.GetTransformedUV(_Add_9ac539502ae37687b2e506d5849f543a_Out_2));
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_R_4 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.r;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_G_5 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.g;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_B_6 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.b;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.a;
            float _Multiply_da551cc95d964f8086754940028076ab_Out_2;
            Unity_Multiply_float_float(_Split_8b14302553ddc3879748aad6158293a9_A_4, _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7, _Multiply_da551cc95d964f8086754940028076ab_Out_2);
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_R_1 = IN.VertexColor[0];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_G_2 = IN.VertexColor[1];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_B_3 = IN.VertexColor[2];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4 = IN.VertexColor[3];
            float _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            Unity_Multiply_float_float(_Multiply_da551cc95d964f8086754940028076ab_Out_2, _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4, _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2);
            float _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0 = _Cutoff;
            surface.Alpha = _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            surface.AlphaClipThreshold = _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.uv0 = input.texCoord0;
            output.VertexColor = input.color;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            // Name: <None>
            Tags
            {
                "LightMode" = "Universal2D"
            }
        
        // Render State
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define _NORMALMAP 1
        #define _NORMAL_DROPOFF_TS 1
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_2D
        #define _ALPHATEST_ON 1
        #define REQUIRE_OPAQUE_TEXTURE
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 WorldSpacePosition;
             float4 ScreenPosition;
             float4 uv0;
             float4 VertexColor;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.color = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Cutoff;
        float4 _Color;
        float4 _ParticleTexture_TexelSize;
        float4 _ParticleMaskTilingOffset;
        float4 _BumpMap_TexelSize;
        float _BumpScale;
        float4 _NormalTilingOffset;
        float _Distortion;
        float _Metallic;
        float _AO;
        float _Smoothness;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_ParticleTexture);
        SAMPLER(sampler_ParticleTexture);
        TEXTURE2D(_BumpMap);
        SAMPLER(sampler_BumpMap);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_NormalStrength_float(float3 In, float Strength, out float3 Out)
        {
            Out = float3(In.rg * Strength, lerp(1, In.b, saturate(Strength)));
        }
        
        void Unity_Multiply_float3_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float3(float3 A, float3 B, out float3 Out)
        {
            Out = A + B;
        }
        
        void Unity_SceneColor_float(float4 UV, out float3 Out)
        {
            Out = SHADERGRAPH_SAMPLE_SCENE_COLOR(UV.xy);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0 = _Color;
            float4 _ScreenPosition_937829c9b1a6d685baa02d92309ac38b_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float _Property_d1852c92a8c24381991d0dcd5b0251b8_Out_0 = _Distortion;
            UnityTexture2D _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0 = UnityBuildTexture2DStructNoScale(_BumpMap);
            float4 _Property_269571a8a5154482b825042058b5c3b3_Out_0 = _NormalTilingOffset;
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_R_1 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[0];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_G_2 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[1];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_B_3 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[2];
            float _Split_8cbf1573be6f5782bb185dfcee24d55b_A_4 = _Property_269571a8a5154482b825042058b5c3b3_Out_0[3];
            float4 _Combine_845cb1e758f7a887bcd17694c573d134_RGBA_4;
            float3 _Combine_845cb1e758f7a887bcd17694c573d134_RGB_5;
            float2 _Combine_845cb1e758f7a887bcd17694c573d134_RG_6;
            Unity_Combine_float(_Split_8cbf1573be6f5782bb185dfcee24d55b_B_3, _Split_8cbf1573be6f5782bb185dfcee24d55b_A_4, 0, 0, _Combine_845cb1e758f7a887bcd17694c573d134_RGBA_4, _Combine_845cb1e758f7a887bcd17694c573d134_RGB_5, _Combine_845cb1e758f7a887bcd17694c573d134_RG_6);
            float4 _Combine_ccad6919c709958e92daa536f3084a22_RGBA_4;
            float3 _Combine_ccad6919c709958e92daa536f3084a22_RGB_5;
            float2 _Combine_ccad6919c709958e92daa536f3084a22_RG_6;
            Unity_Combine_float(_Split_8cbf1573be6f5782bb185dfcee24d55b_R_1, _Split_8cbf1573be6f5782bb185dfcee24d55b_G_2, 0, 0, _Combine_ccad6919c709958e92daa536f3084a22_RGBA_4, _Combine_ccad6919c709958e92daa536f3084a22_RGB_5, _Combine_ccad6919c709958e92daa536f3084a22_RG_6);
            float4 _UV_1ef8497bd7d05986b5a077de31e42520_Out_0 = IN.uv0;
            float2 _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2;
            Unity_Multiply_float2_float2(_Combine_ccad6919c709958e92daa536f3084a22_RG_6, (_UV_1ef8497bd7d05986b5a077de31e42520_Out_0.xy), _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2);
            float2 _Add_7ad38bda511b1d848b8cdff1293db07a_Out_2;
            Unity_Add_float2(_Combine_845cb1e758f7a887bcd17694c573d134_RG_6, _Multiply_7b7c92dba37a958a955dde2b060c4e7d_Out_2, _Add_7ad38bda511b1d848b8cdff1293db07a_Out_2);
            float4 _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0 = SAMPLE_TEXTURE2D(_Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.tex, _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.samplerstate, _Property_69b7f815b05e9080a80aa1406ac34a33_Out_0.GetTransformedUV(_Add_7ad38bda511b1d848b8cdff1293db07a_Out_2));
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_R_4 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.r;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_G_5 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.g;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_B_6 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.b;
            float _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_A_7 = _SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.a;
            float _Property_e9478b395fb69180a8f07c76b1fc22fe_Out_0 = _BumpScale;
            float3 _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2;
            Unity_NormalStrength_float((_SampleTexture2D_981cf8a05e29f0809d6fb0ec9d5188f4_RGBA_0.xyz), _Property_e9478b395fb69180a8f07c76b1fc22fe_Out_0, _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2);
            float3 _Multiply_322ba3610e564883ae2580757b20ec1d_Out_2;
            Unity_Multiply_float3_float3((_Property_d1852c92a8c24381991d0dcd5b0251b8_Out_0.xxx), _NormalStrength_b32f75d09f123287be599808ec1d904e_Out_2, _Multiply_322ba3610e564883ae2580757b20ec1d_Out_2);
            float3 _Multiply_0152dc430ebb2f82af99ea127ec0d961_Out_2;
            Unity_Multiply_float3_float3((_ScreenPosition_937829c9b1a6d685baa02d92309ac38b_Out_0.xyz), _Multiply_322ba3610e564883ae2580757b20ec1d_Out_2, _Multiply_0152dc430ebb2f82af99ea127ec0d961_Out_2);
            float3 _Add_a4a1d3e0c22bfe89afcd8394f4b37438_Out_2;
            Unity_Add_float3((_ScreenPosition_937829c9b1a6d685baa02d92309ac38b_Out_0.xyz), _Multiply_0152dc430ebb2f82af99ea127ec0d961_Out_2, _Add_a4a1d3e0c22bfe89afcd8394f4b37438_Out_2);
            float3 _SceneColor_de8ab00fb48a9482a2d559a42aaa7b99_Out_1;
            Unity_SceneColor_float((float4(_Add_a4a1d3e0c22bfe89afcd8394f4b37438_Out_2, 1.0)), _SceneColor_de8ab00fb48a9482a2d559a42aaa7b99_Out_1);
            float3 _Multiply_7e33b9e9f3462a89b728fca60c3d7230_Out_2;
            Unity_Multiply_float3_float3((_Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0.xyz), _SceneColor_de8ab00fb48a9482a2d559a42aaa7b99_Out_1, _Multiply_7e33b9e9f3462a89b728fca60c3d7230_Out_2);
            float3 _Multiply_ae385e35933640d38dbea9a7e4323c73_Out_2;
            Unity_Multiply_float3_float3(_Multiply_7e33b9e9f3462a89b728fca60c3d7230_Out_2, (IN.VertexColor.xyz), _Multiply_ae385e35933640d38dbea9a7e4323c73_Out_2);
            float _Split_8b14302553ddc3879748aad6158293a9_R_1 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[0];
            float _Split_8b14302553ddc3879748aad6158293a9_G_2 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[1];
            float _Split_8b14302553ddc3879748aad6158293a9_B_3 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[2];
            float _Split_8b14302553ddc3879748aad6158293a9_A_4 = _Property_ee322bcf8ee6df80b56b3dffe9ea3f70_Out_0[3];
            UnityTexture2D _Property_e96052cb5280048fb7045df6517dadfe_Out_0 = UnityBuildTexture2DStructNoScale(_ParticleTexture);
            float4 _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0 = _ParticleMaskTilingOffset;
            float _Split_09316789d5be448aba41ce9a8a79e989_R_1 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[0];
            float _Split_09316789d5be448aba41ce9a8a79e989_G_2 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[1];
            float _Split_09316789d5be448aba41ce9a8a79e989_B_3 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[2];
            float _Split_09316789d5be448aba41ce9a8a79e989_A_4 = _Property_d1ddfebd1c71f38784ca2d8fae4912f9_Out_0[3];
            float4 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4;
            float3 _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5;
            float2 _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_B_3, _Split_09316789d5be448aba41ce9a8a79e989_A_4, 0, 0, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGBA_4, _Combine_b73d37b980d957808efc77b8e5c6eeec_RGB_5, _Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6);
            float4 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4;
            float3 _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5;
            float2 _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6;
            Unity_Combine_float(_Split_09316789d5be448aba41ce9a8a79e989_R_1, _Split_09316789d5be448aba41ce9a8a79e989_G_2, 0, 0, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGBA_4, _Combine_e5516dc3c548c486a4ac584bc51b3893_RGB_5, _Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6);
            float4 _UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0 = IN.uv0;
            float2 _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2;
            Unity_Multiply_float2_float2(_Combine_e5516dc3c548c486a4ac584bc51b3893_RG_6, (_UV_c1acb6bd7e0ad482a5d924e1e6aa52e5_Out_0.xy), _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2);
            float2 _Add_9ac539502ae37687b2e506d5849f543a_Out_2;
            Unity_Add_float2(_Combine_b73d37b980d957808efc77b8e5c6eeec_RG_6, _Multiply_6835e7e10ef4cb848e2e1ca777876cb5_Out_2, _Add_9ac539502ae37687b2e506d5849f543a_Out_2);
            float4 _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e96052cb5280048fb7045df6517dadfe_Out_0.tex, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.samplerstate, _Property_e96052cb5280048fb7045df6517dadfe_Out_0.GetTransformedUV(_Add_9ac539502ae37687b2e506d5849f543a_Out_2));
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_R_4 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.r;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_G_5 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.g;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_B_6 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.b;
            float _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7 = _SampleTexture2D_bc686c6640e38c83928df8f381a07990_RGBA_0.a;
            float _Multiply_da551cc95d964f8086754940028076ab_Out_2;
            Unity_Multiply_float_float(_Split_8b14302553ddc3879748aad6158293a9_A_4, _SampleTexture2D_bc686c6640e38c83928df8f381a07990_A_7, _Multiply_da551cc95d964f8086754940028076ab_Out_2);
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_R_1 = IN.VertexColor[0];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_G_2 = IN.VertexColor[1];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_B_3 = IN.VertexColor[2];
            float _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4 = IN.VertexColor[3];
            float _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            Unity_Multiply_float_float(_Multiply_da551cc95d964f8086754940028076ab_Out_2, _Split_764e92ea336f4b7c8a951fe14dbd5e46_A_4, _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2);
            float _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0 = _Cutoff;
            surface.BaseColor = _Multiply_ae385e35933640d38dbea9a7e4323c73_Out_2;
            surface.Alpha = _Multiply_62c205cc5153499e9f0d117a755efa01_Out_2;
            surface.AlphaClipThreshold = _Property_b391b90c8ae2468ba5b5d934b7ad3521_Out_0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.WorldSpacePosition = input.positionWS;
            output.ScreenPosition = ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            output.uv0 = input.texCoord0;
            output.VertexColor = input.color;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBR2DPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditorForRenderPipeline "UnityEditor.ShaderGraphLitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}