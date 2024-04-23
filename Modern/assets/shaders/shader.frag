#version 330 core
in vec2 TexCoord;

uniform sampler2D texture1;
uniform vec4 ourColour;

out vec4 FragColour;

void main()
{
//	FragColour = ourColour;
//	FragColour = vec4(TexCoord, 0, 1);
	FragColour = texture(texture1, TexCoord) * ourColour;
}
