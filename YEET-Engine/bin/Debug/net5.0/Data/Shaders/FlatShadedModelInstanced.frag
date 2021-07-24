#version 440 core

out vec4 FragColor;
in vec3 oColor;
in vec3 oNormal;
in vec3 oPos;
in float oHighlight;

uniform vec3 lightColor;

uniform vec3 LightPosition;

uniform bool UsingOverwriteColor;

uniform vec3 OverwriteColor;

void main()
{

    vec3 finalColor = oColor;
    vec3 internalLightColor = lightColor;
    if(UsingOverwriteColor){
        internalLightColor = OverwriteColor;
    
    }

    if(oHighlight==1.0){
        finalColor = vec3(0.0353, 0.3333, 0.5059);
    }


    vec3 norm = normalize(oNormal);
    vec3 lightDir = normalize(LightPosition- oPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec3 diffuse = diff *  internalLightColor;
    vec3 result = (diffuse+0.5) * finalColor;
    FragColor = vec4(result, 1.0);
}