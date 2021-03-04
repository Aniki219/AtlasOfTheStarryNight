const tiles = [];

class Tile {
  constructor(scene = null, x = 0, y = 0, w = 1, h = 1) {
    this.scene = scene;
    this.img = getImage(this.scene + ".png")

    this.x = x;
    this.y = y;
    this.w = w;
    this.h = h;

    this.setVerticies();
    this.dragStart = {x, y};
    this.setStretchInfo();
    this.dragging = false;
    this.stretching = false;

    tiles.push(this);
  }

  update() {
    this.draw();
    this.drag();
    this.stretch();
  }

  setVerticies() {
    let {x, y, w, h} = this;
    this.vertecies = [
      new Vertex(x, 0,   y, 0,   w, h, 3*PI/4),
      new Vertex(x, w/2, y, 0,   w, h, 2*PI/4),
      new Vertex(x, w,   y, 0,   w, h, 1*PI/4),
      new Vertex(x, w,   y, h/2, w, h, 0*PI/4),
      new Vertex(x, w,   y, h,   w, h, 7*PI/4),
      new Vertex(x, w/2, y, h,   w, h, 6*PI/4),
      new Vertex(x, 0,   y, h,   w, h, 5*PI/4),
      new Vertex(x, 0,   y, h/2, w, h, 4*PI/4)
    ];
  }

  drag() {
    if (!getMouseDown(LEFT)) {
      if (this.dragging) {
        this.dragging = false;
      }
    }

    if (!this.dragging) {
      return;
    }

    this.x = round(mouse.cx - (this.w-1)/2);
    this.y = round(mouse.cy - (this.h-1)/2);
    this.setVerticies();
  }

  stretch() {
    if (!getMouseDown(LEFT) || !this.stretchInfo.stretching) {
      this.setStretchInfo();
      return;
    }

    let {stretching, xo, yo, wo, ho, dirx, diry} = this.stretchInfo;

    let dx = 0;
    let dy = 0;
    let dw = 0;
    let dh = 0;

    if (dirx == 1) {
      dw = mouse.cx - (xo + wo - 1);
      this.w = max(1, wo + dw);
    }
    if (dirx == -1) {
      dx = min(wo-1, mouse.cx - xo);
      this.x = xo + dx;
      this.w = wo - dx;
    }
    if (diry == 1) {
      dh = mouse.cy - (yo + ho - 1);
      this.h = max(1, ho + dh);
    }
    if (diry == -1) {
      dy = min(ho-1, mouse.cy - yo);
      this.y = yo + dy;
      this.h = ho - dy;
    }
    this.setVerticies();
  }

  setStretchInfo() {
    this.stretchInfo = {
      stretching: false,
      xo: this.x,
      yo: this.y,
      wo: this.w,
      ho: this.h
    };
  }

  draw() {
    stroke(50);
    fill(255);
    let x = grid.toWorldCoord(this.x);
    let y = grid.toWorldCoord(this.y);
    let w = this.w * grid.gridSize;
    let h = this.h * grid.gridSize;
    if (this.img != null && !(register["mousecenter"] && register["mousecenter"].state) && !zooming
  && !(!this.dragging && isTileDragged())) {
      image(this.img, x, y, w, h);
    } else {
      rect(x, y, w, h);
      fill(50);
      noStroke();
      textSize(16);
      text(this.scene, x + w/2, y+h/2);
    }
    noStroke();

    if (mouse.cx <= this.x + this.w && mouse.cy <= this.y +  this.h) {
      this.highlightVertecies();
    }
  }

  highlightVertecies() {
    if (this.dragging) return;
    if (!checkHover(this)) return;
    for (let v of this.vertecies) {
      let x = grid.toWorldCoord(v.x);
      let y = grid.toWorldCoord(v.y);

      if (dist(mouse.ax, mouse.ay, x, y) < 25) {
        v.clr = WHITE;
        if (dist(mouse.ax, mouse.ay, x, y) < 12) {
          v.clr = LIGHTBLUE;
          if (this.tryToSelect()) {
            let dirx = round(cos(v.angle));
            let diry = round(sin(v.angle));
            this.stretchInfo =
            {
              stretching: true,
              xo : mouse.cx - ((dirx == 1) ? (this.w - 1) : 0),
              yo : mouse.cy - ((diry == 1) ? (this.h - 1) : 0),
              wo : this.w,
              ho : this.h,
              dirx : dirx,
              diry : diry
            }
            pushToEnd(this);
          }
        }
        v.draw();
      }
    }

    let {x, y} = this.getCenter();
    let cenDist = dist(mouse.ax, mouse.ay, x, y);
    if (cenDist < 15) {
      fill(WHITE);
      stroke(GREY);
      if (cenDist < 7) {
        fill(LIGHTBLUE);
        if (this.tryToSelect()) {
          this.dragStart = createVector(this.x, this.y);
          this.dragging = true;
          pushToEnd(this);
        }
      }
      ellipse(x, y, 15, 15);
    }
  }

  tryToSelect() {
    if (!getMousePressed(LEFT)) return false;
    if (!checkHover(this)) return false;
    if (!isTopTile(this)) return false;
    return true;
  }

  getCenter() {
    let x = grid.toWorldCoord(this.x + this.w/2);
    let y = grid.toWorldCoord(this.y + this.h/2);

    return {x, y};
  }
}

function drawTiles() {
  let minX = (GRID_MARGIN + -settings.translate.x) / grid.gridSize - 1;
  let minY = (GRID_MARGIN + -settings.translate.y) / grid.gridSize - 1;
  let maxX = minX + (GRID_MARGIN + width) / (grid.gridSize * settings.scale.x) + 1;
  let maxY = minY + (GRID_MARGIN + height) / (grid.gridSize * settings.scale.y) + 1;
  tiles.forEach(t => {if (t.y + t.h >= minY && t.y <= maxY && t.x <= maxX && t.x + t.w >= minX) t.update()})
}

class Vertex {
  constructor(x, xo, y, yo, w, h, angle) {
    let xx = xo - w/2;
    let yy = yo - h/2;
    let mag = sqrt(xx*xx + yy*yy);

    this.dir = atan2(yo-h/2, xo-w/2);
    this.angle = -angle;

    this.x = x + xo;
    this.y = y + yo;

    this.s = 8;
    this.clr = WHITE;
  }

  draw() {
    let x = grid.toWorldCoord(this.x);
    let y = grid.toWorldCoord(this.y);
    let x2 = 15 * cos(3*PI/4);
    let y2 = 15 * sin(3*PI/4);

    fill(this.clr);
    stroke(0);
    strokeWeight(1);
    push()
      translate(x, y);
      rotate(this.angle);
      triangle(0, 0, x2, y2, x2, -y2);
    pop();
    strokeWeight(1);
  }
}

function pushToEnd(tile) {
  tiles.push(tiles.splice(tiles.indexOf(tile), 1)[0]);
}

function isTopTile(tile, cx = mouse.cx, cy = mouse.cy) {
  sameSquareTiles = checkTilesAtCoord(cx,  cy);
  if (sameSquareTiles.length == 1) return true;
  return (sameSquareTiles[sameSquareTiles.length-1] == tile);
}

function checkHover(tile) {
  return (mouse.cx < tile.x + tile.w &&
          mouse.cx >= tile.x &&
          mouse.cy < tile.y + tile.h &&
          mouse.cy >= tile.y) && isTopTile(tile);
}

function setTileCoords() {
  for(let tile of tiles) {
    tile.coords = [];
    for (let c = 0; c < tile.w; c++) {
      for (let r = 0; r < tile.h; r++) {
        tile.coords.push(new TileCoord(tile.x + c, tile.y + r));
      }
    }
  }
}

function checkTilesAtCoord(x, y) {
  return tiles.filter(tile =>
    tile.coords.find(c => c.x == x && c.y == y)
  );
}

function warnOverlapTiles() {
  for (let xx = 0; xx < 10; xx++) {
    for (let yy = 0; yy < 10; yy++) {
      let tilesAtCoord = checkTilesAtCoord(xx, yy);

      if (tilesAtCoord.length > 1) {
        tilesAtCoord.sort()
        for (tile of tilesAtCoord) {
          noFill();
          stroke(250, 100, 100);
          let {x, y, w, h} = tile;
          x = grid.toWorldCoord(x);
          y = grid.toWorldCoord(y);
          w *= grid.gridSize;
          h *= grid.gridSize;
          rect(x, y, w, h);

          let minx = x;
          let maxx = x + w;
          let miny = y;
          let maxy = y + h;

          for (let i = 0; i <= w+h; i+=10) {
            let x1 = max(0, i - h);
            let y1 = min(i, h);
            let x2 = min(i, w);
            let y2 = max(0, i - w);
            line(x + x1, y + y1, x + x2, y + y2);
          }
        }
      }
    }
  }
}

function isTileDragged() {
  return tiles.filter((t) => t.dragging).length > 0;
}

class TileCoord {
  constructor(x, y) {
    this.x = x;
    this.y = y;
  }
}
