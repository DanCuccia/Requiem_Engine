using Engine.Drawing_Objects;
using Engine.Game_Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Engine.Math_Physics;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Scenes.HUD
{
    /// <summary>The Hud displayed for Enemy Spawn Points within the level editor</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class EnemySpawnPointHud : HudBase
    {
        EnemySpawnLocation spawnPoint;

        /// <summary>default ctor</summary>
        /// <param name="editor">reference to the level editor</param>
        /// <param name="spawnPoint">reference to the selected enemy spawn location object</param>
        public EnemySpawnPointHud(EditorScene editor, EnemySpawnLocation spawnPoint)
            : base(editor) 
        {
            if (spawnPoint == null)
                throw new System.ArgumentNullException("EnemySpawnPointHud::EnemySpawnPointHud - spawnpoint argument null");
            this.spawnPoint = spawnPoint;
        }

        /// <summary>load the list of buttons</summary>
        /// <param name="content">xna content manager</param>
        protected override void LoadButtons(ContentManager content)
        {
            SpriteButton btn = new SpriteButton();
            btn.Initialize("sprites//editor//LE_levelNameBlank", getSpawnCondition(spawnPoint.SpawnCondition));
            btn.Position = new Vector2(Rect.X + ((Rect.Width - btn.FrameSize.X) * 0.5f),
                Rect.Y + 40);
            btn.setExecution(null, toggleConditionButton);
            AddButton(btn);

            btn = new SpriteButton();
            btn.Initialize("sprites//editor//LE_levelNameBlank", getEnemyAIType(spawnPoint.SpawnType));
            btn.Position = new Vector2(Rect.X + ((Rect.Width - btn.FrameSize.X) * 0.5f),
                Rect.Y + 40 + 52);
            btn.setExecution(null, toggleTypeButton);
            AddButton(btn);
        }

        /// <summary>load the list of sliders</summary>
        /// <param name="content">xna content manager</param>
        protected override void LoadSliders(ContentManager content)
        {
            Slider slider = new Slider(content, new Vector2(Rect.X, Rect.Y + 160f), 256f, "Wait Time", onWaitChange, EnemySpawnLocation.MaxWait - EnemySpawnLocation.MinWait);
            slider.SetValue(MyMath.GetScalarBetween(EnemySpawnLocation.MinWait, EnemySpawnLocation.MaxWait, spawnPoint.MaxWaitMillies));
            AddSlider(slider);

            slider = new Slider(content, new Vector2(Rect.X, Rect.Y + 160 + 72f), 256f, "Max Enemies", onEnemyChange, EnemySpawnLocation.MaxEnemy - EnemySpawnLocation.MinEnemy);
            slider.SetValue(MyMath.GetScalarBetween(EnemySpawnLocation.MinEnemy, EnemySpawnLocation.MaxEnemy, spawnPoint.MaxEnemies));
            AddSlider(slider);
        }

        /// <summary>get the string associated by spawn condition</summary>
        /// <param name="type">spawn condition</param>
        /// <returns>spawn condition string name</returns>
        private string getSpawnCondition(Engine.EnemyEnums.EnemySpawnCondition type)
        {
            switch (type)
            {
                case EnemyEnums.EnemySpawnCondition.SPAWN_MANUAL: return "Manual Spawning";
                case EnemyEnums.EnemySpawnCondition.SPAWN_OFF_CAMERA: return "Off-Screen Spawning";
                case EnemyEnums.EnemySpawnCondition.SPAWN_TIMED: return "Timed Spawning";
                default: return "error";
            }
        }

        /// <summary>get the string associated by spawn type</summary>
        /// <param name="type">spawn type</param>
        /// <returns>spawn type string name</returns>
        private string getEnemyAIType(Engine.EnemyEnums.EnemySpawnLocationType type)
        {
            switch (type)
            {
                case EnemyEnums.EnemySpawnLocationType.LOC_DISTANCE_BOSS: return "Distance - Boss";
                case EnemyEnums.EnemySpawnLocationType.LOC_DISTANCE_MOVING_STANDARD: return "Distance Moving - Standard";
                case EnemyEnums.EnemySpawnLocationType.LOC_DISTANCE_SHOOTING_STANDARD: return "Distance Shooting - Standard";
                case EnemyEnums.EnemySpawnLocationType.LOC_PLAYER_LOCAL_BOSS: return "Player's Axis - Boss";
                case EnemyEnums.EnemySpawnLocationType.LOC_PLAYER_LOCAL_STANDARD: return "Player's Axis - Standard";
                default: return "error";
            }
        }

        /// <summary>Increment the spawnCondition enum and reassign the button's label</summary>
        private void toggleConditionButton()
        {
            int val = (int)spawnPoint.SpawnCondition;
            val++;
            if (val >= (int)Engine.EnemyEnums.EnemySpawnCondition.COUNT)
                val = 0;
            spawnPoint.SpawnCondition = (EnemyEnums.EnemySpawnCondition)val;
            buttonList[0].Label = getSpawnCondition(spawnPoint.SpawnCondition);
        }

        /// <summary>Increment the location enemy type enum and reassign the button's label</summary>
        private void toggleTypeButton()
        {
            int val = (int)spawnPoint.SpawnType;
            val++;
            if (val >= (int)Engine.EnemyEnums.EnemySpawnLocationType.COUNT)
                val = 0;
            spawnPoint.SpawnType = (EnemyEnums.EnemySpawnLocationType)val;
            buttonList[1].Label = getEnemyAIType(spawnPoint.SpawnType);
        }

        /// <summary>optional call for extra rendering</summary>
        /// <param name="batch">xna 2D sprite batch</param>
        protected override void Render2DExtra(SpriteBatch batch)
        {
            batch.DrawString(font, "Spawn Condition", new Vector2(
                Rect.X + (Rect.Width * .5f) - (font.MeasureString("Spawn Location Type").X * .5f), 
                Rect.Y + 20f), Color.White);

            batch.DrawString(font, "Spawned Enemy Type", new Vector2(
                Rect.X + (Rect.Width * .5f) - (font.MeasureString("Spawn Location Type").X * .5f),
                Rect.Y + 75f), Color.White);
        }

        private void onWaitChange()
        {
            spawnPoint.MaxWaitMillies = (int)MyMath.GetValueBetween(EnemySpawnLocation.MinWait, EnemySpawnLocation.MaxWait, sliderList[0].GetValue());
        }

        private void onEnemyChange()
        {
            spawnPoint.MaxEnemies = (int)MyMath.GetValueBetween(EnemySpawnLocation.MinEnemy, EnemySpawnLocation.MaxEnemy, sliderList[1].GetValue());
        }

        /// <summary>load the list of sprites</summary>
        /// <param name="content">xna content manager</param>
        protected override void LoadSprites(ContentManager content) { }
    }
}
