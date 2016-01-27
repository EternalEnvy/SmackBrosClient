using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Assimp;
using Assimp.Configs;
using System.IO;

namespace SmackBrosClient
{
    public class cAnimationChannel
    {
        public string Name;
        public List<VectorKey> mPositionKeys;
        public List<QuaternionKey> mRotationKeys;
        public List<VectorKey> mScalingKeys;
    }
    public class cBone 
    {
	    public string Name;
	    public Matrix4x4 Offset, LocalTransform, GlobalTransform, OriginalLocalTransform;
        public cBone Parent;
        public List<cBone> Children;
        public cBone()
        {
            Parent = null;
        }
    }
    public class cAnimEvaluator
    {
        public bool PlayAnimationForward;// play forward == true, play backward == false
        public List<cAnimationChannel> Channels;
        public float mLastTime, TicksPerSecond, Duration;
        public string Name;
        public List<Tuple<int,int,int>> mLastPositions;
        public List<List<Matrix4x4>> Transforms;
        public cAnimEvaluator() 
        {
            mLastTime = 0.0f; 
            TicksPerSecond = 0.0f;
            Duration = 0.0f; 
            PlayAnimationForward = true; 
        }
	    public cAnimEvaluator(Animation pAnim)
        {
            mLastTime = 0.0f;
	        TicksPerSecond = (float)(pAnim.TicksPerSecond != 0.0f ? pAnim.TicksPerSecond : 100.0f);
	        Duration = (float)(pAnim.DurationInTicks);
	        Name = pAnim.Name;
	        //OUTPUT_DEBUG_MSG("Creating Animation named: "<<Name);
	        //Channels.resize(pAnim.MeshAnimationChannelCount);
	        for(int a = 0; a < pAnim.NodeAnimationChannelCount; a++)
            {
                Channels[a].Name = pAnim.NodeAnimationChannels[a].NodeName;
		        for(int i = 0; i < pAnim.NodeAnimationChannels[a].PositionKeyCount; i++)
                    Channels[a].mPositionKeys.Add(pAnim.NodeAnimationChannels[a].PositionKeys[i]); 
                for(int j = 0; j < pAnim.NodeAnimationChannels[a].RotationKeyCount; j++) 
                    Channels[a].mRotationKeys.Add(pAnim.NodeAnimationChannels[a].RotationKeys[j]);
                for(int k = 0; k < pAnim.NodeAnimationChannels[a].ScalingKeyCount; k++) 
                    Channels[a].mScalingKeys.Add(pAnim.NodeAnimationChannels[a].ScalingKeys[k]);
            }
            //mLastPositions.resize( pAnim->mNumChannels, std::make_tuple( 0, 0, 0));
	        //OUTPUT_DEBUG_MSG("Finished Creating Animation named: "<<Name);
	    } 
	    public void Evaluate( float pTime, Dictionary<string,cBone> bones)
        {
	        pTime *= TicksPerSecond;
	
	        float time = 0.0f;
	        if( Duration > 0.0)
		        time = pTime % Duration;

	        // calculate the transformations for each animation channel
	        for(int a = 0; a < Channels.Count(); a++)
            {
		        cAnimationChannel channel = Channels[a];
                cBone bonenode;
                bool found = bones.TryGetValue(channel.Name, out bonenode);
                //std::map<std::string, cBone*>::iterator bonenode = bones.find(channel->Name);

		        if(!found) 
                { 
			        Console.Write("Couldn't find bone");
			        continue;
		        }
		        // ******** Position *****
		        Vector3D presentPosition = new Vector3D(0, 0, 0);
		        if(channel.mPositionKeys.Count > 0){
			        // Look for present frame number. Search from last position if time is after the last time, else from beginning
			        // Should be much quicker than always looking from start for the average use case.
			        int frame = (time >= mLastTime) ? mLastPositions[a].Item1 : 0;
			        while( frame < channel.mPositionKeys.Count() - 1)
                    {
				        if(time < channel.mPositionKeys[frame+1].Time)
                        {
					        break;
				        }
				        frame++;
			        }
			        // interpolate between this frame's value and next frame's value
			        int nextFrame = (frame + 1) % channel.mPositionKeys.Count();
	
			        VectorKey key = channel.mPositionKeys[frame];
			        VectorKey nextKey = channel.mPositionKeys[nextFrame];
			        double diffTime = nextKey.Time - key.Time;
			        if( diffTime < 0.0)
				        diffTime += Duration;
			        if( diffTime > 0)
                    {
				        float factor = (float)((time - key.Time) / diffTime);
				        presentPosition = key.Value + (nextKey.Value - key.Value) * factor;
			        } 
                    else 
                    {
				        presentPosition = key.Value;
			        }
			        mLastPositions[a] = new Tuple<int,int,int>(frame, mLastPositions[a].Item2, mLastPositions[a].Item3);
		        }
		        // ******** Rotation *********
		        Quaternion presentRotation = new Quaternion(1, 0, 0, 0);
		        if(channel.mRotationKeys.Count() > 0)
		        {
			        int frame = (time >= mLastTime) ? mLastPositions[a].Item2 : 0;
			        while( frame < channel.mRotationKeys.Count()  - 1)
                    {
				        if( time < channel.mRotationKeys[frame+1].Time)
				        	break;
			        	frame++;
		        	}

		        	// interpolate between this frame's value and next frame's value
		        	int nextFrame = (frame + 1) % channel.mRotationKeys.Count();

		        	QuaternionKey key = channel.mRotationKeys[frame];
		            QuaternionKey nextKey = channel.mRotationKeys[nextFrame];
		        	double diffTime = nextKey.Time - key.Time;
		        	if( diffTime < 0.0) diffTime += Duration;
		        	if( diffTime > 0) 
                    {
		        		float factor = (float)((time - key.Time) / diffTime);
                        //this next line might be wrong (written awake for 20 hours), but I chanced it
		        		presentRotation *= Quaternion.Slerp(key.Value, nextKey.Value, factor);
		        	} 
                    else presentRotation = key.Value;
	        		mLastPositions[a] = new Tuple<int,int,int>(mLastPositions[a].Item1, frame, mLastPositions[a].Item3);
	        	}

		        // ******** Scaling **********
		        Vector3D presentScaling = new Vector3D(1, 1, 1);
		        if(channel.mScalingKeys.Count() > 0) 
                {
		        	int frame = (time >= mLastTime) ? mLastPositions[a].Item3 : 0;
		        	while( frame < channel.mScalingKeys.Count() - 1){
		        		if( time < channel.mScalingKeys[frame+1].Time)
		        			break;
			        	frame++;
		        	}
		        	presentScaling = channel.mScalingKeys[frame].Value;
		        	mLastPositions[a] = new Tuple<int,int,int>(mLastPositions[a].Item1, mLastPositions[a].Item2, frame);
	        	}

	        	Matrix4x4 mat = new Matrix4x4(presentRotation.GetMatrix());

	        	mat.A1 *= presentScaling.X; mat.B1 *= presentScaling.X; mat.C1 *= presentScaling.X;
	        	mat.A2 *= presentScaling.Y; mat.B2 *= presentScaling.Y; mat.C2 *= presentScaling.Y;
	        	mat.A3 *= presentScaling.Z; mat.B3 *= presentScaling.Z; mat.C3 *= presentScaling.Z;
	        	mat.A4 = presentPosition.X; mat.B4 = presentPosition.Y; mat.C4 = presentPosition.Z;
		        mat.Transpose();
		        
                //may need to write custom TransformMatrix function here
                bonenode.LocalTransform = mat;
                //TransformMatrix(bonenode->second->LocalTransform, mat);
        	}
	        mLastTime = time;
        }
	    /*public void Save(StreamWriter file)
        {
            int nsize = Name.Count();
            file.WriteLine();
	        file.write(reinterpret_cast<char*>(&nsize), sizeof(uint32_t));// the size of the animation name
	        file.write(Name.c_str(), nsize);// the size of the animation name
	        file.write(reinterpret_cast<char*>(&Duration), sizeof(Duration));// the duration
	        file.write(reinterpret_cast<char*>(&TicksPerSecond), sizeof(TicksPerSecond));// the number of ticks per second
	        nsize = static_cast<uint32_t>(Channels.size());// number of animation channels,
	        file.write(reinterpret_cast<char*>(&nsize), sizeof(uint32_t));// the number animation channels
	        for(size_t j(0); j< Channels.size(); j++){// for each channel
		        nsize =static_cast<uint32_t>(Channels[j].Name.size());
		        file.write(reinterpret_cast<char*>(&nsize), sizeof(uint32_t));// the size of the name
		        file.write(Channels[j].Name.c_str(), nsize);// the size of the animation name

		        nsize =static_cast<uint32_t>(Channels[j].mPositionKeys.size());
		        file.write(reinterpret_cast<char*>(&nsize), sizeof(uint32_t));// the number of position keys
		        for(size_t i(0); i< nsize; i++){// for each channel
			        file.write(reinterpret_cast<char*>(&Channels[j].mPositionKeys[i].mTime), sizeof(Channels[j].mPositionKeys[i].mTime));// pos key
			        file.write(reinterpret_cast<char*>(&Channels[j].mPositionKeys[i].mValue), sizeof(Channels[j].mPositionKeys[i].mValue));// pos key
		        }

		        nsize =static_cast<uint32_t>(Channels[j].mRotationKeys.size());
		        file.write(reinterpret_cast<char*>(&nsize), sizeof(uint32_t));// the number of position keys
		        for(size_t i(0); i< nsize; i++){// for each channel
			        file.write(reinterpret_cast<char*>(&Channels[j].mRotationKeys[i].mTime), sizeof(Channels[j].mRotationKeys[i].mTime));// rot key
			        file.write(reinterpret_cast<char*>(&Channels[j].mRotationKeys[i].mValue), sizeof(Channels[j].mRotationKeys[i].mValue));// rot key
		        }

		        nsize =static_cast<uint32_t>(Channels[j].mScalingKeys.size());
		        file.write(reinterpret_cast<char*>(&nsize), sizeof(uint32_t));// the number of position keys
		        for(size_t i(0); i< nsize; i++){// for each channel
			        file.write(reinterpret_cast<char*>(&Channels[j].mScalingKeys[i].mTime), sizeof(Channels[j].mScalingKeys[i].mTime));// rot key
			        file.write(reinterpret_cast<char*>(&Channels[j].mScalingKeys[i].mValue), sizeof(Channels[j].mScalingKeys[i].mValue));// rot key
		        }
	        }
        }*/
	    /*public void Load(StreamReader file)
        {
            uint32_t nsize = 0;
	        file.read(reinterpret_cast<char*>(&nsize), sizeof(uint32_t));// the size of the animation name
	        char temp[250];
	        file.read(temp, nsize);// the size of the animation name
	        temp[nsize]=0;// null char
	        Name=temp;
	        //OUTPUT_DEBUG_MSG("Creating Animation named: "<<Name);
	        file.read(reinterpret_cast<char*>(&Duration), sizeof(Duration));// the duration
	        file.read(reinterpret_cast<char*>(&TicksPerSecond), sizeof(TicksPerSecond));// the number of ticks per second
	        file.read(reinterpret_cast<char*>(&nsize), sizeof(uint32_t));// the number animation channels
	        Channels.resize(nsize);
	        for(size_t j(0); j< Channels.size(); j++){// for each channel
		        nsize =0;
		        file.read(reinterpret_cast<char*>(&nsize), sizeof(uint32_t));// the size of the name
		        file.read(temp, nsize);// the size of the animation name
		        temp[nsize]=0;// null char
		        Channels[j].Name=temp;

		        nsize =static_cast<uint32_t>(Channels[j].mPositionKeys.size());
		        file.read(reinterpret_cast<char*>(&nsize), sizeof(uint32_t));// the number of position keys
		        Channels[j].mPositionKeys.resize(nsize);
		        for(size_t i(0); i< nsize; i++){// for each channel
			        file.read(reinterpret_cast<char*>(&Channels[j].mPositionKeys[i].mTime), sizeof(Channels[j].mPositionKeys[i].mTime));// pos key
			        file.read(reinterpret_cast<char*>(&Channels[j].mPositionKeys[i].mValue), sizeof(Channels[j].mPositionKeys[i].mValue));// pos key
		        }

		        nsize =static_cast<uint32_t>(Channels[j].mRotationKeys.size());
		        file.read(reinterpret_cast<char*>(&nsize), sizeof(uint32_t));// the number of position keys
		        Channels[j].mRotationKeys.resize(nsize);
		        for(size_t i(0); i< nsize; i++){// for each channel
			        file.read(reinterpret_cast<char*>(&Channels[j].mRotationKeys[i].mTime), sizeof(Channels[j].mRotationKeys[i].mTime));// pos key
			        file.read(reinterpret_cast<char*>(&Channels[j].mRotationKeys[i].mValue), sizeof(Channels[j].mRotationKeys[i].mValue));// pos key
		        }

		        nsize =static_cast<uint32_t>(Channels[j].mScalingKeys.size());
		        file.read(reinterpret_cast<char*>(&nsize), sizeof(uint32_t));// the number of position keys
		        Channels[j].mScalingKeys.resize(nsize);
		        for(size_t i(0); i< nsize; i++){// for each channel
			        file.read(reinterpret_cast<char*>(&Channels[j].mScalingKeys[i].mTime), sizeof(Channels[j].mScalingKeys[i].mTime));// pos key
			        file.read(reinterpret_cast<char*>(&Channels[j].mScalingKeys[i].mValue), sizeof(Channels[j].mScalingKeys[i].mValue));// pos key
		        }
	        }
	        mLastPositions.resize( Channels.size(), std::make_tuple( 0, 0, 0));
        }*/
        public List<Matrix4x4> GetTransforms(float dt)
        {
            return Transforms[GetFrameIndexAt(dt)];
        }
	    public int GetFrameIndexAt(float ptime)
        {
            // get a [0.f ... 1.f) value by allowing the percent to wrap around 1
	        ptime *= TicksPerSecond;
            float time = 0.0f;
	        if( Duration > 0.0)
		        time = ptime % Duration;
            float percent = time / Duration;
	        if(!PlayAnimationForward) 
                percent= (percent-1.0f)*-1.0f;
            // this will invert the percent so the animation plays backwards
            return (int)(Transforms.Count * percent);
        } 
    }
    public class SceneAnimator
    {
        public List<cAnimEvaluator> Animations; // a std::vector that holds each animation 
	    public int CurrentAnimIndex; //Current animation index

        //protected:		
        protected cBone Skeleton;/** Root node of the internal scene structure */
        protected Dictionary<string, cBone> BonesByName;/** Name to node map to quickly find nodes by their name */
        protected Dictionary<string, int> BonesToIndex;/** Name to node map to quickly find nodes by their name */
        protected Dictionary<string, int> AnimationNameToId;// find animations quickly
        protected List<cBone> Bones;// DO NOT DELETE THESE when the destructor runs... THEY ARE JUST COPIES!!
        protected List<Matrix4x4> Transforms;// temp array of transfrorms

        public SceneAnimator()
        {
            //SceneAnimator(): Skeleton(0), CurrentAnimIndex(-1) {}
        }
        //~SceneAnimator(){ Release(); }
        void Init(Scene pScene)
        {
            // this must be called to fill the SceneAnimator with valid data
            if(!pScene.HasAnimations) 
                return;
	        Release();

	        Skeleton = CreateBoneTree(pScene.RootNode, null);
	        ExtractAnimations(pScene);
	
	        for (int i = 0; i < pScene.MeshCount; ++i)
            {
		        Mesh mesh = pScene.Meshes[i];
		
		        for (int n = 0; n < mesh.BoneCount; ++n)
                {
			        Bone bone = mesh.Bones[n];
			        //std::map<std::string, cBone*>::iterator found = BonesByName.find(bone->mName.data);
			        if(BonesByName.ContainsKey(bone.Name))
                    {
                        // FOUND IT!!! woohoo, make sure its not already in the bone list
				        bool skip = false;
				        for(int j = 0; j < Bones.Count; j++)
                        {
					        string bname = bone.Name;
					        if(Bones[j].Name == bname) 
                            {
						        skip = true;// already inserted, skip this so as not to insert the same bone multiple times
						        break;
					        }
				        }
				        if(!skip)
                        {   
                            // only insert the bone if it has not already been inserted
                            var found = BonesByName[bone.Name];
					        string tes = found.Name;
                            
                            //this line may require a custom TransposeMatrix function
					        found.Offset = bone.OffsetMatrix;
					        found.Offset.Transpose(); // transpose their matrix to get in the correct format
					        Bones.Add(found);
					        BonesToIndex[bone.Name] = Bones.Count()-1;
				        }
			        } 
		        }
	        }
	        float timestep = 1.0f/30.0f;// 30 per second
	        for(int i = 0; i< Animations.Count(); i++)
            {
                // pre calculate the animations
		        SetAnimIndex(i);
		        float dt = 0;
		        for(float ticks = 0; ticks < Animations[i].Duration; ticks += Animations[i].TicksPerSecond/30.0f)
                {
			        dt +=timestep;
			        Calculate(dt);
                    List<Matrix4x4> trans = new List<Matrix4x4>();
                    for (int a = 0; a < Transforms.Count(); ++a)
                    {
                        Matrix4x4 rotationMatrix = Bones[a].Offset * Bones[a].GlobalTransform;
                        trans.Add(rotationMatrix);
                    }
                    Animations[i].Transforms.Add(trans);
		        }
	        }
	        //OUTPUT_DEBUG_MSG("Finished loading animations with "<<Bones.size()<<" bones");
        }
        void Release()
        {
            // frees all memory and initializes everything to a default state
        }
        void Save(StreamWriter file)
        {

        }
        void Load(StreamReader file)
        {

        }
        bool HasSkeleton() 
        {
            return Bones.Any();// lets the caller know if there is a skeleton present
        }
        // the set animation returns whether the animation changed or is still the same.
        protected void ExtractAnimations(Scene pScene)
        {
            //OUTPUT_DEBUG_MSG("Extracting Animations . . ");
	        for(int i = 0; i < pScene.AnimationCount; i++)
            {
		        Animations.Add(new cAnimEvaluator(pScene.Animations[i])); // add the animations
	        }
	        for(int i = 0; i < Animations.Count(); i++)
            {
                // get all the animation names so I can reference them by name and get the correct id
		        AnimationNameToId.Add(Animations[i].Name, i);
	        }
	        CurrentAnimIndex=0;
	        SetAnimation("Idle");
        }
        bool SetAnimIndex(int pAnimIndex)
        {
            // this takes an index to set the current animation to
            if (pAnimIndex >= Animations.Count()) 
                return false; // no change, or the animations data is out of bounds
            int oldindex = CurrentAnimIndex;
            CurrentAnimIndex = pAnimIndex; // only set this after the checks for good data and the object was actually inserted
            return oldindex != CurrentAnimIndex;
        }
        bool SetAnimation(string name)
        {
            // this takes a string to set the animation to, i.e. SetAnimation("Idle");
            if(AnimationNameToId.ContainsKey(name))
            {
                CurrentAnimIndex = AnimationNameToId[name];
                return true;
            }
            return false;
        }
        // the next two functions are good if you want to change the direction of the current animation. You could use a forward walking animation and reverse it to get a walking backwards
        void PlayAnimationForward() 
        { 
            Animations[CurrentAnimIndex].PlayAnimationForward = true; 
        }
        void PlayAnimationBackward() 
        {   
            Animations[CurrentAnimIndex].PlayAnimationForward = false; 
        }
        //this function will adjust the current animations speed by a percentage. So, passing 100, would do nothing, passing 50, would decrease the speed by half, and 150 increase it by 50%
        void AdjustAnimationSpeedBy(float percent) 
        { 
            Animations[CurrentAnimIndex].TicksPerSecond*=percent/100.0f; 
        }
        //This will set the animation speed
        void AdjustAnimationSpeedTo(float tickspersecond) 
        { 
            Animations[CurrentAnimIndex].TicksPerSecond=tickspersecond; 
        }
        // get the animationspeed... in ticks per second
        float GetAnimationSpeed() 
        { 
            return Animations[CurrentAnimIndex].TicksPerSecond; 
        }
        // get the transforms needed to pass to the vertex shader. This will wrap the dt value passed, so it is safe to pass 50000000 as a valid number
        List<Matrix4x4> GetTransforms(float dt)
        { 
            return Animations[CurrentAnimIndex].GetTransforms(dt); 
        }
        int GetAnimationIndex() 
        { 
            return CurrentAnimIndex; 
        }
        string GetAnimationName() 
        { 
            return Animations[CurrentAnimIndex].Name;  
        }
        //GetBoneIndex will return the index of the bone given its name. The index can be used to index directly into the vector returned from GetTransform
        int GetBoneIndex(string bname)
        {
            foreach(KeyValuePair<string, int> kv in BonesToIndex)
            {
                if(kv.Key == bname)
                    return kv.Value;
            }
            return -1; 
        }
        //GetBoneTransform will return the matrix of the bone given its name and the time. be careful with this to make sure and send the correct dt. If the dt is different from what the model is currently at, the transform will be off
	    Matrix4x4 GetBoneTransform(float dt, string bname) 
        { 
            int bindex = GetBoneIndex(bname); 
            if(bindex == -1) 
                return Matrix4x4.Identity; //identity matrix? 
            return Animations[CurrentAnimIndex].GetTransforms(dt)[bindex]; 
        }
	    // same as above, except takes the index
	    Matrix4x4 GetBoneTransform(float dt, int bindex) 
        {  
            return Animations[CurrentAnimIndex].GetTransforms(dt)[bindex]; 
        } 
	    protected void SaveSkeleton(StreamWriter file, cBone pNode)
        {

        }
	    /*protected cBone LoadSkeleton(StreamReader file, cBone pNode)
        {            
            cBone* internalNode = new cBone();// create a node
	        internalNode->Parent = parent; //set the parent, in the case this is theroot node, it will be null
	        uint32_t nsize=0;
	        file.read(reinterpret_cast<char*>(&nsize), sizeof(uint32_t));// the number of chars
	        char temp[250];
	        file.read(temp, nsize);// the name of the bone
	        temp[nsize]=0;
	        internalNode->Name = temp;
	        BonesByName[internalNode->Name] = internalNode;// use the name as a key
	        file.read(reinterpret_cast<char*>(&internalNode->Offset), sizeof(internalNode->Offset));// the bone offsets
	        file.read(reinterpret_cast<char*>(&internalNode->OriginalLocalTransform), sizeof(internalNode->OriginalLocalTransform));// original bind pose
	
	        internalNode->LocalTransform = internalNode->OriginalLocalTransform;// a copy saved
	        CalculateBoneToWorldTransform(internalNode);

	        file.read(reinterpret_cast<char*>(&nsize), sizeof(uint32_t));// the number of children

	        // continue for all child nodes and assign the created internal nodes as our children
	        for( unsigned int a = 0; a < nsize && file; a++){// recursivly call this function on all children
		        internalNode->Children.push_back(LoadSkeleton(file, internalNode));
	        }
	        return internalNode;
        }*/
	    protected void UpdateTransforms(cBone pNode)
        {
            CalculateBoneToWorldTransform(pNode);
            // update global transform as well
            foreach(cBone c in pNode.Children)
            {
                UpdateTransforms(c);
            }
        }
	    protected void CalculateBoneToWorldTransform(cBone child)
        {
            /** Calculates the global transformation matrix for the given internal node */
            child.GlobalTransform = child.LocalTransform;
            cBone parent = child.Parent;
            while (parent != null)
            {// this will climb the nodes up along through the parents concentating all the matrices to get the Object to World transform, or in this case, the Bone To World transform
                child.GlobalTransform *= parent.LocalTransform;
                parent = parent.Parent;// get the parent of the bone we are working on 
            }
        }
	    protected void Calculate(float pTime)
        {
            if (CurrentAnimIndex < 0 || CurrentAnimIndex >= Animations.Count())
                return; // invalid animation
            Animations[CurrentAnimIndex].Evaluate(pTime, BonesByName);
            UpdateTransforms(Skeleton);
        }
        protected void CalcBoneMatrices()
        {
            for (int a = 0; a < Transforms.Count(); a++)
            {
                Transforms[a] = Bones[a].Offset * Bones[a].GlobalTransform;
                //not sure what this does, it wasa commented out
                /*
                mat4 rotationmat = Transforms[a];
                quat q;
                q.frommatrix(rotationmat);
		
                vec4 dual[2] ;
                UQTtoUDQ( dual, q, Transforms[a].row3_v);
                QuatTransforms[a].row0_v =dual[0];
                QuatTransforms[a].row1_v =dual[1];
                */
            }
        } 
        protected cBone CreateBoneTree(Node pNode, cBone pParent)
        {
            // Recursively creates an internal node structure matching the current scene and animation.
           	cBone internalNode = new cBone();// create a node
            internalNode.Name = pNode.Name; // get the name of the bone
	        internalNode.Parent = pParent; //set the parent, in the case this is theroot node, it will be null

	        BonesByName[internalNode.Name] = internalNode; // use the name as a key

            //this may require a custom MatrixTranpose function
            internalNode.LocalTransform = pNode.Transform;
            internalNode.LocalTransform.Transpose();
	        internalNode.OriginalLocalTransform = internalNode.LocalTransform;// a copy saved
	        CalculateBoneToWorldTransform(internalNode);
	        // continue for all child nodes and assign the created internal nodes as our children
	        for(int a = 0; a < pNode.ChildCount; a++)
            {// recursivly call this function on all children
		        internalNode.Children.Add(CreateBoneTree(pNode.Children[a], internalNode));
	        }
	        return internalNode;
        }
    }
}
