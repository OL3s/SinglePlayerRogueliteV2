shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_linear_mipmap;
uniform float blur_amount : hint_range(0.0, 4.0) = 3.0;
uniform vec4 overlay_color : source_color = vec4(0.08, 0.08, 0.10, 0.30);

void fragment() {
	vec4 blurred = textureLod(screen_texture, SCREEN_UV, blur_amount);
	COLOR = vec4(mix(blurred.rgb, overlay_color.rgb, overlay_color.a), 1.0);
}
