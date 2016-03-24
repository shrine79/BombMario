using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using RC_Framework;

namespace BombMario
{
    public enum FaceDir
    {
        left = -1,
        right = 1
    }

    public enum Status
    {
        talking = 0,
        running = 1,
        jumping = 2
    }


    class player : Sprite3
    {
        public float speed { get; set; }
        private FaceDir dir;
        private FaceDir dir_pre;
        private Status st;
        private Status pre_st;
        private Texture2D[] textures;
        private Vector2[] ani_frames;
        Vector2 origin;



        // Default constructor
        public player() { varInit(FaceDir.left, Status.talking, 0, 0); }
        /// <summary>
        /// standard constructor
        /// </summary>
        /// <param name="visibleZ"></param>
        /// <param name="texZ">default texture</param> 
        /// <param name="x"></param>
        /// <param name="y"></param>
        public player(bool visibleZ, Texture2D texZ, float x, float y) : base(visibleZ, texZ, x, y)
        {
            varInit(FaceDir.left, Status.talking, texZ, x, y);
        }
        /// <summary>
        /// constructor with player facing direction
        /// </summary>
        /// <param name="visibleZ"></param>
        /// <param name="texZ">default texture</param> 
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="dir_v">facing direction</param> 
        public player(bool visibleZ, Texture2D texZ, float x, float y, FaceDir dir_v) : base(visibleZ, texZ, x, y)
        {
            varInit(dir_v, Status.talking, texZ, x, y);
        }


        public player(bool visibleZ, Texture2D texZ, float x, float y, FaceDir dir_v, Vector2 frames_num) : base(visibleZ, texZ, x, y)
        {
            varInit(dir_v, Status.talking, texZ, x, y);
            ani_frames[(int)Status.talking] = frames_num;
            setAnimation((int)ani_frames[(int)Status.talking].X, (int)ani_frames[(int)Status.talking].Y);
        }


        private void varInit(FaceDir dir_v, Status st_v, float x, float y)
        {
            dir = dir_v;
            st = st_v;
            pre_st = st;
            speed = 0;
            origin = new Vector2(x, y);
            textures = new Texture2D[] {null, null, null};
            ani_frames = new Vector2[] { new Vector2(0,0), new Vector2(0, 0), new Vector2(0, 0) };
        }

        private void varInit(FaceDir dir_v, Status st_v, Texture2D tex_v, float x, float y)
        {
            varInit(dir_v, st_v, x, y);
            textures[0] = tex_v;
            
        }

        public void setTextures(Texture2D Tex, Status st_id, Vector2 frame_num)
        {
            textures[(int)st_id] = Tex;
            ani_frames[(int)st_id] = frame_num;
        }

        public FaceDir getDir() { return dir; }
        public void setDir(FaceDir value)
        {
            if(dir != dir_pre)
            {
                // don't change facing direction while jumping
                if (dir != value)
                {
                    dir_pre = dir;
                    dir = value;
                    if (st != Status.jumping)
                    {
                        if (dir == FaceDir.right) setFlip(SpriteEffects.FlipHorizontally);
                        else setFlip(SpriteEffects.None);
                    }
                }
            }
        }

        public Status getSt() { return st; }
        public void setSt(Status value)
        {
            if (st != value)
            {
                pre_st = st;
                st = value;
                // Change animation
                setTexture(textures[(int)value], true);
                setAnimation((int)ani_frames[(int)value].X, (int)ani_frames[(int)value].Y);
            }
        }


        public override void Update(GameTime gameTime)
        {
            switch (st)
            {
                case Status.jumping:
                    if (st != pre_st)
                    {
                        // entering jumping state
                        setDeltaSpeed(new Vector2(getDeltaSpeed().X, -5));
                        // update previous status
                        pre_st = st;
                    }
                    
                    else if (getBoundingBoxAA().Bottom >= origin.Y + 20)
                    {
                        // exiting jumping state
                        setDeltaSpeed(new Vector2(0, 0));
                        setPosY(origin.Y);
                        setSt(Status.talking);
                        if (dir == FaceDir.right) setFlip(SpriteEffects.FlipHorizontally);
                        else setFlip(SpriteEffects.None);
                    }
                    else
                    {
                        // stay in jumping state
                        setDeltaSpeed(new Vector2((int)dir * speed, getDeltaSpeed().Y + 0.2f));
                    }
                    break;
                case Status.running:
                    if (st != pre_st || dir != dir_pre)
                    {
                        // entering running state
                        setDeltaSpeed(new Vector2((int)dir * speed, getDeltaSpeed().Y));
                        // update previous status
                        pre_st = st;
                    }
                    else
                    {
                        // do something here
                    }
                    break;
                case Status.talking:
                    if (st != pre_st)
                    {
                        setDeltaSpeed(new Vector2(0, 0));
                        setPosY(origin.Y);
                        // update previous status
                        pre_st = st;
                    }
                    else
                    {
                        // do something here
                    }
                    break;
            }


            base.Update(gameTime);
        }

        private void setAnimation(int XFrameNum, int YFrameNum)
        {
            setXframes(XFrameNum);
            setYframes(YFrameNum);
            setAnimationSequence(generateAnimationSeq(XFrameNum - 1, YFrameNum - 1), 0, XFrameNum * YFrameNum - 1, 7);
            setAnimFinished(0);
            setWidthHeight(32, 32);
            setBBToFrameInTex();
            setHSoffset(new Vector2(tex.Width / (Xframes * 2), tex.Height / (Yframes * 2)));
            animationStart();
        }

        /// <summary>
        /// Fill a vector2 [] with vector2 entry range from (1,1) to (x, y)
        /// </summary>
        /// <param name="x">x start from 0</param>
        /// <param name="y">y start from 0</param>
        /// <returns></returns>
        protected Vector2[] generateAnimationSeq(int x, int y)
        {
            Vector2[] ret = new Vector2[(x + 1) * (y + 1)];
            for (int i = 0; i <= x; i++)
            {
                for (int j = 0; j <= y; j++)
                {
                    ret[i + j] = new Vector2(i, j);
                }
            }
            return ret;
        }




    }
}
