#version 330 core

out vec4 FragColor;
in vec3 oColor;
uniform vec4[10] colors;
uniform float Size;



vec3 calcColor(){
    for(int x=0;x<10;x++){
        
    }
}


void main()
{
        FragColor = vec4(0.0, map(oColor[1],MinHeight,MaxHeight,0.0,1.0), 0.5,1.0);
}