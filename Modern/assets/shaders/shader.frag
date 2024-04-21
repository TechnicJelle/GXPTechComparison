#version 330 core
in vec2 TexCoord;

uniform sampler2D texture1;
uniform vec4 ourColour;

out vec4 FragColour;

void main()
{
	FragColour = texture(texture1, TexCoord) * ourColour;
}
