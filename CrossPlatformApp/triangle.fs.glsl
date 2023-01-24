#version 450
#pragma shader_stage(fragment)

precision highp float;

layout(location = 0) in float time;
layout(location = 0) out vec4 outColor;

#define iTime time
#define iResolution vec3(vec2(512, 512),1.)

#define B (1.-fract(t*2.))
#define R(p,a,t) mix(a*dot(p,a),p,cos(t))+sin(t)*cross(p,a)
#define H(h) (cos((h)*6.3+vec3(0,23,21))*.5+.5)
void mainImage(out vec4 O, vec2 C)
{
    vec3 p,r=iResolution,c=vec3(0),
    d=normalize(vec3((C-.5*r.xy)/r.y,1));
    float s,e=0.,g=0.,t=iTime;
    for(float i=1.;i<99.;i++)
    {
        p=g*d;;
        p.z-=3.;
        p=R(p,vec3(.577),t*.3);
        s=3.;
        for(int i=0;i<8;++i) {
            p=vec3(1,3.+sin(t)*.3,2)-abs(p-vec3(1,2,1.5+sin(t)*.2)),
            s*=e=9./clamp(dot(p,p),.8,9.);
            p*=e;
        }
        g+=e=abs(p.y/s-.001)+1e-3;
        c+=mix(vec3(1),H(length(p*.2+.5)),.6)*.0015/i/e;  
    }
    c*=c;
    O=vec4(c,1);
}

void main(void)
{
    mainImage(outColor, gl_FragCoord.xy);
}