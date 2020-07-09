<?xml version="1.0" encoding="UTF-8"?>
<tileset version="1.2" tiledversion="1.3.4" name="testTileset" tilewidth="16" tileheight="16" tilecount="256" columns="16">
 <image source="../Tilesets/stone/stone.png" width="256" height="256"/>
 <tile id="0">
  <objectgroup draworder="index" id="2">
   <object id="1" x="0" y="0" width="16" height="16"/>
  </objectgroup>
 </tile>
 <tile id="1" type="Wall">
  <objectgroup draworder="index" id="2">
   <object id="5" template="fullCollision.tx" x="0" y="0"/>
  </objectgroup>
 </tile>
 <tile id="2">
  <objectgroup draworder="index" id="2">
   <object id="1" x="0" y="0" width="16" height="16"/>
  </objectgroup>
 </tile>
 <tile id="4">
  <objectgroup draworder="index" id="2">
   <object id="1" template="fullCollision.tx" x="0" y="0"/>
  </objectgroup>
 </tile>
</tileset>
