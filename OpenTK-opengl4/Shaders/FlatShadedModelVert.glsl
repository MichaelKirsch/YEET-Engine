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
    gl_Position =projection * view *  model * vec4(aPosition, 1.0);
    oColor = aColor;
    oNormal = aNormal;
    oPos = aPosition;
}