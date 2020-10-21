// Copyright (c) 2020 Vuplex Inc. All rights reserved.
//
// Licensed under the Vuplex Commercial Software Library License, you may
// not use this file except in compliance with the License. You may obtain
// a copy of the License at
//
//     https://vuplex.com/commercial-library-license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
Shader "Vuplex/Viewport Shader" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        [KeywordEnum(None, TopBottom, LeftRight)] _StereoMode ("Stereo mode", Float) = 0
        [Toggle(FLIP_X)] _FlipX ("Flip X", Float) = 0
        [Toggle(FLIP_Y)] _FlipY ("Flip Y", Float) = 0
        _Gamma ("Gamma for Gamma Correction", Range(0.01, 3.0)) = 0.6

        [Header(Properties set programmatically)]
        _VideoCutoutRect("Video Cutout Rect", Vector) = (0, 0, 0, 0)
        _CropRect("Crop Rect", Vector) = (0, 0, 0, 0)
        _OverrideStereoToMono ("Override Stereo to Mono", Float) = 0
        _EnableGammaCorrection ("Enable Gamma Correction", Float) = 0
    }

    SubShader {
        Pass {
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }

            Lighting Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
                #pragma multi_compile ___ _STEREOMODE_TOPBOTTOM _STEREOMODE_LEFTRIGHT
                #pragma multi_compile ___ FLIP_X
                #pragma multi_compile ___ FLIP_Y
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                Texture2D _MainTex;
                // Specify linear filtering by using a SamplerState
                // and specifying "linear" in its name.
                // https://docs.unity3d.com/Manual/SL-SamplerStates.html
                SamplerState linear_clamp_sampler;

                float4 _MainTex_ST;
                float _OverrideStereoToMono;

                v2f vert (appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    float2 untransformedUV = v.uv;

                    #ifdef FLIP_X
                        untransformedUV.x = 1.0 - untransformedUV.x;
                    #endif // FLIP_X
                    #ifdef FLIP_Y
                        untransformedUV.y = 1.0 - untransformedUV.y;
                    #endif // FLIP_Y

                    #ifdef _STEREOMODE_TOPBOTTOM
                        untransformedUV.y *= 0.5;
                        if (unity_StereoEyeIndex == 1 && _OverrideStereoToMono != 1.0) {
                            untransformedUV.y += 0.5;
                        }
                    #endif // _STEREOMODE_TOPBOTTOM
                    #ifdef _STEREOMODE_LEFTRIGHT
                        untransformedUV.x *= 0.5;
                        if (unity_StereoEyeIndex != 0 && _OverrideStereoToMono != 1.0) {
                            untransformedUV.x += 0.5;
                        }
                    #endif // _STEREOMODE_LEFTRIGHT

                    o.uv = TRANSFORM_TEX(untransformedUV, _MainTex);
                    return o;
                }

                float3 gammaCorrect(float3 v, float gamma) {
                    float inverse = 1.0 / gamma;
                    return pow(v, float3(inverse, inverse, inverse));
                }

                float4 gammaCorrect(float4 v, float gamma) {
                    return float4(gammaCorrect(v.xyz, gamma), v.w);
                }

                float _Gamma;
                float4 _VideoCutoutRect;
                float4 _CropRect;
                float _EnableGammaCorrection;

                fixed4 frag (v2f i) : SV_Target {

                    fixed4 col = _MainTex.Sample(linear_clamp_sampler, i.uv);

                    if (_EnableGammaCorrection == 1.0) {
                        col = gammaCorrect(col, _Gamma);
                    }

                    float cutoutWidth = _VideoCutoutRect.z;
                    float cutoutHeight = _VideoCutoutRect.w;

                    #ifdef FLIP_Y
                        float nonflippedY = 1.0 - i.uv.y;
                    #else
                        float nonflippedY = i.uv.y;
                    #endif // FLIP_Y

                    #ifdef FLIP_X
                        float nonflippedX = i.uv.x;
                    #else
                        float nonflippedX = 1.0 - i.uv.x;
                    #endif // FLIP_X

                    // Make the pixels transparent if they fall within the video rect cutout and the they're black.
                    // Keeping non-black pixels allows the video controls to still show up on top of the video.
                    bool pointIsInCutout = cutoutWidth != 0.0 &&
                                           cutoutHeight != 0.0 &&
                                           nonflippedX >= _VideoCutoutRect.x &&
                                           nonflippedX <= _VideoCutoutRect.x + cutoutWidth &&
                                           nonflippedY >= _VideoCutoutRect.y &&
                                           nonflippedY <= _VideoCutoutRect.y + cutoutHeight;

                    if (pointIsInCutout && all(col == float4(0.0, 0.0, 0.0, 1.0))) {
                        col = float4(0.0, 0.0, 0.0, 0.0);
                    }

                    float cropWidth = _CropRect.z;
                    float cropHeight = _CropRect.w;
                    bool pointIsOutsideOfCrop = cropWidth != 0.0 &&
                                                cropHeight != 0.0 &&
                                                (nonflippedX < _CropRect.x || nonflippedX > _CropRect.x + cropWidth ||nonflippedY < _CropRect.y || nonflippedY > _CropRect.y + cropHeight);
                    if (pointIsOutsideOfCrop) {
                        col = float4(0.0, 0.0, 0.0, 0.0);
                    }
                    return col;
                }
            ENDCG
        }
    }
    Fallback "Unlit/Texture"
}
