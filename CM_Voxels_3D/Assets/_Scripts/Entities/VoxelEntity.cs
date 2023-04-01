using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMVoxels.Rendering;
using CMVoxels.VoxelModels;

public abstract class VoxelEntity : MonoBehaviour {

	#region Components
	protected World world;
	#endregion

	#region Inspector
	[Header("Rendering")]
	[SerializeField] private Material voxelMaterial;
	[SerializeField] private TextAsset modelFile;

	[Header("Hitbox")]
	[SerializeField] protected Vector2 hitboxSize;
	#endregion

	#region Physics
	protected Vector3 velocity;
	protected float verticalMomentum;
	protected bool isGrounded;

	private void CalculateVelocity() {
		if (verticalMomentum > Physics.gravity.y) {
			verticalMomentum += Time.fixedDeltaTime * Physics.gravity.y;
		}

		velocity += Time.fixedDeltaTime * verticalMomentum * Vector3.up;
		if (velocity.y < 0.25f * Physics.gravity.y) velocity.y = 0.25f * Physics.gravity.y;

		// Collision
		if (IsCollidingWithZ()) velocity.z = 0;
		if (IsCollidingWithX()) velocity.x = 0;

		if (IsCollidingWithGround()) {
			verticalMomentum = 0;
			velocity.y = 0;
			isGrounded = true;
		}

		if (IsCollidingWithCeiling()) {
			verticalMomentum = 0;
			velocity.y = 0;
		}
	}

	private bool IsCollidingWithGround() {
		if (velocity.y > 0) return false;

		float downSpeed = velocity.y;

		if (
			world.GetVoxelAt(transform.position.x - hitboxSize.x, transform.position.y + downSpeed, transform.position.z - hitboxSize.x).GetVoxelType().IsSolid() ||
			world.GetVoxelAt(transform.position.x + hitboxSize.x, transform.position.y + downSpeed, transform.position.z - hitboxSize.x).GetVoxelType().IsSolid() ||
			world.GetVoxelAt(transform.position.x - hitboxSize.x, transform.position.y + downSpeed, transform.position.z + hitboxSize.x).GetVoxelType().IsSolid() ||
			world.GetVoxelAt(transform.position.x + hitboxSize.x, transform.position.y + downSpeed, transform.position.z + hitboxSize.x).GetVoxelType().IsSolid()
			) {

			return true;
		} else {
			return false;
		}

	}

	private bool IsCollidingWithCeiling() {
		if (velocity.y < 0) return false;

		float upSpeed = velocity.y;

		if (
			world.GetVoxelAt(transform.position.x + hitboxSize.x, transform.position.y + hitboxSize.y + upSpeed, transform.position.z - hitboxSize.x).GetVoxelType().IsSolid() ||
			world.GetVoxelAt(transform.position.x - hitboxSize.x, transform.position.y + hitboxSize.y + upSpeed, transform.position.z - hitboxSize.x).GetVoxelType().IsSolid() ||
			world.GetVoxelAt(transform.position.x + hitboxSize.x, transform.position.y + hitboxSize.y + upSpeed, transform.position.z + hitboxSize.x).GetVoxelType().IsSolid() ||
			world.GetVoxelAt(transform.position.x - hitboxSize.x, transform.position.y + hitboxSize.y + upSpeed, transform.position.z + hitboxSize.x).GetVoxelType().IsSolid()
			) {
			return true;
		} else {
			return false;
		}
	}

	private bool IsCollidingWithX() {
		if (velocity.x == 0) return false;

		float xSpeed = velocity.x;

		if (xSpeed > 0) {
			if (
				world.GetVoxelAt(transform.position.x + hitboxSize.x + xSpeed, transform.position.y + hitboxSize.y, transform.position.z - hitboxSize.x).GetVoxelType().IsSolid() ||
				world.GetVoxelAt(transform.position.x + hitboxSize.x + xSpeed, transform.position.y, transform.position.z - hitboxSize.x).GetVoxelType().IsSolid() ||
				world.GetVoxelAt(transform.position.x + hitboxSize.x + xSpeed, transform.position.y + hitboxSize.y, transform.position.z + hitboxSize.x).GetVoxelType().IsSolid() ||
				world.GetVoxelAt(transform.position.x + hitboxSize.x + xSpeed, transform.position.y, transform.position.z + hitboxSize.x).GetVoxelType().IsSolid()
				) {
				return true;
			} else {
				return false;
			}
		} else {
			if (
				world.GetVoxelAt(transform.position.x - hitboxSize.x + xSpeed, transform.position.y, transform.position.z - hitboxSize.x).GetVoxelType().IsSolid() ||
				world.GetVoxelAt(transform.position.x - hitboxSize.x + xSpeed, transform.position.y + hitboxSize.y, transform.position.z - hitboxSize.x).GetVoxelType().IsSolid() ||
				world.GetVoxelAt(transform.position.x - hitboxSize.x + xSpeed, transform.position.y, transform.position.z + hitboxSize.x).GetVoxelType().IsSolid() ||
				world.GetVoxelAt(transform.position.x - hitboxSize.x + xSpeed, transform.position.y + hitboxSize.y, transform.position.z + hitboxSize.x).GetVoxelType().IsSolid()
				) {
				return true;
			} else {
				return false;
			}
		}
	}

	private bool IsCollidingWithZ() {
		if (velocity.x == 0) return false;

		float zSpeed = velocity.z;

		if (zSpeed > 0) {
			if (
				world.GetVoxelAt(transform.position.x - hitboxSize.x, transform.position.y + hitboxSize.y, transform.position.z + hitboxSize.x + zSpeed).GetVoxelType().IsSolid() ||
				world.GetVoxelAt(transform.position.x - hitboxSize.x, transform.position.y               , transform.position.z + hitboxSize.x + zSpeed).GetVoxelType().IsSolid() ||
				world.GetVoxelAt(transform.position.x + hitboxSize.x, transform.position.y + hitboxSize.y, transform.position.z + hitboxSize.x + zSpeed).GetVoxelType().IsSolid() ||
				world.GetVoxelAt(transform.position.x + hitboxSize.x, transform.position.y               , transform.position.z + hitboxSize.x + zSpeed).GetVoxelType().IsSolid()
				) {
				return true;
			} else {
				return false;
			}
		} else {
			if (
				world.GetVoxelAt(transform.position.x - hitboxSize.x, transform.position.y               , transform.position.z - hitboxSize.x + zSpeed).GetVoxelType().IsSolid() ||
				world.GetVoxelAt(transform.position.x - hitboxSize.x, transform.position.y + hitboxSize.y, transform.position.z - hitboxSize.x + zSpeed).GetVoxelType().IsSolid() ||
				world.GetVoxelAt(transform.position.x + hitboxSize.x, transform.position.y               , transform.position.z - hitboxSize.x + zSpeed).GetVoxelType().IsSolid() ||
				world.GetVoxelAt(transform.position.x + hitboxSize.x, transform.position.y + hitboxSize.y, transform.position.z - hitboxSize.x + zSpeed).GetVoxelType().IsSolid()
				) {
				return true;
			} else {
				return false;
			}
		}
	}
	#endregion

	#region Rendering
	protected new VoxelModelRenderer renderer;
	protected VoxelModel model;
	protected VoxelBoneTransform voxelTransform;

	private void InitRender() {
		//model = VoxelModelUtils.Load(modelFile);
		voxelTransform = new VoxelBoneTransform();

		renderer = new VoxelModelRenderer(gameObject, voxelMaterial);
	}
	#endregion

	void Start() {
		world = FindObjectOfType<World>();

		InitRender();
		InitVoxelEntity();
	}

	void Update() {
		UpdateVoxelEntity();
		Render();
	}

	void FixedUpdate() {
		UpdateVoxelEntityPhysics();

		CalculateVelocity();

		transform.Translate(velocity, Space.World);
	}

	protected abstract void InitVoxelEntity();
	protected abstract void UpdateVoxelEntity();
	protected abstract void UpdateVoxelEntityPhysics();
	protected abstract void Render();

	private void OnDrawGizmos() {
		Gizmos.DrawWireCube(transform.position + new Vector3(0, hitboxSize.y * 0.5f, 0), new Vector3(hitboxSize.x * 2, hitboxSize.y, hitboxSize.x * 2));
	}
}
