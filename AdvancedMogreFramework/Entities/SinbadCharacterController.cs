﻿/*
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
using Mogre.PhysX;
using MOIS;
using AdvancedMogreFramework.States;
using AdvancedMogreFramework.Helper;
using AdvancedMogreFramework.Entities;

namespace AdvancedMogreFramework
{
    public enum QUERY_MASK
    {
        AGENT_QUERY_MASK = 1 << 0,
        STATIC_OBJECT_MASK = 1 << 1
    }
    class SinbadCharacterController
    {
	    // all the animations our character has, and a null ID
	    // some of these affect separate body parts and will be blended together
        public const int NUM_ANIMS = 13;          // number of animations the character has
        public const int CHAR_HEIGHT = 5;      // height of character's center of mass above ground
        public const int CAM_HEIGHT = 2;          // height of camera above character's center of mass
        public const int RUN_SPEED = 17;           // character running speed in units per second
        public const float TURN_SPEED = 500.0f;      // character turning in degrees per second
        public const float ANIM_FADE_SPEED = 7.5f;   // animation crossfade speed in % of full weight per second
        public const float JUMP_ACCEL = 30.0f;       // character jump acceleration in upward units per squared second
        public const float GRAVITY = 90.0f;          // gravity in downward units per squared second

        public Mogre.Vector3 Position { get; set; }

        private Camera mCamera;
        private SceneNode mBodyNode;
        private SceneNode mCameraPivot;
        private SceneNode mCameraGoal;
        private SceneNode mCameraNode;
        private float mPivotPitch;
        private Entity mBodyEnt;
        private Entity mSword1;
        private Entity mSword2;
        private RibbonTrail mSwordTrail;
        private AnimationState[] mAnims = new AnimationState[NUM_ANIMS];    // master animation list
        private AnimID mBaseAnimID;                   // current base (full- or lower-body) animation
        private AnimID mTopAnimID;                    // current top (upper-body) animation
        private bool[] mFadingIn = new bool[NUM_ANIMS];            // which animations are fading in
        private bool[] mFadingOut = new bool[NUM_ANIMS];           // which animations are fading out
        private bool mSwordsDrawn;
        private Mogre.Vector3 mKeyDirection;      // player's local intended direction based on WASD keys
        private Mogre.Vector3 mGoalDirection;     // actual intended direction in world-space
        private float mVerticalVelocity;     // for jumping
        private float mTimer;                // general timer to see how long animations have been playing
        private bool mControlled;
        private int Id;
        private Mogre.Vector3 mSpawnPos;
        private AppState mWorld;
        private Physics mPhysics;
        private Scene mPhysicsScene;
        //private Actor mActor;
        //private CharacterController physicsController;
        private RigidBody rigidBody;
        private enum AnimID
	    {
		    ANIM_IDLE_BASE,
		    ANIM_IDLE_TOP,
		    ANIM_RUN_BASE,
		    ANIM_RUN_TOP,
		    ANIM_HANDS_CLOSED,
		    ANIM_HANDS_RELAXED,
		    ANIM_DRAW_SWORDS,
		    ANIM_SLICE_VERTICAL,
		    ANIM_SLICE_HORIZONTAL,
		    ANIM_DANCE,
		    ANIM_JUMP_START,
		    ANIM_JUMP_LOOP,
		    ANIM_JUMP_END,
		    ANIM_NONE
	    };

        public SinbadCharacterController(
            AppState world, 
            Scene physicsScene,
            Camera cam, 
            Mogre.Vector3 spawnPos, 
            int agentId = -1, 
            bool controlled = true)
        {
            mWorld = world;
            mControlled = controlled;
            Id = agentId;
            mSpawnPos = spawnPos;
            mCamera = cam;
            mPhysicsScene = physicsScene;
            mPhysics = physicsScene.Physics;
            setupBody(cam.SceneManager);
            setupPhysics();
            if (mControlled)
            {
                setupCamera(cam);
            }
            setupAnimations();
        }


        public void addTime(float deltaTime)
        {
            updateBody(deltaTime);
            updateAnimations(deltaTime);
            updateFilpTerrain();
            if (mControlled)
            {
                updateCamera(deltaTime);
            }
            updatePhysics(deltaTime);
        }

        public void injectKeyDown(KeyEvent evt)
        {
            if (mControlled)
            {
                if (evt.key == KeyCode.KC_Q && (mTopAnimID == AnimID.ANIM_IDLE_TOP || mTopAnimID == AnimID.ANIM_RUN_TOP))
                {
                    // take swords out (or put them back, since it's the same animation but reversed)
                    setTopAnimation(AnimID.ANIM_DRAW_SWORDS, true);
                    mTimer = 0;
                }
                else if (evt.key == KeyCode.KC_E && !mSwordsDrawn)
                {
                    if (mTopAnimID == AnimID.ANIM_IDLE_TOP || mTopAnimID == AnimID.ANIM_RUN_TOP)
                    {
                        // start dancing
                        setBaseAnimation(AnimID.ANIM_DANCE, true);
                        setTopAnimation(AnimID.ANIM_NONE);
                        // disable hand animation because the dance controls hands
                        mAnims[(int)AnimID.ANIM_HANDS_RELAXED].Enabled = false;
                    }
                    else if (mBaseAnimID == AnimID.ANIM_DANCE)
                    {
                        // stop dancing
                        setBaseAnimation(AnimID.ANIM_IDLE_BASE);
                        setTopAnimation(AnimID.ANIM_IDLE_TOP);
                        // re-enable hand animation
                        mAnims[(int)AnimID.ANIM_HANDS_RELAXED].Enabled = true;
                    }
                }

                // keep track of the player's intended direction
                else if (evt.key == KeyCode.KC_W) mKeyDirection.z = -1;
                else if (evt.key == KeyCode.KC_A) mKeyDirection.x = -1;
                else if (evt.key == KeyCode.KC_S) mKeyDirection.z = 1;
                else if (evt.key == KeyCode.KC_D) mKeyDirection.x = 1;

                else if (evt.key == KeyCode.KC_SPACE && (mTopAnimID == AnimID.ANIM_IDLE_TOP || mTopAnimID == AnimID.ANIM_RUN_TOP))
                {
                    // jump if on ground
                    setBaseAnimation(AnimID.ANIM_JUMP_START, true);
                    setTopAnimation(AnimID.ANIM_NONE);
                    mTimer = 0;
                }

                if (!mKeyDirection.IsZeroLength && mBaseAnimID == AnimID.ANIM_IDLE_BASE)
                {
                    // start running if not already moving and the player wants to move
                    setBaseAnimation(AnimID.ANIM_RUN_BASE, true);
                    if (mTopAnimID == AnimID.ANIM_IDLE_TOP) setTopAnimation(AnimID.ANIM_RUN_TOP, true);
                }
            }
        }

        public void injectKeyUp(KeyEvent evt)
        {
            if (mControlled)
            {
                if (evt.key == KeyCode.KC_W && mKeyDirection.z == -1) mKeyDirection.z = 0;
                else if (evt.key == KeyCode.KC_A && mKeyDirection.x == -1) mKeyDirection.x = 0;
                else if (evt.key == KeyCode.KC_S && mKeyDirection.z == 1) mKeyDirection.z = 0;
                else if (evt.key == KeyCode.KC_D && mKeyDirection.x == 1) mKeyDirection.x = 0;

                if (mKeyDirection.IsZeroLength && mBaseAnimID == AnimID.ANIM_RUN_BASE)
                {
                    // stop running if already moving and the player doesn't want to move
                    setBaseAnimation(AnimID.ANIM_IDLE_BASE);
                    if (mTopAnimID == AnimID.ANIM_RUN_TOP) setTopAnimation(AnimID.ANIM_IDLE_TOP);
                }
            }
        }

        public void injectMouseMove(MouseEvent evt)
        {
            if (mControlled)
            {
                updateCameraGoal(-0.05f * evt.state.X.rel, -0.05f * evt.state.Y.rel, -0.0005f * evt.state.Z.rel);
            }
        }

        public void injectMouseDown(MouseEvent evt, MouseButtonID id)
        {
            if (mControlled)
            {
                if (mSwordsDrawn && (mTopAnimID == AnimID.ANIM_IDLE_TOP || mTopAnimID == AnimID.ANIM_RUN_TOP))
                {
                    // if swords are out, and character's not doing something weird, then SLICE!
                    if (id == MouseButtonID.MB_Left) setTopAnimation(AnimID.ANIM_SLICE_VERTICAL, true);
                    else if (id == MouseButtonID.MB_Right) setTopAnimation(AnimID.ANIM_SLICE_HORIZONTAL, true);
                    mTimer = 0;
                }
            }
        }

        private void setupBody(SceneManager sceneMgr)
        {
            mBodyNode = sceneMgr.RootSceneNode.CreateChildSceneNode(Mogre.Vector3.UNIT_Y * CHAR_HEIGHT);
	        mBodyEnt = sceneMgr.CreateEntity("SinbadBody" + Id, "Sinbad.mesh");
            mBodyEnt.QueryFlags = (uint)QUERY_MASK.AGENT_QUERY_MASK;
	        mBodyNode.AttachObject(mBodyEnt);
            mBodyNode.Position = mSpawnPos;

	        // create swords and attach to sheath
	        mSword1 = sceneMgr.CreateEntity("SinbadSword1" +Id, "Sword.mesh");
	        mSword2 = sceneMgr.CreateEntity("SinbadSword2" +Id, "Sword.mesh");
	        mBodyEnt.AttachObjectToBone("Sheath.L", mSword1);
	        mBodyEnt.AttachObjectToBone("Sheath.R", mSword2);

	        // create a couple of ribbon trails for the swords, just for fun
	        NameValuePairList paramslist=new NameValuePairList();
            paramslist["numberOfChains"] = "2";
	        paramslist["maxElements"] = "80";
	        mSwordTrail = (RibbonTrail)sceneMgr.CreateMovableObject("RibbonTrail", paramslist);
	        //mSwordTrail.MaterialName=("Examples/LightRibbonTrail");
	        mSwordTrail.TrailLength=20;
	        mSwordTrail.Visible=(false);
	        sceneMgr.RootSceneNode.AttachObject(mSwordTrail);


	        for (uint i = 0; i < 2; i++)
	        {
		        mSwordTrail.SetInitialColour(i, 1f, 0.8f, 0f);
		        mSwordTrail.SetColourChange(i, 0.75f, 1.25f, 1.25f, 1.25f);
		        mSwordTrail.SetWidthChange(i, 1);
		        mSwordTrail.SetInitialWidth(i, 0.5f);
	        }

	        mKeyDirection = Mogre.Vector3.ZERO;
	        mVerticalVelocity = 0;
        }

        private void setupAnimations()
        { 
            mBodyEnt.Skeleton.BlendMode=SkeletonAnimationBlendMode.ANIMBLEND_CUMULATIVE;

	        var animNames =new String[]
	        {"IdleBase", "IdleTop", "RunBase", "RunTop", "HandsClosed", "HandsRelaxed", "DrawSwords",
	        "SliceVertical", "SliceHorizontal", "Dance", "JumpStart", "JumpLoop", "JumpEnd"};

	        // populate our animation list
	        for (int i = 0; i < NUM_ANIMS; i++)
	        {
		        mAnims[i] = mBodyEnt.GetAnimationState(animNames[i]);
		        mAnims[i].Loop=(true);
		        mFadingIn[i] = false;
		        mFadingOut[i] = false;
	        }

	        // start off in the idle state (top and bottom together)
            setBaseAnimation(AnimID.ANIM_IDLE_BASE);
            setTopAnimation(AnimID.ANIM_IDLE_TOP);

	        // relax the hands since we're not holding anything
            mAnims[(int)AnimID.ANIM_HANDS_RELAXED].Enabled=(true);

	        mSwordsDrawn = false;
        }

        private void setupCamera(Camera cam)
        {
            mCameraPivot = cam.SceneManager.RootSceneNode.CreateChildSceneNode();
	        // this is where the camera should be soon, and it spins around the pivot
	        mCameraGoal = mCameraPivot.CreateChildSceneNode(new Mogre.Vector3(0, 0, 15));
	        // this is where the camera actually is
	        mCameraNode = cam.SceneManager.RootSceneNode.CreateChildSceneNode();
	        mCameraNode.Position=mCameraPivot.Position + mCameraGoal.Position;
    
	        mCameraPivot.SetFixedYawAxis(true);
	        mCameraGoal.SetFixedYawAxis(true);
	        mCameraNode.SetFixedYawAxis(true);

	        // our model is quite small, so reduce the clipping planes
	        cam.NearClipDistance=0.1f;
	        cam.FarClipDistance=100;
	        mCameraNode.AttachObject(cam);

	        mPivotPitch = 0;
        }

        private void setupPhysics()
        {
            //if (mControlled)
            //{
            //    SimpleShape shape = new SimpleShape(new Mogre.Vector3(50, CHAR_HEIGHT, 0));
            //    CharacterControllerDescription description = new CharacterControllerDescription();
            //    physicsController = new CharacterController();
            //    physicsController.createCharacterController(mBodyNode.Position, shape, description, mPhysicsScene);
            //}
            //else
            //{
                rigidBody = new RigidBody();
                RigidBodyDescription description = new RigidBodyDescription();
                description.Density = 4;
                description.LinearVelocity = new Mogre.Vector3(0, 2, 5);
                ShapeDesc shape = mPhysics.CreateConvexHull(new
                    StaticMeshData(mBodyEnt.GetMesh()));
                rigidBody.CreateDynamic(
                    mBodyNode.Position, 
                    mBodyNode.Orientation.ToRotationMatrix(), 
                    description, 
                    mPhysicsScene, 
                    shape);
                //BodyDesc bodyDesc = new BodyDesc();
                //bodyDesc.LinearVelocity = new Mogre.Vector3(0, 2, 5);
                //
                //// the actor properties control the mass, position and orientation
                //// if you leave the body set to null it will become a static actor and wont move
                //ActorDesc actorDesc = new ActorDesc();
                //actorDesc.Density = 4;
                //actorDesc.Body = bodyDesc;
                //actorDesc.GlobalPosition = mBodyNode.Position;
                //actorDesc.GlobalOrientation = mBodyNode.Orientation.ToRotationMatrix();

                // a quick trick the get the size of the physics shape right is to use the bounding box of the entity
                //actorDesc.Shapes.Add(
                //    mPhysics.CreateConvexHull(new
                //    StaticMeshData(mBodyEnt.GetMesh())));
                //
                //// finally, create the actor in the physics scene
                //mActor = mPhysicsScene.CreateActor(actorDesc);
                if (!mControlled)
                    ((SinbadState)mWorld).AddActorNode(new ActorNode(mBodyNode, rigidBody.Actor));
            //}
        }

        private void updateBody(float deltaTime)
        {
            Position = mBodyNode.Position;
            mGoalDirection = Mogre.Vector3.ZERO;   // we will calculate this

	        if (mKeyDirection != Mogre.Vector3.ZERO && mBaseAnimID != AnimID.ANIM_DANCE)
	        {
		        // calculate actually goal direction in world based on player's key directions
		        mGoalDirection += mKeyDirection.z * mCameraNode.Orientation.ZAxis;
		        mGoalDirection += mKeyDirection.x * mCameraNode.Orientation.XAxis;
		        mGoalDirection.y = 0;
		        mGoalDirection.Normalise();

		        Quaternion toGoal = mBodyNode.Orientation.ZAxis.GetRotationTo(mGoalDirection);

		        // calculate how much the character has to turn to face goal direction
		        float yawToGoal = toGoal.Yaw.ValueDegrees;
		        // this is how much the character CAN turn this frame
		        float yawAtSpeed = yawToGoal / Mogre.Math.Abs(yawToGoal) * deltaTime * TURN_SPEED;
		        // reduce "turnability" if we're in midair
		        if (mBaseAnimID == AnimID. ANIM_JUMP_LOOP) yawAtSpeed *= 0.2f;

		        // turn as much as we can, but not more than we need to
		        if (yawToGoal < 0) yawToGoal = System.Math.Min(0, System.Math.Max(yawToGoal, yawAtSpeed)); //yawToGoal = Math::Clamp<Real>(yawToGoal, yawAtSpeed, 0);
		        else if (yawToGoal > 0) yawToGoal = System.Math.Max(0, System.Math.Min(yawToGoal, yawAtSpeed)); //yawToGoal = Math::Clamp<Real>(yawToGoal, 0, yawAtSpeed);
			
		        mBodyNode.Yaw(new Degree(yawToGoal));

		        // move in current body direction (not the goal direction)
		        mBodyNode.Translate(0, 0, deltaTime * RUN_SPEED * mAnims[(int)mBaseAnimID].Weight,
			        Node.TransformSpace. TS_LOCAL);
	        }

	        if (mBaseAnimID ==AnimID. ANIM_JUMP_LOOP)
	        {
		        // if we're jumping, add a vertical offset too, and apply gravity
		        mBodyNode.Translate(0, mVerticalVelocity * deltaTime, 0, Node.TransformSpace.TS_LOCAL);
		        mVerticalVelocity -= GRAVITY * deltaTime;
			
		        Mogre.Vector3 pos = mBodyNode.Position;
		        if (pos.y <= CHAR_HEIGHT)
		        {
			        // if we've hit the ground, change to landing state
			        pos.y = CHAR_HEIGHT;
			        mBodyNode.Position=pos;
                    setBaseAnimation(AnimID.ANIM_JUMP_END, true);
			        mTimer = 0;
		        }
	        }
        }

        private void updateAnimations(float deltaTime)
        {
            float baseAnimSpeed = 1;
	        float topAnimSpeed = 1;

	        mTimer += deltaTime;

	        if (mTopAnimID == AnimID.ANIM_DRAW_SWORDS)
	        {
		        // flip the draw swords animation if we need to put it back
		        topAnimSpeed = mSwordsDrawn ? -1 : 1;

		        // half-way through the animation is when the hand grasps the handles...
		        if (mTimer >= mAnims[(int)mTopAnimID].Length / 2 &&
			        mTimer - deltaTime < mAnims[(int)mTopAnimID].Length / 2)
		        {
			        // so transfer the swords from the sheaths to the hands
			        mBodyEnt.DetachAllObjectsFromBone();
			        mBodyEnt.AttachObjectToBone(mSwordsDrawn ? "Sheath.L" : "Handle.L", mSword1);
			        mBodyEnt.AttachObjectToBone(mSwordsDrawn ? "Sheath.R" : "Handle.R", mSword2);
			        // change the hand state to grab or let go
			        mAnims[(int)AnimID.ANIM_HANDS_CLOSED].Enabled=!mSwordsDrawn;
			        mAnims[(int)AnimID.ANIM_HANDS_RELAXED].Enabled=mSwordsDrawn;

			        // toggle sword trails
			        if (mSwordsDrawn)
			        {
				        mSwordTrail.Visible=false;
				        mSwordTrail.RemoveNode(mSword1.ParentNode);
				        mSwordTrail.RemoveNode(mSword2.ParentNode);
			        }
			        else
			        {
				        mSwordTrail.Visible=true;
				        mSwordTrail.AddNode(mSword1.ParentNode);
				        mSwordTrail.AddNode(mSword2.ParentNode);
			        }
		        }

		        if (mTimer >= mAnims[(int)mTopAnimID].Length)
		        {
			        // animation is finished, so return to what we were doing before
			        if (mBaseAnimID == AnimID.ANIM_IDLE_BASE) setTopAnimation(AnimID.ANIM_IDLE_TOP);
			        else
			        {
				        setTopAnimation(AnimID.ANIM_RUN_TOP);
				        mAnims[(int)AnimID.ANIM_RUN_TOP].TimePosition=mAnims[(int)AnimID.ANIM_RUN_BASE].TimePosition;
			        }
			        mSwordsDrawn = !mSwordsDrawn;
		        }
	        }
	        else if (mTopAnimID == AnimID.ANIM_SLICE_VERTICAL || mTopAnimID == AnimID.ANIM_SLICE_HORIZONTAL)
	        {
		        if (mTimer >= mAnims[(int)mTopAnimID].Length)
		        {
			        // animation is finished, so return to what we were doing before
			        if (mBaseAnimID == AnimID.ANIM_IDLE_BASE) setTopAnimation(AnimID.ANIM_IDLE_TOP);
			        else
			        {
				        setTopAnimation(AnimID.ANIM_RUN_TOP);
				        mAnims[(int)AnimID.ANIM_RUN_TOP].TimePosition=mAnims[(int)AnimID.ANIM_RUN_BASE].TimePosition;
			        }
		        }

		        // don't sway hips from side to side when slicing. that's just embarrasing.
		        if (mBaseAnimID == AnimID.ANIM_IDLE_BASE) baseAnimSpeed = 0;
	        }
	        else if (mBaseAnimID == AnimID.ANIM_JUMP_START)
	        {
		        if (mTimer >= mAnims[(int)mBaseAnimID].Length)
		        {
			        // takeoff animation finished, so time to leave the ground!
			        setBaseAnimation(AnimID.ANIM_JUMP_LOOP, true);
			        // apply a jump acceleration to the character
			        mVerticalVelocity = JUMP_ACCEL;
		        }
	        }
	        else if (mBaseAnimID == AnimID.ANIM_JUMP_END)
	        {
		        if (mTimer >= mAnims[(int)mBaseAnimID].Length)
		        {
			        // safely landed, so go back to running or idling
			        if (mKeyDirection == Mogre. Vector3.ZERO)
			        {
                        setBaseAnimation(AnimID.ANIM_IDLE_BASE);
                        setTopAnimation(AnimID.ANIM_IDLE_TOP);
			        }
			        else
			        {
                        setBaseAnimation(AnimID.ANIM_RUN_BASE, true);
                        setTopAnimation(AnimID.ANIM_RUN_TOP, true);
			        }
		        }
	        }

	        // increment the current base and top animation times
            if (mBaseAnimID != AnimID.ANIM_NONE) mAnims[(int)mBaseAnimID].AddTime(deltaTime * baseAnimSpeed);
            if (mTopAnimID != AnimID.ANIM_NONE) mAnims[(int)mTopAnimID].AddTime(deltaTime * topAnimSpeed);

	        // apply smooth transitioning between our animations
	        fadeAnimations(deltaTime);
        }

        private void fadeAnimations(float deltaTime)
        {
            for (int i = 0; i < NUM_ANIMS; i++)
	        {
		        if (mFadingIn[i])
		        {
			        // slowly fade this animation in until it has full weight
			        float newWeight = mAnims[i].Weight + deltaTime * ANIM_FADE_SPEED;
			        mAnims[i].Weight=Framework.Instance.Clamp(newWeight, 0, 1);
			        if (newWeight >= 1) mFadingIn[i] = false;
		        }
		        else if (mFadingOut[i])
		        {
			        // slowly fade this animation out until it has no weight, and then disable it
			        float newWeight = mAnims[i].Weight - deltaTime * ANIM_FADE_SPEED;
                    mAnims[i].Weight=Framework.Instance.Clamp(newWeight, 0, 1);
			        if (newWeight <= 0)
			        {
				        mAnims[i].Enabled=false;
				        mFadingOut[i] = false;
			        }
		        }
	        }
        }

        private void updateCamera(float deltaTime)
        {
            // place the camera pivot roughly at the character's shoulder
	        mCameraPivot.Position=mBodyNode.Position + Mogre.Vector3.UNIT_Y * CAM_HEIGHT;
	        // move the camera smoothly to the goal
	        Mogre.Vector3 goalOffset = mCameraGoal._getDerivedPosition() - mCameraNode.Position;
	        mCameraNode.Translate(goalOffset * deltaTime * 9.0f);
	        // always look at the pivot
	        mCameraNode.LookAt(mCameraPivot._getDerivedPosition(), Node.TransformSpace.TS_WORLD);
        }

        private void updateCameraGoal(float deltaYaw, float deltaPitch, float deltaZoom)
        {
            mCameraPivot.Yaw(new Degree(deltaYaw), Node.TransformSpace.TS_WORLD);

	        // bound the pitch
	        if (!(mPivotPitch + deltaPitch > 25 && deltaPitch > 0) &&
		        !(mPivotPitch + deltaPitch < -60 && deltaPitch < 0))
	        {
		        mCameraPivot.Pitch(new Degree(deltaPitch), Node.TransformSpace.TS_LOCAL);
		        mPivotPitch += deltaPitch;
	        }
		
	        float dist = mCameraGoal._getDerivedPosition().DotProduct(mCameraPivot._getDerivedPosition());
	        float distChange = deltaZoom * dist;

	        // bound the zoom
	        if (!(dist + distChange < 8 && distChange < 0) &&
		        !(dist + distChange > 25 && distChange > 0))
	        {
		        mCameraGoal.Translate(0, 0, distChange, Node.TransformSpace.TS_LOCAL);
	        }
        }

        private void setBaseAnimation(AnimID id, bool reset = false)
        {
            if (mBaseAnimID >= 0 && (int)mBaseAnimID < NUM_ANIMS)
            {
                // if we have an old animation, fade it out
                mFadingIn[(int)mBaseAnimID] = false;
                mFadingOut[(int)mBaseAnimID] = true;
            }

            mBaseAnimID = id;

            if (id != AnimID.ANIM_NONE)
            {
                // if we have a new animation, enable it and fade it in
                mAnims[(int)id].Enabled=(true);
                mAnims[(int)id].Weight=0;
                mFadingOut[(int)id] = false;
                mFadingIn[(int)id] = true;
                if (reset) mAnims[(int)id].TimePosition=0;
            }
        }

        private void setTopAnimation(AnimID id, bool reset = false)
        {
            if (mTopAnimID >= 0 && (int)mTopAnimID < NUM_ANIMS)
            {
                // if we have an old animation, fade it out
                mFadingIn[(int)mTopAnimID] = false;
                mFadingOut[(int)mTopAnimID] = true;
            }

            mTopAnimID = id;

            if (id != AnimID.ANIM_NONE)
            {
                // if we have a new animation, enable it and fade it in
                mAnims[(int)id].Enabled=true;
                mAnims[(int)id].Weight=0;
                mFadingOut[(int)id] = false;
                mFadingIn[(int)id] = true;
                if (reset) mAnims[(int)id].TimePosition=0;
            }
        }

        public void updateFilpTerrain()
        {
            Ray ray = new Ray(Position, Mogre.Vector3.NEGATIVE_UNIT_Y);
            RaySceneQuery rayQuery = mCamera.SceneManager.CreateRayQuery(ray);
            RaySceneQueryResult rayQueryResult = rayQuery.Execute();
            foreach (var result in rayQueryResult)
            {
                if (result.worldFragment != null)
                {
                    mBodyNode.SetPosition(
                        Position.x,
                        result.worldFragment.singleIntersection.y + 10,
                        Position.z);
                }
            }
        }

        private void updatePhysics(double deltaTime)
        {
            mPhysicsScene.FlushStream();
            mPhysicsScene.FetchResults(SimulationStatuses.AllFinished, true);
            mPhysicsScene.Simulate(deltaTime);
        }
    }
}
