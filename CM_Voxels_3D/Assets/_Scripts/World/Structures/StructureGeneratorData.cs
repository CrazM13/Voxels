using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StructureGeneratorData {

	[System.Serializable]
	public class GenerationData {

		public float spacing;
		public float randomness;
		public string[] allowedBiomes;

	}

	public GenerationData generation;
	public string[] structures;
}
