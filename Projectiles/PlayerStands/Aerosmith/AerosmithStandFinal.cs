using JoJoStands.Buffs.Debuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoStands.Projectiles.PlayerStands.Aerosmith
{
    public class AerosmithStandFinal : StandClass   //Aerosmith is special so I'll just leave him the same but with the shader stuff included in the Stand Class
    {
        public override string Texture
        {
            get { return Mod.Name + "/Projectiles/PlayerStands/Aerosmith/Aerosmith"; }
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true;
            Projectile.width = 40;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 0;
            Projectile.ignoreWater = true;
        }

        public override float ProjectileSpeed => 12f;       //how fast the Projectile the minion shoots goes
        public override int ProjectileDamage => 75;
        public override int ShootTime => 8;      //+2 every tier
        public override int TierNumber => 4;
        public override StandAttackType StandType => StandAttackType.Ranged;
        public override string PoseSoundName => "VolareVia";
        public override string SpawnSoundName => "Aerosmith";
        public override bool CanUseRangeIndicators => false;

        private bool bombless = false;
        private bool remoteMode = false;
        private int leftMouse = 0;
        private int rightMouse = 0;
        private int accelerationTimer = 0;
        private SoundEffectInstance aerosmithWhirrSound;
        private const int AccelerationTime = 1 * 60;
        private const float MaxFlightSpeed = 10f;
        private const float WhirrSoundDistance = 98 * 16f;
        private const float AerosmithHoverHeightOffset = 3.5f * 16f;
        public static SoundEffect AerosmithWhirrSoundEffect;


        public override void OnSpawn(IEntitySource source)
        {
            aerosmithWhirrSound = AerosmithWhirrSoundEffect.CreateInstance();
        }

        public override void AI()
        {
            SelectFrame();
            UpdateStandInfo();
            UpdateStandSync();
            if (shootCount > 0)
                shootCount--;

            Player player = Main.player[Projectile.owner];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();

            if (mPlayer.standOut)
                Projectile.timeLeft = 2;
            if (accelerationTimer > 0)
                accelerationTimer--;

            mPlayer.aerosmithWhoAmI = Projectile.whoAmI;
            newProjectileDamage = (int)(newProjectileDamage * MathHelper.Clamp(1f - (Projectile.Distance(player.Center) / (350f * 16f)), 0.5f, 1f));

            Vector2 rota = Projectile.Center - Main.MouseWorld;
            Projectile.rotation = (-rota * Projectile.direction).ToRotation();
            bombless = player.HasBuff(ModContent.BuffType<AbilityCooldown>());
            Projectile.tileCollide = true;      //this is here becaues in AutoMode when Aerosmith goes out too far, it changes to false

            if (Projectile.velocity.X > 0.5f)
                Projectile.spriteDirection = 1;
            if (Projectile.velocity.X < -0.5f)
                Projectile.spriteDirection = -1;

            if (leftMouse > 0)
                leftMouse--;
            if (rightMouse > 0)
                rightMouse--;

            if (remoteMode)
            {
                player.aggro -= 1200;
                float halfScreenWidth = (float)Main.screenWidth / 2f;
                float halfScreenHeight = (float)Main.screenHeight / 2f;
                mPlayer.standRemoteModeCameraPosition = Projectile.Center - new Vector2(halfScreenWidth, halfScreenHeight);
            }

            if (Projectile.owner != Main.myPlayer)
            {
                float playerDistance = Vector2.Distance(Main.player[Main.myPlayer].Center, Projectile.Center);
                if (playerDistance <= WhirrSoundDistance && !SoundEngine.AreSoundsPaused)
                {
                    aerosmithWhirrSound.Volume = (1f - Math.Clamp((playerDistance * 1.4f) / WhirrSoundDistance, 0f, 1f)) * 0.6f;
                    aerosmithWhirrSound.Volume *= Projectile.velocity.Length() / MaxFlightSpeed;
                    aerosmithWhirrSound.Volume *= Main.soundVolume;
                    aerosmithWhirrSound.Pitch = Math.Clamp((Main.player[Main.myPlayer].Center.X - Projectile.Center.X) / WhirrSoundDistance, -1f, 1f);
                    if (aerosmithWhirrSound.State != SoundState.Playing)
                        aerosmithWhirrSound.Play();
                }
                else
                {
                    if (aerosmithWhirrSound.State != SoundState.Stopped)
                        aerosmithWhirrSound.Stop();
                }
            }
            else
            {
                if (!remoteMode)
                {
                    float playerDistance = Vector2.Distance(Main.player[Projectile.owner].Center, Projectile.Center);
                    if (playerDistance <= WhirrSoundDistance && !SoundEngine.AreSoundsPaused)
                    {
                        if (aerosmithWhirrSound.State != SoundState.Playing)
                            aerosmithWhirrSound.Play();

                        aerosmithWhirrSound.Volume = (1f - Math.Clamp(playerDistance / WhirrSoundDistance, 0f, 1f)) * 0.6f;
                        aerosmithWhirrSound.Volume *= Projectile.velocity.Length() / MaxFlightSpeed;
                        aerosmithWhirrSound.Volume *= Main.soundVolume;
                        aerosmithWhirrSound.Pitch = (1f - ((1f - Math.Clamp((playerDistance * 1.4f) / WhirrSoundDistance, 0.6f, 1f)) * 2f)) * 0.4f;
                        float xDifference = Main.player[Main.myPlayer].Center.X - Projectile.Center.X;
                        if ((int)xDifference == 0)
                            xDifference = 1;
                        int relativeDirection = (int)Math.Ceiling(xDifference * 100) / (int)(Math.Abs(xDifference * 100));
                        aerosmithWhirrSound.Pan = Math.Clamp(playerDistance / WhirrSoundDistance, -1f, 1f) * -relativeDirection;
                    }
                    else
                    {
                        if (aerosmithWhirrSound.State != SoundState.Stopped)
                            aerosmithWhirrSound.Stop();
                    }
                }
                else
                {
                    if (!SoundEngine.AreSoundsPaused)
                    {
                        aerosmithWhirrSound.Volume = (Math.Abs(Projectile.velocity.Length()) / MaxFlightSpeed) * 0.6f;
                        aerosmithWhirrSound.Volume *= Main.soundVolume;
                        aerosmithWhirrSound.Pitch = Math.Clamp(((Projectile.velocity.Length() * 2 - MaxFlightSpeed) / 2f) / MaxFlightSpeed, -0.4f, 0.3f);
                        if (aerosmithWhirrSound.State != SoundState.Playing)
                            aerosmithWhirrSound.Play();
                    }
                    else
                        aerosmithWhirrSound.Stop();
                }
            }

            //For Aerosmith the functions are nearly the same so both controls will be bunched up.
            if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Manual || mPlayer.standControlStyle == MyPlayer.StandControlStyle.Remote)
            {
                if (Main.mouseLeft && Projectile.owner == Main.myPlayer)
                {
                    leftMouse = 10;

                    float mouseDistance = Vector2.Distance(Main.MouseWorld, Projectile.Center);

                    Projectile.spriteDirection = Projectile.direction;
                    accelerationTimer += 2;
                    if (accelerationTimer >= AccelerationTime)
                        accelerationTimer = AccelerationTime;

                    if (mouseDistance > 40f)
                    {
                        Projectile.velocity = Main.MouseWorld - Projectile.Center;
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= MaxFlightSpeed + player.moveSpeed;
                        Projectile.velocity *= accelerationTimer / (float)AccelerationTime;
                    }
                    else
                    {
                        Projectile.velocity = Main.MouseWorld - Projectile.Center;
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= (mouseDistance * (MaxFlightSpeed + player.moveSpeed)) / 40f;
                        Projectile.velocity *= accelerationTimer / (float)AccelerationTime;
                    }
                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.rotation = (Projectile.velocity * Projectile.direction).ToRotation();
                    if (remoteMode)
                    {
                        if (Projectile.Distance(player.Center) > 80f && Math.Abs(Projectile.velocity.X) > 0.8f)
                        {
                            Projectile.velocity.X *= 0.95f;
                            Projectile.velocity.Y *= 0.92f;
                            if (Math.Abs(Projectile.velocity.X) <= 0.8f)
                                Projectile.velocity.Y = 0f;
                            Projectile.netUpdate = true;
                        }
                    }
                }
                if (remoteMode && Math.Abs(Projectile.velocity.X) >= 8f && Main.rand.Next(1, 100 + 1) <= 15)
                {
                    int amountOfCloudDusts = Main.rand.Next(1, 2 + 1);
                    for (int i = 0; i < amountOfCloudDusts; i++)
                    {
                        float speedScale = Main.rand.Next(-24, -16 + 1) / 10f;
                        speedScale *= 0.3f;
                        float dustScale = Main.rand.Next(8, 12 + 1) / 10f;
                        int dustIndex = Dust.NewDust(Projectile.Center + new Vector2(16 * Projectile.direction, -8), 32, 16, DustID.Cloud, Projectile.velocity.X * speedScale, 0f, Alpha: 80, Scale: dustScale);
                        Main.dust[dustIndex].noGravity = true;
                    }
                }
                if (Main.mouseRight && Projectile.owner == Main.myPlayer)
                {
                    rightMouse = 10;

                    if (accelerationTimer > (3 * AccelerationTime) / 4)
                        accelerationTimer -= 2;
                    if (mouseX > Projectile.Center.X)
                        Projectile.spriteDirection = 1;
                    if (mouseX < Projectile.Center.X)
                        Projectile.spriteDirection = -1;
                    if (shootCount <= 0)
                    {
                        shootCount += newShootTime;
                        Vector2 shootVel = Main.MouseWorld - Projectile.Center;
                        if (shootVel == Vector2.Zero)
                            shootVel = new Vector2(0f, 1f);

                        shootVel.Normalize();
                        shootVel *= 32f;
                        int projIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ModContent.ProjectileType<StandBullet>(), newProjectileDamage, 3f, Projectile.owner);
                        Main.projectile[projIndex].netUpdate = true;
                        SoundEngine.PlaySound(SoundID.Item11, Projectile.position);
                    }
                }
                if (!remoteMode && leftMouse == 0)
                {
                    if (Projectile.Distance(player.Center) < 12 * 16f)
                    {
                        if (Projectile.Center.X >= player.Center.X + (10 * 16f) || WorldGen.SolidTile((int)(Projectile.Center.X / 16) + 2, (int)(Projectile.Center.Y / 16f) + 1))
                        {
                            Projectile.velocity.X = -2.4f;
                            Projectile.spriteDirection = Projectile.direction = -1;
                            Projectile.netUpdate = true;
                        }
                        if (Projectile.Center.X <= player.Center.X - (10 * 16f) || WorldGen.SolidTile((int)(Projectile.Center.X / 16) - 2, (int)(Projectile.Center.Y / 16f) + 1))
                        {
                            Projectile.velocity.X = 2.4f;
                            Projectile.spriteDirection = Projectile.direction = 1;
                            Projectile.netUpdate = true;
                        }
                        if (Math.Abs(Projectile.velocity.X) > 2.4f)
                            Projectile.velocity.X *= 0.95f;

                        if (Projectile.Center.Y > player.Center.Y - AerosmithHoverHeightOffset - 4 && Projectile.Center.Y < player.Center.Y - AerosmithHoverHeightOffset + 4)
                        {
                            Projectile.velocity.Y = 0f;
                            Projectile.netUpdate = true;
                        }
                        else
                        {
                            Vector2 velocity = (player.Center + new Vector2(0f, -AerosmithHoverHeightOffset)) - Projectile.Center;
                            velocity.Normalize();
                            velocity.Y *= 6f;
                            Projectile.velocity.Y = velocity.Y * ((Vector2.Distance(player.Center + new Vector2(0f, -AerosmithHoverHeightOffset), Projectile.Center)) / (12 * 16f));
                        }
                    }
                    else if (Projectile.Distance(player.Center) > 16 * 16f)
                    {
                        Projectile.tileCollide = false;
                        Projectile.velocity = player.Center - Projectile.Center;
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= MaxFlightSpeed + player.moveSpeed;
                        Projectile.netUpdate = true;
                    }
                    else
                    {
                        Projectile.tileCollide = false;
                        Projectile.velocity = player.Center - Projectile.Center;
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= player.moveSpeed + 6f;
                        Projectile.netUpdate = true;
                    }
                }
                if (SpecialKeyPressed(false) && Projectile.owner == Main.myPlayer)
                {
                    remoteMode = !remoteMode;
                    if (remoteMode)
                    {
                        Main.NewText("Remote Mode: Active");
                        mPlayer.standControlStyle = MyPlayer.StandControlStyle.Remote;
                    }
                    else
                    {
                        Main.NewText("Remote Mode: Disabled");
                        mPlayer.standControlStyle = MyPlayer.StandControlStyle.Manual;
                    }
                }
                if (SecondSpecialKeyPressed(false) && !bombless)
                {
                    shootCount += newShootTime;
                    Projectile.frame = 2;
                    int projIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<AerosmithBomb>(), 0, 3f, Projectile.owner, 568 * (float)mPlayer.standDamageBoosts);
                    Main.projectile[projIndex].netUpdate = true;
                    player.AddBuff(ModContent.BuffType<AbilityCooldown>(), mPlayer.AbilityCooldownTime(5));
                }
            }
            else if (mPlayer.standControlStyle == MyPlayer.StandControlStyle.Auto)
            {
                remoteMode = false;
                Projectile.rotation = (Projectile.velocity * Projectile.direction).ToRotation();
                NPC target = FindNearestTarget(28f * 16f);
                if (target == null)
                {
                    if (Projectile.Distance(player.Center) < 12 * 16f)
                    {
                        if (Projectile.Center.X >= player.Center.X + (10 * 16f) || WorldGen.SolidTile((int)(Projectile.Center.X / 16) + 2, (int)(Projectile.Center.Y / 16f) + 1))
                        {
                            Projectile.velocity.X = -2.4f;
                            Projectile.spriteDirection = Projectile.direction = -1;
                            Projectile.netUpdate = true;
                        }
                        if (Projectile.Center.X <= player.Center.X - (10 * 16f) || WorldGen.SolidTile((int)(Projectile.Center.X / 16) - 2, (int)(Projectile.Center.Y / 16f) + 1))
                        {
                            Projectile.velocity.X = 2.4f;
                            Projectile.spriteDirection = Projectile.direction = 1;
                            Projectile.netUpdate = true;
                        }
                        if (Math.Abs(Projectile.velocity.X) > 2.4f)
                            Projectile.velocity.X *= 0.95f;

                        if (Projectile.Center.Y > player.Center.Y - AerosmithHoverHeightOffset - 16 - 4 && Projectile.Center.Y < player.Center.Y - AerosmithHoverHeightOffset - 16 + 4)
                        {
                            Projectile.velocity.Y = 0f;
                            Projectile.netUpdate = true;
                        }
                        else
                        {
                            Vector2 velocity = (player.Center + new Vector2(0f, -AerosmithHoverHeightOffset - 16)) - Projectile.Center;
                            velocity.Normalize();
                            velocity.Y *= 6f;
                            Projectile.velocity.Y = velocity.Y * ((Vector2.Distance(player.Center + new Vector2(0f, -AerosmithHoverHeightOffset - 16), Projectile.Center)) / (12 * 16f));
                        }
                    }
                    else if (Projectile.Distance(player.Center) > 16 * 16f)
                    {
                        Projectile.tileCollide = false;
                        Projectile.velocity = player.Center - Projectile.Center;
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= MaxFlightSpeed + player.moveSpeed;
                        Projectile.netUpdate = true;
                    }
                    else
                    {
                        Projectile.tileCollide = false;
                        Projectile.velocity = player.Center - Projectile.Center;
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= player.moveSpeed + 6f;
                        Projectile.netUpdate = true;
                    }
                }
                if (target != null)
                {
                    if (Projectile.Distance(target.Center) > 3 * 16f)
                    {
                        Vector2 velocity = target.position - Projectile.Center;
                        velocity.Normalize();
                        velocity *= MaxFlightSpeed;
                        Projectile.velocity = velocity;

                        Projectile.direction = 1;
                        if (Projectile.velocity.X < 0f)
                            Projectile.direction = -1;
                        Projectile.spriteDirection = Projectile.direction;
                        Projectile.netUpdate = true;
                    }

                    if (Projectile.Distance(target.Center) <= 24 * 16f)
                    {
                        Projectile.velocity *= 0.86f;
                        if (shootCount <= 0 && Main.myPlayer == Projectile.owner)
                        {
                            shootCount += newShootTime;
                            Vector2 shootVel = target.Center - Projectile.Center;
                            if (shootVel == Vector2.Zero)
                                shootVel = new Vector2(0f, 1f);

                            shootVel.Normalize();
                            shootVel *= ProjectileSpeed;
                            int projIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, shootVel, ModContent.ProjectileType<StandBullet>(), newProjectileDamage, 3f, Projectile.owner);
                            Main.projectile[projIndex].netUpdate = true;
                            SoundEngine.PlaySound(SoundID.Item11, Projectile.position);
                        }
                    }
                }
            }
        }

        public override void StandKillEffects()
        {
            if (aerosmithWhirrSound != null)
                aerosmithWhirrSound.Stop();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(bombless);
            writer.Write(remoteMode);
            writer.Write(leftMouse);
            writer.Write(rightMouse);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            bombless = reader.ReadBoolean();
            remoteMode = reader.ReadBoolean();
            leftMouse = reader.ReadInt32();
            rightMouse = reader.ReadInt32();
        }

        public void SelectFrame()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frame += 1;
                Projectile.frameCounter = 0;
                if (!bombless)
                {
                    if (Projectile.frame >= 4)
                        Projectile.frame = 2;
                }
                else
                {
                    if (Projectile.frame >= 2)
                        Projectile.frame = 0;
                }
            }
        }
    }
}