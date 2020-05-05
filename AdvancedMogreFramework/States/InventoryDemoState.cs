using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Mogre.PhysX;
using Mogre_Procedural.MogreBites;
using MOIS;
using RMOgre;
using Vector3 = Mogre.Vector3;

namespace AdvancedMogreFramework.States
{
    public class InventoryDemoState : AppState
    {
        SceneNode m_pOgreHeadNode;
        Entity m_pOgreHeadEntity;
        MaterialPtr m_pOgreHeadMat;
        MaterialPtr m_pOgreHeadMatHigh;

        const uint SHOW_ENTITY_ON_MAIN_VIEWPORT_FLAG = 1;
        const uint SHOW_ENTITY_ON_OTHER_VIEWPORT_FLAG = 1 << 1; //or 1 << n
        const uint SHOW_ENTITY_ON_ALL_VIEWPORT_FLAG = 0xFFFFFFFF;

        Vector3 CameraPos = new Vector3(0, 1, 3);
        Vector3 lookPos = new Vector3(0, 1, 0);
        uint mask = 1 << 1;
        private TerrainGlobalOptions mTerrainGlobals;
        private TerrainGroup mTerrainGroup;
        private bool mTerrainImported;

        public override void enter()
        {
            AdvancedMogreFramework.Singleton.m_pLog.LogMessage("Entering GameState...");
            AdvancedMogreFramework.lastState = "GameState";
            m_pSceneMgr = AdvancedMogreFramework.Singleton.m_pRoot.CreateSceneManager(SceneType.ST_GENERIC, "GameSceneMgr");
            ColourValue cvAmbineLight = new ColourValue(0.7f, 0.7f, 0.7f);
            m_pSceneMgr.AmbientLight = cvAmbineLight;//(Ogre::ColourValue(0.7f, 0.7f, 0.7f));

            m_pCamera = m_pSceneMgr.CreateCamera("GameCamera");
            Mogre.Vector3 vectCameraPostion = new Mogre.Vector3(5, 60, 60);
            m_pCamera.Position = vectCameraPostion;
            Mogre.Vector3 vectorCameraLookAt = new Mogre.Vector3(5, 20, 0);
            m_pCamera.LookAt(vectorCameraLookAt);
            m_pCamera.NearClipDistance = 5;

            m_pCamera.AspectRatio = AdvancedMogreFramework.Singleton.m_pViewport.ActualWidth / AdvancedMogreFramework.Singleton.m_pViewport.ActualHeight;

            AdvancedMogreFramework.Singleton.m_pViewport.Camera = m_pCamera;

            AdvancedMogreFramework.Singleton.m_pTrayMgr.showFrameStats(TrayLocation.TL_BOTTOMLEFT);
            AdvancedMogreFramework.Singleton.m_pTrayMgr.showLogo(TrayLocation.TL_BOTTOMRIGHT);
            AdvancedMogreFramework.Singleton.m_pTrayMgr.showCursor();

            AdvancedMogreFramework.Singleton.m_pMouse.MouseMoved += mouseMoved;
            AdvancedMogreFramework.Singleton.m_pMouse.MousePressed += mousePressed;
            AdvancedMogreFramework.Singleton.m_pMouse.MouseReleased += mouseReleased;
            AdvancedMogreFramework.Singleton.m_pKeyboard.KeyPressed += keyPressed;
            AdvancedMogreFramework.Singleton.m_pKeyboard.KeyReleased += keyReleased;

            createScene();
        }

        public override void exit()
        {
            AdvancedMogreFramework.Singleton.m_pLog.LogMessage("Leaving GameState...");

            AdvancedMogreFramework.Singleton.m_pMouse.MouseMoved -= mouseMoved;
            AdvancedMogreFramework.Singleton.m_pMouse.MousePressed -= mousePressed;
            AdvancedMogreFramework.Singleton.m_pMouse.MouseReleased -= mouseReleased;
            AdvancedMogreFramework.Singleton.m_pKeyboard.KeyPressed -= keyPressed;
            AdvancedMogreFramework.Singleton.m_pKeyboard.KeyReleased -= keyReleased;

            if (m_pSceneMgr != null)
                m_pSceneMgr.DestroyCamera(m_pCamera);
            AdvancedMogreFramework.Singleton.m_pRoot.DestroySceneManager(m_pSceneMgr);
        }
        public void createScene()
        {
            Mogre.Vector3 vectLightPos = new Mogre.Vector3(75, 75, 75);
            m_pSceneMgr.CreateLight("Light").Position = vectLightPos;//(75, 75, 75);
            
            //DotSceneLoader pDotSceneLoader = new DotSceneLoader();
            //pDotSceneLoader.ParseDotScene("CubeScene.xml", "General", m_pSceneMgr, m_pSceneMgr.RootSceneNode);
            //pDotSceneLoader = null;
            
            //m_pSceneMgr.GetEntity("Cube01").QueryFlags = 1 << 1;
            //m_pSceneMgr.GetEntity("Cube02").QueryFlags = 1 << 1;//(CUBE_MASK);
            //m_pSceneMgr.GetEntity("Cube03").QueryFlags = 1 << 1;//(CUBE_MASK);
            
            m_pOgreHeadEntity = m_pSceneMgr.CreateEntity("Cube", "ogrehead.mesh");
            m_pOgreHeadEntity.QueryFlags = 1 << 0;
            m_pOgreHeadNode = m_pSceneMgr.RootSceneNode.CreateChildSceneNode("CubeNode");
            m_pOgreHeadNode.AttachObject(m_pOgreHeadEntity);
            Mogre.Vector3 vectOgreHeadNodePos = new Mogre.Vector3(0, 0, -25);
            m_pOgreHeadNode.Position = vectOgreHeadNodePos;// (Vector3(0, 0, -25));
            
            m_pOgreHeadMat = m_pOgreHeadEntity.GetSubEntity(1).GetMaterial();
            m_pOgreHeadMatHigh = m_pOgreHeadMat.Clone("OgreHeadMatHigh");
            ColourValue cvAmbinet = new Mogre.ColourValue(1, 0, 0);
            m_pOgreHeadMatHigh.GetTechnique(0).GetPass(0).Ambient = cvAmbinet;
            ColourValue cvDiffuse = new Mogre.ColourValue(1, 0, 0, 0);
            m_pOgreHeadMatHigh.GetTechnique(0).GetPass(0).Diffuse = cvDiffuse;

            Vector3 lightDir = new Vector3(0.55f, -0.3f, 0.75f);
            lightDir.Normalise();

            Light light = m_pSceneMgr.CreateLight("tstLight");
            light.Type = Light.LightTypes.LT_DIRECTIONAL;
            light.Direction = lightDir;
            light.DiffuseColour = ColourValue.White;
            light.SpecularColour = new ColourValue(0.4f, 0.4f, 0.4f);

            m_pSceneMgr.AmbientLight = new ColourValue(0.2f, 0.2f, 0.2f);

            GenerateTerrain(light);
        }

        private void GenerateTerrain(Light light)
        {
            mTerrainGlobals = new TerrainGlobalOptions();
            mTerrainGroup = new TerrainGroup(m_pSceneMgr, Terrain.Alignment.ALIGN_X_Z, 513, 12000.0f);
            mTerrainGroup.SetFilenameConvention("BasicTutorialTerrain3", "dat");
            mTerrainGroup.Origin = Vector3.ZERO;

            ConfigureTerrainDefaults(light);

            for (int x = 0; x <= 0; ++x)
            {
                for (int y = 0; y <= 0; ++y)
                {
                    DefineTerrain(x, y);
                }
            }

            mTerrainGroup.LoadAllTerrains(true);
            if (mTerrainImported)
            {
                foreach (TerrainGroup.TerrainSlot t in mTerrainGroup.GetTerrainIterator())
                {
                    InitBlendMaps(t.instance);
                }
            }
            mTerrainGroup.FreeTemporaryResources();
            mTerrainGroup.SaveAllTerrains(true);
        }

        protected void ConfigureTerrainDefaults(Light light)
        {
            // Configure global
            mTerrainGlobals.MaxPixelError = 8;
            // testing composite map
            mTerrainGlobals.CompositeMapDistance = 3000;

            // Important to set these so that the terrain knows what to use for derived (non-realtime) data
            mTerrainGlobals.LightMapDirection = light.Direction;
            mTerrainGlobals.CompositeMapAmbient = m_pSceneMgr.AmbientLight;
            mTerrainGlobals.CompositeMapDiffuse = light.DiffuseColour;

            // Configure default import settings for if we use imported image
            Terrain.ImportData defaultimp = mTerrainGroup.DefaultImportSettings;

            defaultimp.terrainSize = 513;
            defaultimp.worldSize = 12000.0f; // due terrain.png is 8 bpp
            defaultimp.inputScale = 600;
            defaultimp.minBatchSize = 33;
            defaultimp.maxBatchSize = 65;

            // textures
            defaultimp.layerList.Add(new Terrain.LayerInstance());
            defaultimp.layerList.Add(new Terrain.LayerInstance());
            defaultimp.layerList.Add(new Terrain.LayerInstance());

            defaultimp.layerList[0].worldSize = 100;
            defaultimp.layerList[0].textureNames.Add("dirt_grayrocky_diffusespecular.dds");
            defaultimp.layerList[0].textureNames.Add("dirt_grayrocky_normalheight.dds");

            defaultimp.layerList[1].worldSize = 30;
            defaultimp.layerList[1].textureNames.Add("grass_green-01_diffusespecular.dds");
            defaultimp.layerList[1].textureNames.Add("grass_green-01_normalheight.dds");

            defaultimp.layerList[2].worldSize = 200;
            defaultimp.layerList[2].textureNames.Add("growth_weirdfungus-03_diffusespecular.dds");
            defaultimp.layerList[2].textureNames.Add("growth_weirdfungus-03_normalheight.dds");
        }

        protected void DefineTerrain(int x, int y)
        {
            string filename = mTerrainGroup.GenerateFilename(x, y);

            if (ResourceGroupManager.Singleton.ResourceExists(mTerrainGroup.ResourceGroup, filename))
                mTerrainGroup.DefineTerrain(x, y);
            else
            {
                Image img = new Image();
                GetTerrainImage(x % 2 != 0, y % 2 != 0, img);
                mTerrainGroup.DefineTerrain(x, y, img);
                mTerrainImported = true;
            }
        }
        protected void GetTerrainImage(bool flipX, bool flipY, Image img)
        {
            img.Load("terrain.png", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME);

            if (flipX)
                img.FlipAroundX();

            if (flipY)
                img.FlipAroundY();
        }
        protected unsafe void InitBlendMaps(Terrain terrain)
        {
            TerrainLayerBlendMap blendMap0 = terrain.GetLayerBlendMap(1);
            TerrainLayerBlendMap blendMap1 = terrain.GetLayerBlendMap(2);

            float minHeight0 = 70, minHeight1 = 70;
            float fadeDist0 = 40, fadeDist1 = 15;

            float* pBlend1 = blendMap1.BlendPointer;

            for (int y = 0; y < terrain.LayerBlendMapSize; ++y)
                for (int x = 0; x < terrain.LayerBlendMapSize; ++x)
                {
                    float tx, ty;

                    blendMap0.ConvertImageToTerrainSpace((uint)x, (uint)y, out tx, out ty);

                    float height = terrain.GetHeightAtTerrainPosition(tx, ty);
                    float val = (height - minHeight0) / fadeDist0;
                    val = Clamp(val, 0, 1);

                    val = (height - minHeight1) / fadeDist1;
                    val = Clamp(val, 0, 1);
                    *pBlend1++ = val;
                }

            blendMap0.Dirty();
            blendMap0.Update();
            blendMap1.Dirty();
            blendMap1.Update();
        }
        protected float Clamp(float value, float min, float max)
        {
            if (value <= min)
                return min;
            else if (value >= max)
                return max;

            return value;
        }

        public virtual bool keyPressed(KeyEvent keyEventRef)
        {
            return true;
        }
        public bool keyReleased(KeyEvent keyEventRef)
        {
            return true;
        }

        public bool mouseMoved(MouseEvent evt)
        {
            return true;
        }
        public bool mousePressed(MouseEvent evt, MouseButtonID id)
        {
            return true;
        }
        public bool mouseReleased(MouseEvent evt, MouseButtonID id)
        {
            return true;
        }

        void createCameraLook(string CameraName, string TextureName, string materialName, SceneNode m_pSceneNode)           
        {
            //create a camera look this scenenode(attached entitys)
            Camera pCameraLookMe = m_pSceneMgr.CreateCamera(CameraName);
            pCameraLookMe.Position = (CameraPos);
            pCameraLookMe.LookAt(lookPos);
            pCameraLookMe.SetAutoTracking(true, m_pSceneNode, lookPos);
            pCameraLookMe.FarClipDistance = 0;
            pCameraLookMe.NearClipDistance = (1.0f);
            pCameraLookMe.AutoAspectRatio = (true);
            m_pSceneNode.AttachObject(pCameraLookMe);

            //let's create a rttTex and take the camera
            TexturePtr pRttCameraLookMe = TextureManager.Singleton.CreateManual(TextureName,
                ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME, TextureType.TEX_TYPE_2D,
                128, 128, 0, PixelFormat.PF_R8G8B8, (int)TextureUsage.TU_RENDERTARGET);
            RenderTarget rttTex = pRttCameraLookMe.GetBuffer().GetRenderTarget();

            Viewport pVp = rttTex.AddViewport(pCameraLookMe);
            //alpha background
            pVp.BackgroundColour = new ColourValue(0, 0, 0, 0);
            //i just set the mainRenderWnd's viewport's visibilitymask to 1, 
            //so this viewport need to set 1 << 1, if you add more the mask must 1 << n
            pVp.SetVisibilityMask(mask);
            //this viewport don't need sky,shadow,overlay
            pVp.OverlaysEnabled = (false);
            pVp.SkiesEnabled = (false);
            pVp.ShadowsEnabled = (false);
            //every frame render,or not
            rttTex.IsAutoUpdated = (true);

            MaterialPtr mat = MaterialManager.Singleton.Create(materialName, ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME);
            mat.GetTechnique(0).GetPass(0).CreateTextureUnitState(TextureName);
        }

        public override void update(double timeSinceLastFrame)
        {
            m_FrameEvent.timeSinceLastFrame = (float)timeSinceLastFrame;
            if (AdvancedMogreFramework.Singleton.m_pTrayMgr != null)
            {
                AdvancedMogreFramework.Singleton.m_pTrayMgr.frameRenderingQueued(m_FrameEvent);
            }
        }
    }
}
