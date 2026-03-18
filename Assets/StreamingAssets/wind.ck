Noise n;
Gain src;
n => src => blackhole;
// requirement shiz
0 => int mach;
if (mach == 1){
    me.dir() + "print.ck" => string printPath;
    Machine.add(printPath) => int printID;
}

// requirement shiz
SinOsc s => blackhole;
HevyMetl h => blackhole;
0 => s.freq;
0 => s.gain;
0 => h.gain;
0 => h.freq;
// sound chain
ResonZ hiPass;
Gain hiGain;
ResonZ loPass;
Gain loGain;
ResonZ filterWindSpeed;
Gain speedWindGain;

Gain buss => Pan2 pan => Gain master => dac;

// filter q settings
0.1 => hiPass.Q;
0.3 => loPass.Q;
0.4 => filterWindSpeed.Q;

// base gains
0.5 => loGain.gain;
0.5 => hiGain.gain;
0.5 => speedWindGain.gain;

// signal routing
src => loPass => loGain => buss;
src => hiPass => hiGain => buss;
src => filterWindSpeed => speedWindGain => buss;

// base filter freqs
200 => float loFreq;
3500 => float hiFreq;
500 => float speedWindFreq;

loPass.set(loFreq,2);
hiPass.set(hiFreq,2);
filterWindSpeed.set(speedWindFreq,2);

0.5 => master.gain;


//unity vars
global float playerElevation;
global float playerSpeed;
global float playerPosX;
global float playerPosY;
global float windVolume;


//smoothing within func
0.1 => float smooth;

0 => float elevSmoothed;
0 => float speedSmoothed;
0 => float posXSmoothed;
0 => float posYSmoothed;


// clamp helper so it doesnt go crazy
fun float clamp(float x, float a, float b) {
    if(x < a) return a;
    if(x > b) return b;
    return x;
}


// wind variation
Noise modN => Gain modGain;
0.2 => modGain.gain;

SinOsc sLow => Gain slowGain;
0.03 => slowGain.gain;
0.02 => sLow.freq;    

SinOsc sFast => Gain fastGain;
0.06 => fastGain.gain;
0.01 => sFast.freq;    


// gust gen 
0 => global float gust;

spork ~ gustEngine();

fun void gustEngine() {
    while (true) {

        (4 + Math.random2f(0, 8))::second => now;

        float i;

        // gust ramp up
        0 => i;
        while (i < 1) {
            i => gust;
            i + 0.02 => i;
            40::ms => now;
        }

        // gust ramp down
        1 => i;
        while (i > 0) {
            i => gust;
            i - 0.05 => i;
            40::ms => now;
        }
    }
}


//smooth sporks
spork ~ smoothGlobals();
spork ~ updateMaster();


//wind loop
while(true)
{
     (modN.last() * 0.15) + slowGain.last() + fastGain.last() + (gust * 0.4) => float mod;

    // elevation effect to lower
    (elevSmoothed * 0.25) => float elevEffect; 
    elevEffect * elevEffect => elevEffect;    

    // Low band
    (160 + elevEffect * 30 + mod * 40) => loFreq;
    clamp(loFreq, 60, 2000) => loFreq;
    loPass.set(loFreq, 2);

    // High band
    (2600 + elevEffect * 120 + mod * 250) => hiFreq;
    clamp(hiFreq, 800, 9000) => hiFreq;
    hiPass.set(hiFreq, 1.5);

    // Gains shift with speed and  mod
    (0.05 + speedSmoothed * 0.15 + mod * 0.05) => loGain.gain;
    (0.04 + speedSmoothed * 0.08 + mod * 0.03) => hiGain.gain;

    (120 + speedSmoothed * 500 + mod * 200 + gust * 350) => speedWindFreq;
    clamp(speedWindFreq, 40, 4000) => speedWindFreq;
    filterWindSpeed.set(speedWindFreq, 1.2);

    (0.0003 + speedSmoothed * 0.003 + mod * 0.0008) => speedWindGain.gain;

    // Panning 
    (-0.2 + posXSmoothed * 0.008) => pan.pan;

    20::ms => now;
}

// unity thread smoothing
fun void smoothGlobals() {
    while(true) {
        elevSmoothed + smooth*(playerElevation - elevSmoothed) => elevSmoothed;
        speedSmoothed + smooth*(playerSpeed    - speedSmoothed) => speedSmoothed;
        posXSmoothed + smooth*(playerPosX      - posXSmoothed) => posXSmoothed;
        posYSmoothed + smooth*(playerPosY      - posYSmoothed) => posYSmoothed;
        20::ms => now;
    }
}

//master gain change
fun void updateMaster() {
    while(true) {
        windVolume * 0.5 => master.gain;
        20::ms => now;
    }
}
