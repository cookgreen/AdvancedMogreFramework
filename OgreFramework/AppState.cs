#define T
using System;
using System.Collections.Generic;
using System.Text;
using Mogre;
using Mogre_Procedural.MogreBites;
using MOIS;

namespace org.ogre.framework
{
    public class AppStateListener
    {
        public AppStateListener() { }

        public virtual void ManageAppState(string stateName, AppState state) { }

        public virtual AppState FindByName(string stateName) { return null; }
        public virtual void ChangeAppState(AppState state) { }
        public virtual bool PushAppState(AppState state) { return false; }
        public virtual void PopAppState() { }
        public virtual void PauseAppState() { }
        public virtual void Shutdown() { }
        public virtual void PopAllAndPushAppState<T>(AppState state) where T:AppState { }
};
    public class AppState : SdkTrayListener
    {
        public static void Create<T>(AppStateListener parent, string name) where T : AppState, new()
        {
            T myAppState = new T();
            myAppState.listener = parent;				
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
 
	    protected AppState	findByName(String stateName){return listener.FindByName(stateName);}
        protected void changeAppState(AppState state) { listener.ChangeAppState(state); }
        protected bool pushAppState(AppState state) { return listener.PushAppState(state); }
        protected void popAppState() { listener.PopAppState(); }
        protected void shutdown() { listener.Shutdown(); }
        protected void popAllAndPushAppState<T>(AppState state) where T:AppState{ listener.PopAllAndPushAppState<T>(state); }

        protected AppStateListener listener;

        protected Camera camera;
        protected SceneManager sceneMgr;
        protected FrameEvent frameEvent;
    }
}