using System;
using System.Collections.Generic;
using System.Text;
using Mogre;
using MOIS;
using Mogre_Procedural;
using Mogre_Procedural.MogreBites;
using MyGUI.Sharp;

namespace org.ogre.framework
{
    public class OgreFramework 
    {
        public Root root;
        public RenderWindow renderWnd;
        public Viewport viewport;
        public Log log;
        public Timer timer;

        public MOIS.InputManager inputMgr;
        public Keyboard keyboard;
        public Mouse mouse;

        public SdkTrayManager trayMgr;

        public Gui Gui;

        public static string lastState;

        public OgreFramework() { }

        public bool InitOgre(string wndTitle, string iconFile, string logFileName, string resourceFile)
        {
            LogManager logMgr = new LogManager();

            log = LogManager.Singleton.CreateLog(logFileName + ".log", true, true, false);
            log.SetDebugOutputEnabled(true);
 
            root = new Root();
 
            if(!root.ShowConfigDialog())
                return false;
               renderWnd = root.Initialise(true, wndTitle);

            if (!string.IsNullOrEmpty(iconFile))
            {
                IntPtr hwnd;
                renderWnd.GetCustomAttribute("WINDOW", out hwnd);
                Helper.SetWindowIcon(new System.Drawing.Icon(System.IO.Path.Combine(Environment.CurrentDirectory, iconFile)), hwnd);
            }

            viewport = renderWnd.AddViewport(null);
            ColourValue cv=new ColourValue(0.5f,0.5f,0.5f);
            viewport.BackgroundColour = cv;

            viewport.Camera = null;
 
            int hWnd = 0;
            renderWnd.GetCustomAttribute("WINDOW", out hWnd);
            inputMgr = MOIS.InputManager.CreateInputSystem((uint)hWnd);
            keyboard = (Keyboard)inputMgr.CreateInputObject(MOIS.Type.OISKeyboard, true);
            mouse =  (Mouse)inputMgr.CreateInputObject(MOIS.Type.OISMouse, true);

            mouse.MouseMoved+=new MouseListener.MouseMovedHandler(MouseMoved);
            mouse.MousePressed += new MouseListener.MousePressedHandler(MousePressed);
            mouse.MouseReleased += new MouseListener.MouseReleasedHandler(MouseReleased);

            keyboard.KeyPressed += new KeyListener.KeyPressedHandler(KeyPressed);
            keyboard.KeyReleased += new KeyListener.KeyReleasedHandler(KeyReleased);

            MOIS.MouseState_NativePtr mouseState = mouse.MouseState;
                mouseState.width = viewport.ActualWidth;
                mouseState.height = viewport.ActualHeight;

 
            String secName, typeName, archName;
            ConfigFile cf=new ConfigFile();
            cf.Load(resourceFile, "\t:=",true);
 
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

            trayMgr = new SdkTrayManager("TrayMgr", renderWnd, mouse, null);
 
            timer = new Timer();
            timer.Reset();
 
            renderWnd.IsActive=true;
 
            return true;
        }
        public void UpdateOgre(double timeSinceLastFrame)
        {
        }

        public bool KeyPressed(KeyEvent keyEventRef)
        {
             if(keyboard.IsKeyDown(MOIS.KeyCode.KC_V))
            {
                renderWnd.WriteContentsToTimestampedFile("Screenshot_", ".jpg");
                return true;
            }
 
            if(keyboard.IsKeyDown(MOIS.KeyCode.KC_O))
            {
                if(trayMgr.isLogoVisible())
                {
                    trayMgr.hideFrameStats();
                    trayMgr.hideLogo();
                }
                else
                {
                    trayMgr.showFrameStats(TrayLocation.TL_BOTTOMLEFT);
                    trayMgr.showLogo(TrayLocation.TL_BOTTOMRIGHT);
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
        private static OgreFramework instance;
        public static OgreFramework Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new OgreFramework();
                }
                return instance;
            }
        }
    }
}
