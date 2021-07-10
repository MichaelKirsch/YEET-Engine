#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec3 aColor;


out vec3 oColor;
out vec3 oNormal;
out vec3 oPos;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;


void main()
{
    mat3 NormalMatrix = transpose(inverse(mat3(model))); 
    vec4 finalPos = projection * view *  model * vec4(aPosition, 1.0);
    gl_Position = finalPos;
    oColor = aColor;
    oNormal = NormalMatrix*aNormal;
    oPos = vec3(finalPos);
    
}