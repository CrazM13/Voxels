using CMUI;
using CMVoxels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : VoxelEntity {

	#region Components
	private new Transform camera;
	#endregion

	#region Inspector
	[Header("Movement")]
	[SerializeField] private float walkSpeed = 3f;
	[SerializeField] private float sprintSpeed = 6f;
	[SerializeField] private float jumpForce = 5f;

	[Header("Player Components")]
	[SerializeField] private Transform selection;
	#endregion

	#region Input
	private Vector2 movementInput;
	private Vector2 mouseInput;
	private bool jumpRequest;

	private bool isSprinting;

	private bool isAttacking;
	private bool isInteracting;

	private bool isInputAllowed;
	public bool AllowInputs {
		get => isInputAllowed;
		set {

			if (isInputAllowed && !value) {
				movementInput = Vector2.zero;
				mouseInput = Vector2.zero;
				jumpRequest = false;

				isSprinting = false;

				isAttacking = false;
				isInteracting = false;

				Cursor.lockState = CursorLockMode.None;
			} else if (!isInputAllowed && value) {
				Cursor.lockState = CursorLockMode.Locked;
			}

			isInputAllowed = value;
		}
	}

	private void GetInputs() {

		if (!isInputAllowed) {
			if (!MenuManager.AreMenusActive) AllowInputs = true;
			else return;
		}

		movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

		isSprinting = Input.GetButton("Sprint");

		if (isGrounded && Input.GetButtonDown("Jump")) jumpRequest = true;

		isAttacking = Input.GetMouseButtonDown(0);
		isInteracting = Input.GetMouseButtonDown(1);

		// Menu Hotkeys
		if (Input.GetKeyDown(KeyCode.M)) {
			Menu newMenu = MenuManager.OpenMenu("Menu");
			if (newMenu is InventoryMenu inventoryMenu) {
				inventoryMenu.TargetInventory = GetInventory();
				inventoryMenu.RefreshUI();
			}
			AllowInputs = false;
		}
	}
	#endregion

	#region Physics
	private void Jump() {
		verticalMomentum = jumpForce;
		jumpRequest = false;
		isGrounded = false;
	}
	#endregion

	#region Chunk Loading
	private Vector2Int currentChunk;

	private void UpdateChunkLoading() {
		Vector2Int newChunk = world.ConvertPositionToChunk(transform.position);
		if (newChunk != currentChunk) {
			world.UnloadChunks(currentChunk);
			world.LoadChunks(newChunk);
			currentChunk = newChunk;
		}
	}

	private void InitChunkLoading() {
		Vector2Int newChunk = world.ConvertPositionToChunk(transform.position);
		world.LoadChunks(newChunk);
		currentChunk = newChunk;
	}
	#endregion

	#region Inventory
	private Inventory inventory = new Inventory();

	public Inventory GetInventory() => inventory;
	#endregion

	protected override void InitVoxelEntity() {
		camera = Camera.main.transform;

		AllowInputs = true;
		transform.position = world.GetWorldSpawn();

		InitChunkLoading();
	}

	protected override void UpdateVoxelEntity() {
		GetInputs();

		RaycastCursor(0.1f, 4f);

		if (selection.gameObject.activeSelf) {
			Vector3Int selectPos = Vector3Int.FloorToInt(selection.position);

			if (isAttacking) {
				Voxel pickedUpVoxel = world.GetVoxelAt(selectPos).GetVoxelType();
				world.SetVoxel(selectPos, Voxels.AIR);

				inventory.AddItemStack(new ItemStack(pickedUpVoxel));
			}

			if (isInteracting) {
				world.GetVoxelAt(selectPos).GetVoxelType().OnInteract(world, selectPos, this);
			}
		}

		UpdateChunkLoading();
	}

	protected override void UpdateVoxelEntityPhysics() {
		float speedValue = isSprinting ? sprintSpeed : walkSpeed;

		velocity = speedValue * Time.fixedDeltaTime * ((transform.forward * movementInput.y) + (transform.right * movementInput.x)).normalized;

		transform.Rotate(Vector3.up, mouseInput.x);
		camera.Rotate(Vector3.right, -mouseInput.y);

		if (jumpRequest) Jump();
	}

	protected override void Render() { /*MT*/ }

	private void RaycastCursor(float step, float maxReach) {

		float distance = 0;

		while (distance < maxReach) {

			Vector3Int pos = Vector3Int.RoundToInt(camera.position + (camera.forward * distance));

			if (world.GetVoxelAt(pos).GetVoxelType().GetVoxelID() != Voxels.AIR.GetVoxelID()) {

				selection.position = pos;

				selection.gameObject.SetActive(true);

				return;

			}

			distance += step;

		}

		selection.gameObject.SetActive(false);

	}

}
