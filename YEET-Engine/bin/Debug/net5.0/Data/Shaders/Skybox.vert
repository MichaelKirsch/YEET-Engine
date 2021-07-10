#version 330 core

layout (location = 0) in vec3 aPosition;

out vec3 finalPos;
uniform vec3 cameraPosition;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
void main()
{
    vec3 pos = projection * view * vec4(aPosition+cameraPosition, 1.0); 
    gl_Position = pos;
    finalPos = pos;
}