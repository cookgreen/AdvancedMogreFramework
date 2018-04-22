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
using AdvancedMogreFramework.Screen;
using Mogre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdvancedMogreFramework.States
{
    class CreditState : GameState
    {
        public override void enter()
        {
            m_pSceneMgr = AdvancedMogreFramework.Singleton.m_pRoot.CreateSceneManager(Mogre.SceneType.ST_GENERIC, "CreditSceneMgr");
            ColourValue cvAmbineLight = new ColourValue(0.7f, 0.7f, 0.7f);
            m_pSceneMgr.AmbientLight = cvAmbineLight;

            m_pCamera = m_pSceneMgr.CreateCamera("GameCamera");
            Mogre.Vector3 vectCameraPostion = new Mogre.Vector3(5, 60, 60);
            m_pCamera.Position = vectCameraPostion;
            Mogre.Vector3 vectorCameraLookAt = new Mogre.Vector3(5, 20, 0);
            m_pCamera.LookAt(vectorCameraLookAt);
            m_pCamera.NearClipDistance = 5;
            m_pCamera.AspectRatio = AdvancedMogreFramework.Singleton.m_pViewport.ActualWidth / AdvancedMogreFramework.Singleton.m_pViewport.ActualHeight;

            AdvancedMogreFramework.Singleton.m_pViewport.Camera = m_pCamera;

            ScreenManager.Instance.ChangeScreen("Credit");
            ScreenManager.Instance.OnCurrentScreenExit += OnCurrentScreenExit;

            AdvancedMogreFramework.Singleton.m_pMouse.MousePressed += MousePressed;
        }

        private bool MousePressed(MOIS.MouseEvent arg, MOIS.MouseButtonID id)
        {
            if (id == MOIS.MouseButtonID.MB_Right)
            {
                changeAppState(findByName("MenuState"));
            }
            return true;
        }

        private void OnCurrentScreenExit()
        {
            changeAppState(findByName("MenuState"));
        }

        public override bool pause()
        {
            return base.pause();
        }

        public override void resume()
        {
            base.resume();
        }

        public override void update(double timeSinceLastFrame)
        {
            ScreenManager.Instance.UpdateCurrentScreen((float)timeSinceLastFrame);
        }

        public override void exit()
        {
            m_pSceneMgr.DestroyCamera(m_pCamera);
            AdvancedMogreFramework.Singleton.m_pRoot.DestroySceneManager(m_pSceneMgr);
            ScreenManager.Instance.Dispose();
        }
    }
}
