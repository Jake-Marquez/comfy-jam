shader_type canvas_item;

// Set every frame by Flashlight.cs. Both are in viewport pixels.
uniform vec2 light_pos;
uniform vec2 viewport_size = vec2(1152.0, 648.0);

uniform bool light_on = true;
uniform float radius = 120.0;
// Width of the fade between lit and dark at the circle's edge.
uniform float softness = 40.0;
uniform float darkness : hint_range(0.0, 1.0) = 0.95;
uniform vec4 dark_tint : source_color = vec4(0.02, 0.02, 0.05, 1.0);

void fragment() {
	// COLOR already holds texture * modulate here.
	vec2 pixel = SCREEN_UV * viewport_size;
	float dist = distance(pixel, light_pos);
	float lit = light_on ? 1.0 - smoothstep(radius - softness, radius, dist) : 0.0;

	vec3 dark = mix(COLOR.rgb, dark_tint.rgb, darkness);
	COLOR.rgb = mix(dark, COLOR.rgb, lit);
}
