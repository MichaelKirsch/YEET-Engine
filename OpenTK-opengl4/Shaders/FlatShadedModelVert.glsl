#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec3 aColor;


out vec3 oColor;


uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
void main()
{
    gl_Position =projection * view * vec4(aPosition, 1.0);
    oColor = aColor;
}