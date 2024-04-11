# Built-in Audio Player
I don't know why I made this<br>
One of my friend sent me a program that can play ```steel pipe``` sound with a button<br>
And my another friend want to change the audio but he can't decompile it

## Environment Dependence
Godot 4.2.1 Stable Mono Official<br>
.NET 8.0.100 (well actually, .net8 is not necessary, if you want to use other version of .net, just change the `TargetFramework` in `BuiltInAudioPlayer.csproj`)<br>

## How 2 use
Export this project to ```exe```<br>
then copy the ```options.ini``` at the same folder with exe file<br>
<br>
Find a audio file (technically it can be ```.ogg```/```.wav```/```.mp3```, but only ```.ogg``` is useable now and I don't know why, ```mp3``` is not loadable, and ```wav``` can only make noise)<br>
Rename the file to ```audio.<file extension>```<br>
then put it at the same folder with exe file<br>
Run the exe and it will work<br>
It should be ok to pack them into single exe, ```options.ini``` won't be edit by program<br>

## options.ini
```AdditionLabel```: Addition Instructions, only show "Current: " if False ```(default: True)```<br>
```Pauseble```: Allow user to pause audio, remove pause button when False ```(default: True)```<br>
```AllowSelecting```: When False, it won't pop-up a window to select audio file when no audio file was found ```(default: True)```<br>
```ShowProgressBar```: Show progress bar if True ```(default: True)```<br>
```AllowDragProgressBar```: Progress bar will be draggable if True (Only works when ```ShowProgressBar``` is True) ```(default: True)```<br>
```ForceLocale```: Lock language and remove language option, change it to "" to disable this feature ```(default: "")```<br>

## To-do list
- [ ] Fix ```.wav```<br>
- [ ] Fix ```.mp3```<br>
- [x] Draggable progress bar