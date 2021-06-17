#version 330 core

out vec4 FragColor;
in vec3 rgb;
in vec3 oNormal;
in vec3 oPos;

uniform vec3 LightPosition;

vec3 lightColor = vec3(1,1,1);
vec3 lighpos = vec3(50,100,50);

void main()
{
        vec3 norm = normalize(oNormal);
        vec3 lightDir = normalize(lighpos- oPos);  
        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse = diff * lightColor;
        vec3 result = (diffuse+0.5) * rgb;
        FragColor = vec4(result, 1.0);
}