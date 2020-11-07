const GRID_MARGIN = 25;

class Grid {
  constructor(gridSize = 100) {
    this.gridSize = gridSize;
  }

  update() {
    stroke(255, 255, 255);
    for (let i = GRID_MARGIN; i <= width-GRID_MARGIN; i += this.gridSize) {
      line(i, GRID_MARGIN, i, height - GRID_MARGIN);
      line(GRID_MARGIN, i, width-GRID_MARGIN, i);
    }

    if (mouse.in) {
      let x = mouse.cx * this.gridSize + GRID_MARGIN;
      let y = mouse.cy * this.gridSize + GRID_MARGIN;
      strokeWeight(6);
      stroke(150, 150, 225 + 25*sin(frameCount * 1 * 2*PI/60));
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
