 Advanced Mogre Framework
=============
##Introduction
Advanced Mogre Framework is an advanced framework for mogre base on Advanced Ogre Framework(See:[http://www.ogre3d.org/tikiwiki/tiki-index.php?page=Advanced+Ogre+Framework](http://www.ogre3d.org/tikiwiki/tiki-index.php?page=Advanced+Ogre+Framework))

##Usage:
load the solution file with your visual studio(2010 2012 or 2013) and compile it

##Based on##
* Mogre(include in the Mogre SDK)
* MOIS(include in the Mogre SDK)
* MogreBites(dll file in the project folder, you can use it directly)
<p>(<b>Remember MogreBites is a part of Mogre_Procedural created by <i>andyhebear1</i> on ogre3d forum, based on GPLv2</b>)

##Something you need to modify##
* After loading the project, expand the reference, right-click the mogre.dll, mois.dll, mogrebites.dll and click remove, after that, right-click the reference, click add. In the Browse tab, click "browse.." button and locate to "your AdvancedMogreFramework Folder/bin/x86/release" folder and add it

##Notice##
* You only can build this project in release mode, because mogre only support release mode(if you use Mogre SDK 1.7.1, Mogre 1.7.4 can build Release and Debug)

##License##
GPLv2+LGPL
