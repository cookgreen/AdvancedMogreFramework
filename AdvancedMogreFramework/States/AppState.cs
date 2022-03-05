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
#define T
using System;
using System.Collections.Generic;
using System.Text;
using Mogre;
using Mogre_Procedural.MogreBites;
using MOIS;

namespace AdvancedMogreFramework.States
{
    public class AppStateListener
    {
	    public AppStateListener(){}
	    ~AppStateListener(){}

        public virtual void ManageAppState(string stateName, AppState state) { }

        public virtual AppState FindByName(string stateName) { return null; }
        public virtual void ChangeAppState(AppState state) { }
        public virtual bool PushAppState(AppState state) { return false; }
        public virtual void PopAppState() { }
        public virtual void PauseAppState() { }
        public virtual void Shutdown() { }
        public virtual void PopAllAndPushAppState<T>(AppState state) where T:AppState { }
};
    public class AppState :SdkTrayListener
    {
        public static void Create<T>(AppStateListener parent, string name) where T : AppState, new()
        {
            T myAppState=new T();				
	        myAppState.mListener = parent;					
	        parent.ManageAppState(name, myAppState);
        }
 
	    public void Destroy()
        {
        }
 
	    public virtual void Enter(){}
	    public virtual void Exit(){}
	    public virtual bool Pause(){return false;}
	    public virtual void Resume(){}
        public virtual void Update(double timeSinceLastFrame) { }
        public AppState(){}
 
	    protected AppState	findByName(String stateName){return mListener.FindByName(stateName);}
        protected void changeAppState(AppState state) { mListener.ChangeAppState(state); }
        protected bool pushAppState(AppState state) { return mListener.PushAppState(state); }
        protected void popAppState() { mListener.PopAppState(); }
        protected void shutdown() { mListener.Shutdown(); }
        protected void popAllAndPushAppState<T>(AppState state) where T:AppState{ mListener.PopAllAndPushAppState<T>(state); }

        protected AppStateListener mListener;

        protected Camera mCamera;
        protected SceneManager mSceneMgr;
        protected FrameEvent mFrameEvent;
    }
}