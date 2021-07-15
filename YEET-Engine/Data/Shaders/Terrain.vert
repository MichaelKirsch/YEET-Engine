#version 330 core

layout (location = 0) in vec3 aPosition;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

uniform vec3 offset;

out vec4 opos;

void main()
{
    vec4 pos  = projection * view * model * vec4(aPosition+offset, 1.0);
    opos = pos;
    gl_Position = pos;
}