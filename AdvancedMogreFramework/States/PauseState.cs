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
            mFrameEvent        = new FrameEvent();
        }

        public override void Enter()
        {
            Framework.Instance.mLog.LogMessage("Entering PauseState...");
            m_bQuit = false;
 
            mSceneMgr = Framework.Instance.mRoot.CreateSceneManager(SceneType.ST_GENERIC, "PauseSceneMgr");
            ColourValue cvAmbineLight=new ColourValue(0.7f,0.7f,0.7f);
            mSceneMgr.AmbientLight=cvAmbineLight;
 
            mCamera = mSceneMgr.CreateCamera("PauseCam");
            Mogre.Vector3 vectCamPos=new Mogre.Vector3(0,25,-50);
            mCamera.Position=vectCamPos;
            Mogre.Vector3 vectCamLookAt=new Mogre.Vector3(0,0,0);
            mCamera.LookAt(vectCamLookAt);
            mCamera.NearClipDistance=1;
 
            mCamera.AspectRatio=Framework.Instance.mViewport.ActualWidth /
            Framework.Instance.mViewport.ActualHeight;
 
            Framework.Instance.mViewport.Camera=mCamera;

            Framework.Instance.mTrayMgr.destroyAllWidgets();
            Framework.Instance.mTrayMgr.showCursor();
            switch(Framework.lastState)
            {
                case "GameState":
                Framework.Instance.mTrayMgr.createButton(TrayLocation.TL_CENTER, "BackToGameBtn", "Return to GameState", 250);
                break;
                case "SinbadState":
                Framework.Instance.mTrayMgr.createButton(TrayLocation.TL_CENTER, "BackToSinbadBtn", "Return to SinbadState", 250);
                break;
            }
            Framework.Instance.mTrayMgr.createButton(TrayLocation.TL_CENTER, "BackToMenuBtn", "Return to Menu", 250);
            Framework.Instance.mTrayMgr.createButton(TrayLocation.TL_CENTER, "ExitBtn", "Exit AdvancedOgreFramework", 250);
            Framework.Instance.mTrayMgr.createLabel(TrayLocation.TL_TOP, "PauseLbl", "Pause mode", 250);

            Framework.Instance.mMouse.MouseMoved += mouseMoved;
            Framework.Instance.mMouse.MousePressed += mousePressed;
            Framework.Instance.mMouse.MouseReleased += mouseReleased;
            Framework.Instance.mKeyboard.KeyPressed += keyPressed;
            Framework.Instance.mKeyboard.KeyReleased += keyReleased;

            m_bQuestionActive = true;
 
            createScene();
        }
        public void createScene()
        { }
        public override void Exit()
        {
            Framework.Instance.mLog.LogMessage("Leaving PauseState...");

            Framework.Instance.mMouse.MouseMoved -= mouseMoved;
            Framework.Instance.mMouse.MousePressed -= mousePressed;
            Framework.Instance.mMouse.MouseReleased -= mouseReleased;
            Framework.Instance.mKeyboard.KeyPressed -= keyPressed;
            Framework.Instance.mKeyboard.KeyReleased -= keyReleased;

            mSceneMgr.DestroyCamera(mCamera);
            if(mSceneMgr!=null)
                Framework.Instance.mRoot.DestroySceneManager(mSceneMgr);

            Framework.Instance.mTrayMgr.clearAllTrays();
            Framework.Instance.mTrayMgr.destroyAllWidgets();
            Framework.Instance.mTrayMgr.setListener(null);
        }

        public bool keyPressed(KeyEvent keyEventRef)
        {
            if(Framework.Instance.mKeyboard.IsKeyDown(KeyCode.KC_ESCAPE) && !m_bQuestionActive)
            {
                m_bQuit = true;
                return true;
            }

            Framework.Instance.KeyPressed(keyEventRef);
 
            return true;
        }
        public bool keyReleased(KeyEvent keyEventRef)
        {
            Framework.Instance.KeyReleased(keyEventRef);
 
            return true;
        }

        public bool mouseMoved(MouseEvent evt)
        {
            if (Framework.Instance.mTrayMgr.injectMouseMove(evt)) return true;
            return true;
        }
        public bool mousePressed(MouseEvent evt, MouseButtonID id)
        {
            if (Framework.Instance.mTrayMgr.injectMouseDown(evt, id)) return true;
            return true;
        }
        public bool mouseReleased(MouseEvent evt, MouseButtonID id)
        {
            if (Framework.Instance.mTrayMgr.injectMouseUp(evt, id)) return true;
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
                Framework.Instance.mTrayMgr.closeDialog();
 
            m_bQuestionActive = false;
        }

        public override void Update(double timeSinceLastFrame)
        {
            mFrameEvent.timeSinceLastFrame = (float)timeSinceLastFrame;
            Framework.Instance.mTrayMgr.frameRenderingQueued(mFrameEvent);
 
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
