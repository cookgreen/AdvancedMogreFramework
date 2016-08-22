using System;
using System.Collections.Generic;
using System.Text;
using Mogre;
using MOIS;
using Mogre_Procedural.MogreBites;

namespace Mogre_Advanced_Framework
{
    class MenuState : AppState
    {
        public MenuState()
        {
            m_bQuit         = false;
            m_FrameEvent    = new FrameEvent();
        }
        public override void enter()
        {
            AdvancedMogreFramework.m_pLog.LogMessage("Entering MenuState...");
            m_bQuit = false;
 
            m_pSceneMgr = AdvancedMogreFramework.m_pRoot.CreateSceneManager(Mogre.SceneType.ST_GENERIC, "MenuSceneMgr");
            ColourValue cvAmbineLight=new ColourValue(0.7f,0.7f,0.7f);
            m_pSceneMgr.AmbientLight=cvAmbineLight;
 
            m_pCamera = m_pSceneMgr.CreateCamera("MenuCam");
            m_pCamera.SetPosition(0,25,-50);
            Mogre.Vector3 vectorCameraLookat=new Mogre.Vector3(0,0,0);
            m_pCamera.LookAt(vectorCameraLookat);
            m_pCamera.NearClipDistance=1;//setNearClipDistance(1);
 
            m_pCamera.AspectRatio=AdvancedMogreFramework.m_pViewport.ActualWidth / AdvancedMogreFramework.m_pViewport.ActualHeight;
 
            AdvancedMogreFramework.m_pViewport.Camera=m_pCamera;

            AdvancedMogreFramework.m_pTrayMgr.destroyAllWidgets();
            AdvancedMogreFramework.m_pTrayMgr.showFrameStats(TrayLocation.TL_BOTTOMLEFT);
            AdvancedMogreFramework.m_pTrayMgr.showLogo(TrayLocation.TL_BOTTOMRIGHT);
            AdvancedMogreFramework.m_pTrayMgr.showCursor();
            AdvancedMogreFramework.m_pTrayMgr.createButton(TrayLocation.TL_CENTER, "EnterBtn", "Enter GameState", 250);
            AdvancedMogreFramework.m_pTrayMgr.createButton(TrayLocation.TL_CENTER, "ExitBtn", "Exit AdvancedOgreFramework", 250);
            AdvancedMogreFramework.m_pTrayMgr.createLabel(TrayLocation.TL_TOP, "MenuLbl", "Menu mode", 250);

            AdvancedMogreFramework.m_pMouse.MouseMoved += new MouseListener.MouseMovedHandler(mouseMoved);
            AdvancedMogreFramework.m_pMouse.MousePressed += new MouseListener.MousePressedHandler(mousePressed);
            AdvancedMogreFramework.m_pMouse.MouseReleased += new MouseListener.MouseReleasedHandler(mouseReleased);
            AdvancedMogreFramework.m_pKeyboard.KeyPressed += new KeyListener.KeyPressedHandler(keyPressed);
            AdvancedMogreFramework.m_pKeyboard.KeyReleased += new KeyListener.KeyReleasedHandler(keyReleased);
            //AdvancedMogreFramework.m_pKeyboard.KeyPressed += new KeyListener.KeyPressedHandler(keyPressed);
            createScene();
        }
        public void createScene()
        { }
        public override void exit()
        {
            AdvancedMogreFramework.m_pLog.LogMessage("Leaving MenuState...");
 
            m_pSceneMgr.DestroyCamera(m_pCamera);
            if(m_pSceneMgr!=null)
                AdvancedMogreFramework.m_pRoot.DestroySceneManager(m_pSceneMgr);

            AdvancedMogreFramework.m_pTrayMgr.clearAllTrays();
            AdvancedMogreFramework.m_pTrayMgr.destroyAllWidgets();
            AdvancedMogreFramework.m_pTrayMgr.setListener(null);
        }

        public bool keyPressed(KeyEvent keyEventRef)
        {
            if(AdvancedMogreFramework.m_pKeyboard.IsKeyDown(KeyCode.KC_ESCAPE))
            {
                m_bQuit = true;
                return true;
            }

            AdvancedMogreFramework.keyPressed(keyEventRef);
            return true;
        }
        public bool keyReleased(KeyEvent keyEventRef)
        {
            AdvancedMogreFramework.keyReleased(keyEventRef);
            return true;
        }

        public bool mouseMoved(MouseEvent evt)
        {
            if (AdvancedMogreFramework.m_pTrayMgr.injectMouseMove(evt)) return true;
            return true;
        }
        public bool mousePressed(MouseEvent evt, MouseButtonID id)
        {
            if (AdvancedMogreFramework.m_pTrayMgr.injectMouseDown(evt, id)) return true;
            return true;
        }
        public bool mouseReleased(MouseEvent evt, MouseButtonID id)
        {
            if (AdvancedMogreFramework.m_pTrayMgr.injectMouseUp(evt, id)) return true;
            return true;
        }

        public override void buttonHit(Button button)
        {
            if (button.getName() == "ExitBtn")
                m_bQuit = true;
            else if (button.getName() == "EnterBtn")
                changeAppState(findByName("GameState"));
        }

        public override void update(double timeSinceLastFrame)
        {
            m_FrameEvent.timeSinceLastFrame = (float)timeSinceLastFrame;
            AdvancedMogreFramework.m_pTrayMgr.frameRenderingQueued(m_FrameEvent);
 
            if(m_bQuit == true)
            {
                shutdown();
                return;
            }
        }

        protected bool m_bQuit;
    }
}
