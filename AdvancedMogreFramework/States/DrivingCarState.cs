using Mogre;
using MOIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdvancedMogreFramework.States
{
    public class DrivingCarState : AppState
    {
        public override void enter()
        {
            AdvancedMogreFramework.Singleton.m_pLog.LogMessage("Entering GameState...");
            AdvancedMogreFramework.lastState = "GameState";
            mSceneMgr = AdvancedMogreFramework.Singleton.m_pRoot.CreateSceneManager(SceneType.ST_GENERIC, "GameSceneMgr");
            ColourValue cvAmbineLight = new ColourValue(0.7f, 0.7f, 0.7f);
            mSceneMgr.AmbientLight = cvAmbineLight;//(Ogre::ColourValue(0.7f, 0.7f, 0.7f));

            mCamera = mSceneMgr.CreateCamera("GameCamera");
            Mogre.Vector3 vectCameraPostion = new Mogre.Vector3(5, 60, 60);
            mCamera.Position = vectCameraPostion;
            Mogre.Vector3 vectorCameraLookAt = new Mogre.Vector3(5, 20, 0);
            mCamera.LookAt(vectorCameraLookAt);
            mCamera.NearClipDistance = 5;

            mCamera.AspectRatio = AdvancedMogreFramework.Singleton.m_pViewport.ActualWidth / AdvancedMogreFramework.Singleton.m_pViewport.ActualHeight;

            AdvancedMogreFramework.Singleton.m_pViewport.Camera = mCamera;
        }

        public override void exit()
        {
            AdvancedMogreFramework.Singleton.m_pLog.LogMessage("Leaving GameState...");

            AdvancedMogreFramework.Singleton.m_pMouse.MouseMoved -= mouseMoved;
            AdvancedMogreFramework.Singleton.m_pMouse.MousePressed -= mousePressed;
            AdvancedMogreFramework.Singleton.m_pMouse.MouseReleased -= mouseReleased;
            AdvancedMogreFramework.Singleton.m_pKeyboard.KeyPressed -= keyPressed;
            AdvancedMogreFramework.Singleton.m_pKeyboard.KeyReleased -= keyReleased;

            if (mSceneMgr != null)
                mSceneMgr.DestroyCamera(mCamera);
            AdvancedMogreFramework.Singleton.m_pRoot.DestroySceneManager(mSceneMgr);
        }

        public virtual bool keyPressed(KeyEvent keyEventRef)
        {
            return true;
        }
        public bool keyReleased(KeyEvent keyEventRef)
        {
            return true;
        }

        public bool mouseMoved(MouseEvent evt)
        {
            return true;
        }
        public bool mousePressed(MouseEvent evt, MouseButtonID id)
        {
            return true;
        }
        public bool mouseReleased(MouseEvent evt, MouseButtonID id)
        {
            return true;
        }
    }
}
