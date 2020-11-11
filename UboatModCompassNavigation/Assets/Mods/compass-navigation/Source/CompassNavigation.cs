using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

using Harmony;

using DWS.Common.InjectionFramework;
using DWS.Common.Resources;

using UBOAT.Game.Core.Serialization;
using UBOAT.Game.UI;
using UBOAT.ModAPI;
using UBOAT.ModAPI.Core.InjectionFramework;

/*
This is the entry point of the mod

It just triggers the harmony patches and the injection framework
*/

namespace UBOAT.Mods.Couaky.CompassNavigation {
	[NonSerializedInGameState]
	public class CompassNavigation : IUserMod {

		[Inject] private static GameUI gameUI;
		[Inject] private static ResourceManager resourceManager;

		public void OnLoaded() {
			// logged text can be read in output_log.txt file in LocalLow folder; it should also appear in the in-game console
			Debug.Log("Compass Navigation mod V1.0.2 by Couaky loaded !");

			// tell Harmony to apply patches from this mod
			var harmony = HarmonyInstance.Create("com.couaky.compassnavigation");
			harmony.PatchAll();

			// listen for the main scene loading to spawn custom UI
			SceneEventsListener.OnSceneAwake += SceneEventsListener_OnSceneAwake;
		}

		private void SceneEventsListener_OnSceneAwake(Scene scene) {
			try {
				// it's a workaround for B126 that makes [Inject] tags work more consistently for mods, it won't be necessary in B127
				InjectionFramework.Instance.InjectIntoAssembly(Assembly.GetExecutingAssembly());
			} catch (Exception e) {
				// it's best to catch all exceptions in event listeners; they should preferrably not throw exceptions in C# due to unpredictable results of that
				Debug.LogException(e);
			}
		}
	}
}
