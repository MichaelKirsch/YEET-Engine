#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 acol;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
out vec3 rgb;

void main()
{
    gl_Position =projection * view * vec4(aPosition, 1.0);
    rgb= acol;
}