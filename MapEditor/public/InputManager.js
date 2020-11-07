const mouse = {
  left: false,
  right: false
}

const register = {};
const registerPressed = {};

function inputUpdate() {
  mouse.ax = mouseX;
  mouse.ay = mouseY;
  mouse.x = mouseX - GRID_MARGIN;
  mouse.y = mouseY - GRID_MARGIN;
  mouse.cx = floor(mouse.x / grid.gridSize);
  mouse.cy = floor(mouse.y / grid.gridSize);

  for (key in registerPressed) {
    if (registerPressed[key] > 0) {
      registerPressed[key]--;
    }
  }
}

function mousePressed() {
  if (mouseButton == LEFT) {
    mouse.left = true;
    selectTile();
  }
}

function mouseReleased() {
  if (mouseButton == LEFT) {
    mouse.left = false;
  }
}

function keyPressed() {
  register[keyCode] = true;
  registerPressed[keyCode] = 2;
}

function keyReleased() {
  register[keyCode] = false;
}

function getKey(key) {
  return register[key.charCodeAt(0)];
}

function getKeyDown(key) {
  return registerPressed[key.charCodeAt(0)] > 0;
}

function getKeyCode(code) {
  return register[code];
}
