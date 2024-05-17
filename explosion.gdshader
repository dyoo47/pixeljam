shader_type canvas_item;
uniform float radius = 8.0f;
uniform float border_width = 2.0f;
uniform vec3 circle_color = vec3(0.796f, 0.859f, 0.988f);

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// Called for every pixel the material is visible on.
	COLOR = vec4(circle_color, 0.0f);
	vec2 pos = UV / TEXTURE_PIXEL_SIZE;
	vec2 center = 0.5f / TEXTURE_PIXEL_SIZE;
	float innerDist = distance(pos, center);
	if(innerDist <= radius + border_width && innerDist >= radius){
		COLOR.a = 1.0f;
	}
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
