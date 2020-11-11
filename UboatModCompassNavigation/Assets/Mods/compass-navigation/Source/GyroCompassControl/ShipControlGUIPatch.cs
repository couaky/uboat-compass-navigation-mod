using System;
using UnityEngine;

using DWS.Common.Resources;

using Harmony;
using UBOAT.ModAPI.Core.InjectionFramework;
using UBOAT.Game.UI;
using UBOAT.Game.Scene.Entities;
using UBOAT.Game.Core.Data;

/*
ShipControlGUIPatch

This harmony patch add the control of the new GyroCompassControler
Patch Awake to add the compass control
Patch UpdateRudder to manage the compass control inputs and mouse
*/

namespace UBOAT.Mods.Couaky.CompassNavigation {

	// Global usefull variables
	public static class Global {
		public static GyroCompassControler gyroCompassControler;
	}

	[HarmonyPatch(typeof(ShipControlGUINew), "Awake")]
	public class ShipControlGUIPatchAwake {

		[Inject] private static ResourceManager resourceManager;

		private static void Postfix(ref GameObject ___rudderContainer) {
			try {
				Transform rudderRootTransform = ___rudderContainer.transform;
				GameObject compassControler = resourceManager.InstantiatePrefab("UI/GyroCompassControler/GyroCompassControler");
				Global.gyroCompassControler = compassControler.GetComponent<GyroCompassControler>();
				compassControler.transform.SetParent(rudderRootTransform.transform, false);
			} catch (Exception e) {
				// it's best to catch all exceptions in event listeners; they should preferrably not throw exceptions in C# due to unpredictable results of that
				Debug.LogException(e);
			}
		}
	}

	[HarmonyPatch(typeof(ShipControlGUINew), "UpdateRudder")]
	public class ShipControlGUIPatchUpdateRudder {
		[Inject]
    	private static PlayerShip playerShip;
		[Inject]
    	private static UserSettings userSettings;

		private static void Postfix() {
			float currentShipBearing = playerShip.transform.eulerAngles.y;
			Global.gyroCompassControler.currentBearingAngle = currentShipBearing;

			float targetBearingAngle;
			Vector2 targetDirection = ShipControlGUIPatchUpdateRudder.playerShip.MobileSandboxGroup.Route.TargetDirection;
			targetBearingAngle = Vector2.Angle(Vector2.up, targetDirection);
			if (targetDirection.x < 0.0f) {
				targetBearingAngle = 360.0f - targetBearingAngle;
			}
			if (!ShipControlGUIPatchUpdateRudder.playerShip.MobileSandboxGroup.Route.IsIndefinite && ShipControlGUIPatchUpdateRudder.playerShip.MobileSandboxGroup.Route.Count > 0) {
				// In this case the targetBearingAngle is relative to the current bearing
				// Because the targetDirection is transformed into the group transform space
				// So Vector2.up is the uboat forward
				targetBearingAngle += currentShipBearing;
				targetBearingAngle = Mathf.Repeat(targetBearingAngle, 360.0f);
			}
			Global.gyroCompassControler.targetBearingAngle = targetBearingAngle;

			Vector2 mouseVector = (Vector2) (UnityEngine.Input.mousePosition - Global.gyroCompassControler.externalDisk.position);
			float controlerRadius = 100.0f * ShipControlGUIPatchUpdateRudder.userSettings.ControlSettings.uiScale;
			if (mouseVector.magnitude < controlerRadius) {
				Global.gyroCompassControler.selectedBearingAngleEnabled = true;
				float selectedBearing = Vector2.Angle(Vector2.up, mouseVector);
				if (mouseVector.x < 0.0f) {
					selectedBearing = 360.0f - selectedBearing;
				}
				selectedBearing += currentShipBearing;
				selectedBearing = Mathf.Repeat(selectedBearing, 360.0f);
				Global.gyroCompassControler.selectedBearingAngle = selectedBearing;

				if (UnityEngine.Input.GetMouseButtonDown(0)) {
					// Vector3.forward is north oriented
					Quaternion rotation = Quaternion.Euler(0.0f, selectedBearing, 0.0f);
					ShipControlGUIPatchUpdateRudder.playerShip.MobileSandboxGroup.Route.TargetDirection = (rotation * Vector3.forward).GetXZ();
				}
			} else {
				Global.gyroCompassControler.selectedBearingAngleEnabled = false;
			}
		}
	}

}
