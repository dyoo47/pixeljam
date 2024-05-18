shader_type canvas_item;
uniform sampler2D noise: repeat_enable;
uniform sampler2D specular: repeat_enable;
uniform sampler2D diffuse: repeat_enable;
uniform vec3 water_color = vec3(0.388f, 0.608f, 1.0f);
uniform vec2 node_size = vec2(512.0f, 512.0f);

float sigmoid(float x) {
	return 1.0f / (1.0f + exp(-1.0f * x));
}

varying vec2 vert;
void vertex() {
	// Called for every vertex the material is visible on.
	vert = VERTEX;
}

void fragment() {
	COLOR = texture(TEXTURE, UV);
	if(COLOR.rgb == vec3(0.0f, 0.0f, 1.0f)){
		vec2 w_uv = vert / node_size;
		w_uv += vec2((sin(TIME/4.0f) / 16.0f + 1.0f) / 2.0f);
		vec3 sample = texture(noise, w_uv).xyz;
		
		vec2 s_uv = vert / node_size;
		s_uv += vec2((sin(TIME/3.8f)) / 32.0f);
		vec3 spec_sample = texture(specular, s_uv).xyz;
		
		COLOR.rgb = water_color;
		if(sample.r > 0.8f){
			COLOR.rgb += vec3(0.3f);
			if(spec_sample.r > 0.6f){
				COLOR.rgb = vec3(1.0f);
			}
		}else{
			COLOR.rgb += 0.2f * texture(diffuse, w_uv).xyz;
		}
	}
}


//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
