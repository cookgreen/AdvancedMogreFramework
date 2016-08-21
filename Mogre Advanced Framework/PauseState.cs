using System;
using System.Collections.Generic;
using System.Text;
using Mogre;
using MOIS;
using Mogre_Procedural.MogreBites;

namespace Mogre_Advanced_Framework
{
    class PauseState : AppState
    {
        public PauseState()
        {
            m_bQuit             = false;
            m_bQuestionActive   = false;
            m_FrameEvent        = new FrameEvent();
        }

        public override void enter()
        {
            AdvancedMogreFramework.m_pLog.LogMessage("Entering PauseState...");
 
            m_pSceneMgr = AdvancedMogreFramework.m_pRoot.CreateSceneManager(SceneType.ST_GENERIC, "PauseSceneMgr");
            ColourValue cvAmbineLight=new ColourValue(0.7f,0.7f,0.7f);
            m_pSceneMgr.AmbientLight=cvAmbineLight;
 
            m_pCamera = m_pSceneMgr.CreateCamera("PauseCam");
            Mogre.Vector3 vectCamPos=new Mogre.Vector3(0,25,-50);
            m_pCamera.Position=vectCamPos;
            Mogre.Vector3 vectCamLookAt=new Mogre.Vector3(0,0,0);
            m_pCamera.LookAt(vectCamLookAt);
            m_pCamera.NearClipDistance=1;
 
            m_pCamera.AspectRatio=AdvancedMogreFramework.m_pViewport.ActualWidth /
            AdvancedMogreFramework.m_pViewport.ActualHeight;
 
            AdvancedMogreFramework.m_pViewport.Camera=m_pCamera;

            AdvancedMogreFramework.m_pTrayMgr.destroyAllWidgets();
            AdvancedMogreFramework.m_pTrayMgr.showCursor();
            AdvancedMogreFramework.m_pTrayMgr.createButton(TrayLocation.TL_CENTER, "BackToGameBtn", "Return to GameState", 250);
            AdvancedMogreFramework.m_pTrayMgr.createButton(TrayLocation.TL_CENTER, "BackToMenuBtn", "Return to Menu", 250);
            AdvancedMogreFramework.m_pTrayMgr.createButton(TrayLocation.TL_CENTER, "ExitBtn", "Exit AdvancedOgreFramework", 250);
            AdvancedMogreFramework.m_pTrayMgr.createLabel(TrayLocation.TL_TOP, "PauseLbl", "Pause mode", 250);
 
            m_bQuit = false;
 
            createScene();
        }
        public void createScene()
        { }
        public override void exit()
        {
            AdvancedMogreFramework.m_pLog.LogMessage("Leaving PauseState...");
 
            m_pSceneMgr.DestroyCamera(m_pCamera);
            if(m_pSceneMgr!=null)
                AdvancedMogreFramework.m_pRoot.DestroySceneManager(m_pSceneMgr);

            AdvancedMogreFramework.m_pTrayMgr.clearAllTrays();
            AdvancedMogreFramework.m_pTrayMgr.destroyAllWidgets();
            AdvancedMogreFramework.m_pTrayMgr.setListener(null);
        }

        public bool keyPressed(KeyEvent keyEventRef)
        {
            if(AdvancedMogreFramework.m_pKeyboard.IsKeyDown(KeyCode.KC_ESCAPE) && !m_bQuestionActive)
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
            if(button.getName() == "ExitBtn")
            {
                AdvancedMogreFramework.m_pTrayMgr.showYesNoDialog("Sure?", "Really leave?");
                m_bQuestionActive = true;
            }
            else if(button.getName() == "BackToGameBtn")
            {
                popAllAndPushAppState(findByName("GameState"));
                m_bQuit = true;
            }
            else if(button.getName() == "BackToMenuBtn")
                popAllAndPushAppState(findByName("MenuState"));
        }
        public override void yesNoDialogClosed(string question, bool yesHit)
        {
            if(yesHit == true)
                shutdown();
            else
                AdvancedMogreFramework.m_pTrayMgr.closeDialog();
 
            m_bQuestionActive = false;
        }

        public override void update(double timeSinceLastFrame)
        {
            m_FrameEvent.timeSinceLastFrame = (float)timeSinceLastFrame;
            AdvancedMogreFramework.m_pTrayMgr.frameRenderingQueued(m_FrameEvent);
 
            if(m_bQuit == true)
            {
                popAppState();
                return;
            }
        }

        private bool m_bQuit;
        private bool m_bQuestionActive;
    }
}
