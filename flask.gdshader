shader_type canvas_item;

uniform sampler2D water_height;
uniform vec3 water_color = vec3(0.53f, 0.68f, 0.93f);
uniform float px_height = 32.0f;
uniform float border_height = 4.0f;
uniform float height_coef = 10.0f;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	COLOR = texture(TEXTURE, UV);
	vec2 half = UV / 2.0f;
	vec2 newUV = vec2(0.25f + half.x + sin(TIME) / 4.0f, 0.25f + half.y + cos(TIME) / 4.0f);
	float targetHeight = texture(water_height, vec2((sin(TIME/4.0f) + 1.0f) / 2.0f, newUV.x)).r * height_coef;
	float bgHeight = texture(water_height, vec2((cos(TIME/3.0f) + 1.0f) / 2.0f, newUV.x)).r * height_coef;
	if(COLOR.xyz == vec3(0.0f, 0.0f, 1.0f)){
		float height = UV.y / TEXTURE_PIXEL_SIZE.y;
		float bigWave = 1.0f / TEXTURE_PIXEL_SIZE.y - bgHeight - border_height - px_height;
		float smallWave = 1.0f / TEXTURE_PIXEL_SIZE.y - targetHeight - px_height;
		if(height >= bigWave){
			COLOR.rgb = water_color;
			if(height >= smallWave){
				COLOR.rgb = water_color + vec3(0.1f);
				//COLOR.rgb += texture(water_height, newUV).r / 3.0f;
			}
			COLOR.a = texture(water_height, newUV).r / 2.0f + 0.4f;
		}else{
			COLOR.a = 0.0f;
		}
	}
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
