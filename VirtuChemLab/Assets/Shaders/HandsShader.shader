Shader "Unlit/HandsShader"
{
    Properties
    {
		_Color("Always visible Color", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType" = "Transparent"}
        LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		Lighting Off
		ZTest Always
		ZWrite Off


        Pass
        {
		
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID //Insert
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				
				UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata v)
            {
                v2f o;
				UNITY_SETUP_INSTANCE_ID(v); //Insert
				UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert
				
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
			
			float4 _Color;
			
            fixed4 frag (v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}
