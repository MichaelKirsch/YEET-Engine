#version 330 core
out vec4 FragColor;

in vec2 TexCoord;

uniform sampler2D colorTexture;
uniform sampler2D depthTexture;

uniform vec3 cameraPos;
uniform vec3 cameraFront;
uniform mat4 invperspective;
uniform mat4 invview;
uniform vec3 screen;
uniform float time;
uniform vec3 sunpos;
uniform vec3 suncolor;


float time_factor = 0.001;

float rand(vec2 n) { 
	return fract(sin(dot(n, vec2(12.9898, 4.1414))) * 43758.5453);
}

float noise(vec2 p){
    p+=vec2(time)*time_factor;
	vec2 ip = floor(p);
	vec2 u = fract(p);
	u = u*u*(3.0-2.0*u);
	
	float res = mix(
		mix(rand(ip),rand(ip+vec2(1.0,0.0)),u.x),
		mix(rand(ip+vec2(0.0,1.0)),rand(ip+vec2(1.0,1.0)),u.x),u.y);
	return sin(time*time_factor*2*(res*res));
}

float noiseCloud(vec2 p){
    p+=vec2(time)*time_factor*0.1;
	vec2 ip = floor(p);
	vec2 u = fract(p);
	u = u*u*(3.0-2.0*u);
	
	float res = mix(
		mix(rand(ip),rand(ip+vec2(1.0,0.0)),u.x),
		mix(rand(ip+vec2(0.0,1.0)),rand(ip+vec2(1.0,1.0)),u.x),u.y);
	return (res*res);
}


vec3 getNormal(vec2 p){
    vec3 p1 = vec3(p.x-0.1,noise(vec2(p.x-0.1,p.y-0.1)),p.y-0.1);
    vec3 p2 = vec3(p.x+0.1,noise(vec2(p.x+0.1,p.y-0.1)),p.y-0.1);
    vec3 p3 = vec3(p.x,noise(vec2(p.x,p.y+0.1)),p.y+0.1);
    vec3 A = p2-p1;
    vec3 B = p3-p1;
    return vec3(A.y*B.z - A.z*B.y,A.z * B.x - A.x * B.z,A.x * B.y - A.y * B.x);
}


vec4 calc_pos(){
    vec2 ScreenSpaceCoordinates = gl_FragCoord.xy / screen.xy;
    vec2 ClipSpaceCoordinates = ScreenSpaceCoordinates * 2.0f - 1.0f;
    vec4 Clip = vec4(ClipSpaceCoordinates, texture(depthTexture,TexCoord).x * 2.0f - 1.0f, 1.0f);
    Clip = invperspective * Clip ;
    vec3 ViewSpace = Clip.xyz / Clip.w; // Perspective division
    vec4 WorldSpace = invview * vec4(ViewSpace, 1.0f); // if needed
    return WorldSpace;

    //eturn (vec4(screen_x,screen_y,depth_buffer_value,1)*inverse(perspective)*inverse(view));
}


void main()
{
    vec4 col=texture(colorTexture,TexCoord);
    vec4 depth=texture(depthTexture,TexCoord);
    vec4 sppos=calc_pos();
    
    float distance = length(vec3(cameraPos-sppos.xyz));


    if(sppos.y<=0.0){
        vec3 norm = normalize(getNormal(vec2(sppos.x,sppos.z)));
        vec3 lightDir = normalize(sunpos- sppos.xyz);
        float diff = max(dot(norm, lightDir), 0.0);
        vec3 diffuse = diff *  suncolor;
        vec3 result = (diffuse+0.5) * vec3(0.3255, 0.2824, 0.902);
        result = vec3(0.447, 1, 0.439);
        FragColor = vec4(result, 1.0);
        return;
    }

    if(distance>499){
        FragColor = vec4(0,0,1, 1.0);
        return;
    }

    else{
        if(depth.x>0.99998){
            FragColor = vec4(0.0784, 0.5137, 0.5725, 1.0);
        }
        else{
            FragColor = col;
        }
    }
    
}