const $ = require('jquery');
const fs = require('fs');

var val;
var dataObj;

function setup() {
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
  drawTiles();

  if (getKeyCode(CONTROL) && getKeyDown("S")) {
    saveData();
    console.log("Saved!");
  }
}

function getDataObj() {
  let rawdata = fs.readFileSync(`${__dirname}/../data/test.json`);
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
    fileObj[sceneName] = true;
  }

  let dataScenes = dataObj.scenes;
  let purgedScenes = [];
  for (let scene of dataScenes) {
    if (files.includes(scene.name + ".unity")) {
      purgedScenes.push(scene);
    }
  }
  dataObj.scenes = purgedScenes;

  for(let sceneName in fileObj) {
    if (!dataObj.scenes.find(scene => scene.name == sceneName)) {
      let newScene = new Scene(sceneName)
      dataObj.scenes.push(newScene);
    }
  }
}

function saveData() {
  for (let t of tiles) {
    dataObj.scenes.find(s => s.name == t.scene).x = t.x;
    dataObj.scenes.find(s => s.name == t.scene).y = t.y;
  }
  jsondata = JSON.stringify(dataObj);
  fs.writeFileSync(`${__dirname}/../data/test.json`, jsondata);
}
