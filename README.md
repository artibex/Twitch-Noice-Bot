# Twitch Noice Bot

### What is this Bot for?
This Twitch bot is using a Text to Speech Synth to read out the chat for you. Listen to the whole conversation with your community while not even looking away from the thing you do!

<br></br>
### Key Features
- Engage with your Community while not getting distrected by a Textwall, just listen the chatmembers what they have to say!
- Quick Start! (Ready in 1 minute)
- Easy to use
- Adjustable Spam Protection (Cut out really long chat messages for example)
- Aliaslist (XxXCoolKidXxX is spoken as "John")
- Blacklist (Remove those pesky bot's out of your ear)
- Custom Bridgewords (John says Kappa; John: Kappa; John tells you kappa)
- Adjustable TTS speed (keep it slow or gotta go fast!)
- No installation requiered! (Just a Folder)
- Helpful for streamers or users with a visual handicap
- Helpful for streamers with a reading disability

<br></br>
### Installation Guide
1. Download the latest release
2. Unzip all files into a empty folder
3. Go into the "Settings.txt" and add your channelname after "channel=" example: "channel=korpsian"
4. Go into the "Settings.txt" and add your oauth key after "oauth=" you can generate one [here](https://twitchapps.com/tmi/) example: oauth="oauth:SoMeCoOlTeXt123456789"

### Settings Explenations
| Command                 	| Explenation                                                                                      	|
|-------------------------	|--------------------------------------------------------------------------------------------------	|
| ircclient=              	| The IRC Server you are Connecting to                                                             	|
| port=                   	| The Port you are using to connect to the IRC Server                                              	|
| botname=                	| The name of the Bot                                                                              	|
| channelname=            	| The name of the channel you want to connect to                                                   	|
| oauth=                  	| The OAuth Key you use to establish this connection                                               	|
| ttsbasespeed=           	| The normal speed of the Text ot Speech Synth                                                     	|
| ttsmaxspeed=            	| The fastest speed the Speech Synth will go (longer texts will be spoken faster                   	|
| maxtextlength=          	| The max text length the bot will speak, after that it will stop                                  	|
| spamthreshold=          	| if a user is spamming a unwanted characters (e.g. 30 or more) the comment get's removed          	|
| removeemojis=           	| If true, all Unicode Emojis will be removed of a commant (Twitch Emotes are still active)        	|
| badcharlist=            	| all the bad characters that count to the spamthreshold (e.g. someone is spamming ##############) 	|
| ttsoutputdevice=        	| Preconfigure the Audio Device ID so the programm is starting up without askin for a device       	|
| soundboardoutputdevice= 	| Preconfigure the Audio Device ID so the programm is starting up without askin for a device       	|

<br></br>
### File Explenations

#### Aliaslist.text
In the AliasList.txt are all Usernames collected with the desiered nickname.
File example "AliasList.txt":
korpsian,the developer
XXCoolKid1000XX,John
ls_1235433,Pepe
[Username],[Nickname] (without [])

<br></br>
#### Blacklist.txt
In the Blacklist.txt file are all ignored Users to find. File Example:
Cloudbot
AudioAlert
Followerbot
XxUncoolTrollxX
[Username] (without [])

<br></br>
#### BridgeWordList.txt
In the BridgeWordList.txt are all connector words to find. "Example: John [says] Hello!". You can mix it up by putting more then one in the file.
File Example "BridgeWordList.txt":
says
say
speaks
speak
writes
[Bridgeword] (without [])

<br></br>
#### Whitelist.txt (CURRENLTY UNUSED)
In the whitelist are all users that are allowed to control the advanced bot settings like mute/unmute tts, define new audio device ID for example.
File Example "Whitelist.txt":
MyName
MyModerator
MyOtherModerator
SomeChillDude
[User] (without [])

<br></br>

## Planned Features
- Soundboard Capability, let the chat have some fun in your stream
- Chat Commands (Mute/Unmute the TTS with one command, Play a sound)
- Set the TTS volume
- Change Audio Device ID on the fly
- Reload the files
- A GUI

<br></br>
## Credits
This Software was made by Korbinian Maag (aka Korpsian on Twitch)
[![paypal](https://www.paypalobjects.com/en_US/DK/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=TRFFCPEAG82H2)
