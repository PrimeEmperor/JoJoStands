using JoJoStands.Tiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace JoJoStands.Items.Tiles
{
    public class ShotintheDark : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shot in the Dark");
            Tooltip.SetDefault("`D.Storm`");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 36;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.Blue;
            Item.createTile = ModContent.TileType<ShotintheDarkTile>();
        }
    }
}