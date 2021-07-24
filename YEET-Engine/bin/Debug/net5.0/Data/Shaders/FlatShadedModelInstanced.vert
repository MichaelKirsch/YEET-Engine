#version 440 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec3 aColor;



layout(std430, binding = 0) buffer modelData
{
    float data[];
};

out vec3 oColor;
out vec3 oNormal;
out vec3 oPos;
out float oHighlight;

uniform mat4 view;
uniform mat4 projection;


void main()
{ 
    vec4 finalPos = projection * view  * vec4(aPosition+vec3(data[gl_InstanceID*4],data[(gl_InstanceID*4)+1],data[(gl_InstanceID*4)+2]), 1.0);
    oHighlight = data[(gl_InstanceID*4)+3];
    gl_Position = finalPos;
    oColor = aColor;
    oNormal = aNormal;
    oPos = vec3(finalPos);
}