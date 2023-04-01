using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JitteredGrid {

	private float spacing;
	private float jitter;

	public JitteredGrid(float spacing, float jitter) {
		this.spacing = spacing;
		this.jitter = jitter;
	}

	public Vector2 GetNearest(Vector2 position) {
		// FIX THIS. It doesn't work, all points in the square area go to the same pre-jitter point. It doesn't actually find the nearest you moron
		float x = Snapping.Snap(position.x, spacing);
		float y = Snapping.Snap(position.y, spacing);

		return GetNewPosition(new Vector2(x, y));
	}

	private Vector2 GetNewPosition(Vector2 position) {
		float xPrime = position.x + (NoiseF(position) * jitter);
		float yPrime = position.y + (NoiseG(position) * jitter);

		return new Vector2(xPrime, yPrime);
	}

	protected virtual float NoiseF(Vector2 position) {
		return Mathf.PerlinNoise(position.x * 0.17f, position.y * 0.3f);
	}

	protected virtual float NoiseG(Vector2 position) {
		return Mathf.PerlinNoise(position.y * 0.251f, position.x * 0.07f);
	}

	public float GetSpacing() => spacing;
	public float GetJitter() => jitter;

}
