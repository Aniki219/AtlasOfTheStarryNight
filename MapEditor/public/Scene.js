class Scene {
  constructor(scene, x = 0, y = 0, w = 1,  h = 1, coords = []) {
    this.scene = scene;
    this.x = x;
    this.y = y;
    this.w = h;
    this.h = h;
    this.coords = coords;
  }
}

function createSceneTiles() {
  for (let s of dataObj.scenes) {
    new Tile(s.scene, s.x, s.y, s.w, s.h);
  }
}
