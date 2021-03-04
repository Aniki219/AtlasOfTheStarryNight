const mouse = {
  left: false,
  right: false
}

const register = {};

function inputUpdate() {
  let smx = mouseX / settings.scale.x - settings.translate.x;
  let smy = mouseY / settings.scale.y - settings.translate.y;

  mouse.ax = smx;
  mouse.ay = smy;
  mouse.x = (smx - GRID_MARGIN);
  mouse.y = (smy - GRID_MARGIN);
  mouse.cx = floor(mouse.x / grid.gridSize);
  mouse.cy = floor(mouse.y / grid.gridSize);
}

function mousePressed() {
  register["mouse" + mouseButton] = new ButtonState(true);
}

function mouseReleased() {
  register["mouse" + mouseButton] = new ButtonState(false);
}

function mouseWheel(event) {
  zooming = true;
  
  let scaleStep = settings.scale.x/10;
  let ds = (event.delta >= 0) ? -1 : 1;

  settings.scale.x = max(.16, settings.scale.x + 1.6*ds*scaleStep);
  settings.scale.y = max(.09, settings.scale.y + 0.9*ds*scaleStep);

  let newMouseax = mouseX / settings.scale.x - settings.translate.x;
  let newMouseay = mouseY / settings.scale.y - settings.translate.y;

  settings.translate.x += newMouseax - mouse.ax;
  settings.translate.y += newMouseay - mouse.ay;
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

// document.addEventListener("DOMContentLoaded", function(event) {
//   document.getElementsByTagName("BODY")[0].addEventListener("click", function(event){
//     console.log("event")
//     event.preventDefault();
//   });
// });
