class Scene {
  constructor(scene, x = 0, y = 0, w = 1,  h = 1, coords = []) {
    this.scene = scene;
    this.position = {x, y};
    this.size = {x: w, y: h};
    this.coords = coords;
  }
}

function createSceneTiles() {
  for (let s of dataObj.scenes) {
    new Tile(s.scene, s.position.x, s.position.y, s.size.x, s.size.y);
  }
}
