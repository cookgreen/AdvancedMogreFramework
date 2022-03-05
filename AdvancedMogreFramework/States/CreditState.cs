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
    public class CreditState : AppState
    {
        public override void Enter()
        {
            mSceneMgr = AdvancedMogreFramework.Instance.mRoot.CreateSceneManager(Mogre.SceneType.ST_GENERIC, "CreditSceneMgr");
            ColourValue cvAmbineLight = new ColourValue(0.7f, 0.7f, 0.7f);
            mSceneMgr.AmbientLight = cvAmbineLight;

            mCamera = mSceneMgr.CreateCamera("GameCamera");
            Mogre.Vector3 vectCameraPostion = new Mogre.Vector3(5, 60, 60);
            mCamera.Position = vectCameraPostion;
            Mogre.Vector3 vectorCameraLookAt = new Mogre.Vector3(5, 20, 0);
            mCamera.LookAt(vectorCameraLookAt);
            mCamera.NearClipDistance = 5;
            mCamera.AspectRatio = AdvancedMogreFramework.Instance.mViewport.ActualWidth / AdvancedMogreFramework.Instance.mViewport.ActualHeight;

            AdvancedMogreFramework.Instance.mViewport.Camera = mCamera;

            ScreenManager.Instance.ChangeScreen("Credit");
            ScreenManager.Instance.OnCurrentScreenExit += OnCurrentScreenExit;

            AdvancedMogreFramework.Instance.mMouse.MousePressed += MousePressed;
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

        public override bool Pause()
        {
            return base.Pause();
        }

        public override void Resume()
        {
            base.Resume();
        }

        public override void Update(double timeSinceLastFrame)
        {
            ScreenManager.Instance.UpdateCurrentScreen((float)timeSinceLastFrame);
        }

        public override void Exit()
        {
            mSceneMgr.DestroyCamera(mCamera);
            AdvancedMogreFramework.Instance.mRoot.DestroySceneManager(mSceneMgr);
            ScreenManager.Instance.Dispose();
            AdvancedMogreFramework.Instance.mMouse.MousePressed -= MousePressed;
        }
    }
}
