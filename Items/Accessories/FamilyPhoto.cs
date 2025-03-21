using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoStands.Items.Accessories
{
    public class FamilyPhoto : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Family Photo");
            Tooltip.SetDefault("Increases damage resistance from a successful stand melee attack");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 25);
            Item.maxStack = 1;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MyPlayer>().familyPhoto = true;
        }
    }
}