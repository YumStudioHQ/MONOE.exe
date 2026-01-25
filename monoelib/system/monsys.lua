return {
  console = require('monoelib.system.console'),
  fswatcher = require('monoelib.system.fswatcher'),
  threading = require('monoelib.system.threading'),
  timer = require('monoelib.system.timer'),
  info = require('monoelib.engine').info,
  path = require('monoelib.io.path'),
  windows = {
    win = require('monoelib.io.window'),
    main = require('monoelib.io.mainwin'),
  },
  io = {
    keyboard = require('monoelib.io.keyboard'),
    mouse = require('monoelib.io.mouse'),
  },
}