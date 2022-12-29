using org.ogre.framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mogre;
using Mogre_Procedural.MogreBites;
using MOIS;
using Vector3 = Mogre.Vector3;

namespace org.ogre.framework.app
{
    public class MainMenuState : AppState
    {
        private bool m_bQuit = false;

        public override void Enter()
        {
            FontManager.Singleton.GetByName("SdkTrays/Caption").Load();

            OgreFramework.Instance.log.LogMessage("Entering MainMenu...");
            m_bQuit = false;

            sceneMgr = OgreFramework.Instance.root.CreateSceneManager(Mogre.SceneType.ST_GENERIC, "MainMenuSceneMgr");
            ColourValue cvAmbineLight = new ColourValue(0.7f, 0.7f, 0.7f);
            sceneMgr.AmbientLight = cvAmbineLight;

            camera = sceneMgr.CreateCamera("MainMenuCamera");
            camera.SetPosition(0, 25, -50);
            Mogre.Vector3 vectorCameraLookat = new Mogre.Vector3(0, 0, 0);
            camera.LookAt(vectorCameraLookat);
            camera.NearClipDistance = 1;

            camera.AspectRatio = OgreFramework.Instance.viewport.ActualWidth / OgreFramework.Instance.viewport.ActualHeight;

            OgreFramework.Instance.viewport.Camera = camera;

            sceneMgr.SetSkyBox(true, "Examples/SpaceSkyBox", 5000);

            Light light = sceneMgr.CreateLight();
            light.Type = Light.LightTypes.LT_POINT;
            light.Position = new Vector3(-250, 200, 0);
            light.SetSpecularColour(255, 255, 255);

            OgreFramework.Instance.trayMgr.showFrameStats(TrayLocation.TL_BOTTOMLEFT);
            OgreFramework.Instance.trayMgr.showLogo(TrayLocation.TL_BOTTOMRIGHT);
            OgreFramework.Instance.trayMgr.showCursor();

            OgreFramework.Instance.trayMgr.destroyAllWidgets();

            OgreFramework.Instance.trayMgr.createLabel(TrayLocation.TL_TOP, "lbTitle", "AdvancedOgreFramework", 200);
            OgreFramework.Instance.trayMgr.createButton(TrayLocation.TL_CENTER, "btnSinbad", "Enter Sinbad", 250);
            OgreFramework.Instance.trayMgr.createButton(TrayLocation.TL_CENTER, "btnExit", "Exit", 250);

            OgreFramework.Instance.mouse.MouseMoved += mouseMoved;
            OgreFramework.Instance.mouse.MousePressed += mousePressed;
            OgreFramework.Instance.mouse.MouseReleased += mouseReleased;
            OgreFramework.Instance.keyboard.KeyPressed += keyPressed;
            OgreFramework.Instance.keyboard.KeyReleased += keyReleased;
        }

        public override void Exit()
        {
            OgreFramework.Instance.trayMgr.destroyAllWidgets();

            sceneMgr.DestroyCamera(camera);
            Root.Singleton.DestroySceneManager(sceneMgr);
        }

        public bool keyPressed(KeyEvent keyEventRef)
        {
            if (OgreFramework.Instance.keyboard.IsKeyDown(MOIS.KeyCode.KC_ESCAPE))
            {
                m_bQuit = true;
                return true;
            }

            OgreFramework.Instance.KeyPressed(keyEventRef);
            return true;
        }
        public bool keyReleased(KeyEvent keyEventRef)
        {
            OgreFramework.Instance.KeyReleased(keyEventRef);
            return true;
        }

        public bool mouseMoved(MouseEvent evt)
        {
            if (OgreFramework.Instance.trayMgr.injectMouseMove(evt)) return true;
            return true;
        }
        public bool mousePressed(MouseEvent evt, MouseButtonID id)
        {
            if (OgreFramework.Instance.trayMgr.injectMouseDown(evt, id)) return true;
            return true;
        }
        public bool mouseReleased(MouseEvent evt, MouseButtonID id)
        {
            if (OgreFramework.Instance.trayMgr.injectMouseUp(evt, id)) return true;
            return true;
        }

        public override void buttonHit(Button button)
        {
            if (button.getName() == "btnExit")
            {
                m_bQuit = true;
            }
            else if (button.getName() == "btnSinbad")
            {
                changeAppState(findByName("SinbadState"));
            }
        }

        public override void Update(double timeSinceLastFrame)
        {
            if(m_bQuit)
            {
                shutdown();
                return;
            }
        }
    }
}
