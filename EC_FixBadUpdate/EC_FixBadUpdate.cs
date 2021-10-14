using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using HEdit;

namespace EC_FixBadUpdate
{
	[BepInProcess("EmotionCreators")]
	[BepInPlugin(nameof(EC_FixBadUpdate), nameof(EC_FixBadUpdate), VERSION)]
	public class EC_FixBadUpdate : BaseUnityPlugin
	{
		public const string VERSION = "1.0.0";

		private new static ManualLogSource Logger;

		private void Awake()
		{
			Logger = base.Logger;
			
			Harmony.CreateAndPatchAll(typeof(EC_FixBadUpdate));
		}
		
		[HarmonyTranspiler, HarmonyPatch(typeof(HEditGlobal), "CreateChara", typeof(int))]
		public static IEnumerable<CodeInstruction> HEditGlobal_CreateChara_ForceZeroId(IEnumerable<CodeInstruction> instructions)
		{
			var il = instructions.ToList();
            
			var index = il.FindIndex(instruction => instruction.opcode == OpCodes.Ldfld && (instruction.operand as FieldInfo)?.Name == "objCharaParentNull");
			if (index <= 0)
			{
				Logger.Log(LogLevel.Message | LogLevel.Error, "Failed transpiling HEditGlobal_CreateChara_ForceZeroId Ldfld index not found!");
				return il;
			}
    
			il[index + 1] = new CodeInstruction(OpCodes.Ldc_I4_0);

			return il;
		}
	}
}