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
        public override void Enter()
        {
            Framework.Instance.mLog.LogMessage("Entering GameState...");
            Framework.lastState = "GameState";
            mSceneMgr = Framework.Instance.mRoot.CreateSceneManager(SceneType.ST_GENERIC, "GameSceneMgr");
            ColourValue cvAmbineLight = new ColourValue(0.7f, 0.7f, 0.7f);
            mSceneMgr.AmbientLight = cvAmbineLight;//(Ogre::ColourValue(0.7f, 0.7f, 0.7f));

            mCamera = mSceneMgr.CreateCamera("GameCamera");
            Mogre.Vector3 vectCameraPostion = new Mogre.Vector3(5, 60, 60);
            mCamera.Position = vectCameraPostion;
            Mogre.Vector3 vectorCameraLookAt = new Mogre.Vector3(5, 20, 0);
            mCamera.LookAt(vectorCameraLookAt);
            mCamera.NearClipDistance = 5;

            mCamera.AspectRatio = Framework.Instance.mViewport.ActualWidth / Framework.Instance.mViewport.ActualHeight;

            Framework.Instance.mViewport.Camera = mCamera;
        }

        public override void Exit()
        {
            Framework.Instance.mLog.LogMessage("Leaving GameState...");

            Framework.Instance.mMouse.MouseMoved -= mouseMoved;
            Framework.Instance.mMouse.MousePressed -= mousePressed;
            Framework.Instance.mMouse.MouseReleased -= mouseReleased;
            Framework.Instance.mKeyboard.KeyPressed -= keyPressed;
            Framework.Instance.mKeyboard.KeyReleased -= keyReleased;

            if (mSceneMgr != null)
                mSceneMgr.DestroyCamera(mCamera);
            Framework.Instance.mRoot.DestroySceneManager(mSceneMgr);
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
