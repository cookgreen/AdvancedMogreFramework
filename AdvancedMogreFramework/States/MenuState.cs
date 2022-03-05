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
    public class MenuState : AppState
    {
        public MenuState()
        {
            m_bQuit         = false;
            mFrameEvent    = new FrameEvent();
        }
        public override void Enter()
        {
            AdvancedMogreFramework.Instance.mLog.LogMessage("Entering MenuState...");
            m_bQuit = false;

            //if (AdvancedMogreFramework.Singleton.m_pVorbis == null)
            //{
            //    AdvancedMogreFramework.Singleton.m_pVorbis = new NAudio.Vorbis.VorbisWaveReader(@".\vivaldi_winter_allegro.ogg");
            //    AdvancedMogreFramework.Singleton.m_pWaveOut = new NAudio.Wave.WaveOut();
            //    AdvancedMogreFramework.Singleton.m_pWaveOut.Init(AdvancedMogreFramework.Singleton.m_pVorbis);
            //    AdvancedMogreFramework.Singleton.m_pWaveOut.Play();
            //}
 
            mSceneMgr = AdvancedMogreFramework.Instance.mRoot.CreateSceneManager(Mogre.SceneType.ST_GENERIC, "MenuSceneMgr");
            ColourValue cvAmbineLight=new ColourValue(0.7f,0.7f,0.7f);
            mSceneMgr.AmbientLight=cvAmbineLight;
 
            mCamera = mSceneMgr.CreateCamera("MenuCam");
            mCamera.SetPosition(0,25,-50);
            Mogre.Vector3 vectorCameraLookat=new Mogre.Vector3(0,0,0);
            mCamera.LookAt(vectorCameraLookat);
            mCamera.NearClipDistance=1;//setNearClipDistance(1);
 
            mCamera.AspectRatio=AdvancedMogreFramework.Instance.mViewport.ActualWidth / AdvancedMogreFramework.Instance.mViewport.ActualHeight;
 
            AdvancedMogreFramework.Instance.mViewport.Camera=mCamera;

            AdvancedMogreFramework.Instance.mTrayMgr.showFrameStats(TrayLocation.TL_BOTTOMLEFT);
            AdvancedMogreFramework.Instance.mTrayMgr.showLogo(TrayLocation.TL_BOTTOMRIGHT);
            AdvancedMogreFramework.Instance.mTrayMgr.showCursor();

            BuildMainMenu();

            AdvancedMogreFramework.Instance.mMouse.MouseMoved += mouseMoved;
            AdvancedMogreFramework.Instance.mMouse.MousePressed += mousePressed;
            AdvancedMogreFramework.Instance.mMouse.MouseReleased += mouseReleased;
            AdvancedMogreFramework.Instance.mKeyboard.KeyPressed += keyPressed;
            AdvancedMogreFramework.Instance.mKeyboard.KeyReleased += keyReleased;
            createScene();
        }

        private void BuildMainMenu()
        {

            AdvancedMogreFramework.Instance.mTrayMgr.destroyAllWidgets();
            AdvancedMogreFramework.Instance.mTrayMgr.createButton(TrayLocation.TL_CENTER, "EnterBtn", "Enter GameState", 250);
            AdvancedMogreFramework.Instance.mTrayMgr.createButton(TrayLocation.TL_CENTER, "EnterSinbadBtn", "Enter SinbadState", 250);
            AdvancedMogreFramework.Instance.mTrayMgr.createButton(TrayLocation.TL_CENTER, "EnterPhysxBtn", "View Physx Demos", 250);
            AdvancedMogreFramework.Instance.mTrayMgr.createButton(TrayLocation.TL_CENTER, "EnterGameBtn", "View Game Demos", 250);
            AdvancedMogreFramework.Instance.mTrayMgr.createButton(TrayLocation.TL_CENTER, "EnterCreditBtn", "Credit", 250);
            AdvancedMogreFramework.Instance.mTrayMgr.createButton(TrayLocation.TL_CENTER, "ExitBtn", "Exit", 250);
            AdvancedMogreFramework.Instance.mTrayMgr.createLabel(TrayLocation.TL_TOP, "MenuLbl", "AdvancedMogreFramework", 250);
        }

        private void BuildPhysxGUI()
        {
            AdvancedMogreFramework.Instance.mTrayMgr.destroyAllWidgets();
            AdvancedMogreFramework.Instance.mTrayMgr.createButton(TrayLocation.TL_CENTER, "EnterBasicCubeBtn", "Basic Cube", 250);
            AdvancedMogreFramework.Instance.mTrayMgr.createButton(TrayLocation.TL_CENTER, "EnterNewtonCradleBtn", "Newton's Cradle", 250);
            AdvancedMogreFramework.Instance.mTrayMgr.createButton(TrayLocation.TL_CENTER, "EnterClothBtn", "Cloth", 250);
            AdvancedMogreFramework.Instance.mTrayMgr.createButton(TrayLocation.TL_CENTER, "EnterCharacterControllerBtn", "Character Controller", 250);
            AdvancedMogreFramework.Instance.mTrayMgr.createButton(TrayLocation.TL_CENTER, "BackBtn", "Back", 250);
            AdvancedMogreFramework.Instance.mTrayMgr.createLabel(TrayLocation.TL_TOP, "PhysxLbl", "View Physx Demos", 250);
        }

        private void BuildGameGUI()
        {
            AdvancedMogreFramework.Instance.mTrayMgr.destroyAllWidgets();
            AdvancedMogreFramework.Instance.mTrayMgr.createButton(TrayLocation.TL_CENTER, "EnterDrivingCarBtn", "Driving Car", 250);
            AdvancedMogreFramework.Instance.mTrayMgr.createButton(TrayLocation.TL_CENTER, "EnterInventoryBtn", "Inventory Demo", 250);
            AdvancedMogreFramework.Instance.mTrayMgr.createButton(TrayLocation.TL_CENTER, "BackBtn", "Back", 250);
            AdvancedMogreFramework.Instance.mTrayMgr.createLabel(TrayLocation.TL_TOP, "GameLbl", "View Game Demos", 250);
        }

        public void createScene()
        { }
        public override void Exit()
        {
            AdvancedMogreFramework.Instance.mLog.LogMessage("Leaving MenuState...");

            AdvancedMogreFramework.Instance.mMouse.MouseMoved -= mouseMoved;
            AdvancedMogreFramework.Instance.mMouse.MousePressed -= mousePressed;
            AdvancedMogreFramework.Instance.mMouse.MouseReleased -= mouseReleased;
            AdvancedMogreFramework.Instance.mKeyboard.KeyPressed -= keyPressed;
            AdvancedMogreFramework.Instance.mKeyboard.KeyReleased -= keyReleased;

            mSceneMgr.DestroyCamera(mCamera);
            if(mSceneMgr!=null)
                AdvancedMogreFramework.Instance.mRoot.DestroySceneManager(mSceneMgr);

            AdvancedMogreFramework.Instance.mTrayMgr.clearAllTrays();
            AdvancedMogreFramework.Instance.mTrayMgr.destroyAllWidgets();
            AdvancedMogreFramework.Instance.mTrayMgr.setListener(null);
        }

        public bool keyPressed(KeyEvent keyEventRef)
        {
            if(AdvancedMogreFramework.Instance.mKeyboard.IsKeyDown(MOIS.KeyCode.KC_ESCAPE))
            {
                m_bQuit = true;
                return true;
            }

            AdvancedMogreFramework.Instance.KeyPressed(keyEventRef);
            return true;
        }
        public bool keyReleased(KeyEvent keyEventRef)
        {
            AdvancedMogreFramework.Instance.KeyReleased(keyEventRef);
            return true;
        }

        public bool mouseMoved(MouseEvent evt)
        {
            if (AdvancedMogreFramework.Instance.mTrayMgr.injectMouseMove(evt)) return true;
            return true;
        }
        public bool mousePressed(MouseEvent evt, MouseButtonID id)
        {
            if (AdvancedMogreFramework.Instance.mTrayMgr.injectMouseDown(evt, id)) return true;
            return true;
        }
        public bool mouseReleased(MouseEvent evt, MouseButtonID id)
        {
            if (AdvancedMogreFramework.Instance.mTrayMgr.injectMouseUp(evt, id)) return true;
            return true;
        }

        public override void buttonHit(Button button)
        {
            if (button.getName() == "ExitBtn")
                m_bQuit = true;
            else if (button.getName() == "EnterBtn")
                changeAppState(findByName("GameState"));
            else if (button.getName() == "EnterSinbadBtn")
                changeAppState(findByName("SinbadState"));
            else if (button.getName() == "EnterCreditBtn")
                changeAppState(findByName("CreditState"));
            else if (button.getName() == "EnterPhysxBtn")
                BuildPhysxGUI();
            else if (button.getName() == "EnterGameBtn")
                BuildGameGUI();
            else if (button.getName() == "EnterBasicCubeBtn")
                changeAppState(findByName("BasicCubeState"));
            else if (button.getName() == "EnterNewtonCradleBtn")
                changeAppState(findByName("CreditState"));
            else if (button.getName() == "EnterClothBtn")
                changeAppState(findByName("CreditState"));
            else if (button.getName() == "EnterCharacterControllerBtn")
                changeAppState(findByName("CreditState"));
            else if (button.getName() == "EnterDrivingCarBtn")
                changeAppState(findByName("CreditState"));
            else if (button.getName() == "EnterInventoryBtn")
                changeAppState(findByName("InventoryDemoState"));
            else if (button.getName() == "BackBtn")
                BuildMainMenu();
        }

        public override void Update(double timeSinceLastFrame)
        {
            mFrameEvent.timeSinceLastFrame = (float)timeSinceLastFrame;
            AdvancedMogreFramework.Instance.mTrayMgr.frameRenderingQueued(mFrameEvent);
 
            if(m_bQuit == true)
            {
                shutdown();
                return;
            }
        }

        protected bool m_bQuit;
    }
}
