using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelSkybox : MonoBehaviour {

	#region Singleton
	public static VoxelSkybox Instance { get; private set; }

	private void Awake() {
		if (Instance) Destroy(this);
		else Instance = this;
	}
	#endregion

	[SerializeField] private Gradient skyColourOverTime;
	[SerializeField] private Gradient skyEmmisionOverTime;

	[Header("Time Settings")]
	[SerializeField, Min(1)] private float dayLength;

	private float gameTime;
	private float DayTime => gameTime % dayLength;

	private Skybox skybox;

	public Color SkyColour { get; private set; }
	public Color SkyboxEmmisions { get; private set; }

	// Start is called before the first frame update
	void Start() {
		skybox = Camera.main.GetComponent<Skybox>();

		VoxelLightingData.InitShaderLighting();
	}

	// Update is called once per frame
	void Update() {
		gameTime += Time.deltaTime;

		float dayPercentage = DayTime / dayLength;
		SkyColour = skyColourOverTime.Evaluate(dayPercentage);
		SkyboxEmmisions = skyEmmisionOverTime.Evaluate(dayPercentage);

		skybox.material.SetColor("_SkyboxColour", SkyColour);
		skybox.material.SetFloat("_StarVisibility", 1 - SkyColour.a);

		Shader.SetGlobalFloat("SkyLightLevel", Mathf.Clamp(SkyColour.a, 0.05f, 1));
		Shader.SetGlobalColor("SkyLightColour", SkyboxEmmisions);
	}

}
