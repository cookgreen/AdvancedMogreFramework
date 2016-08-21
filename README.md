 Advanced Mogre Framework
=============
An advanced Framework for mogre

##How to use it?
load this solution file with your visual studio 2013 and compile it

##Requires##
* Mogre(include in the Mogre SDK)
* MOIS(include in the Mogre SDK)
* MogreBites(dll file in the project folder, you can use it directly)
<p>(<b>Remember MogreBites is a part of Mogre_Procedural created by <i>andyhebear1</i> on ogre3d forum, based on GPLv2</b>)

##Something you need to modify##
* After loading the project, expand the reference, right-click the mogre.dll, mois.dll, mogrebites.dll and click remove, after that, right-click the reference, click add. In the Browse tab, click "browse.." button and locate to your mogre sdk folder and add your own mogre.dll and mois.dll. Mogrebites.dll is in the "your AdvancedMogreFramework Folder/bin/x86/release" folder.
* Right-click the project, click the property, in the "Debug" tab, modify the "Working directory" to "your mogre sdk directory/bin/release"

##Notice##
* You only can build this project in release mode, because mogre only support release mode

##License##
GPLv2+LGPL
