const GRID_MARGIN = 25;

class Grid {
  constructor(gridSize = 100) {
    this.gridSize = gridSize;
  }

  update() {
    stroke(GREY);

    let minX = GRID_MARGIN + floor(-settings.translate.x / this.gridSize) * this.gridSize;
    let minY = GRID_MARGIN + floor(-settings.translate.y / this.gridSize) * this.gridSize;
    let maxX = width / settings.scale.x - settings.translate.x;
    let maxY = width / settings.scale.y - settings.translate.y;

    for (let i = minY; i <= maxY; i += this.gridSize) {
      line(minX, i, maxX, i);
    }

    for (let i = minX; i <= maxX; i += this.gridSize) {
      line(i, minY, i, maxY);
    }

    if (mouse.in) {
      let x = mouse.cx * this.gridSize + GRID_MARGIN;
      let y = mouse.cy * this.gridSize + GRID_MARGIN;
      strokeWeight(3);
      stroke(LIGHTBLUE);
      noFill();
      rect(x, y, this.gridSize, this.gridSize);
      strokeWeight(1);
    }
  }

  toWorldCoord(c) {
    return GRID_MARGIN + c * this.gridSize;
  }
}

const grid = new Grid();
