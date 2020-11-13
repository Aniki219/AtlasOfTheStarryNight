const mouse = {
  left: false,
  right: false
}

const register = {};

function inputUpdate() {
  mouse.ax = mouseX;
  mouse.ay = mouseY;
  mouse.x = mouseX - GRID_MARGIN;
  mouse.y = mouseY - GRID_MARGIN;
  mouse.cx = floor(mouse.x / grid.gridSize);
  mouse.cy = floor(mouse.y / grid.gridSize);
}

function mousePressed() {
  if (mouseButton == LEFT) {
    register["mouse" + mouseButton] = new ButtonState(true);
  }
}

function mouseReleased() {
  if (mouseButton == LEFT) {
    register["mouse" + mouseButton] = new ButtonState(false);
  }
}

function keyPressed() {
  register[keyCode] = new ButtonState(true);
}

function keyReleased() {
  register[keyCode] = new ButtonState(false);
}

function getKey(key) {
  return register[key.charCodeAt(0)] && register[key.charCodeAt(0)].state;
}

function getKeyDown(key) {
  return register[key.charCodeAt(0)] && register[key.charCodeAt(0)].pressed();
}

function getKeyCode(code) {
  return register[code] && register[code].state;
}

function getKeyPressed(code) {
  return register[code] && register[code].pressed();
}

function getMouseDown(btn) {
  return register["mouse" + btn] && register["mouse" + btn].state;
}

function getMousePressed(btn) {
  return register["mouse" + btn] && register["mouse" + btn].pressed();
}

class ButtonState {
  constructor(state) {
    this.state = state;
    this.time = frameCount;
  }

  pressed() {
    return frameCount - this.time == 1;
  }
}
