#version 330 core

layout (location = 0) in vec3 aPosition;

out vec3 oColor;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
void main()
{
    gl_Position =projection * view * vec4(aPosition, 1.0);
    oColor = aPosition;
}