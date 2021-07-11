#version 330 core
out vec4 FragColor;
  
in vec2 TexCoord;

uniform sampler2D colorTexture;
uniform sampler2D depthTexture;

void main()
{
    vec4 col = texture(colorTexture, TexCoord);
    vec4 depth =  texture(depthTexture, TexCoord);
    if(depth.x>0.998){
        FragColor = vec4(0, 0.662, 0.980,1.0);
    }
    else{
FragColor = col;
    }
    
}