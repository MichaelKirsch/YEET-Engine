#version 330 core

out vec4 FragColor;
uniform vec3 rgb=vec3(1.0,0,0);

in vec4 opos;

float rand(vec2 n) { 
	return fract(sin(dot(n, vec2(12.9898, 4.1414))) * 43758.5453);
}

float noise(vec2 p){
	vec2 ip = floor(p);
	vec2 u = fract(p);
	u = u*u*(3.0-2.0*u);
	
	float res = mix(
		mix(rand(ip),rand(ip+vec2(1.0,0.0)),u.x),
		mix(rand(ip+vec2(0.0,1.0)),rand(ip+vec2(1.0,1.0)),u.x),u.y);
	return (res*res);
}


void main()
{
    float factor = 0.01;
    
    FragColor = mix(vec4(0.0078, 0.4314, 0.098,1),vec4(0.0, 0.7137, 0.1529, 1.0), noise(vec2(opos.x*factor,opos.z*factor))) ;
}