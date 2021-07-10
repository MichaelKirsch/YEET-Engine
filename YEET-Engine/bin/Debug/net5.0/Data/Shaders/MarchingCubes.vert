#version 330 core

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 acol;
layout (location = 2) in vec3 anorm;

uniform vec3 offset;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
out vec3 rgb;
out vec3 oPos;
out vec3 oNormal;

void main()
{
    vec4 pos = projection * view * vec4(vec3(aPosition+offset), 1.0);
    gl_Position = pos;
    oPos = vec3(pos);
    rgb= acol;
    oNormal = anorm;
}