using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmackBrosClient
{
    class Smacker
    {
        public Vector2 Position;
        public short smackerID;
        public SmackerState state;
        public int EnumeratedState
        {
            get
            {
                return (int)state;
            }
            set
            {
                state = (SmackerState)value;
            }
        }
    }
    enum SmackerState
    {
        DeadDown,//0
        DeadRight,
        DeadLeft,
        DeadUpStar,
        DeadUpCamera,
        Rebirth,//5
        RebirthWait,
        Standing,
        WalkSlow,
        WalkMedium,
        WalkFast,//10
        Turn,
        TurnRun,
        Dash,
        Run,
        RunBrake,//15
        JumpSquat,
        JumpForward,
        JumpBackward,
        JumpAirF,
        JumpAirB,//20
        Fall,
        FallF,
        FallB,
        FallAerial,
        FallAerialF,//25
        FallAerialB,
        FallSpecial,
        FallSpecialF,
        FallSpecialB,
        Tumbling,//30
        Squat,
        SquatWait,
        SquatRv,//going from crouch to stand
        Landing,
        LandingFallSpecial,//35
        Attack11,
        Attack12,
        Attack13,
        Attack100Start,
        Attack100Loop,//40
        Attack100End,
        AttackDash,
        AttackUtilt,
        AttackDtilt,
        AttackFtilt,//45
        AttackFsmash,
        AttackDsmash,
        AttackUsmash,
        AttackAirNeutral,
        AttackAirUp,//50
        AttackAirForward,
        AttackAirBack,
        AttackAirDown,
        LandingAirNeutral,
        LandingAirUp,//55
        LandingAirForward,
        LandingAirBack,
        LandingAirDown,
        Special,
        DamageAir,//60
        DamageGround,
        ShieldOn,
        ShieldHold,
        ShieldBreak,
        ShieldOff,//65
        ShieldStunned,
        ShieldReflect,
        NoTechBounceUp,
        DownWaitUp, //Laying on ground facing up
        DownDamageUp, //Getting hit laying on ground facing up //70
        DownStandUp,
        DownSpotUp,
        DownAttackU, //  Get up attack from ground face up
        DownBackU,
        DownForwardU,//75
        NoTechBounceDown,
        DownWaitDown,
        DownDamageDown,
        TechInPlace,
        TechForward,//80
        TechBack,
        TechWall,
        Grab,
        GrabPull, //pulling character in after grab
        GrabDash, //boost grab //80
        GrabDashPull, //pull after boost grab
        GrabWait,
        GrabPummel,
        GrabBreak, //grab broken
        ThrowUp, //85
        ThrowDown,
        ThrowForward,
        ThrowBack,
        CapturePull,
        CaptureWait,//90
        CaptureDamage,
        CaptureBreak,
        CaptureThrowUp,
        CaptureThrowDown,
        CaptureThrowForward,//95
        CaptureThrowBack,
        AirDodge,
        Helpless,
        LedgeHang,
    }
}
