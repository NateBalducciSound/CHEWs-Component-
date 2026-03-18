global float rainIntensity;   
global float windIntensity;   
global float movement;        
global float elevation;       
global float isIndoors;   
global float rainVolume; 
global float rainLPF; 
global float rainHPF;

global float dropLPF; 
global float dropHPF;   
global float masterLPF; 
global float masterHPF; 

// Default values
0.5 => rainVolume;      
0.4 => rainIntensity;
0.2 => windIntensity;
0.0 => movement;
0.0 => elevation;
0   => isIndoors;

200 => rainHPF;
5000 => rainLPF;

4000 => dropLPF;
200  => dropHPF;

8000 => masterLPF;
80   => masterHPF;
//sound chain
Gain rainMaster => LPF masterLPFNode => HPF masterHPFNode => dac;

Noise rainBed => LPF bedLPF => HPF bedHPF => Pan2 bedPan => rainMaster;

//main bulk of sound
rainBed => bedPan => LPF indoorLPF => Gain indoorGain => rainMaster;
LPF outdoorLPF => Gain outdoorGain => rainMaster;

outdoorGain.gain(1.0);

// link rain LPF
rainLPF => outdoorLPF.freq;

// indoor base tone (for fade)
200 => indoorLPF.freq;
0.0 => indoorGain.gain;

// bed tone
5000 => bedLPF.freq;
200  => bedHPF.freq;


//conitnuosly update values and scale
fun void updateBed() {
    while(true) {
        (0.001 + rainIntensity*0.02 + windIntensity*0.015) => rainBed.gain;

        (2500 + rainIntensity*2000) => bedLPF.freq;
        windIntensity*Math.random2f(-0.5,0.5) => bedPan.pan;

        rainVolume => rainMaster.gain;

        rainHPF  => masterHPFNode.freq;
        rainLPF  => masterLPFNode.freq;

        40::ms => now;
    }
}
spork ~ updateBed();
//class for little drops on higher rain density
class Drop {
    Impulse imp;
    ResonZ m1; ResonZ m2; ResonZ m3;
    Noise splash => LPF spl_lpf => HPF spl_hpf => Gain splGain;


    LPF dropLPFNode;
    HPF dropHPFNode;

    ADSR env;
    Pan2 pan;
    Gain mix;

    fun void init() {
        
        imp => m1 => env => mix => dropLPFNode => dropHPFNode => pan => outdoorLPF;
        imp => m2 => env => mix => dropLPFNode => dropHPFNode => pan => outdoorLPF;
        imp => m3 => env => mix => dropLPFNode => dropHPFNode => pan => outdoorLPF;

        splash => splGain => dropLPFNode => dropHPFNode => pan => outdoorLPF;

        
        imp => m1 => env => mix => dropLPFNode => dropHPFNode => pan => indoorLPF;
        imp => m2 => env => mix => dropLPFNode => dropHPFNode => pan => indoorLPF;
        imp => m3 => env => mix => dropLPFNode => dropHPFNode => pan => indoorLPF;

        splash => splGain => dropLPFNode => dropHPFNode => pan => indoorLPF;

        env.set(1::ms, 12::ms, 0.0, 40::ms);
        0.0 => splGain.gain;

        spl_lpf.freq(5000);
        spl_hpf.freq(200);

        dropLPF => dropLPFNode.freq;
        dropHPF => dropHPFNode.freq;
    }
    //modes for resz
    fun void setModes(float size) {
        (600 + (1.0-size)*400 + Math.random2f(-30,30)) => m1.freq;
        (1500 + (1.0-size)*900 + Math.random2f(-60,60)) => m2.freq;
        (2600 + (1.0-size)*1600 + Math.random2f(-120,120)) => m3.freq;

        0.7 => m1.Q;
        0.7 => m2.Q;
        0.7 => m3.Q;
    }

    fun void detail(float size) {
        size * 0.0002 => splGain.gain;
        (7000-size*1200) => spl_lpf.freq;
        (250 + size*80) => spl_hpf.freq;
    }

    fun void play(float amp, float size, float pos) {
        Math.random2f(0.5,3.0) => imp.next;
        amp => env.gain;
        pos => pan.pan;

        setModes(size);
        detail(size);

        // dynamic filter updates
        dropLPF  => dropLPFNode.freq;
        dropHPF  => dropHPFNode.freq;

        env.keyOn();
        6::ms => now;
        env.keyOff();
    }
}

//drop class array spawner
Drop drops[32];
for(0 => int i; i < drops.size(); i++) drops[i].init();

fun Drop getDrop() { return drops[Math.random2(0, drops.size()-1)]; }


fun void spawnLoop() {
    while(true) {
        ((1.0 - rainIntensity) * 450::ms) + Math.random2f(4,25)::ms => now;
        spork ~ spawnDrop();
    }
}
spork ~ spawnLoop();

fun void spawnDrop() {
    Drop d; getDrop() @=> d;

    rainIntensity*0.001=> float amp;
    Math.random2f(0.3,1.3) => float size;
    movement*0.4 +
    windIntensity*Math.random2f(-0.25,0.25) +
    Math.random2f(-1,1)*0.2
        => float pos;

    d.play(amp, size, pos);
}
//fade for indoor trigger
fun void indoorFade() {
    0.1 => float fadeFactor;
    while(true) {
        float targetOut;
        float targetIn;
        float freqTarget;

        if(isIndoors == 1) {
            0.0 => targetOut;
            1.0 => targetIn;
            1000 => freqTarget; // indoor LPF
        } else {
            1.0 => targetOut;
            0.0 => targetIn;
            5000 => freqTarget;
        }

        outdoorGain.gain() + (targetOut - outdoorGain.gain())*fadeFactor => outdoorGain.gain;
        indoorGain.gain()  + (targetIn  - indoorGain.gain())*fadeFactor  => indoorGain.gain;
        indoorLPF.freq()   + (freqTarget - indoorLPF.freq())*fadeFactor => indoorLPF.freq;

        10::ms => now;
    }
}
spork ~ indoorFade();


// Keep script alive
while(true) 1::second => now;
