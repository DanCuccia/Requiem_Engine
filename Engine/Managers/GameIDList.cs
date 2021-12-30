#pragma warning disable 1591
namespace Engine.Managers
{
    /// <summary>All Major Game ID's are found in here</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class GameIDList
    {
        #region ID LIST

        #region Editor
        public const int Button_Editor_CameraToggle         = 201;
        public const int Button_Editor_SaveLevel            = 202;
        public const int Button_Editor_LoadLevel            = 203;
        public const int Button_Editor_LevelFileToggle      = 204;
        public const int Button_Editor_PlaceNewObject       = 205;
        public const int Button_Editor_Finalize             = 206;
        public const int Button_Editor_DeleteSelected       = 207;
        public const int Button_Editor_ThumbnailScrollLeft  = 208;
        public const int Button_Editor_ThumbnailScrollRight = 209;
        public const int Button_Editor_ScaleXUp             = 210;
        public const int Button_Editor_ScaleYUp             = 211;
        public const int Button_Editor_ScaleZUp             = 212;
        public const int Button_Editor_RotateXUp            = 213;
        public const int Button_Editor_RotateYUp            = 214;
        public const int Button_Editor_RotateZUp            = 215;
        public const int Button_Editor_ScaleXDown           = 216;
        public const int Button_Editor_ScaleYDown           = 217;
        public const int Button_Editor_ScaleZDown           = 218;
        public const int Button_Editor_RotateXDown          = 219;
        public const int Button_Editor_RotateYDown          = 220;
        public const int Button_Editor_RotateZDown          = 221;
        public const int Button_Editor_ResetWorld           = 222;
        public const int Button_Editor_xyzUp                = 223;
        public const int Button_Editor_xyzDown              = 224;
        public const int Button_Editor_rotate45X            = 225;
        public const int Button_Editor_rotate45Y            = 226;
        public const int Button_Editor_rotate45Z            = 227;
        public const int Button_Editor_collision            = 228;
        public const int Button_Editor_logical              = 229;
        public const int Button_Editor_MaterialUp           = 230;
        public const int Button_Editor_MaterialDown         = 231;
        public const int Button_Editor_SelectorActive       = 232;
        public const int Button_Editor_CamPosX              = 233;
        public const int Button_Editor_CamNegX              = 234;
        public const int Button_Editor_CamPosZ              = 235;
        public const int Button_Editor_CamNegZ              = 236;
        public const int Button_Editor_LookDown             = 237;
        public const int Button_LightHud_BulbToggle         = 238;
        public const int Button_Editor_NewLight             = 239;
        public const int Button_Editor_NewTrigger           = 240;
        public const int Button_Editor_CopyObject           = 241;
        public const int Button_Editor_ToggleDepthRender    = 242;
        public const int Button_Editor_NewParticleSystem    = 243;
        public const int Button_MegaHud_UseGravity          = 244;
        public const int Button_Editor_NewEnemyLocation     = 245;
        public const int Button_TriggerHud_Repeatable       = 256;
        #endregion Editor

        #region Particle Systems
        public const int PS_MegaParticle_Smoke              = 401;
        public const int PS_BillBoardParticle_Glitter       = 402;
        public const int PS_BillBoardParticle_Tornado       = 403;
        public const int PS_BillBoardParticle_Lightning     = 404;
        public const int PS_MegaParticle_Opening            = 405;
        public const int PS_MegaParticle_FireBall           = 406;
        public const int PS_BillBoardParticle_Healing       = 407;
        public const int PS_BillBoardParticle_Vampire       = 408;
        public const int PS_BillBoardParticle_StarField     = 409;
        #endregion Particle Systems

        #region Shaders
        public const int Shader_Null                        = 501;
        public const int Shader_Diffuse                     = 503;
        public const int Shader_Normals                     = 504;
        public const int Shader_Water                       = 505;
        public const int Shader_Line                        = 506;
        public const int Shader_NoLightNull                 = 507;
        public const int Shader_AnimatedDiffuse             = 508;
        public const int Shader_AnimatedNormals             = 509;
        public const int Shader_AnimatedNull                = 510;
        public const int Shader_ParallaxOcclusion           = 511;
        public const int Shader_Billboard                   = 512;
        public const int Shader_Depth                       = 513;
        public const int Shader_MegaParticle                = 515;
        public const int Shader_NebulaCloud                 = 516;
        public const int Shader_AnimatedClouds              = 517;
        #endregion Shaders

        #region PreFabs
        public const int PreFab_SpawnPoint                  = 601;
        #endregion PreFabs

        #endregion ID LIST
    }
}
