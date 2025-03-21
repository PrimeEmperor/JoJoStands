﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics.Shaders;
using Terraria.UI;

namespace JoJoStands.UI
{
    public class VoidBar : UIState
    {
        public DragableUIPanel voidBarUI;
        public static bool Visible;
        public static Texture2D VoidBarTexture;
        public static Texture2D VoidBarBarTexture;
        private UIText voidText;

        public override void Update(GameTime gameTime)
        {
            MyPlayer mPlayer = Main.player[Main.myPlayer].GetModPlayer<MyPlayer>();
            voidText.SetText("Void: " + mPlayer.voidCounter);

            if (mPlayer.creamTier == 0)
                Visible = false;

            base.Update(gameTime);
        }

        public override void OnInitialize()
        {
            voidBarUI = new DragableUIPanel();
            voidBarUI.HAlign = 0.9f;
            voidBarUI.VAlign = 0.9f;
            voidBarUI.Width.Set(66f * 1.5f, 0f);
            voidBarUI.Height.Set(60f * 1.5f, 0f);
            voidBarUI.BackgroundColor = new Color(0, 0, 0, 0);
            voidBarUI.BorderColor = new Color(0, 0, 0, 0);

            voidText = new UIText("");
            voidText.HAlign = 0.5f;
            voidText.VAlign = 1.4f;
            voidText.Width.Set(22f, 0f);
            voidText.Height.Set(22f, 0f);
            voidBarUI.Append(voidText);

            Append(voidBarUI);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Player player = Main.player[Main.myPlayer];
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();

            spriteBatch.Draw(VoidBarTexture, voidBarUI.GetClippingRectangle(spriteBatch), new Rectangle(0, 0, VoidBarTexture.Width, VoidBarTexture.Height), Color.White);
            float normalizedVoidCounter = 1f - ((float)mPlayer.voidCounter / (float)mPlayer.voidCounterMax);
            MiscShaderData voidGradientShader = JoJoStandsShaders.GetShaderInstance(JoJoStandsShaders.VoidBarGradient);
            voidGradientShader.UseOpacity(normalizedVoidCounter);

            UITools.DrawUIWithShader(spriteBatch, voidGradientShader, VoidBarBarTexture, voidBarUI.GetClippingRectangle(spriteBatch));
        }

        /*private void PreDrawVoidCounterGradient(MyPlayer mPlayer, SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            MiscShaderData voidGradientShader = GameShaders.Misc["JoJoStandsVoidGradient"];
            voidGradientShader.UseOpacity(mPlayer.voidCounter / mPlayer.voidCounterMax);

            voidGradientShader.Apply(null);
            spriteBatch.Draw(VoidBarBarTexture, voidBarUI.GetClippingRectangle(spriteBatch), new Rectangle(0, 0, VoidBarTexture.Width, VoidBarTexture.Height), Color.White);
        }

        private void PostDrawVoidCounterGradient(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }*/
    }
}