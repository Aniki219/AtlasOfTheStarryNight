const tiles = [];

class Tile {
  constructor(scene = null, x = 0, y = 0) {
    this.vertecies = [
      new Vertex(x+0,   y+0,   0*PI/4),
      new Vertex(x+0.5, y+0,   1*PI/4),
      new Vertex(x+1,   y+0,   2*PI/4),
      new Vertex(x+1,   y+0.5, 3*PI/4),
      new Vertex(x+1,   y+1,   4*PI/4),
      new Vertex(x+0.5, y+1,   5*PI/4),
      new Vertex(x+0,   y+1,   6*PI/4),
      new Vertex(x+0,   y+0.5, 7*PI/4)
    ]
    this.scene = scene;
    this.x = x;
    this.y = y;
    this.dragStart = {x, y};
    this.selected = false;
    tiles.push(this);
  }

  update() {
    this.draw();
    this.drag();
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
        v.draw()
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
  constructor(x, y, dir) {
    this.x = x;
    this.y = y;
    this.s = 8;
    this.dir = dir;
    this.clr = color(255,255,255);
  }

  draw() {
    let x = grid.toWorldCoord(this.x);
    let y = grid.toWorldCoord(this.y);
    let x2 = 10 * cos(3*PI/4);
    let y2 = 10 * sin(3*PI/4);

    stroke(150,150,200);
    strokeWeight(8);
    push()
      translate(x, y);
      rotate(-3*PI/4+this.dir);
      line(0, 0, x2, y2);
      line(0, 0, x2, -y2);
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
