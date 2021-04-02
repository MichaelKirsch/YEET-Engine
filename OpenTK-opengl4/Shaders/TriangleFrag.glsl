#version 330 core

out vec4 FragColor;
in vec3 oColor;
uniform vec3 rgb;
uniform float MaxHeight;
uniform float MinHeight;

float map(float x, float in_min, float in_max, float out_min, float out_max)
{
  return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
}

void main()
{
        FragColor = vec4(0.0, map(oColor[1],MinHeight,MaxHeight,0.0,1.0), 0.5,1.0);
}