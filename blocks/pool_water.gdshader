shader_type canvas_item;
uniform sampler2D noise: repeat_enable;
uniform sampler2D specular: repeat_enable;
uniform sampler2D diffuse: repeat_enable;
uniform sampler2D concrete_noise: repeat_enable;
uniform sampler2D grass_noise: repeat_enable;
uniform vec3 water_color = vec3(0.388f, 0.608f, 1.0f);
uniform vec3 concrete_color = vec3(0.843f, 0.827f, 0.816f);
uniform vec3 grass_color = vec3(0.216f, 0.58f, 0.431f);
uniform vec2 node_size = vec2(512.0f, 512.0f);

float sigmoid(float x) {
	return 1.0f / (1.0f + exp(-1.0f * x));
}

float terrace(float h) {
	float w = 0.4f;
	float k = floor(h / w);
	float f = (h - k*w) / 2.0f;
	float s = min(2.0f * f, 1.0f);
	return (k+s) * w;
}
varying vec2 world_pos;
void vertex() {
	// Called for every vertex the material is visible on.
	world_pos = (MODEL_MATRIX * vec4(VERTEX, 0.0, 1.0)).xy;
}

void fragment() {
	COLOR = texture(TEXTURE, UV);
	vec2 w_uv = world_pos / node_size;
	if(COLOR.rgb == vec3(0.0f, 0.0f, 1.0f)){
		w_uv += vec2((sin(TIME/4.0f) / 16.0f + 1.0f) / 2.0f);
		vec3 sample = texture(noise, w_uv).xyz;
		
		vec2 s_uv = world_pos / node_size;
		s_uv += vec2((sin(TIME/3.0f)) / 32.0f);
		vec3 spec_sample = texture(specular, s_uv).xyz;
		
		COLOR.rgb = water_color;
		if(sample.r > 0.8f){
			COLOR.rgb += vec3(0.3f);
			if(spec_sample.r > 0.6f){
				COLOR.rgb = vec3(1.0f);
			}
		}else{
			COLOR.rgb += 0.4f * texture(diffuse, w_uv).xyz;
		}
	}else if(COLOR.rgb == vec3(1.0f, 0.0f, 0.0f)){
		COLOR.rgb = concrete_color + 0.1f * terrace(texture(concrete_noise, w_uv).r);
	}else if(COLOR.rgb == vec3(0.0f, 1.0f, 0.0f)){
		COLOR.rgb = grass_color + 0.1f * terrace(texture(grass_noise, w_uv).r);
	}
}


//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
