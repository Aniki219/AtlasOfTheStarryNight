const GRID_MARGIN = 0;

class Grid {
  constructor(gridSize = 100) {
    this.gridSize = gridSize;
  }

  update() {
    stroke(255, 50);
    strokeWeight(.5);

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
      this.highlightGridSquares();
    }
  }

  highlightGridSquares() {
    let x = mouse.cx * this.gridSize + GRID_MARGIN;
    let y = mouse.cy * this.gridSize + GRID_MARGIN;

    noStroke()
    let topTile = getTopTile(mouse.cx, mouse.cy);

    if (this.isTileStretching()) return;
    //IF we are dragging a tile
    if (isTileDragged()) {
      //TODO: multiple tiles
      let tile = getDraggedTiles()[0];

      //Highlight space it will occupy on grid
      stroke(WHITE);
      fill(255, 55);
      rect(x-grid.toWorldCoord(grid.toGridScale(tile.dragOffset.x)), y-grid.toWorldCoord(grid.toGridScale(tile.dragOffset.y)), tile.w*this.gridSize, tile.h*this.gridSize);
      
      //IF there is a tile beneath it
      tile.getCoords().forEach(coord => {
        let tilesAtCoord = getTilesAtCoord(round(coord.x), round(coord.y));
        tilesAtCoord.forEach(tac => {
          //Highlight that tile with Red
          fill(255, 0, 0, 100);
          rect(grid.toWorldCoord(round(coord.x)), grid.toWorldCoord(round(coord.y)), 1 * grid.gridSize, 1 * grid.gridSize);
        })
      });
    } else {
    //If we are not dragging a tile
      
      //IF we are hovering over a tile
      if (topTile) {

        //Highlight that tile white and give it a white outline
        fill(200, 55);
        rect(grid.toWorldCoord(topTile.x), grid.toWorldCoord(topTile.y), this.gridSize*topTile.w, this.gridSize*topTile.h);
      } else {

        //IF we are not hovering over a tile THEN draw a flashing box on our current selected grid tile
        let a = 60+40*sin(10*(currentTime/100)/60*PI);
        fill(100, 120, 150, a);
        stroke(WHITE);
        rect(x, y, grid.gridSize, grid.gridSize);
      }
    }
  }

  drawStretchLines() {
    let tile = this.getStretchingTile();
    if (!tile) return;
    noFill();
    stroke(255);
    rect(grid.toWorldCoord(tile.x), grid.toWorldCoord(tile.y), mouse.x - grid.toWorldCoord(tile.x), mouse.y - grid.toWorldCoord(tile.y));
  }

  isTileStretching() {
    return tiles.find(t => t.isStretching) != undefined;
  }

  getStretchingTile() {
    return tiles.find(t => t.isStretching);
  }

  toWorldCoord(c) {
    return GRID_MARGIN + c * this.gridSize;
  }

  toGridScale(p) {
    return floor((p - GRID_MARGIN) / this.gridSize);
  }
}

const grid = new Grid();
