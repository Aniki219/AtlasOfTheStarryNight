const child_process = require('child_process');

// This function will output the lines from the script 
// and will return the full combined output
// as well as exit code when it's done (using the callback).
module.exports = {
  openTiledMap: (filename) => {
    let command = `start ${__dirname}/../Assets/StreamingAssets/TiledMapFiles/${filename}.tmx`
    child_process.spawn(command, [], {
      encoding: 'utf8',
      shell: true
    });
  }
}