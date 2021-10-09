using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoStands.Items
{
    public class TuskAct3 : StandItemClass
    {
        public override int standSpeed => 35;
        public override int standType => 2;
        public override int standTier => 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tusk (ACT 3)");
            Tooltip.SetDefault("Hold left-click to shoot and control a spinning nail and right-click to shoot a slow wormhole to shoot out of!\nSpecial: Create a wormhole to travel in!\nSecond Special: Switch to previous acts!");
        }

        public override void SetDefaults()
        {
            item.damage = 122;
            item.width = 32;
            item.height = 32;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = 5;
            item.maxStack = 1;
            item.knockBack = 2f;
            item.value = 0;
            item.noUseGraphic = true;
            item.rare = ItemRarityID.LightPurple;
        }

        public override bool ManualStandSpawning(Player player)
        {
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();

            mPlayer.standType = 2;
            mPlayer.equippedTuskAct = standTier;
            mPlayer.tuskActNumber = standTier;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType("TuskAct2"));
            recipe.AddIngredient(ItemID.HallowedBar, 11);
            recipe.AddIngredient(mod.ItemType("WillToFight"), 2);
            recipe.AddIngredient(mod.ItemType("WillToProtect"));
            recipe.AddTile(mod.TileType("RemixTableTile"));
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}