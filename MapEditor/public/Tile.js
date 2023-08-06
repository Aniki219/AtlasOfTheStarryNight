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
    this.dragOffset = {x: 0, y: 0};
    this.setStretchInfo();
    this.isDragging = false;
    this.isStretching = false;

    tiles.push(this);
  }

  update() {
    this.drag();
    this.stretch();
    this.draw();
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
      if (this.isDragging) {
        this.isDragging = false;
        let xCoordOffset = grid.toGridScale(this.dragOffset.x);
        let yCoordOffset = grid.toGridScale(this.dragOffset.y);
        this.x = mouse.cx - xCoordOffset;
        this.y = mouse.cy - yCoordOffset;
        this.setVerticies();
      }
      return;
    }

    if (this.isDragging) {
      this.x = (mouse.x - this.dragOffset.x - 5) / grid.gridSize;
      this.y = (mouse.y - this.dragOffset.y - 5) / grid.gridSize;
    }
  }

  stretch() {
    if (!this.isStretching || !getMouseDown(LEFT)) {
      if (this.isStretching) {
        sortTiles();
        this.isStretching = false;
      }
      this.setStretchInfo()
      return;
    }

    let {xo, yo, wo, ho, dirx, diry} = this.stretchInfo;

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

    let x = grid.toWorldCoord(this.x);
    let y = grid.toWorldCoord(this.y);
    let w = this.w * grid.gridSize;
    let h = this.h * grid.gridSize;
    if (this.img != null) {
      fill(50,50);
      rect(x, y, w, h);
      noFill();
      image(this.img, x, y, w, h);
    } else {
      stroke(50);
      fill(0,20,100);
      rect(x, y, w, h);
    }
    if ((checkHover(this) && !isTileDragged()) || this.isDragging) {
      stroke(255);
      strokeWeight(1);
      noFill();
      rect(x, y, w, h);
    }
    noStroke();

    noStroke();
    textSize(11);
    let maxchars = 16 * this.w;
    let sceneName;
    if (this.scene.length > maxchars) {
      sceneName = this.scene.substring(0, maxchars - 3) + "...";
    } else {
      sceneName = this.scene.substring(0, maxchars);
    }

    if (this.img) {
      if (checkHover(this)) {
        fill(255);
        rect(x+3, y + 3, w-6, 10)
        fill(0);
        text(sceneName, x + w/2, y+8);
      }
    } else {
      fill(255);
      text(sceneName, x + w/2, y+h/2);
    }

    if (mouse.cx <= this.x + this.w && mouse.cy <= this.y +  this.h) {
      this.highlightVertecies();
    }
  }

  highlightVertecies() {
    if (this.isDragging) return;
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
              xo : mouse.cx - ((dirx == 1) ? (this.w - 1) : 0),
              yo : mouse.cy - ((diry == 1) ? (this.h - 1) : 0),
              wo : this.w,
              ho : this.h,
              dirx : dirx,
              diry : diry
            }
            this.isStretching = true;
            pushToEnd(this);
            return;
          }
        }
        v.draw();
      }
    }

    if (this.tryToSelect()) {
      this.dragStart = createVector(this.x, this.y);
      this.dragOffset = createVector(mouse.x - grid.toWorldCoord(this.x), 
                                     mouse.y - grid.toWorldCoord(this.y));
      this.isDragging = true;
      pushToEnd(this);
    }
  }

  getCoords() {
    let coords = [];
    for (let c = 0; c < this.w; c++) {
      for (let r = 0; r < this.h; r++) {
        coords.push(createVector(this.x + c, this.y + r));
      }
    }
    return coords;
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

function drawTiles(dragging = false) {
  let minX = (GRID_MARGIN + -settings.translate.x) / grid.gridSize - 1;
  let minY = (GRID_MARGIN + -settings.translate.y) / grid.gridSize - 1;
  let maxX = minX + (GRID_MARGIN + width) / (grid.gridSize * settings.scale.x) + 1;
  let maxY = minY + (GRID_MARGIN + height) / (grid.gridSize * settings.scale.y) + 1;
  if (!dragging) {
    tiles.filter(t => !t.dragging).forEach(t => {if (t.y + t.h >= minY && t.y <= maxY && t.x <= maxX && t.x + t.w >= minX) t.update()})
  } else {
    let t = tiles.find(t => t.dragging)
    if (t) t.update();
  }
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
  sameSquareTiles = getTilesAtCoord(cx,  cy);
  if (sameSquareTiles.length == 1) return true;
  return (sameSquareTiles[sameSquareTiles.length-1] == tile);
}

function getTopTile(cx = mouse.cx, cy = mouse.cy) {
  sameSquareTiles = getTilesAtCoord(cx,  cy);
  if (sameSquareTiles.length == 0) return null;
  return sameSquareTiles[sameSquareTiles.length-1];
}

function checkHover(tile) {
  return (mouse.cx < tile.x + tile.w &&
          mouse.cx >= tile.x &&
          mouse.cy < tile.y + tile.h &&
          mouse.cy >= tile.y) && isTopTile(tile);
}

function getTilesAtCoord(x, y) {
  return tiles.filter(tile =>
    tile.getCoords().find(coord => x == coord.x && y == coord.y)
  );
}

function warnOverlapTiles() {
  for (let xx = -10; xx < 10; xx++) {
    for (let yy = -10; yy < 10; yy++) {
      let tilesAtCoord = getTilesAtCoord(xx, yy);

      if (tilesAtCoord.length > 1) {
          noFill();
          strokeWeight(1);
          stroke(250, 50, 50);

          x = grid.toWorldCoord(xx);
          y = grid.toWorldCoord(yy);
          w = grid.gridSize;
          h = grid.gridSize;
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

function isTileDragged() {
  return getDraggedTiles().length > 0;
}

function getDraggedTiles() {
  return tiles.filter((t) => t.isDragging);
}

function sortTiles() {
  tiles.sort((a,b) => b.w*b.h - a.w*a.h);
}
