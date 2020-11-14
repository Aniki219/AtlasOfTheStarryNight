const $ = require('jquery');
const fs = require('fs');

var val;
var dataObj;

function setup() {
  LIGHTBLUE = color(180, 180, 250);
  GREY = color(80, 80, 80);
  WHITE = color(255, 255, 255);
  getDataObj();
  getSceneNames();

  let cnv = createCanvas(1000, 750);
  cnv.parent("canvas")
  cnv.mouseOut(() => mouse.in = false);
  cnv.mouseOver(() => mouse.in = true);

  createSceneTiles();

  textAlign(CENTER, CENTER);
}

function draw() {
  background(40);
  inputUpdate();
  grid.update();
  setTileCoords();
  drawTiles();
  warnOverlapTiles();

  if (getKeyCode(CONTROL) && getKeyDown("S")) {
    saveData();
    console.log("Saved!");
  }
}

function getDataObj() {
  let rawdata = fs.readFileSync(`${__dirname}/../data/test.json`);
  dataObj = {};
  dataObj = JSON.parse(rawdata);
  if (!dataObj.hasOwnProperty("scenes")) {
    dataObj.scenes = [];
  }
}

function getSceneNames() {
  scenes = [];
  let files = fs.readdirSync(`${__dirname}/../../Assets/Scenes/WorldMap`);
  let fileObj = {};

  for(fn of files) {
    let sceneName = fn.split(".")[0];
    if (sceneName == "template") continue;
    fileObj[sceneName] = true;
  }

  let dataScenes = dataObj.scenes;
  let purgedScenes = [];
  for (let scene of dataScenes) {
    if (files.includes(scene.scene + ".unity")) {
      purgedScenes.push(scene);
    }
  }
  dataObj.scenes = purgedScenes;

  for(let sceneName in fileObj) {
    if (!dataObj.scenes.find(s => s.scene == sceneName)) {
      let newScene = new Scene(sceneName)
      dataObj.scenes.push(newScene);
    }
  }
}

function saveData() {
  for (let t of tiles) {
    let {scene, x, y, w, h, coords} = t;
    let position = {x, y};
    let size = {x: w, y: h};
    let sceneObj = dataObj.scenes.find(s => s.scene == scene);
    if (sceneObj) {
      Object.assign(sceneObj, {scene, position, size, coords});
    }
  }
  jsondata = JSON.stringify(dataObj, null, 2);
  fs.writeFileSync(`${__dirname}/../data/test.json`, jsondata);
}
