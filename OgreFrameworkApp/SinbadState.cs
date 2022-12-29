/*
-----------------------------------------------------------------------------
This source file is part of OgreFramework
For the latest info, see https://github.com/cookgreen/OgreFramework
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
using System.Linq;
using System.Text;
using Mogre;
using MOIS;
using Mogre_Procedural.MogreBites;
using Mogre_Procedural.MogreBites.Addons;
using org.ogre.framework;

namespace org.ogre.framework.app
{
    class SinbadState : AppState
    {
	    ParamsPanel		m_pDetailsPanel;   		// sample details panel
	    bool						m_bQuit;
	    bool						m_pCursorWasVisible;		// was cursor visible before dialog appeared
	    bool						m_pDragLook;              // click and drag to free-look
        public SdkCameraMan	m_pCameraMan;
	    SinbadCharacterController	sinbadMainCharater;
	    NameValuePairList		mInfo=new NameValuePairList();    // custom sample info
        public SinbadState()
        {
            m_bQuit = false;
            m_pDetailsPanel = null;
            camera = null;
            m_pCameraMan = null;
            sinbadMainCharater = null;
        }

        public override void Enter()
        {
            OgreFramework.Instance.log.LogMessage("Entering SinbadState...");
            OgreFramework.lastState = "SinbadState";
            sceneMgr = OgreFramework.Instance.root.CreateSceneManager(SceneType.ST_GENERIC, "SinbadSceneMgr");

            camera = sceneMgr.CreateCamera("MainCamera");
	        OgreFramework.Instance.viewport.Camera = camera;
            camera.AspectRatio = (float)OgreFramework.Instance.viewport.ActualWidth / (float)OgreFramework.Instance.viewport.ActualHeight;
	        camera.NearClipDistance=5;

	        m_pCameraMan = new SdkCameraMan(camera);

            OgreFramework.Instance.mouse.MouseMoved += mouseMoved;
            OgreFramework.Instance.mouse.MousePressed += mousePressed;
            OgreFramework.Instance.mouse.MouseReleased += mouseReleased;
            OgreFramework.Instance.keyboard.KeyPressed += keyPressed;
            OgreFramework.Instance.keyboard.KeyReleased += keyReleased;

            OgreFramework.Instance.root.FrameRenderingQueued += FrameRenderingQueued;

            buildGUI();
 
            createScene();
        }
        public void createScene()
        {
            // set background and some fog
	        OgreFramework.Instance.viewport.BackgroundColour=new ColourValue(1.0f, 1.0f, 0.8f);
	        sceneMgr.SetFog(FogMode.FOG_LINEAR, new ColourValue(1.0f, 1.0f, 0.8f), 0, 15, 100);

            // set shadow properties
            sceneMgr.ShadowTechnique=ShadowTechnique.SHADOWTYPE_TEXTURE_MODULATIVE;
            sceneMgr.ShadowColour=new ColourValue(0.5f, 0.5f, 0.5f);
            sceneMgr.SetShadowTextureSize(1024);
            sceneMgr.ShadowTextureCount=1;

	        // disable default camera control so the character can do its own
	        m_pCameraMan.setStyle(CameraStyle.CS_MANUAL);
            // use a small amount of ambient lighting
            sceneMgr.AmbientLight=new ColourValue(0.3f, 0.3f, 0.3f);

	        // add a bright light above the scene
	        Light light = sceneMgr.CreateLight();
	        light.Type=(Light.LightTypes. LT_POINT);
	        light.Position=new Mogre.Vector3(-10, 40, 20);
	        light.SpecularColour=ColourValue.White;

	        // create a floor mesh resource
	        MeshManager.Singleton.CreatePlane("floor", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME,
		        new Plane(Mogre.Vector3.UNIT_Y, 0), 100, 100, 10, 10, true, 1, 10, 10, Mogre.Vector3.UNIT_Z);

	        // create a floor entity, give it a material, and place it at the origin
            Entity floor = sceneMgr.CreateEntity("Floor", "floor");
            floor.SetMaterialName("Examples/Rockwall");
	        floor.CastShadows=(false);
            sceneMgr.RootSceneNode.AttachObject(floor);

	        // create our character controller
	        sinbadMainCharater = new SinbadCharacterController(camera);

	        OgreFramework.Instance.trayMgr.toggleAdvancedFrameStats();

	        StringVector items=new StringVector();
	        items.Insert(items.Count,"Help");
	        ParamsPanel help = OgreFramework.Instance.trayMgr.createParamsPanel(TrayLocation. TL_TOPLEFT, "HelpMessage", 100, items);
	        help.setParamValue("Help", "H / F1");
        }
        public override void Exit()
        {
            OgreFramework.Instance.log.LogMessage("Leaving SinbadState...");

            OgreFramework.Instance.mouse.MouseMoved -= mouseMoved;
            OgreFramework.Instance.mouse.MousePressed -= mousePressed;
            OgreFramework.Instance.mouse.MouseReleased -= mouseReleased;
            OgreFramework.Instance.keyboard.KeyPressed -= keyPressed;
            OgreFramework.Instance.keyboard.KeyReleased -= keyReleased;
            OgreFramework.Instance.root.FrameRenderingQueued -= FrameRenderingQueued;

            sceneMgr.DestroyCamera(camera);
	        if (m_pCameraMan!=null) m_pCameraMan=null;
	
            if(sceneMgr !=null)
                OgreFramework.Instance.root.DestroySceneManager(sceneMgr);
        }
        public override bool Pause()
        {
            OgreFramework.Instance.log.LogMessage("Pausing SinbadState...");
            return false;
        }
        public override void Resume()
        {
            OgreFramework.Instance.log.LogMessage("Resuming SinbadState...");
 
            buildGUI();

			OgreFramework.Instance.viewport.Camera = camera;
            m_bQuit = false;
        }

        void buildGUI()
        {
            OgreFramework.Instance.trayMgr.showFrameStats(TrayLocation.TL_BOTTOMLEFT);
	        OgreFramework.Instance.trayMgr.showLogo(TrayLocation.TL_BOTTOMRIGHT);
	        OgreFramework.Instance.trayMgr.createLabel(TrayLocation.TL_TOP, "GameLbl", "Game mode", 250);
	        OgreFramework.Instance.trayMgr.showCursor();


            // create a params panel for displaying sample details
	        StringVector items=new StringVector();
	        items.Insert(items.Count,"cam.pX");
	        items.Insert(items.Count,"cam.pY");
	        items.Insert(items.Count,"cam.pZ");
	        items.Insert(items.Count,"");
	        items.Insert(items.Count,"cam.oW");
	        items.Insert(items.Count,"cam.oX");
	        items.Insert(items.Count,"cam.oY");
	        items.Insert(items.Count,"cam.oZ");
	        items.Insert(items.Count,"");
	        items.Insert(items.Count,"Filtering");
	        items.Insert(items.Count,"Poly Mode");

            m_pDetailsPanel = OgreFramework.Instance.trayMgr.createParamsPanel(TrayLocation.TL_NONE, "DetailsPanel", 200, items);
	        m_pDetailsPanel.hide();

	        m_pDetailsPanel.setParamValue(9, "Bilinear");
	        m_pDetailsPanel.setParamValue(10, "Solid");
        }

        public bool keyPressed(KeyEvent evt)
        {
            if (!OgreFramework.Instance.trayMgr.isDialogVisible()) sinbadMainCharater.injectKeyDown(evt);
	        if(OgreFramework.Instance.keyboard.IsKeyDown(KeyCode.KC_ESCAPE))
            {
                pushAppState(findByName("PauseState"));
                return true;
            }

	        if (evt.key == KeyCode.KC_H || evt.key == KeyCode.KC_F1)   // toggle visibility of help dialog
			{
				if (!OgreFramework.Instance.trayMgr.isDialogVisible() && mInfo["Help"] != "") OgreFramework.Instance.trayMgr.showOkDialog("Help", mInfo["Help"]);
				else OgreFramework.Instance.trayMgr.closeDialog();
			}

			if (OgreFramework.Instance.trayMgr.isDialogVisible()) return true;   // don't process any more keys if dialog is up

			if (evt.key == KeyCode.KC_F)   // toggle visibility of advanced frame stats
			{
				OgreFramework.Instance.trayMgr.toggleAdvancedFrameStats();
			}
			else if (evt.key == KeyCode.KC_G)   // toggle visibility of even rarer debugging details
			{
				if (m_pDetailsPanel.getTrayLocation() == TrayLocation.TL_NONE)
				{
					OgreFramework.Instance.trayMgr.moveWidgetToTray(m_pDetailsPanel, TrayLocation.TL_TOPRIGHT, 0);
					m_pDetailsPanel.show();
				}
				else
				{
					OgreFramework.Instance.trayMgr.removeWidgetFromTray(m_pDetailsPanel);
					m_pDetailsPanel.hide();
				}
			}
			else if (evt.key == KeyCode.KC_T)   // cycle polygon rendering mode
			{
				String newVal;
				TextureFilterOptions tfo;
				uint aniso;

                switch (Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(m_pDetailsPanel.getParamValue(9))))
				{
				case "B":
					newVal = "Trilinear";
					tfo = TextureFilterOptions.TFO_TRILINEAR;
					aniso = 1;
					break;
				case "T":
					newVal = "Anisotropic";
					tfo = TextureFilterOptions.TFO_ANISOTROPIC;
					aniso = 8;
					break;
				case "A":
					newVal = "None";
					tfo = TextureFilterOptions.TFO_NONE;
					aniso = 1;
					break;
				default:
					newVal = "Bilinear";
					tfo = TextureFilterOptions.TFO_BILINEAR;
					aniso = 1;
                    break;
				}

				MaterialManager.Singleton.SetDefaultTextureFiltering(tfo);
				MaterialManager.Singleton.DefaultAnisotropy=aniso;
				m_pDetailsPanel.setParamValue(9, newVal);
			}
			else if (evt.key == KeyCode.KC_R)   // cycle polygon rendering mode
			{
				String newVal;
				PolygonMode pm;

				switch (camera.PolygonMode)
				{
				case PolygonMode.PM_SOLID:
					newVal = "Wireframe";
					pm = PolygonMode.PM_WIREFRAME;
					break;
				case PolygonMode.PM_WIREFRAME:
					newVal = "Points";
					pm = PolygonMode.PM_POINTS;
					break;
				default:
					newVal = "Solid";
					pm = PolygonMode.PM_SOLID;
                    break;
				}

				camera.PolygonMode=pm;
				m_pDetailsPanel.setParamValue(10, newVal);
			}
			else if(evt.key == KeyCode.KC_F5)   // refresh all textures
			{
				TextureManager.Singleton.ReloadAll();
			}
			else if (evt.key == KeyCode.KC_SYSRQ)   // take a screenshot
			{
                OgreFramework.Instance.renderWnd.WriteContentsToTimestampedFile("screenshot", ".png");
			}

			m_pCameraMan.injectKeyDown(evt);
			return true;
        }
        public bool keyReleased(KeyEvent keyEventRef)
        {
            sinbadMainCharater.injectKeyUp(keyEventRef);
	        m_pCameraMan.injectKeyUp(keyEventRef);
            //OgreFramework.Singleton.keyPressed(keyEventRef);
            return true;
        }

        public bool mouseMoved(MouseEvent arg)
        {
            if (!OgreFramework.Instance.trayMgr.isDialogVisible()) sinbadMainCharater.injectMouseMove(arg);
            if (OgreFramework.Instance.trayMgr.injectMouseMove(arg)) return true;
	        m_pCameraMan.injectMouseMove(arg);
  
            return true;
        }
        public bool mousePressed(MouseEvent arg, MouseButtonID id)
        {
            // relay input events to character controller
	        if (!OgreFramework.Instance.trayMgr.isDialogVisible()) sinbadMainCharater.injectMouseDown(arg, id);
	        if (OgreFramework.Instance.trayMgr.injectMouseDown(arg, id)) return true;
            
	        if (m_pDragLook && id == MouseButtonID.MB_Left)
	        {
		        m_pCameraMan.setStyle(CameraStyle.CS_FREELOOK);
                OgreFramework.Instance.trayMgr.hideCursor();
	        }

            m_pCameraMan.injectMouseDown(arg, id);
            return true;
        }
        public bool mouseReleased(MouseEvent arg, MouseButtonID id)
        {
            if (OgreFramework.Instance.trayMgr.injectMouseUp(arg, id)) return true;
            
	        if (m_pDragLook && id == MouseButtonID.MB_Left)
	        {
		        m_pCameraMan.setStyle(CameraStyle.CS_MANUAL);
                OgreFramework.Instance.trayMgr.showCursor();
	        }

            m_pCameraMan.injectMouseUp(arg, id);

	        return true;
        }

        public override void Update(double timeSinceLastFrame)
        {
            frameEvent.timeSinceLastFrame = (int)timeSinceLastFrame;

	        //m_pChara.addTime((float)timeSinceLastFrame);

            OgreFramework.Instance.trayMgr.frameRenderingQueued(frameEvent);

            if(m_bQuit == true)
            {
                popAppState();
                return;
            }
        }
        public bool FrameRenderingQueued(FrameEvent evt)
        {
            // let character update animations and camera
            sinbadMainCharater.addTime(evt.timeSinceLastFrame);
            return true;
        }
}
}
