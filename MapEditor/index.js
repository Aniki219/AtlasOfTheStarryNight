const { app, BrowserWindow } = require('electron')
require('electron-reload')(`${__dirname}/public`)



function createWindow () {
  const win = new BrowserWindow({
    width: 1400,
    height: 830,
    webPreferences: {
      nodeIntegration: true,
			useContentSize: true,
			enableRemoteModule: true
    },
    backgroundColor: 'rgb(15,20,30)',
  })

  win.loadURL(`file:///${__dirname}/public/index.html`);
  win.toggleDevTools();
}


app.on('ready', createWindow)

app.on('window-all-closed', () => { app.quit() })

app.on('activate', () => {
  if (BrowserWindow.getAllWindows().length === 0) {
    createWindow()
  }
})
