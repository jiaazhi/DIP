# Study Crossing
![login](/docs/images/login.jpg)
![cover](/docs/images/cover.jpg)
Study Crossing is a multiplayer game serving as an online learning platform for primary school students, built using Unity.

## How to run
1. Download game from [releases](https://github.com/jiaazhi/DIP/releases) under `Study Crossing v1.0`
2. Extract zip file and navigate to extracted folder
3. Run executable `StudyCrossing.exe`

## Project structure
The project is split up into 2 sub-projects:
- 3D Virtual Classroom, a Unity3D project containing all the game assets and scripts for building the game executable.

  GitHub page: https://github.com/pamtdoh/StudyCrossing

- Website, a python web app for handling user auth and serving the WebView component in-game. A live preview of the website can be accessed [here](http://52.187.60.115/login).
  
  GitHub page: https://github.com/richardsonqiu/virtualclassroom_web

## Set up classroom project
1. Clone the project or download directly from [releases](https://github.com/jiaazhi/DIP/releases) under `Study Crossing Source Code v1.0`
```
git clone https://github.com/pamtdoh/StudyCrossing.git
```
2. Open project with Unity version `2019.2.14f1`

## Set up website project
1. Run the following commands:
```
git clone https://github.com/richardsonqiu/virtualclassroom_web.git
cd virtualclassroom_web
py -m venv venv
pip install -r requirements.txt
```
2. Create a `.env` file to contain the environmental variables:
```
DATABASE_URI=
SECRET_KEY=
```
3. Run the application:
```
py app.py
```

## References for docs
- Group report
- Poster - [/docs/Poster.png](/docs/Poster.png)
- Video = [/docs/DemoVideo.mp4](/docs/DemoVideo.mp4)
- Slides
