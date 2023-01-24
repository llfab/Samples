#version 450
#pragma shader_stage(vertex)

layout(binding = 0) uniform UniformBufferObject {
    float time;
} ubo;

layout(location = 0) out float outTime;


vec2 positions[] = vec2[](
    vec2(1, -1),
    vec2(-1, -1),
    vec2(1, 1),
    vec2(1, 1),
    vec2(-1, 1),
    vec2(-1, -1)
);

void main() {
    gl_Position = vec4(positions[gl_VertexIndex], 0.0, 1.0);
    outTime = ubo.time;
}