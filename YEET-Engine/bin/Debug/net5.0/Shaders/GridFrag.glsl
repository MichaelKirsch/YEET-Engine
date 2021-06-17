#version 330 core

out vec4 FragColor;
uniform vec3 rgb=vec3(1.0,0,0);

void main()
{
        FragColor = vec4(rgb,1.0);
}