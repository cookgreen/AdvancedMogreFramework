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

namespace Advanced_Mogre_Framework
{
    class AppStateListener
    {
	    public AppStateListener(){}
	    ~AppStateListener(){}

        public virtual void manageAppState(String stateName, AppState state) { }

        public virtual AppState findByName(String stateName) { return null; }
        public virtual void changeAppState(AppState state) { }
        public virtual bool pushAppState(AppState state) { return false; }
        public virtual void popAppState() { }
        public virtual void pauseAppState() { }
        public virtual void shutdown() { }
        public virtual void popAllAndPushAppState<T>(AppState state) where T:AppState { }
};
    class AppState :SdkTrayListener
    {
        public static void create<T>(AppStateListener parent, String name) where T : AppState, new()
        {
            T myAppState=new T();				
	        myAppState.m_pParent = parent;					
	        parent.manageAppState(name, myAppState);
        }
 
	    public void destroy()
        {
        }
 
	    public virtual void enter(){}
	    public virtual void exit(){}
	    public virtual bool pause(){return false;}
	    public virtual void resume(){}
        public virtual void update(double timeSinceLastFrame) { }
        public AppState(){}
 
	    protected AppState	findByName(String stateName){return m_pParent.findByName(stateName);}
        protected void changeAppState(AppState state) { m_pParent.changeAppState(state); }
        protected bool pushAppState(AppState state) { return m_pParent.pushAppState(state); }
        protected void popAppState() { m_pParent.popAppState(); }
        protected void shutdown() { m_pParent.shutdown(); }
        protected void popAllAndPushAppState<T>(AppState state) where T:AppState{ m_pParent.popAllAndPushAppState<T>(state); }

        protected AppStateListener m_pParent;

        protected Camera m_pCamera;
        protected SceneManager m_pSceneMgr;
        protected FrameEvent m_FrameEvent;
    }
}