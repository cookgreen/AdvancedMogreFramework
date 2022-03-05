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
using Mogre_Procedural;
using Mogre_Procedural.MogreBites;

namespace AdvancedMogreFramework
{
    class AdvancedMogreFramework 
    {
        public Root mRoot;
        public RenderWindow mRenderWnd;
        public Viewport mViewport;
        public Log mLog;
        public Timer mTimer;

        public MOIS.InputManager mInputMgr;
        public Keyboard mKeyboard;
        public Mouse mMouse;

        public SdkTrayManager mTrayMgr;

        public NAudio.Vorbis.VorbisWaveReader mVorbis;
        public NAudio.Wave.WaveOut mWaveOut;

        public static string lastState;
        public AdvancedMogreFramework()
        {
            mRoot = null;
            mRenderWnd = null;
            mViewport = null;
            mLog = null;
            mTimer = null;

            mInputMgr = null;
            mKeyboard = null;
            mMouse = null;
            mTrayMgr = null;
         }
        ~AdvancedMogreFramework()
        {
            //LogManager.Singleton.LogMessage("Shutdown OGRE...");
            //if (AdvancedMogreFramework.m_pTrayMgr != null) m_pTrayMgr = null;
            //if (AdvancedMogreFramework.m_pInputMgr != null) InputManager.DestroyInputSystem(m_pInputMgr);
            //if (AdvancedMogreFramework.m_pRoot != null) m_pRoot = null;
        }

        public bool InitOgre(string wndTitle)
        {
            LogManager logMgr = new LogManager();
 
            mLog = LogManager.Singleton.CreateLog("OgreLogfile.log", true, true, false);
            mLog.SetDebugOutputEnabled(true);
 
            mRoot = new Root();
 
            if(!mRoot.ShowConfigDialog())
                return false;
               mRenderWnd = mRoot.Initialise(true, wndTitle);
 
            mViewport = mRenderWnd.AddViewport(null);
            ColourValue cv=new ColourValue(0.5f,0.5f,0.5f);
            mViewport.BackgroundColour=cv;
 
            mViewport.Camera=null;
 
            int hWnd = 0;
            //ParamList paramList;
            mRenderWnd.GetCustomAttribute("WINDOW", out hWnd);
 
            mInputMgr = InputManager.CreateInputSystem((uint)hWnd);
            mKeyboard = (MOIS.Keyboard)mInputMgr.CreateInputObject(MOIS.Type.OISKeyboard, true);
            mMouse =  (MOIS.Mouse)mInputMgr.CreateInputObject(MOIS.Type.OISMouse, true);

            mMouse.MouseMoved+=new MouseListener.MouseMovedHandler(MouseMoved);
            mMouse.MousePressed += new MouseListener.MousePressedHandler(MousePressed);
            mMouse.MouseReleased += new MouseListener.MouseReleasedHandler(MouseReleased);

            mKeyboard.KeyPressed += new KeyListener.KeyPressedHandler(KeyPressed);
            mKeyboard.KeyReleased += new KeyListener.KeyReleasedHandler(KeyReleased);

            MOIS.MouseState_NativePtr mouseState = mMouse.MouseState;
                mouseState.width = mViewport.ActualWidth;
                mouseState.height = mViewport.ActualHeight;
            //m_pMouse.MouseState = tempMouseState;

 
            String secName, typeName, archName;
            ConfigFile cf=new ConfigFile();
            cf.Load("resources.cfg","\t:=",true);
 
            ConfigFile.SectionIterator seci = cf.GetSectionIterator();
            while (seci.MoveNext())
            {
                secName = seci.CurrentKey;
                ConfigFile.SettingsMultiMap settings = seci.Current;
                foreach (KeyValuePair<string, string> pair in settings)
                {
                    typeName = pair.Key;
                    archName = pair.Value;
                    ResourceGroupManager.Singleton.AddResourceLocation(archName, typeName, secName);
                }
            }
            TextureManager.Singleton.DefaultNumMipmaps=5;
            ResourceGroupManager.Singleton.InitialiseAllResourceGroups(); 
 
            mTrayMgr = new SdkTrayManager("AOFTrayMgr", mRenderWnd, mMouse, null);
 
            mTimer = new Timer();
            mTimer.Reset();
 
            mRenderWnd.IsActive=true;
 
            return true;
        }
        public void UpdateOgre(double timeSinceLastFrame)
        {
        }

        public bool KeyPressed(KeyEvent keyEventRef)
        {
             if(mKeyboard.IsKeyDown(MOIS.KeyCode.KC_V))
            {
                mRenderWnd.WriteContentsToTimestampedFile("AMOF_Screenshot_", ".jpg");
                return true;
            }
 
            if(mKeyboard.IsKeyDown(MOIS.KeyCode.KC_O))
            {
                if(mTrayMgr.isLogoVisible())
                {
                    mTrayMgr.hideFrameStats();
                    mTrayMgr.hideLogo();
                }
                else
                {
                    mTrayMgr.showFrameStats(TrayLocation.TL_BOTTOMLEFT);
                    mTrayMgr.showLogo(TrayLocation.TL_BOTTOMRIGHT);
                }
            }
 
            return true;
        }
        public bool KeyReleased(KeyEvent keyEventRef)
        {
            return true;
        }

        public bool MouseMoved(MouseEvent evt)
        {
            return true;
        }
        public bool MousePressed(MouseEvent evt, MouseButtonID id)
        {
            return true;
        }
        public bool MouseReleased(MouseEvent evt, MouseButtonID id)
        {
            return true;
        }
        public float Clamp(float val, float minval, float maxval)
        {
            return System.Math.Max(System.Math.Min(val, maxval), minval);
        }
        public static AdvancedMogreFramework instance;
        public static AdvancedMogreFramework Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AdvancedMogreFramework();
                }
                return instance;
            }
        }
    }
}
