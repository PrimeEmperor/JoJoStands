using JoJoStands.Items.Vampire;
using Terraria;

namespace JoJoStands.Buffs.Debuffs
{
    public class Sunburn : JoJoBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sunburn");
            Description.SetDefault("You're burning in the sunlight!");
            Main.debuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void UpdateBuffOnPlayer(Player player)
        {
            VampirePlayer vPlayer = player.GetModPlayer<VampirePlayer>();
            if (player.lifeRegen > 0)
                player.lifeRegen = 0;

            player.lifeRegenTime = (int)(60 * vPlayer.sunburnRegenTimeMultiplier);
            player.lifeRegen -= (int)(60 * vPlayer.sunburnDamageMultiplier);
            player.moveSpeed *= vPlayer.sunburnMoveSpeedMultiplier;

            if (Main.rand.Next(0, 2) == 0)
                Dust.NewDust(player.position, player.width, player.height, 169, player.velocity.X * -0.5f, player.velocity.Y * -0.5f);
        }

        public override void UpdateBuffOnNPC(NPC npc)
        {
            if (npc.lifeRegen > 0)
                npc.lifeRegen = 0;

            npc.lifeRegenExpectedLossPerSecond = 30;
            npc.lifeRegen -= 60;     //losing 30 health
            if (Main.rand.Next(0, 2) == 0)
                Dust.NewDust(npc.position, npc.width, npc.height, 169, npc.velocity.X * -0.5f, npc.velocity.Y * -0.5f);
        }
    }
}