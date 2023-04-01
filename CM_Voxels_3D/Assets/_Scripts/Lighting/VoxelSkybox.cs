using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelSkybox : MonoBehaviour {

	#region Const
	private const float SUNRISE_START = 0.25f;
	private const float SUNRISE_END = 0.375f;

	private const float SUNSET_START = 0.7083f;
	private const float SUNSET_END = 0.8333f;
	#endregion

	#region Singleton
	public static VoxelSkybox Instance { get; private set; }

	private void Awake() {
		if (Instance) Destroy(this);
		else Instance = this;
	}
	#endregion

	[SerializeField] private Color daySkyColour;
	[SerializeField] private Color nightSkyColour;
	[SerializeField] private Color horizonSkyColour;

	[SerializeField, Min(1)] private float dayLength;

	private float gameTime;
	private float DayTime => gameTime % dayLength;

	private Camera mainCamera;

	public Color SkyColour { get; private set; }

	// Start is called before the first frame update
	void Start() {
		mainCamera = Camera.main;

		VoxelLightingData.InitShaderLighting();
	}

	// Update is called once per frame
	void Update() {
		gameTime += Time.deltaTime;

		float skyProgression = DayTimeToSkyProgression();
		SkyColour = Color.Lerp(daySkyColour, nightSkyColour, skyProgression);

		SkyColour = Color.Lerp(horizonSkyColour, SkyColour, DistanceToHorizon());

		mainCamera.backgroundColor = SkyColour;
		Shader.SetGlobalFloat("SkyLightLevel", Mathf.Clamp(1 - skyProgression, 0.05f, 1));
		//Shader.SetGlobalColor("SkyLightColour", SkyColour);
	}

	private float DayTimeToSkyProgression() {

		float dayPercentage = DayTime / dayLength;
		

		if (dayPercentage < SUNRISE_START || dayPercentage > SUNSET_END) {
			return 1; // Night
		} else if (dayPercentage > SUNRISE_END && dayPercentage < SUNSET_START) {
			return 0; // Day
		} else if (dayPercentage <= SUNRISE_END) {
			return Mathf.InverseLerp(SUNRISE_END, SUNRISE_START, dayPercentage); // Sun Rise
		} else {
			return Mathf.InverseLerp(SUNSET_START, SUNSET_END, dayPercentage); // Sun Set
		}
	}

	private float DistanceToHorizon() {
		float dayPercentage = DayTime / dayLength;

		if (dayPercentage < SUNRISE_START || dayPercentage > SUNSET_END) {
			return 1; // Night
		} else if (dayPercentage > SUNRISE_END && dayPercentage < SUNSET_START) {
			return 1; // Day
		} else if (dayPercentage <= SUNRISE_END) {
			return 0.5f + (Mathf.Abs(dayPercentage - ((SUNRISE_START + SUNRISE_END) / 2f)) / (SUNRISE_END - SUNRISE_START)); // Sun Rise
		} else {
			return 0.5f + (Mathf.Abs(dayPercentage - ((SUNSET_START + SUNSET_END) / 2f)) / (SUNSET_END - SUNSET_START)); // Sun Set
		}

	}

}
