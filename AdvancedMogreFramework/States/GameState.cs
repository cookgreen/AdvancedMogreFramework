/*
-----------------------------------------------------------------------------
This source file is part of AdvancedMogreFramework
For the latest info, see https://github.com/cookgreen/AdvancedMogreFramework
Copyright (c) 2016-2020 Cook Green

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Text;
using Mogre;
using RMOgre;
using MOIS;
using Mogre_Procedural.MogreBites;
using Mogre.PhysX;

namespace AdvancedMogreFramework.States
{
    enum QueryFlags
    {
        OGRE_HEAD_MASK = 1 << 0,
        CUBE_MASK = 1 << 1
    };
    public class GameState : AppState
    {
        SceneNode m_pOgreHeadNode;
        Entity m_pOgreHeadEntity;
        MaterialPtr m_pOgreHeadMat;
        MaterialPtr m_pOgreHeadMatHigh;

        ParamsPanel m_pDetailsPanel;
        bool m_bQuit;

        Mogre.Vector3 m_TranslateVector;
        float m_MoveSpeed;
        Degree m_RotateSpeed;
        float m_MoveScale;
        Degree m_RotScale;

        RaySceneQuery m_pRSQ;
        SceneNode m_pCurrentObject;
        Entity m_pCurrentEntity;
        bool m_bRMouseDown, m_bLMouseDown;
        bool m_bSettingsMode;

        private Scene physxScene;
        private Physics physx;
        private bool paused;
        private TerrainGlobalOptions mTerrainGlobals;
        private TerrainGroup mTerrainGroup;
        private bool mTerrainImported;

        public GameState()
        {
            m_MoveSpeed = 0.1f;
            m_RotateSpeed = 0.3f;

            m_bLMouseDown = false;
            m_bRMouseDown = false;
            m_bQuit = false;
            m_bSettingsMode = false;

            m_pDetailsPanel = null;

            physx = Physics.Create();
            SceneDesc desc = new SceneDesc();
            desc.Gravity = new Mogre.Vector3(0, -9.8f, 0);
            physxScene = physx.CreateScene(desc);

            paused = false;
        }

        public override void Enter()
        {
            AdvancedMogreFramework.Instance.mLog.LogMessage("Entering GameState...");
            AdvancedMogreFramework.lastState = "GameState";
            mSceneMgr=AdvancedMogreFramework.Instance.mRoot.CreateSceneManager(SceneType.ST_GENERIC, "GameSceneMgr");
            ColourValue cvAmbineLight=new ColourValue(0.7f,0.7f,0.7f);
            mSceneMgr.AmbientLight=cvAmbineLight;//(Ogre::ColourValue(0.7f, 0.7f, 0.7f));
 
            Ray r=new Ray();
            m_pRSQ = mSceneMgr.CreateRayQuery(r);
            m_pRSQ.QueryMask=1<<0;
 
            mCamera = mSceneMgr.CreateCamera("GameCamera");
            Mogre.Vector3 vectCameraPostion=new Mogre.Vector3(5,60,60);
            mCamera.Position=vectCameraPostion;
            Mogre.Vector3 vectorCameraLookAt=new Mogre.Vector3(5,20,0);
            mCamera.LookAt(vectorCameraLookAt);
            mCamera.NearClipDistance=5;
 
            mCamera.AspectRatio=AdvancedMogreFramework.Instance.mViewport.ActualWidth / AdvancedMogreFramework.Instance.mViewport.ActualHeight;

            AdvancedMogreFramework.Instance.mViewport.Camera=mCamera;
            m_pCurrentObject = null;

 
            buildGUI();
 
            createScene();
        }
        public void createScene()
        {
            Mogre.Vector3 vectLightPos=new Mogre.Vector3(75,75,75);
            var l = mSceneMgr.CreateLight("Light");//(75, 75, 75);
            l.Position = vectLightPos;

            DotSceneLoader pDotSceneLoader = new DotSceneLoader();
            pDotSceneLoader.ParseDotScene("CubeScene.xml", "General", mSceneMgr, mSceneMgr.RootSceneNode);
            pDotSceneLoader=null;

            mSceneMgr.GetEntity("Cube01").QueryFlags = 1 << 1;
            mSceneMgr.GetEntity("Cube02").QueryFlags=1<<1;//(CUBE_MASK);
            mSceneMgr.GetEntity("Cube03").QueryFlags=1<<1;//(CUBE_MASK);

            m_pOgreHeadEntity = mSceneMgr.CreateEntity("Cube", "ogrehead.mesh");
            m_pOgreHeadEntity.QueryFlags=1<<0;
            m_pOgreHeadNode = mSceneMgr.RootSceneNode.CreateChildSceneNode("CubeNode");
            m_pOgreHeadNode.AttachObject(m_pOgreHeadEntity);
            Mogre.Vector3 vectOgreHeadNodePos = new Mogre.Vector3(0,0,-25);
            m_pOgreHeadNode.Position = vectOgreHeadNodePos;// (Vector3(0, 0, -25));

            m_pOgreHeadMat = m_pOgreHeadEntity.GetSubEntity(1).GetMaterial();
            m_pOgreHeadMatHigh = m_pOgreHeadMat.Clone("OgreHeadMatHigh");
            ColourValue cvAmbinet = new Mogre.ColourValue(1, 0, 0);
            m_pOgreHeadMatHigh.GetTechnique(0).GetPass(0).Ambient = cvAmbinet;
            ColourValue cvDiffuse = new Mogre.ColourValue(1, 0, 0,0);
            m_pOgreHeadMatHigh.GetTechnique(0).GetPass(0).Diffuse = cvDiffuse;

            physxScene.Simulate(0);

            Mogre.Vector3 lightDir = new Mogre.Vector3(0.55f, -0.3f, 0.75f);
            lightDir.Normalise();

            //Light light = m_pSceneMgr.CreateLight("tstLight");
            //light.Type = Light.LightTypes.LT_SPOTLIGHT;
            //light.Direction = lightDir;
            //light.DiffuseColour = ColourValue.White;
            //light.SpecularColour = new ColourValue(0.4f, 0.4f, 0.4f);
            //light.Position = new Mogre.Vector3(0, 10, 10);

            mSceneMgr.AmbientLight = new ColourValue(0.2f, 0.2f, 0.2f);

            GenerateTerrain(l);
        }

        private void GenerateTerrain(Light light)
        {
            mTerrainGlobals = new TerrainGlobalOptions();
            mTerrainGroup = new TerrainGroup(mSceneMgr, Terrain.Alignment.ALIGN_X_Z, 513, 12000.0f);
            mTerrainGroup.SetFilenameConvention("BasicTutorialTerrain3", "dat");
            mTerrainGroup.Origin = Mogre.Vector3.ZERO;

            ConfigureTerrainDefaults(light);

            TerrainMaterialGeneratorA.SM2Profile profile = (TerrainMaterialGeneratorA.SM2Profile)mTerrainGlobals.GetDefaultMaterialGenerator().ActiveProfile;
            profile.SetLayerParallaxMappingEnabled(true);
            profile.SetLayerSpecularMappingEnabled(false);
            profile.SetLightmapEnabled(true);
            profile.SetLayerNormalMappingEnabled(true);
            mTerrainGroup.FreeTemporaryResources();

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
            //mTerrainGroup.SaveAllTerrains(true);
        }

        protected void ConfigureTerrainDefaults(Light light)
        {
            // Configure global
            mTerrainGlobals.MaxPixelError = 8;
            // testing composite map
            mTerrainGlobals.CompositeMapDistance = 3000;

            // Important to set these so that the terrain knows what to use for derived (non-realtime) data
            mTerrainGlobals.LightMapDirection = light.Direction;
            mTerrainGlobals.CompositeMapAmbient = mSceneMgr.AmbientLight;
            mTerrainGlobals.CompositeMapDiffuse = light.DiffuseColour;

            mTerrainGlobals.LightMapSize = 1024;

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
        public override void Exit()
        {
            AdvancedMogreFramework.Instance.mLog.LogMessage("Leaving GameState...");

            AdvancedMogreFramework.Instance.mMouse.MouseMoved -= mouseMoved;
            AdvancedMogreFramework.Instance.mMouse.MousePressed -= mousePressed;
            AdvancedMogreFramework.Instance.mMouse.MouseReleased -= mouseReleased;
            AdvancedMogreFramework.Instance.mKeyboard.KeyPressed -= keyPressed;
            AdvancedMogreFramework.Instance.mKeyboard.KeyReleased -= keyReleased;

            if (mSceneMgr!=null)
                mSceneMgr.DestroyCamera(mCamera);
                mSceneMgr.DestroyQuery(m_pRSQ);
                AdvancedMogreFramework.Instance.mRoot.DestroySceneManager(mSceneMgr);
        }
        public override bool Pause()
        {
            AdvancedMogreFramework.Instance.mLog.LogMessage("Pausing GameState...");
            paused = true;
            return true;
        }
        public override void Resume()
        {
            AdvancedMogreFramework.Instance.mLog.LogMessage("Resuming GameState...");
            paused = false;
            buildGUI();

            AdvancedMogreFramework.Instance.mViewport.Camera=mCamera;
            m_bQuit = false;
        }
 
	    public void moveCamera()
        {
            if (mCamera != null)
            {
                if (AdvancedMogreFramework.Instance.mKeyboard.IsKeyDown(KeyCode.KC_LSHIFT))
                    mCamera.MoveRelative(m_TranslateVector);
                mCamera.MoveRelative(m_TranslateVector / 10);
            }
        }
        public void getInput()
        {
            m_TranslateVector = Mogre.Vector3.ZERO;

            if(m_bSettingsMode == false)
            {
                if(AdvancedMogreFramework.Instance.mKeyboard.IsKeyDown(KeyCode.KC_A))
                    m_TranslateVector.x = -2;
 
                if(AdvancedMogreFramework.Instance.mKeyboard.IsKeyDown(KeyCode.KC_D))
                    m_TranslateVector.x = 2;
 
                if(AdvancedMogreFramework.Instance.mKeyboard.IsKeyDown(KeyCode.KC_W))
                    m_TranslateVector.z = -2;
 
                if(AdvancedMogreFramework.Instance.mKeyboard.IsKeyDown(KeyCode.KC_S))
                    m_TranslateVector.z = 2;
 
                if(AdvancedMogreFramework.Instance.mKeyboard.IsKeyDown(KeyCode.KC_Q))
                    m_TranslateVector.y = -2;
 
                if(AdvancedMogreFramework.Instance.mKeyboard.IsKeyDown(KeyCode.KC_E))
                    m_TranslateVector.y = 2;
 
        //camera roll
                if(AdvancedMogreFramework.Instance.mKeyboard.IsKeyDown(KeyCode.KC_Z))
                    mCamera.Roll(new Angle(-10));
 
                if(AdvancedMogreFramework.Instance.mKeyboard.IsKeyDown(KeyCode.KC_X))
                    mCamera.Roll(new Angle(10));
 
        //reset roll
                if (AdvancedMogreFramework.Instance.mKeyboard.IsKeyDown(KeyCode.KC_C))
                    mCamera.Roll(-(mCamera.RealOrientation.Roll));
            }
        }
        public void buildGUI()
        {
            AdvancedMogreFramework.Instance.mTrayMgr.showFrameStats(TrayLocation.TL_BOTTOMLEFT);
            AdvancedMogreFramework.Instance.mTrayMgr.showLogo(TrayLocation.TL_BOTTOMRIGHT);
            AdvancedMogreFramework.Instance.mTrayMgr.createLabel(TrayLocation.TL_TOP, "GameLbl", "Game mode", 250);
            AdvancedMogreFramework.Instance.mTrayMgr.showCursor();
 
            List<string> items=new List<string>();
            items.Insert(items.Count,"cam.pX");
            items.Insert(items.Count,"cam.pY");
            items.Insert(items.Count,"cam.pZ");
            items.Insert(items.Count,"cam.oW");
            items.Insert(items.Count,"cam.oX");
            items.Insert(items.Count,"cam.oY");
            items.Insert(items.Count,"cam.oZ");
            items.Insert(items.Count,"Mode");

            m_pDetailsPanel = AdvancedMogreFramework.Instance.mTrayMgr.createParamsPanel(TrayLocation.TL_TOPLEFT, "DetailsPanel", 200, items.ToArray());
            m_pDetailsPanel.show();
 
            string infoText = "[TAB] - Switch input mode\n\n[W] - Forward / Mode up\n[S] - Backwards/ Mode down\n[A] - Left\n";
            infoText.Insert(infoText.Length,"[D] - Right\n\nPress [SHIFT] to move faster\n\n[O] - Toggle FPS / logo\n");
            infoText.Insert(infoText.Length,"[Print] - Take screenshot\n\n[ESC] - Exit");
            AdvancedMogreFramework.Instance.mTrayMgr.createTextBox(TrayLocation.TL_RIGHT, "InfoPanel", infoText, 300, 220);
 
            StringVector chatModes=new StringVector();
            chatModes.Insert(chatModes.Count,"Solid mode");
            chatModes.Insert(chatModes.Count,"Wireframe mode");
            chatModes.Insert(chatModes.Count,"Point mode");
            AdvancedMogreFramework.Instance.mTrayMgr.createLongSelectMenu(TrayLocation.TL_TOPRIGHT, "ChatModeSelMenu", "ChatMode", 200, 3, chatModes);

            AdvancedMogreFramework.Instance.mMouse.MouseMoved += mouseMoved;
            AdvancedMogreFramework.Instance.mMouse.MousePressed += mousePressed;
            AdvancedMogreFramework.Instance.mMouse.MouseReleased += mouseReleased;
            AdvancedMogreFramework.Instance.mKeyboard.KeyPressed += keyPressed;
            AdvancedMogreFramework.Instance.mKeyboard.KeyReleased += keyReleased;
        }

        public virtual bool keyPressed(KeyEvent keyEventRef)
        {
            if(m_bSettingsMode == true)
            {
                if(AdvancedMogreFramework.Instance.mKeyboard.IsKeyDown(KeyCode.KC_S))
                {
                    SelectMenu pMenu = (SelectMenu)AdvancedMogreFramework.Instance.mTrayMgr.getWidget("ChatModeSelMenu");
                    if(pMenu.getSelectionIndex() + 1 < (int)pMenu.getNumItems())
                        pMenu.selectItem((uint)pMenu.getSelectionIndex() + 1);
                }
 
                if(AdvancedMogreFramework.Instance.mKeyboard.IsKeyDown(KeyCode.KC_W))
                {
                    SelectMenu pMenu = (SelectMenu)AdvancedMogreFramework.Instance.mTrayMgr.getWidget("ChatModeSelMenu");
                    if(pMenu.getSelectionIndex() - 1 >= 0)
                        pMenu.selectItem((uint)pMenu.getSelectionIndex() - 1);
                }
             }
 
            if(AdvancedMogreFramework.Instance.mKeyboard.IsKeyDown(KeyCode.KC_ESCAPE))
            {
                pushAppState(findByName("PauseState"));
                return true;
            }
 
            if(AdvancedMogreFramework.Instance.mKeyboard.IsKeyDown(KeyCode.KC_I))
            {
                if(m_pDetailsPanel.getTrayLocation() == TrayLocation.TL_NONE)
                {
                    AdvancedMogreFramework.Instance.mTrayMgr.moveWidgetToTray(m_pDetailsPanel, TrayLocation.TL_TOPLEFT, 0);
                    m_pDetailsPanel.show();
                }
                else
                {
                    AdvancedMogreFramework.Instance.mTrayMgr.removeWidgetFromTray(m_pDetailsPanel);
                    m_pDetailsPanel.hide();
                }
            }
 
            if(AdvancedMogreFramework.Instance.mKeyboard.IsKeyDown(KeyCode.KC_TAB))
            {
                m_bSettingsMode = !m_bSettingsMode;
                return true;
            }
 
            if(m_bSettingsMode && AdvancedMogreFramework.Instance.mKeyboard.IsKeyDown(KeyCode.KC_RETURN) ||
                AdvancedMogreFramework.Instance.mKeyboard.IsKeyDown(KeyCode.KC_NUMPADENTER))
            {
            }
 
            if(!m_bSettingsMode || (m_bSettingsMode && !AdvancedMogreFramework.Instance.mKeyboard.IsKeyDown(KeyCode.KC_O)))
                AdvancedMogreFramework.Instance.KeyPressed(keyEventRef);
 
                return true;
        }
        public bool keyReleased(KeyEvent keyEventRef)
        {
            AdvancedMogreFramework.Instance.KeyPressed(keyEventRef);
            return true;
        }

        public bool mouseMoved(MouseEvent evt)
        {
            if (AdvancedMogreFramework.Instance.mTrayMgr.injectMouseMove(evt)) return true;
 
            if(m_bRMouseDown)
            {
                Degree deCameraYaw = new Degree(evt.state.X.rel * -0.1f);
                mCamera.Yaw(deCameraYaw);
                Degree deCameraPitch = new Degree(evt.state.Y.rel * -0.1f);
                mCamera.Pitch(deCameraPitch);
            }
 
            return true;
        }
        public bool mousePressed(MouseEvent evt, MouseButtonID id)
        {
            if (AdvancedMogreFramework.Instance.mTrayMgr.injectMouseDown(evt, id)) return true;
 
            if(id == MouseButtonID.MB_Left)
            {
                onLeftPressed(evt);
                m_bLMouseDown = true;
            }
            else if (id == MouseButtonID.MB_Right)
            {
                m_bRMouseDown = true;
            }
 
            return true;
        }
	    public bool mouseReleased(MouseEvent evt, MouseButtonID id)
        {
            if (AdvancedMogreFramework.Instance.mTrayMgr.injectMouseUp(evt, id)) return true;
 
            if(id == MouseButtonID.MB_Left)
            {
                m_bLMouseDown = false;
            }
            else if(id == MouseButtonID.MB_Right)
            {
                m_bRMouseDown = false;
            }
 
            return true;
        }

        public void onLeftPressed(MouseEvent evt)
        {
            if(m_pCurrentObject!=null)
            {
                m_pCurrentObject.ShowBoundingBox=false;
                m_pCurrentEntity.GetSubEntity(1).SetMaterial(m_pOgreHeadMat);
            }
 
            Ray mouseRay = mCamera.GetCameraToViewportRay(AdvancedMogreFramework.Instance.mMouse.MouseState.X.abs / (float)evt.state.width,
            AdvancedMogreFramework.Instance.mMouse.MouseState.Y.abs / (float)evt.state.height);
            if (m_pRSQ == null)
            {
                return;
            }
            m_pRSQ.Ray=mouseRay;
            //m_pRSQ.SortByDistance=true;
 
            RaySceneQueryResult result = m_pRSQ.Execute();
 
            foreach(RaySceneQueryResultEntry itr in result)
            {
                if(itr.movable!=null)
                {
                    if (string.IsNullOrEmpty(itr.movable.Name))
                    {
                        continue;
                    }
                    var ent = mSceneMgr.GetEntity(itr.movable.Name);
                    AdvancedMogreFramework.Instance.mLog.LogMessage("MovableName: " + itr.movable.Name);
                    m_pCurrentObject = ent.ParentSceneNode;
                    AdvancedMogreFramework.Instance.mLog.LogMessage("ObjName " + m_pCurrentObject.Name);
                    m_pCurrentObject.ShowBoundingBox=true;
                    m_pCurrentEntity = mSceneMgr.GetEntity(itr.movable.Name);
                    m_pCurrentEntity.GetSubEntity(1).SetMaterial(m_pOgreHeadMatHigh);
                    break;
                }
            }
        }
        public override void itemSelected(SelectMenu menu)
        {
            switch(menu.getSelectionIndex())
            {
            case 0:
                mCamera.PolygonMode=(PolygonMode.PM_SOLID);break;
            case 1:
                mCamera.PolygonMode=(PolygonMode.PM_WIREFRAME);break;
            case 2:
                mCamera.PolygonMode=(PolygonMode.PM_POINTS);break;
    }
        }

        public override void Update(double timeSinceLastFrame)
        {
            if (physxScene != null && !paused)
            {
                physxScene.FlushStream();
                physxScene.FetchResults(SimulationStatuses.AllFinished, false);
                physxScene.Simulate(timeSinceLastFrame);
            }

            mFrameEvent.timeSinceLastFrame = (float)timeSinceLastFrame;
            if (AdvancedMogreFramework.Instance.mTrayMgr != null)
            {
                AdvancedMogreFramework.Instance.mTrayMgr.frameRenderingQueued(mFrameEvent);
            }
 
            if(m_bQuit == true)
            {
                popAppState();
                return;
            }
            if (AdvancedMogreFramework.Instance.mTrayMgr != null)
            {
                if (!AdvancedMogreFramework.Instance.mTrayMgr.isDialogVisible())
                {
                    if (m_pDetailsPanel!=null && m_pDetailsPanel.isVisible())
                    {
                        m_pDetailsPanel.setParamValue(0, mCamera.DerivedPosition.x.ToString());
                        m_pDetailsPanel.setParamValue(1, mCamera.DerivedPosition.y.ToString());
                        m_pDetailsPanel.setParamValue(2, mCamera.DerivedPosition.z.ToString());
                        m_pDetailsPanel.setParamValue(3, mCamera.DerivedOrientation.w.ToString());
                        m_pDetailsPanel.setParamValue(4, mCamera.DerivedOrientation.x.ToString());
                        m_pDetailsPanel.setParamValue(5, mCamera.DerivedOrientation.y.ToString());
                        m_pDetailsPanel.setParamValue(6, mCamera.DerivedOrientation.z.ToString());
                        if (m_bSettingsMode)
                            m_pDetailsPanel.setParamValue(7, "Buffered Input");
                        else
                            m_pDetailsPanel.setParamValue(7, "Un-Buffered Input");
                    }
                }
            }
 
            m_MoveScale = m_MoveSpeed   * (float)timeSinceLastFrame;
            m_RotScale  = m_RotateSpeed * (float)timeSinceLastFrame;
 
            m_TranslateVector = Mogre.Vector3.ZERO;
 
            getInput();
            moveCamera();
        }
    }
}
