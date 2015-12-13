using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmackBrosClient
{
    class Hitbox
    {       
        private static bool hitBoxesVisible = false;
        private int curFrame = 0;
        private int totalFrames = 0;
        private int owner;
        private bool isActive = true;

        public List<int> collidedWith = new List<int>();
        public Vector2 position;    
    
        public Hitbox(Vector2 startPos, int ownerID)
        {
            owner = ownerID;
            position = startPos;
        }
        public void Update(List<Smacker> smackers)
        {
            if (curFrame >= totalFrames)
                isActive = true;
            curFrame++;
            foreach(Smacker smacker in smackers)
            {
                //collision logic
            }
        }
        public bool HitBoxFinished
        {
            get { return isActive; }
        }
    }
}
