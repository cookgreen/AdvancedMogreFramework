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
using MOIS;
using Mogre_Procedural.MogreBites;

namespace AdvancedMogreFramework.States
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
            AdvancedMogreFramework.Singleton.m_pLog.LogMessage("Entering PauseState...");
            m_bQuit = false;
 
            mSceneMgr = AdvancedMogreFramework.Singleton.m_pRoot.CreateSceneManager(SceneType.ST_GENERIC, "PauseSceneMgr");
            ColourValue cvAmbineLight=new ColourValue(0.7f,0.7f,0.7f);
            mSceneMgr.AmbientLight=cvAmbineLight;
 
            mCamera = mSceneMgr.CreateCamera("PauseCam");
            Mogre.Vector3 vectCamPos=new Mogre.Vector3(0,25,-50);
            mCamera.Position=vectCamPos;
            Mogre.Vector3 vectCamLookAt=new Mogre.Vector3(0,0,0);
            mCamera.LookAt(vectCamLookAt);
            mCamera.NearClipDistance=1;
 
            mCamera.AspectRatio=AdvancedMogreFramework.Singleton.m_pViewport.ActualWidth /
            AdvancedMogreFramework.Singleton.m_pViewport.ActualHeight;
 
            AdvancedMogreFramework.Singleton.m_pViewport.Camera=mCamera;

            AdvancedMogreFramework.Singleton.m_pTrayMgr.destroyAllWidgets();
            AdvancedMogreFramework.Singleton.m_pTrayMgr.showCursor();
            switch(AdvancedMogreFramework.lastState)
            {
                case "GameState":
                AdvancedMogreFramework.Singleton.m_pTrayMgr.createButton(TrayLocation.TL_CENTER, "BackToGameBtn", "Return to GameState", 250);
                break;
                case "SinbadState":
                AdvancedMogreFramework.Singleton.m_pTrayMgr.createButton(TrayLocation.TL_CENTER, "BackToSinbadBtn", "Return to SinbadState", 250);
                break;
            }
            AdvancedMogreFramework.Singleton.m_pTrayMgr.createButton(TrayLocation.TL_CENTER, "BackToMenuBtn", "Return to Menu", 250);
            AdvancedMogreFramework.Singleton.m_pTrayMgr.createButton(TrayLocation.TL_CENTER, "ExitBtn", "Exit AdvancedOgreFramework", 250);
            AdvancedMogreFramework.Singleton.m_pTrayMgr.createLabel(TrayLocation.TL_TOP, "PauseLbl", "Pause mode", 250);

            AdvancedMogreFramework.Singleton.m_pMouse.MouseMoved += mouseMoved;
            AdvancedMogreFramework.Singleton.m_pMouse.MousePressed += mousePressed;
            AdvancedMogreFramework.Singleton.m_pMouse.MouseReleased += mouseReleased;
            AdvancedMogreFramework.Singleton.m_pKeyboard.KeyPressed += keyPressed;
            AdvancedMogreFramework.Singleton.m_pKeyboard.KeyReleased += keyReleased;

            m_bQuestionActive = true;
 
            createScene();
        }
        public void createScene()
        { }
        public override void exit()
        {
            AdvancedMogreFramework.Singleton.m_pLog.LogMessage("Leaving PauseState...");

            AdvancedMogreFramework.Singleton.m_pMouse.MouseMoved -= mouseMoved;
            AdvancedMogreFramework.Singleton.m_pMouse.MousePressed -= mousePressed;
            AdvancedMogreFramework.Singleton.m_pMouse.MouseReleased -= mouseReleased;
            AdvancedMogreFramework.Singleton.m_pKeyboard.KeyPressed -= keyPressed;
            AdvancedMogreFramework.Singleton.m_pKeyboard.KeyReleased -= keyReleased;

            mSceneMgr.DestroyCamera(mCamera);
            if(mSceneMgr!=null)
                AdvancedMogreFramework.Singleton.m_pRoot.DestroySceneManager(mSceneMgr);

            AdvancedMogreFramework.Singleton.m_pTrayMgr.clearAllTrays();
            AdvancedMogreFramework.Singleton.m_pTrayMgr.destroyAllWidgets();
            AdvancedMogreFramework.Singleton.m_pTrayMgr.setListener(null);
        }

        public bool keyPressed(KeyEvent keyEventRef)
        {
            if(AdvancedMogreFramework.Singleton.m_pKeyboard.IsKeyDown(KeyCode.KC_ESCAPE) && !m_bQuestionActive)
            {
                m_bQuit = true;
                return true;
            }

            AdvancedMogreFramework.Singleton.keyPressed(keyEventRef);
 
            return true;
        }
        public bool keyReleased(KeyEvent keyEventRef)
        {
            AdvancedMogreFramework.Singleton.keyReleased(keyEventRef);
 
            return true;
        }

        public bool mouseMoved(MouseEvent evt)
        {
            if (AdvancedMogreFramework.Singleton.m_pTrayMgr.injectMouseMove(evt)) return true;
            return true;
        }
        public bool mousePressed(MouseEvent evt, MouseButtonID id)
        {
            if (AdvancedMogreFramework.Singleton.m_pTrayMgr.injectMouseDown(evt, id)) return true;
            return true;
        }
        public bool mouseReleased(MouseEvent evt, MouseButtonID id)
        {
            if (AdvancedMogreFramework.Singleton.m_pTrayMgr.injectMouseUp(evt, id)) return true;
            return true;
        }

        public override void buttonHit(Button button)
        {
            if(button.getName() == "ExitBtn")
            {
                //AdvancedMogreFramework.m_pTrayMgr.showYesNoDialog("Sure?", "Really leave?");
                //m_bQuestionActive = true;
                shutdown();
            }
            else if(button.getName() == "BackToGameBtn")
            {
                popAllAndPushAppState<PauseState>(findByName("GameState"));
                m_bQuit = true;
            }
            else if (button.getName() == "BackToSinbadBtn")
            {
                popAllAndPushAppState<PauseState>(findByName("SinbadState"));
                m_bQuit = true;
            }
            else if(button.getName() == "BackToMenuBtn")
                popAllAndPushAppState<PauseState>(findByName("MenuState"));
        }
        public override void yesNoDialogClosed(string question, bool yesHit)
        {
            if(yesHit == true)
                shutdown();
            else
                AdvancedMogreFramework.Singleton.m_pTrayMgr.closeDialog();
 
            m_bQuestionActive = false;
        }

        public override void update(double timeSinceLastFrame)
        {
            m_FrameEvent.timeSinceLastFrame = (float)timeSinceLastFrame;
            AdvancedMogreFramework.Singleton.m_pTrayMgr.frameRenderingQueued(m_FrameEvent);
 
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
