# 🔊Twitch Noice Bot🔊

### What is this Bot for?
- This Twitch bot is using a Text to Speech Synth to read out the chat for you. Listen to the whole conversation with your community while not even looking away from the thing you do!
- Need a soundboard that is scalable and can hold an unlimited amount of soundfiles with subcategories?

<br></br>
### 🔑Key Features🔑
- Engage with your Community while not getting distrected by a Textwall, just listen the chatmembers what they have to say!
- Quick Start! (Ready in 1 minute)
- Adjustable Spam Protection (Cut out really long chat messages for example)
- Aliaslist (XxXCoolKidXxX is spoken as "John")
- Blacklist (Remove those pesky bot's out of your ear)
- Custom Bridgewords (John says Kappa; John: Kappa; John tells you kappa)
- Adjustable TTS speed (keep it slow or gotta go fast!)
- No installation requiered! (Just start the .exe)
- Helpful for streamers or users with a visual handicap etc.
- Get a notification sound when someone writes something in chat!
- Load an infinite amount of soundfiles
- Play soundfiles by Name/ID or random
- Keep control of your library with sub-libraries (!play haha plays a soundfile out of that folder)

<br></br>
### 🛠️Installation Guide🛠️
1. Download the latest release
2. Unzip all files into a empty folder
3. Go into the "Settings.txt" and add your channelname after "channel=" example: "channel=korpsian"
4. Go into the "Settings.txt" and add your oauth key after "oauth=" you can generate one [here](https://twitchapps.com/tmi/) example: oauth=oauth:SoMeCoOlTeXt123456789

### Settings Explenations
If you need a detailed explenation of what each settings does just read it in the file.

<br></br>
### 🗄️File Explenations🗄️

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
#### Whitelist.txt
In the whitelist are all users that are allowed to control the bot like mute/unmute tts, define new audio device ID or !play (normal viewers should use thair channel points) example.  
File Example "Whitelist.txt":  
MyName  
MyModerator  
MyOtherModerator  
SomeChillDude  
[User] (without [])

<br></br>
## 💡Planned Features💡
- Add "play random file out of XY sub-library" with a channel point redemption
- Bugfixing
- A GUI

<br></br>
## Credits
This Software was made by Korbinian Maag (aka Korpsian on Twitch)

[![paypal](https://www.paypalobjects.com/en_US/DK/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=TRFFCPEAG82H2)
