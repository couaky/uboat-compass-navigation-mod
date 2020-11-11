using Harmony;
using UBOAT.Game.UI.Map;
using UBOAT.Game.Core.Data;
using UBOAT.ModAPI.Core.InjectionFramework;
using UnityEngine;
using TMPro;

/*
MapLineUIPatch

This harmony patch improve the MapLineUI to display distances with a floating precision and the line bearing
*/

namespace UBOAT.Mods.Couaky.CompassNavigation {
	[HarmonyPatch(typeof(MapLineUI), "PlaceLine")]
	public class MapLineUIPatch {
		[Inject] private static UserSettings userSettings;

		// Show length with better precision + bearing
		private static void Postfix(ref TextMeshProUGUI ___distanceLabel, ref float ___length, ref MapPointUI ___startPoint, ref MapPointUI ___endPoint) {
			string pachedLength = "";

			// The following code is a new implementation of KilometersToPreferredUnits method
			// to show distance as a float value
			// See the original method source at the end of the file
			float value = ___length * (1f / 1000f);
			switch (MapLineUIPatch.userSettings.GameplaySettings.units) {
				case Units.Nautical:
					value *= 0.5399568f;
					if (value < 1.0f) {
						pachedLength = Mathf.FloorToInt(value * 10f).ToString() + " cables";
					} else {
						pachedLength = value.ToString("F1") + " nmi";
					}
					break;
				default:
					if (value < 1.0f) {
						pachedLength = (Mathf.FloorToInt(value * 100f) * 10).ToString() + " m";
					} else {
						pachedLength = value.ToString("F1") + " km";
					}
					break;
			}
			// End of new KilometersToPreferredUnits

			// Add the bearing of the line
			Vector2 position1 = new Vector2(___startPoint.Position.x, ___startPoint.Position.y + 10.0f);
			Vector2 position2 = ___startPoint.Position;
			Vector2 position3 = ___endPoint.Position;
			Vector2 vector2_1 = position1 - position2;
			Vector2 vector2_2 = position3 - position2;
			float bearing = Mathf.Acos(Vector2.Dot(vector2_1.normalized, vector2_2.normalized)) * 57.29578f;
			if ((position3.x - position2.x) < 0.0f) {
				bearing = 360.0f - bearing;
			}
			string bearingStr = bearing.ToString("F1") + "°";

			___distanceLabel.text = "\n" + pachedLength + " - " + bearingStr;
		}
	}
}

/*
The original method to stringify distances
public static string KilometersToPreferredUnits(float value)
{
	switch (UnitsUtility.userSettings.GameplaySettings.units)
	{
	case Units.Nautical:
		value *= 0.5399568f;
		int num1 = Mathf.FloorToInt(value);
		return num1 != 0 ? num1.ToString() + " nmi" : Mathf.FloorToInt(value * 10f).ToString() + " cables";
	default:
		int num2 = Mathf.FloorToInt(value);
		return num2 != 0 ? num2.ToString() + " km" : (Mathf.FloorToInt(value * 100f) * 10).ToString() + " m";
	}
}
*/
