using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace TownNPCHome
{
    internal class QuickHomeInterface : ModSystem
    {
        private int _multiplayerCooldown = 0;

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
            if (Main.playerInventory) {
                int InventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
                if (InventoryIndex != -1) {
                    layers.Insert(InventoryIndex + 1, new LegacyGameInterfaceLayer(
                    "TownNPCHome: Quick Home Feature",
                    delegate {
                        // vanilla source code
                        int mH = 0;
                        if (Main.mapEnabled) {
                            if (!Main.mapFullscreen && Main.mapStyle == 1)
                                mH = 256;
                            if (mH + Main.instance.RecommendedEquipmentAreaPushUp > Main.screenHeight)
                                mH = Main.screenHeight - Main.instance.RecommendedEquipmentAreaPushUp;
                        }
                        int yPos = mH + 142;
                        int accessorySlots = 8 + Main.LocalPlayer.GetAmountOfExtraAccessorySlotsToShow();
                        if (Main.screenHeight < 950 && accessorySlots >= 10) {
                            yPos -= (int)(56f * 0.85f/*Main.inventoryScale*/ * (float)(accessorySlots - 9));
                        }
                        // the position of the "housing" icon
                        Vector2 iconPosition = new(Main.screenWidth - 128, yPos);
                        Vector2 size = TextureAssets.EquipPage[5].Size();
                        if (Collision.CheckAABBvAABBCollision(iconPosition, size, Main.MouseScreen, Vector2.One) && Main.mouseItem.stack < 1) {
                            Main.hoverItemName += Language.ActiveCulture.Name == "zh-Hans" ? "（右键单击以将所有城镇NPC传送回家）" : " (Right click to teleport all town NPCs home)";
                            if (Main.mouseRight && Main.mouseRightRelease && _multiplayerCooldown <= 0) {
                                foreach (var npc in from n in Main.npc where n is not null && n.active && n.townNPC && !n.homeless select n) {
                                    TownNPCHome.TownEntitiesTeleportToHome(npc, npc.homeTileX, npc.homeTileY);
                                }
                                SoundEngine.PlaySound(SoundID.Chat);
                                if (Main.netMode == NetmodeID.MultiplayerClient) {
                                    _multiplayerCooldown = 90; // we don't want a player to crash the server using this.
                                }
                            }
                        }
                        _multiplayerCooldown--;
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
                }
            }
        }
    }
}
