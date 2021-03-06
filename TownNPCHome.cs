using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace TownNPCHome
{
    public class TownNPCHome : Mod
    {
        public override void Load() {
            On.Terraria.WorldGen.moveRoom += WorldGen_moveRoom;
        }

        private void WorldGen_moveRoom(On.Terraria.WorldGen.orig_moveRoom orig, int x, int y, int n) {
            orig.Invoke(x, y, n);
            TownEntitiesTeleportToHome(Main.npc[n], Main.npc[n].homeTileX, Main.npc[n].homeTileY);
        }

        internal static void TownEntitiesTeleportToHome(NPC npc, int homeFloorX, int homeFloorY) {
            var targetMethod = npc.GetType().GetMethod("AI_007_TownEntities_TeleportToHome", BindingFlags.Instance | BindingFlags.NonPublic, new Type[] { typeof(int), typeof(int) });
            targetMethod.Invoke(npc, new object[] { homeFloorX, homeFloorY });
        }
    }
}