class Scene {
  constructor(name, x = 0, y = 0) {
    this.name = name;
    this.x = x;
    this.y = y;
  }
}

function createSceneTiles() {
  for (let scene of dataObj.scenes) {
    new Tile(scene.name, scene.x, scene.y);
  }
}
