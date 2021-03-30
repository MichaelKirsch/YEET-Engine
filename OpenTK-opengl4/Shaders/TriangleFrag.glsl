#version 330 core

out vec4 FragColor;
uniform vec4 ourColor; // we set this variable in the OpenGL code.
uniform float red;
void main()
{
    FragColor = vec4(red,0.0,0.0,1.0);
}