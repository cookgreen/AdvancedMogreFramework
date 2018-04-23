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
using System.Linq;
using System.Text;
using Mogre;
using Mogre_Procedural.MogreBites;

namespace AdvancedMogreFramework
{
    public class CreditScreen : IScreen
    {
        private int time;
        private List<Widget> elements;
        private List<string> elementNames;
        private StringVector strCreditLst;
        private float alpha;

        public event Action OnScreenExit;

        public CreditScreen()
        {
            time = 0;
            strCreditLst = new StringVector();
            StringBuilder creditBuilder = new StringBuilder();
            creditBuilder.AppendLine("Advanced Mogre Framework");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("CopyRight 2016-2020 Cook Green");
            creditBuilder.AppendLine("");
            strCreditLst.Add(creditBuilder.ToString());
            creditBuilder.Clear();
            creditBuilder.AppendLine("Deign: Cook Green");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("Programming: Cook Green");
            creditBuilder.AppendLine("");
            strCreditLst.Add(creditBuilder.ToString());
            creditBuilder.Clear();
            creditBuilder.AppendLine("All Credit List:");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("Ogre - Open Source 3D Render Engine");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("Steve 'sinbad' Streeting");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("Matias 'dark_sylinc' Goldberg");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("Eugene Golushkov");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("Pavel 'paroj' Rojtberg");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("Dave 'masterfalcon' Rogers");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("Murat 'Wolfmanfx' Sari");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("Philip 'spacegaier' Allgaier");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("Assaf Raman");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("Mogre - C++/CLI Wrapper for Ogre");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("GantZ");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("MogreBites - GUI Library for Mogre");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("rains soft(andyhebear)");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("NAudio - DotNet Platform Sound Library");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("markheath");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("NVorbis - Ogg Format Support For NAudio");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("ioctlLR");
            creditBuilder.AppendLine("");
            creditBuilder.AppendLine("");
            strCreditLst.Add(creditBuilder.ToString());
        }
        public void Init()
        {
            elements = new List<Widget>();
            elementNames = new List<string>();
        }

        public void Run()
        {
            AdvancedMogreFramework.Singleton.m_pTrayMgr.hideCursor();
        }

        public void Update(float timeSinceLastFrame)
        {
            if (time >= 0 && time <= 2000)
            {
                if (!elementNames.Contains("lbCredit0"))
                {
                    elements.Add(AdvancedMogreFramework.Singleton.m_pTrayMgr.createStaticText(TrayLocation.TL_NONE, "lbCredit0", strCreditLst[0]));
                    elements[0].getOverlayElement().MetricsMode = GuiMetricsMode.GMM_RELATIVE;
                    elements[0].getOverlayElement().Left = 0.5f;
                    elements[0].getOverlayElement().Top = 0.5f;
                    elementNames.Add("lbCredit0");
                }
                ColourValue elementColor = elements[0].getOverlayElement().Colour;
                alpha = elementColor.a;
                if (alpha > 0.0f)
                {
                    alpha -= 0.0005f;
                    elements[0].getOverlayElement().Colour = new ColourValue(
                        elementColor.r,
                        elementColor.g,
                        elementColor.b,
                        float.Parse(alpha.ToString("0.00")));
                }
            }
            else if (time > 2000 && time <= 4000)
            {
                if (elementNames.Contains("lbCredit0"))
                {
                    elements.Remove(elements.Find(o => o.getName() == "lbCredit0"));
                    elementNames.Remove("lbCredit0");
                    AdvancedMogreFramework.Singleton.m_pTrayMgr.destroyWidget("lbCredit0");
                }
                if (!elementNames.Contains("lbCredit1"))
                {
                    elements.Add(AdvancedMogreFramework.Singleton.m_pTrayMgr.createStaticText(TrayLocation.TL_NONE, "lbCredit1", strCreditLst[1]));
                    elements[0].getOverlayElement().MetricsMode = GuiMetricsMode.GMM_RELATIVE;
                    elements[0].getOverlayElement().Left = 0.5f;
                    elements[0].getOverlayElement().Top = 0.5f;
                    elementNames.Add("lbCredit1");
                }
                ColourValue elementColor = elements[0].getOverlayElement().Colour;
                alpha = elementColor.a;
                if (alpha > 0.0f)
                {
                    alpha -= 0.0005f;
                    elements[0].getOverlayElement().Colour = new ColourValue(
                        elementColor.r,
                        elementColor.g,
                        elementColor.b,
                        float.Parse(alpha.ToString("0.00")));
                }
            }
            else if (time > 4000 && time <= 12000)
            {
                if (elementNames.Contains("lbCredit1"))
                {
                    elements.Remove(elements.Find(o => o.getName() == "lbCredit1"));
                    elementNames.Remove("lbCredit1");
                    AdvancedMogreFramework.Singleton.m_pTrayMgr.destroyWidget("lbCredit1");
                }
                if (!elementNames.Contains("lbCredit2"))
                {
                    elements.Add(AdvancedMogreFramework.Singleton.m_pTrayMgr.createStaticText(TrayLocation.TL_NONE, "lbCredit2", strCreditLst[2]));
                    elements[0].getOverlayElement().MetricsMode = GuiMetricsMode.GMM_RELATIVE;
                    elements[0].getOverlayElement().Left = 0.5f;
                    elements[0].getOverlayElement().Top = 1.0f;
                    elementNames.Add("lbCredit2");
                }
                if (elements[0].getOverlayElement().Top > -1.0f)
                {
                    elements[0].getOverlayElement().Top -= 0.00025f;
                }
            }
            else
            {
                OnScreenExit?.Invoke();
            }
            time++;
        }

        public void Exit()
        {
            AdvancedMogreFramework.Singleton.m_pTrayMgr.destroyAllWidgets();
            time = 0;
            elements.Clear();
        }
    }
}
