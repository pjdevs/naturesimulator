using Godot;
using System;

public class MapGenerator : MultiMeshInstance
{
	[Export]
	public int Width = 20;
	[Export]
	public int Length = 20;
	[Export]
	public int Height = 3;

	private RandomNumberGenerator random;

	public override void _Ready()
	{
		// Init vars
		random = new RandomNumberGenerator();

		Color grass = Color.Color8(100, 200, 25);
		Color water = Color.Color8(0, 130, 200);

		// Set meshes count
		Multimesh.InstanceCount = Width * Length * Height;

		// Create map
		Mesh mesh = Multimesh.Mesh;
		Vector3 meshSize = mesh.GetAabb().Size;
		int instance = 0;

		Console.WriteLine(meshSize);

		for (int h=0; h<Height; h++)
		{
			for (int l=0; l<Length; l++)
			{
				for (int w=0; w<Width; w++)
				{
					Color currentColor;

					if (h <= Height - 2)
						currentColor = grass;
					else
					{
						int color = random.RandiRange(0, 1);

						currentColor = color == 0 ? grass : water;
					}
					Multimesh.SetInstanceColor(instance, currentColor);

					Transform position = Transform.Identity.Translated(new Vector3(w * meshSize.x, h * meshSize.y, l * meshSize.z));
					Multimesh.SetInstanceTransform(instance, position);

					Console.WriteLine(position.origin);

					instance++;
				}	
			}	
		}

	}
}
