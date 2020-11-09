const tiles = [];

class Tile {
  constructor(scene = null, x = 0, y = 0, w = 1, h = 1) {
    this.scene = scene;
    this.x = x;
    this.y = y;
    this.w = w;
    this.h = h;
    this.vertecies = this.setVerticies();
    this.dragStart = {x, y};
    this.stretchDir = {x, y};
    this.selected = false;
    this.stretch = false;
    tiles.push(this);
  }

  update() {
    this.draw();
    this.drag();
  }

  setVerticies() {
    let {x, y, w, h} = this;
    return [
      new Vertex(x, 0,   y, 0,   3*PI/4),
      new Vertex(x, w/2, y, 0,   2*PI/4),
      new Vertex(x, w,   y, 0,   1*PI/4),
      new Vertex(x, w,   y, h/2, 0*PI/4),
      new Vertex(x, w,   y, h,   7*PI/4),
      new Vertex(x, w/2, y, h,   6*PI/4),
      new Vertex(x, 0,   y, h,   5*PI/4),
      new Vertex(x, 0,   y, h/2, 4*PI/4)
    ]
  }

  drag() {
    if (!this.selected) {
      return;
    }

    if (!mouse.left) {
      this.selected = false;
    }

    if (mouse.cx != this.dragStart.x) {
      let dx = mouse.cx - this.dragStart.x;
      for (let v of this.vertecies) {
        v.x+=dx;
      }
      this.dragStart.x = this.x = mouse.cx;
    }

    if (mouse.cy != this.dragStart.y) {
      let dy = mouse.cy - this.dragStart.y;
      for (let v of this.vertecies) {
        v.y+=dy;
      }
      this.dragStart.y = this.y = mouse.cy;
    }
  }

  draw() {
    stroke(50);
    strokeWeight(2);
    fill(255);
    beginShape();
    for (let v of this.vertecies) {
      let x = grid.toWorldCoord(v.x);
      let y = grid.toWorldCoord(v.y);
      vertex(x, y);
    }
    endShape();
    fill(0);
    strokeWeight(1);
    text(this.scene, this.getCenter().x, this.getCenter().y);

    if (mouse.cx == this.x && mouse.cy == this.y) {
      this.highlightVertecies();
    }
  }

  highlightVertecies() {
    for (let v of this.vertecies) {
      let x = grid.toWorldCoord(v.x);
      let y = grid.toWorldCoord(v.y);

      if (dist(mouse.ax, mouse.ay, x, y) < 30) {
        v.draw();
        if (dist(mouse.ax, mouse.ay, x, y) < 10) {
          v.draw();
        }
      }
    }
    let {x, y} = this.getCenter();
    let cenDist = dist(mouse.ax, mouse.ay, x, y);
    if (cenDist < 25) {
      fill(255);
      stroke(0);
      if (cenDist < 7) {
        fill(150, 150, 200);
        noStroke();
      }
      ellipse(x, y, 15, 15);
    }
  }

  getCenter() {
    let x = 0;
    let y = 0;
    for (let v of this.vertecies) {
      x += v.x;
      y += v.y;
    }
    x = grid.toWorldCoord(x/this.vertecies.length);
    y = grid.toWorldCoord(y/this.vertecies.length);

    return {x, y};
  }
}

function drawTiles() {
  tiles.forEach(t => t.update())
}

class Vertex {
  constructor(x, xo, y, yo) {
    let xx = xo - 0.5;
    let yy = yo - 0.5;
    let mag = sqrt(xx*xx + yy*yy);
    this.ax = x+0.55*xx/mag+0.5;
    this.ay = y+0.55*yy/mag+0.5;

    this.x = x+xo;
    this.y = y+yo;
    this.s = 8;
    this.dir = atan2(yo-0.5, xo-0.5);
    this.clr = color(255,255,255);
  }

  draw() {
    let x = grid.toWorldCoord(this.ax);
    let y = grid.toWorldCoord(this.ay);
    let x2 = 15 * cos(3*PI/4);
    let y2 = 15 * sin(3*PI/4);

    fill(this.clr);
    stroke(0);
    strokeWeight(2);
    push()
      translate(x, y);
      rotate(this.dir);
      triangle(0, 0, x2, y2, x2, -y2);
    pop();
    strokeWeight(1);
  }
}

function selectTile() {
  let topTile = null;
  for (let tile of tiles) {
    let {x, y} = tile.getCenter();
    let cenDist = dist(mouse.ax, mouse.ay, x, y);
    if (cenDist < 7) {
      topTile = tile;
    }
    console.log(cenDist);
  }
  if (topTile != null) {
    topTile.selected = true;
    topTile.dragStart.x = mouse.cx;
    topTile.dragStart.y = mouse.cy;
  }
}
