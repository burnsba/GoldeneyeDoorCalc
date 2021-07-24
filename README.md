# GoldeneyeDoorCalc
Doors, how do they work?

-----

# Introduction

This program anylyzes door configuration for each level in Goldeneye 007. The number of frames a door takes to complete the entire open sequence is calculated. The results for each door are written to output. Additionally, for each stage, the doors are grouped by the "OpenTime", and compared against the same door using PAL configuration settings. This prints a summary of the doors and summary of differences across versions.

See full results of output in file [out.txt](out.txt).

Example level output:

    jungle (NTSC)
    
    Preset=0x0, obj=0x1b5718, pos=(18528, -2788), ToMaxSpeedFrames=34=0.57 s, OpenFrames=93=1.55 s
    Preset=0x1, obj=0x1b5818, pos=(18528, -2915), ToMaxSpeedFrames=34=0.57 s, OpenFrames=93=1.55 s
    
    
    jungle (PAL)
    
    Preset=0x0, obj=0x19caa8, pos=(18528, -2788), ToMaxSpeedFrames=34=0.68 s, OpenFrames=83=1.66 s
    Preset=0x1, obj=0x19cba8, pos=(18528, -2915), ToMaxSpeedFrames=34=0.68 s, OpenFrames=83=1.66 s
    
    Level summary:
    Doors with (NTSC) OpenFrames=93 => 1.55 s NTSC, 1.66 s PAL, delta = 0.11 s
    Max door delta time = 0.11 s
    Min door delta time = 0.11 s
    
-----

# Technical Details

This program uses configuration data extracted from memory once the level is loaded, using the bizhawk emulator. I extracted the level date for each stage on NTSC then did the same for PAL.

Setup is dumped using a modified version of `dump_map_data.lua` from https://github.com/whiteted-strats/GE_Wiki_Maps to include door configuration options. See my fork at https://github.com/burnsba/GE_Wiki_Maps/blob/master/dump_map_data.lua .

I then converted the level information from python to json using the script in `data/convert_to_json.py` in my forked repo.

Each door is run through an "opening" algorithm. The exact method used is not currently known for Goldeneye, but we do have an exact match from Perfect Dark; it is assumed this algorithm is identical.

The "opening" algorithm simply steps through the door state similar to what would happen in-game and counts the number of frames it takes for the door to open. The appropriate framerate conversion is then applied to calculate the amount of time it takes for the door to fully open. A summary of each door is then printed:

    Preset=0x1, obj=0x1b5818, pos=(18528, -2915), ToMaxSpeedFrames=34=0.57 s, OpenFrames=93=1.55 s
    
The preset and object key are given by the lua script; these differ from the `setup` configurations available from the PerfectGold editor. The `pos` is the truncated (x,z) position of the door. `ToMaxSpeedFrames` is the number of frames it took for the door to reach max speed, and the `OpenFrames` value is the number of frames it took to complete the open sequence. Both the frame values are converted to seconds according to the system.

Since many doors share the same configuration parameters, doors are grouped by the OpenFrames paramater to provide a summary of results for the level. The difference between NTSC and PAL is calculated for each group, and a total min and max delta are tracked for each level.