using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoStands.Projectiles.PlayerStands.Tusk
{
    public class TuskAct4Stand : StandClass
    {
        public override string PoseSoundName => "ItsBeenARoundaboutPath";
        public override string PunchSoundName => "Tusk_Ora";
        public override bool CanUseSaladDye => true;

        public override float MaxDistance => 98f;
        public override int PunchDamage => 162;
        public override int PunchTime => 12;
        public override int HalfStandHeight => 37;
        public override int FistWhoAmI => 0;
        public override int TierNumber => 4;
        public override StandAttackType StandType => StandAttackType.Ranged;
        public override int StandOffset => 20;

        private int goldenRectangleEffectTimer = 256;
        private bool playedSpawnCry = false;

        public override void AI()
        {
            SelectAnimation();
            UpdateStandInfo();
            UpdateStandSync();
            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();

            if (shootCount > 0)
                shootCount--;
            if (player.whoAmI == Main.myPlayer && mPlayer.tuskActNumber == 4)         //Making an owner check cause tuskActNumber isn't in sync with other players, causing TA4 to die for everyone else
                Projectile.timeLeft = 2;

            if (goldenRectangleEffectTimer >= 215)
            {
                if (JoJoStands.SoundsLoaded && !playedSpawnCry)
                {
                    SoundStyle sound = new SoundStyle("JoJoStandsSounds/Sounds/SoundEffects/Chumimiiin", SoundType.Sound);
                    sound.Volume = MyPlayer.ModSoundsVolume;
                    SoundEngine.PlaySound(sound, Projectile.Center);
                    playedSpawnCry = true;
                }
                for (int i = 0; i < Main.rand.Next(4, 6 + 1); i++)
                {
                    Vector2 dustSpeed = Projectile.velocity + new Vector2(Main.rand.NextFloat(-5f, 5f + 1f), Main.rand.NextFloat(-5f, 5f + 1f));
                    Dust.NewDust(Projectile.position - new Vector2(0f, HalfStandHeight), Projectile.width, HalfStandHeight * 2, DustID.IchorTorch, dustSpeed.X, dustSpeed.Y);
                }
            }
            if (goldenRectangleEffectTimer > 0)
                goldenRectangleEffectTimer -= 2;

            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual)
            {
                if (Main.mouseLeft && Projectile.owner == Main.myPlayer)
                {
                    Punch();
                }
                else
                {
                    if (player.whoAmI == Main.myPlayer)
                        attackFrames = false;
                }
                if (!attackFrames)
                {
                    StayBehind();
                }
            }

            else if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Auto)
            {
                NPC target = FindNearestTarget(9f * 16f);
                if (target != null)
                {
                    attackFrames = true;
                    idleFrames = false;
                    PlayPunchSound();

                    Vector2 velocity = (target.position + new Vector2(0f, -4f)) - Projectile.position;
                    velocity.Normalize();
                    Projectile.velocity = velocity * 4f;

                    Projectile.direction = 1;
                    if ((target.position - Projectile.Center).X < 0f)
                        Projectile.direction = -1;

                    Projectile.spriteDirection = Projectile.direction;

                    if (Main.myPlayer == Projectile.owner)
                    {
                        if (shootCount <= 0)
                        {
                            shootCount += newPunchTime;
                            Vector2 shootVel = target.position - Projectile.Center;
                            if (shootVel == Vector2.Zero)
                                shootVel = new Vector2(0f, 1f);

                            shootVel.Normalize();
                            shootVel *= ProjectileSpeed;
                            int projIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ModContent.ProjectileType<Fists>(), newPunchDamage, PunchKnockback, Projectile.owner);
                            Main.projectile[projIndex].netUpdate = true;
                            Main.projectile[projIndex].timeLeft = 3;
                            Projectile.netUpdate = true;
                        }
                    }
                }
                else
                {
                    attackFrames = false;
                    idleFrames = true;
                }
                if (target == null || (!attackFrames && idleFrames))
                {
                    StayBehind();
                }
            }
        }

        private readonly Vector2 rectangleCenterOffset = new Vector2(57f, 36f);

        public override bool PreDrawExtras()
        {
            if (goldenRectangleEffectTimer > 0)
            {
                Vector2 rectangleOffset = Vector2.Zero;
                /*if (Projectile.spriteDirection == 1)
                {
                    rectangleOffset = new Vector2(-30f, 0f);
                }*/
                Main.EntitySpriteDraw((Texture2D)ModContent.Request<Texture2D>("JoJoStands/Extras/GoldenSpinComplete"), ((Projectile.Center + new Vector2(-10 * Projectile.spriteDirection, 0f)) + rectangleOffset) - Main.screenPosition - rectangleCenterOffset, null, Color.White * (((float)goldenRectangleEffectTimer * 3.9215f) / 1000f), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
            return true;
        }

        public override void SelectAnimation()
        {
            if (attackFrames)
            {
                idleFrames = false;
                PlayAnimation("Attack");
            }
            if (idleFrames)
            {
                PlayAnimation("Idle");
            }
        }

        public override void PlayAnimation(string animationName)
        {
            if (Main.netMode != NetmodeID.Server)
                standTexture = GetStandTexture("JoJoStands/Projectiles/PlayerStands/Tusk", "/TuskAct4_" + animationName);

            if (animationName == "Idle")
            {
                AnimateStand(animationName, 4, 12, true);
            }
            if (animationName == "Attack")
            {
                AnimateStand(animationName, 4, newPunchTime, true);
            }
        }
    }
}