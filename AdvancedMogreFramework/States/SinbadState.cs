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
using System.Linq;
using System.Text;
using Mogre;
using Mogre.PhysX;
using MOIS;
using Mogre_Procedural.MogreBites;
using Mogre_Procedural.MogreBites.Addons;
using org.critterai.nav;
using AdvancedMogreFramework.Helper;

namespace AdvancedMogreFramework.States
{
    class SinbadState : AppState
    {
	    ParamsPanel		m_pDetailsPanel;   		// sample details panel
	    bool						m_bQuit;
	    //bool						m_pCursorWasVisible;		// was cursor visible before dialog appeared
	    bool						m_pDragLook = false;              // click and drag to free-look
        public SdkCameraMan	m_pCameraMan;
	    SinbadCharacterController	m_pChara;
	    NameValuePairList		mInfo=new NameValuePairList();    // custom sample info
        public List<SinbadCharacterController> agents;
        private Physics physics;
        private Scene physicsScene;
        private List<ActorNode> actorNodeList;
        public SinbadState()
        {
            m_bQuit = false;
            m_pDetailsPanel = null;
            mCamera = null;
            m_pCameraMan = null;
            m_pChara = null;
            agents = new List<SinbadCharacterController>();
            actorNodeList = new List<ActorNode>();
        }

        public override void Enter()
        {
            AdvancedMogreFramework.Instance.mLog.LogMessage("Entering SinbadState...");
            AdvancedMogreFramework.lastState = "SinbadState";
            mSceneMgr = AdvancedMogreFramework.Instance.mRoot.CreateSceneManager(SceneType.ST_GENERIC, "SinbadSceneMgr");

            mCamera = mSceneMgr.CreateCamera("MainCamera");
	        AdvancedMogreFramework.Instance.mViewport.Camera=mCamera;
            mCamera.AspectRatio = (float)AdvancedMogreFramework.Instance.mViewport.ActualWidth / (float)AdvancedMogreFramework.Instance.mViewport.ActualHeight;
	        mCamera.NearClipDistance=5;

	        m_pCameraMan = new SdkCameraMan(mCamera);

            AdvancedMogreFramework.Instance.mMouse.MouseMoved += mouseMoved;
            AdvancedMogreFramework.Instance.mMouse.MousePressed += mousePressed;
            AdvancedMogreFramework.Instance.mMouse.MouseReleased += mouseReleased;
            AdvancedMogreFramework.Instance.mKeyboard.KeyPressed += keyPressed;
            AdvancedMogreFramework.Instance.mKeyboard.KeyReleased += keyReleased;

            AdvancedMogreFramework.Instance.mRoot.FrameRenderingQueued += FrameRenderingQueued;

            buildGUI();

            physics = Physics.Create();
            SceneDesc physicsSceneDesc = new SceneDesc();
            physicsSceneDesc.Gravity = new Mogre.Vector3(0.0f, -9.8f, 0.0f);
            physicsSceneDesc.UpAxis = 1;
            physicsScene = physics.CreateScene(physicsSceneDesc);
            physicsScene.Materials[0].Restitution = 0.5f;
            physicsScene.Materials[0].StaticFriction = 0.5f;
            physicsScene.Materials[0].DynamicFriction = 0.5f;
            physicsScene.Simulate(0);

            createScene();
        }
        public void createScene()
        {
            // set background and some fog
	        AdvancedMogreFramework.Instance.mViewport.BackgroundColour=new ColourValue(1.0f, 1.0f, 0.8f);
	        mSceneMgr.SetFog(FogMode.FOG_LINEAR, new ColourValue(1.0f, 1.0f, 0.8f), 0, 15, 100);

	        // set shadow properties
	        mSceneMgr.ShadowTechnique=ShadowTechnique.SHADOWTYPE_TEXTURE_MODULATIVE;
	        mSceneMgr.ShadowColour=new ColourValue(0.5f, 0.5f, 0.5f);
	        mSceneMgr.SetShadowTextureSize(1024);
	        mSceneMgr.ShadowTextureCount=1;

	        // disable default camera control so the character can do its own
	        m_pCameraMan.setStyle(CameraStyle.CS_MANUAL);
	        // use a small amount of ambient lighting
	        mSceneMgr.AmbientLight=new ColourValue(0.3f, 0.3f, 0.3f);

	        // add a bright light above the scene
	        Light light = mSceneMgr.CreateLight();
	        light.Type=(Light.LightTypes. LT_POINT);
	        light.Position=new Mogre.Vector3(-10, 40, 20);
	        light.SpecularColour=ColourValue.White;

	        // create a floor mesh resource
	        MeshManager.Singleton.CreatePlane("floor", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME,
		        new Plane(Mogre.Vector3.UNIT_Y, 0), 100, 100, 10, 10, true, 1, 10, 10, Mogre.Vector3.UNIT_Z);

            // create a floor entity, give it a material, and place it at the origin
            SceneProp floorSceneProp = new SceneProp(
                this,
                mSceneMgr,
                mSceneMgr.RootSceneNode,
                physicsScene,
                "Floor",
                "floor"
                );
            floorSceneProp.SetMaterialName("Examples/Rockwall");

            //Navmesh
            Navmesh floorNavMesh = MeshToNavmesh.LoadNavmesh(floorSceneProp.Entity); 
            NavmeshQuery query;
            NavmeshPoint retStartPoint;
            NavmeshPoint retEndPoint;
            org.critterai.Vector3 pointStart = new org.critterai.Vector3(0, 0, 0);
            org.critterai.Vector3 pointEnd = new org.critterai.Vector3(0, 0, 0);
            org.critterai.Vector3 extents = new org.critterai.Vector3(2, 2, 2);

            NavStatus status = NavmeshQuery.Create(floorNavMesh, 100, out query);
            Console.WriteLine("Status returned when NavmeshQuery was built: " + status);

            NavmeshQueryFilter filter = new NavmeshQueryFilter();
            filter.IncludeFlags = 1;

            status = query.GetNearestPoint(pointStart, extents,filter, out retStartPoint);
            Console.WriteLine("\nStatus of startPoint GetNearestPoint: " + status);
            status = query.GetNearestPoint(pointEnd, extents, filter, out retEndPoint);
            Console.WriteLine("\nStatus of endPoint GetNearestPoint: " + status);

            uint[] path = new uint[100];
            int pathCount;

            status = query.FindPath(retStartPoint, retEndPoint, filter, path, out pathCount);
            Console.WriteLine("\nStatus of Find path: " + status);

            // create our character controller
            m_pChara = new SinbadCharacterController(this, physicsScene, mCamera,new Mogre.Vector3(0,5,0), 0);
            SinbadCharacterController bot1 = new SinbadCharacterController(this, physicsScene, mCamera, new Mogre.Vector3(-10, 5, 0), 1, false);
            SinbadCharacterController bot2 = new SinbadCharacterController(this, physicsScene, mCamera, new Mogre.Vector3(0, 5, -10), 2, false);
            SinbadCharacterController bot3 = new SinbadCharacterController(this, physicsScene, mCamera, new Mogre.Vector3(10, 5, 0), 3, false);
            agents.Add(m_pChara);
            agents.Add(bot1);
            agents.Add(bot2);
            agents.Add(bot3);

	        AdvancedMogreFramework.Instance.mTrayMgr.toggleAdvancedFrameStats();

	        StringVector items=new StringVector();
	        items.Insert(items.Count,"Help");
	        ParamsPanel help = AdvancedMogreFramework.Instance.mTrayMgr.createParamsPanel(TrayLocation. TL_TOPLEFT, "HelpMessage", 100, items);
	        help.setParamValue("Help", "H / F1");
        }
        public override void Exit()
        {
            AdvancedMogreFramework.Instance.mLog.LogMessage("Leaving SinbadState...");

            AdvancedMogreFramework.Instance.mMouse.MouseMoved -= mouseMoved;
            AdvancedMogreFramework.Instance.mMouse.MousePressed -= mousePressed;
            AdvancedMogreFramework.Instance.mMouse.MouseReleased -= mouseReleased;
            AdvancedMogreFramework.Instance.mKeyboard.KeyPressed -= keyPressed;
            AdvancedMogreFramework.Instance.mKeyboard.KeyReleased -= keyReleased;
            AdvancedMogreFramework.Instance.mRoot.FrameRenderingQueued -= FrameRenderingQueued;

            mSceneMgr.DestroyCamera(mCamera);
	        if (m_pCameraMan!=null) m_pCameraMan=null;
	
            if(mSceneMgr!=null)
                AdvancedMogreFramework.Instance.mRoot.DestroySceneManager(mSceneMgr);
        }
        public override bool Pause()
        {
            AdvancedMogreFramework.Instance.mLog.LogMessage("Pausing SinbadState...");
            return true;
        }
        public override void Resume()
        {
            AdvancedMogreFramework.Instance.mLog.LogMessage("Resuming SinbadState...");
 
            buildGUI();

            AdvancedMogreFramework.Instance.mViewport.Camera=mCamera;
            m_bQuit = false;
        }

        void buildGUI()
        {
            AdvancedMogreFramework.Instance.mTrayMgr.showFrameStats(TrayLocation.TL_BOTTOMLEFT);
	        AdvancedMogreFramework.Instance.mTrayMgr.showLogo(TrayLocation.TL_BOTTOMRIGHT);
	        AdvancedMogreFramework.Instance.mTrayMgr.createLabel(TrayLocation.TL_TOP, "GameLbl", "Game mode", 250);
	        AdvancedMogreFramework.Instance.mTrayMgr.showCursor();


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

            m_pDetailsPanel = AdvancedMogreFramework.Instance.mTrayMgr.createParamsPanel(TrayLocation.TL_NONE, "DetailsPanel", 200, items);
	        m_pDetailsPanel.hide();

	        m_pDetailsPanel.setParamValue(9, "Bilinear");
	        m_pDetailsPanel.setParamValue(10, "Solid");
        }

        public bool keyPressed(KeyEvent evt)
        {
            if (!AdvancedMogreFramework.Instance.mTrayMgr.isDialogVisible()) m_pChara.injectKeyDown(evt);
	        if(AdvancedMogreFramework.Instance.mKeyboard.IsKeyDown(KeyCode.KC_ESCAPE))
            {
                pushAppState(findByName("PauseState"));
                return true;
            }

	        if (evt.key == KeyCode.KC_H || evt.key == KeyCode.KC_F1)   // toggle visibility of help dialog
			{
				if (!AdvancedMogreFramework.Instance.mTrayMgr.isDialogVisible() && mInfo["Help"] != "") AdvancedMogreFramework.Instance.mTrayMgr.showOkDialog("Help", mInfo["Help"]);
				else AdvancedMogreFramework.Instance.mTrayMgr.closeDialog();
			}

			if (AdvancedMogreFramework.Instance.mTrayMgr.isDialogVisible()) return true;   // don't process any more keys if dialog is up

			if (evt.key == KeyCode.KC_F)   // toggle visibility of advanced frame stats
			{
				AdvancedMogreFramework.Instance.mTrayMgr.toggleAdvancedFrameStats();
			}
			else if (evt.key == KeyCode.KC_G)   // toggle visibility of even rarer debugging details
			{
				if (m_pDetailsPanel.getTrayLocation() == TrayLocation.TL_NONE)
				{
					AdvancedMogreFramework.Instance.mTrayMgr.moveWidgetToTray(m_pDetailsPanel, TrayLocation.TL_TOPRIGHT, 0);
					m_pDetailsPanel.show();
				}
				else
				{
					AdvancedMogreFramework.Instance.mTrayMgr.removeWidgetFromTray(m_pDetailsPanel);
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

				switch (mCamera.PolygonMode)
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

				mCamera.PolygonMode=pm;
				m_pDetailsPanel.setParamValue(10, newVal);
			}
			else if(evt.key == KeyCode.KC_F5)   // refresh all textures
			{
				TextureManager.Singleton.ReloadAll();
			}
			else if (evt.key == KeyCode.KC_SYSRQ)   // take a screenshot
			{
                AdvancedMogreFramework.Instance.mRenderWnd.WriteContentsToTimestampedFile("screenshot", ".png");
			}
            else if (evt.key == KeyCode.KC_X)
            {
                //spawn a new agent
                SinbadCharacterController agent = new SinbadCharacterController(this,physicsScene, mCamera,m_pChara.Position,agents.Count, false);
                agents.Add(agent);
            }

			m_pCameraMan.injectKeyDown(evt);
			return true;
        }
        public bool keyReleased(KeyEvent keyEventRef)
        {
            for (int i = 0; i < agents.Count; i++)
            {
                agents[i].injectKeyUp(keyEventRef);
            }
	        m_pCameraMan.injectKeyUp(keyEventRef);
            AdvancedMogreFramework.Instance.KeyPressed(keyEventRef);
            return true;
        }

        public bool mouseMoved(MouseEvent arg)
        {
            if (!AdvancedMogreFramework.Instance.mTrayMgr.isDialogVisible())
                for (int i = 0; i < agents.Count; i++)
                {
                    agents[i].injectMouseMove(arg);
                }
            if (AdvancedMogreFramework.Instance.mTrayMgr.injectMouseMove(arg)) return true;
	        m_pCameraMan.injectMouseMove(arg);
  
            return true;
        }
        public bool mousePressed(MouseEvent arg, MouseButtonID id)
        {
            // relay input events to character controller
	        if (!AdvancedMogreFramework.Instance.mTrayMgr.isDialogVisible()) m_pChara.injectMouseDown(arg, id);
	        if (AdvancedMogreFramework.Instance.mTrayMgr.injectMouseDown(arg, id)) return true;
            
	        if (m_pDragLook && id == MouseButtonID.MB_Left)
	        {
		        m_pCameraMan.setStyle(CameraStyle.CS_FREELOOK);
                AdvancedMogreFramework.Instance.mTrayMgr.hideCursor();
	        }

            m_pCameraMan.injectMouseDown(arg, id);
            return true;
        }
        public bool mouseReleased(MouseEvent arg, MouseButtonID id)
        {
            if (AdvancedMogreFramework.Instance.mTrayMgr.injectMouseUp(arg, id)) return true;
            
	        if (m_pDragLook && id == MouseButtonID.MB_Left)
	        {
		        m_pCameraMan.setStyle(CameraStyle.CS_MANUAL);
                AdvancedMogreFramework.Instance.mTrayMgr.showCursor();
	        }

            m_pCameraMan.injectMouseUp(arg, id);

	        return true;
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
            mFrameEvent.timeSinceLastFrame = (int)timeSinceLastFrame;

            AdvancedMogreFramework.Instance.mTrayMgr.frameRenderingQueued(mFrameEvent);

            if(m_bQuit == true)
            {
                popAppState();
                return;
            }
 
	        if (!AdvancedMogreFramework.Instance.mTrayMgr.isDialogVisible())
	        {
		        m_pCameraMan.frameRenderingQueued(mFrameEvent);   // if dialog isn't up, then update the camera

		        if (m_pDetailsPanel.isVisible())   // if details panel is visible, then update its contents
		        {
			        m_pDetailsPanel.setParamValue(0, StringConverter.ToString(mCamera.DerivedPosition.x));
                    m_pDetailsPanel.setParamValue(1, StringConverter.ToString(mCamera.DerivedPosition.y));
                    m_pDetailsPanel.setParamValue(2, StringConverter.ToString(mCamera.DerivedPosition.z));
                    m_pDetailsPanel.setParamValue(4, StringConverter.ToString(mCamera.DerivedOrientation.w));
                    m_pDetailsPanel.setParamValue(5, StringConverter.ToString(mCamera.DerivedOrientation.x));
                    m_pDetailsPanel.setParamValue(6, StringConverter.ToString(mCamera.DerivedOrientation.y));
                    m_pDetailsPanel.setParamValue(7, StringConverter.ToString(mCamera.DerivedOrientation.z));
		        }	
	        }

            for (int i = 0; i < actorNodeList.Count; i++)
            {
                actorNodeList[i].Update((float)timeSinceLastFrame);
            }

            physicsScene.FlushStream();
            physicsScene.FetchResults(SimulationStatuses.AllFinished, true);
            physicsScene.Simulate(timeSinceLastFrame);
        }
        public bool FrameRenderingQueued(FrameEvent evt)
        {
            // let character update animations and camera
            foreach (var agent in agents)
            {
                agent.addTime(evt.timeSinceLastFrame);
            }
            return true;
        }
        public void AddActorNode(ActorNode actorNode)
        {
            actorNodeList.Add(actorNode);
        }
}
}
