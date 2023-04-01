using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RNG {

	private const long MODULUS = 281_474_976_710_656;
	private const long MULTIPLIER = 25_214_903_917;
	private const long INCREMENT = 11;
	private long seed = 0;

	public RNG(long seed) {
		this.seed = seed;
	} 

	public int NextInt() {
		seed = ((MULTIPLIER * seed) + INCREMENT) % MODULUS;
		return (int) Mathf.Log(seed, 2);
	}

}
