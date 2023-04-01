using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureGenerator {

	private JitteredGrid jitteredGrid;
	private List<string> allowedBiomes;

	private RNG random;

	private Structure[] structures;

	public StructureGenerator(string jsonGenerator) {
		StructureGeneratorData data = JsonUtility.FromJson<StructureGeneratorData>(jsonGenerator);

		jitteredGrid = new JitteredGrid(data.generation.spacing, data.generation.randomness);
		allowedBiomes = new List<string>(data.generation.allowedBiomes);

		structures = new Structure[data.structures.Length];
		for (int i = 0; i < structures.Length; i++) {
			structures[i] = DataManager.Structures.LoadResource(data.structures[i]);
		}

		random = new RNG(0);
	}

	public Structure GetRandomStructure() {
		int randomIndex = random.NextInt() % structures.Length;
		return structures[randomIndex];
	}

	public bool CanPlaceAt(World world, Vector3 position) {
		Vector2Int intPosition = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z));
		Vector2Int targetPosition = Vector2Int.FloorToInt(jitteredGrid.GetNearest(new Vector2(position.x, position.z)));

		if (intPosition == targetPosition) {
			string currentBiome = world.GetWorldGenerator().GetBiome(position).BiomeName;
			return allowedBiomes.Contains(currentBiome);
		}

		return false;
	}

}
