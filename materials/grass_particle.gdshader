shader_type canvas_item;
uniform sampler2D grass_noise: repeat_enable;
uniform vec3 grass_color = vec3(0.216f, 0.58f, 0.431f);
uniform vec2 node_size = vec2(512.0f, 512.0f);

float terrace(float h) {
	float w = 0.4f;
	float k = floor(h / w);
	float f = (h - k*w) / 2.0f;
	float s = min(2.0f * f, 1.0f);
	return (k+s) * w;
}

varying vec2 world_pos;
varying flat vec2 bottom_pos;
void vertex() {
	bottom_pos = (MODEL_MATRIX * vec4(VERTEX, 0.0, 1.0)).xy + vec2(8.0f, 16.0f);
	// Called for every vertex the material is visible on.
	world_pos = (MODEL_MATRIX * vec4(VERTEX, 0.0, 1.0)).xy;
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec2 w_uv = bottom_pos / node_size;
	COLOR.rgb = grass_color + 0.1f * terrace(texture(grass_noise, w_uv).r);
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
