shader_type canvas_item;
uniform sampler2D specular;
uniform vec2 spin_direction = vec2(0.3f, 0.2f);
uniform vec3 border_color = vec3(1.0f);
uniform vec3 water_base_color = vec3(0.53f, 0.68f, 0.93f);

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	COLOR = texture(TEXTURE, UV);
	// Border
	bool contrary = 
		texture(TEXTURE, UV + vec2(TEXTURE_PIXEL_SIZE.x, 0.0f)).a > 0.0f ||
		texture(TEXTURE, UV - vec2(TEXTURE_PIXEL_SIZE.x, 0.0f)).a > 0.0f ||
		texture(TEXTURE, UV + vec2(0.0f, TEXTURE_PIXEL_SIZE.y)).a > 0.0f ||
		texture(TEXTURE, UV - vec2(0.0f, TEXTURE_PIXEL_SIZE.y)).a > 0.0f ||
		texture(TEXTURE, UV + vec2(TEXTURE_PIXEL_SIZE)).a > 0.0f ||
		texture(TEXTURE, UV - vec2(TEXTURE_PIXEL_SIZE)).a > 0.0f;
	
	// Spherical bulge
	float bulge_factor = -0.8f;
	vec2 center = vec2(0.5, 0.5);
	float dist = distance(UV, center);
	float distortion_factor = bulge_factor * (1.0 - pow(dist, 2));
	vec2 new_uv = UV + (UV - vec2(0.5)) * distortion_factor;
	new_uv += vec2(TIME) * spin_direction;
	new_uv.x = mod(new_uv.x, 1.0f);
	new_uv.y = mod(new_uv.y, 1.0f);
	float spec = texture(specular, new_uv).r;
	if(COLOR.a > 0.0f){
		//COLOR.rgb = vec3(1.0f);
		COLOR.rgb = water_base_color + vec3(spec / 2.0f);
		COLOR.a = spec;
		//COLOR.a = texture(specular, UV).r;
	}else if(COLOR.a == 0.0f && contrary){
		COLOR = vec4(border_color, 1.0f);
	}
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
