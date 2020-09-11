# Second-Screen-IP

## Overview 👁️
Simple application for transmitting screen between computers via IP connection in the same area newtork.  
Mobile application allows to send your computer screen to mobile device with Android system.  
Everything <b>wireless</b>, no more cables needed! 

## Description 📋
Project contains two systems: for PC and Android devices. Each system allows to cast the screen one device to another in real time.   
Each system contains two apps: sender and receiver, there is NO proxy server.   
In the PC receiver app you can change window size without changing screencast aspect ratio. 

## Tools 🛠️
### Software
1. MS Visual Studio 2019  
2. C#.NET Framework 4  
2a. Windows Forms (for PC app)  
2b. Xamarin (for Android app)  

## How to run ⚙️
### PC app
Just run .exe files. Sender is a command app and receiver is a normal window app.  
In the sender app type <i>help</i> to list all available commands.  
Set receiver device IPv4 address and type <i>start</i>.  
To stop transmitting data, just type <i>stop</i> in the sender app.

### Adnroid app
run sender.exe file on your computer, command window will open with app IP adress.
launch receiver app on a phone with android.
create an account if you havent already and log in using login and password you specified.
enter previously mentioned IP adress of sender app and click Connect button that will start transmition.
in order to end transmition click Stop button in receiver app.

## How to compile 💻
Open both projects in Visual Studio. Build, and run both applictions. Just it, nothing more. 

## Future improvements ✏️
Try to implement the possibility of cross device data transmission - from PC to mobile and vice versa. MAYBE!

## Addition 💡
The project was conducted during the Telecommunication Project course held by the Institute of Computing Science, Poznan University of Technology.

### Links
- report: https://www.overleaf.com/read/cwvcmrwhmmrh
- videos with working project: 
  - https://www.youtube.com/watch?v=ThuDO2hI9NY
  - https://youtu.be/HW77FS8kEy4
  - https://youtu.be/HDtoel7jDVc
