shader_type canvas_item;
uniform vec3 borderColor = vec3(0.0f);
uniform vec3 startColor = vec3(1.0f, 1.0f, 0.0f);
uniform vec3 endColor = vec3(0.2f, 0.0f, 0.0f);
uniform float tint = 0.0f;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	COLOR = texture(TEXTURE, UV);
	vec3 diff = startColor - endColor;
	bool contrary = 
		texture(TEXTURE, UV + vec2(TEXTURE_PIXEL_SIZE.x, 0.0f)).a > 0.0f ||
		texture(TEXTURE, UV - vec2(TEXTURE_PIXEL_SIZE.x, 0.0f)).a > 0.0f ||
		texture(TEXTURE, UV + vec2(0.0f, TEXTURE_PIXEL_SIZE.y)).a > 0.0f ||
		texture(TEXTURE, UV - vec2(0.0f, TEXTURE_PIXEL_SIZE.y)).a > 0.0f ||
		texture(TEXTURE, UV + vec2(TEXTURE_PIXEL_SIZE)).a > 0.0f ||
		texture(TEXTURE, UV - vec2(TEXTURE_PIXEL_SIZE)).a > 0.0f;
	if(COLOR.a == 0.0f && contrary){
		COLOR = vec4(borderColor, 1);
	}else if(COLOR.a > 0.0f){
		COLOR = vec4(endColor + diff * vec3(1.0f-tint), 1.0f);
	}
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
