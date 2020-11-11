
using TMPro;
using UBOAT.Game.Core.Serialization;
using UnityEngine;

/*
GyroCompassControler

This script manage the GyroCompassControler prefab
Basically that just rotates the disks and display the bearing
*/

namespace UBOAT.Mods.Couaky.CompassNavigation {
	[NonSerializedInGameState]
	public class GyroCompassControler : MonoBehaviour {

		public Transform externalDisk;
		public float externalDiskOffset;

		public Transform internalDisk;
		public float internalDiskOffset;

		public Transform bearingTarget;
		public float bearingTargetOffset;

		public TextMeshProUGUI angleIndicator;

		// Inputs from ShipControlGUIPatch
		[HideInInspector]
		public float currentBearingAngle; // Current bearing of ship
		[HideInInspector]
		public float targetBearingAngle; // The target bearing
		[HideInInspector]
		public bool selectedBearingAngleEnabled; // If the mouse is over the controler
		[HideInInspector]
		public float selectedBearingAngle; // The bearing computed from the mouse position
		
		void Awake() {
			currentBearingAngle = 0.0f;
			targetBearingAngle = 0.0f;
			selectedBearingAngleEnabled = false;
			selectedBearingAngle = 0.0f;
		}

		void Update () {
			int angleInt = Mathf.FloorToInt(currentBearingAngle);

			// Update repeaters positions
			externalDisk.eulerAngles = new Vector3(0.0f, 0.0f, (currentBearingAngle + externalDiskOffset));
			internalDisk.eulerAngles = new Vector3(0.0f, 0.0f, (currentBearingAngle - angleInt) * 360.0f + internalDiskOffset);

			// Update target bearing and angle indicator
			if (selectedBearingAngleEnabled) {
				bearingTarget.eulerAngles = new Vector3(0.0f, 0.0f, (-selectedBearingAngle + currentBearingAngle + bearingTargetOffset));
				angleIndicator.text = Mathf.FloorToInt(Mathf.Repeat(selectedBearingAngle - currentBearingAngle, 360.0f)).ToString() + "°\n" +
										Mathf.FloorToInt(selectedBearingAngle).ToString() + "°";
			} else {
				bearingTarget.eulerAngles = new Vector3(0.0f, 0.0f, (-targetBearingAngle + currentBearingAngle + bearingTargetOffset));
				angleIndicator.text = angleInt.ToString() + "°";
			}
		}
	}
}
