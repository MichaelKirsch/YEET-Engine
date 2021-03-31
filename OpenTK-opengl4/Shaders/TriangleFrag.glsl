#version 330 core

out vec4 FragColor;
in vec3 oColor;
uniform vec3 rgb;
void main()
{
    if(oColor[0]<rgb[0])
    {
        FragColor = vec4(0, 0.031, 1,1.0);
        return;
    }
    if(oColor[0]>=rgb[0] && oColor[0]<rgb[1])
    {
        FragColor = vec4(1, 0.666, 0,1.0);
        return;
    }
    if(oColor[0]>=rgb[1] && oColor[0]<rgb[2])
    {
        FragColor = vec4(0.121, 0.6, 0,1.0);
        return;
    }  
    if(oColor[0]>rgb[2])
    {
        FragColor = vec4(0.458, 0.458, 0.458,1.0);
        return;
    } 
}